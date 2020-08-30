using PromotionEngine.Data;
using PromotionEngine.Models.DBModel;
using PromotionEngine.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PromotionEngine.Service
{
    public class Engine
    {
        List<PromotionCollection> PromotionCollection;
        List<SkuCollection> SkuCollection;
        Dictionary<(int, List<string>), decimal> PromotionAppliedWithPriceForSkus;

        public Engine()
        {
            // Seed mock database
            MockDatabase.Seed();

            // Get promotion and Sku collections from mock database
            PromotionCollection = MockDatabase.PromotionCollection;
            SkuCollection = MockDatabase.SkuCollection;

            // Dictionary will be used to hold SkuIds which are applied a promotion with fixed price
            PromotionAppliedWithPriceForSkus = new Dictionary<(int, List<string>), decimal>();
        }

        public Cart Run(Cart cart)
        {
            // Get SkuIds from cart
            List<string> purchasedSkuIds = cart.PurchasedSkus.Select(a => a.SkuId).ToList();

            // Filter only active promotions which are applicable today
            List<PromotionCollection> todaysActivePromotions = PromotionCollection
                                                    .Where(a => a.IsActive &&
                                                                (DateTime.Now > a.StartDate && DateTime.Now < a.EndDate))
                                                    .ToList();

            // Filter out only those promotions which contains purchased Sku items
            List<PromotionCollection> promotionsWithPurchasedSku = todaysActivePromotions
                                                        .Where(a => a.PromotedSkus
                                                                    .Where(b => purchasedSkuIds.Contains(b.SkuId))
                                                                    .Any())
                                                        .ToList();

            // List for holding purchased Sku items which are not applicable for promotion
            List<PurchasedSku> remainingSkuList = new List<PurchasedSku>();
            Random rnd = new Random();

            // Looping over all the filterd promotions
            foreach (var promotion in promotionsWithPurchasedSku)
            {
                // If the iterated promotion has only one Sku item
                if (promotion.PromotedSkus.Count == 1)
                {
                    var promotedSku = promotion.PromotedSkus.FirstOrDefault();
                    var purchasedSku = cart.PurchasedSkus.FirstOrDefault(a => a.SkuId == promotedSku.SkuId);
                    // Get correponding Sku entry from mock database collection
                    var skuRow = SkuCollection.FirstOrDefault(a => a.SkuId == promotedSku.SkuId);

                    // If purchased Sku item quantity is less than that of promotional sku item quantity apply base price of the Sku item
                    if (purchasedSku.Quantity < promotedSku.Quantity)
                    {
                        PromotionAppliedWithPriceForSkus.Add((rnd.Next(-100, -1), new List<string> { purchasedSku.SkuId }), purchasedSku.Quantity * skuRow.Price);
                    }
                    // If purchased Sku item quantity is same as that of promotional sku item quantity directly apply offer
                    else if (purchasedSku.Quantity == promotedSku.Quantity)
                    {
                        PromotionAppliedWithPriceForSkus.Add((promotion.PromotionId, new List<string> { purchasedSku.SkuId }), promotion.Offer);
                    }
                    // If purchased Sku item quantity is more than that of promotional sku item quantity calculate price with offer
                    else if (purchasedSku.Quantity > promotedSku.Quantity)
                    {
                        var parts = purchasedSku.Quantity / promotedSku.Quantity;
                        var reminder = purchasedSku.Quantity % promotedSku.Quantity;

                        var price = (parts * promotion.Offer) + (reminder * skuRow.Price);
                        PromotionAppliedWithPriceForSkus.Add((promotion.PromotionId, new List<string> { purchasedSku.SkuId }), price);
                    }
                }
                // If the iterated promotion has more than one Sku item
                else if (promotion.PromotedSkus.Count > 1)
                {
                    // List down all the combinations of purchased Sku items
                    // E.g. { 'A' , 'B' , 'C' , 'A,B' , 'B,C' , 'C,D' , 'A,B,C' }
                    var combinedSkuIds = GetAllCombinations(purchasedSkuIds);

                    // Looping over all the combinations
                    foreach (string skuCombo in combinedSkuIds)
                    {
                        // Split current combination into an array
                        // E.g. ['C', 'D']
                        var skuComboArray = skuCombo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        // If number of Sku items in promotion is same as that length of array of combination
                        if (promotion.PromotedSkus.Count == skuComboArray.Length)
                        {
                            // Extract only SkuIds from promotion
                            var promotedSkuIds = promotion.PromotedSkus.Select(a => a.SkuId).ToArray();

                            // If current combination of Sku items matches promotion Sku items
                            bool areEqual = promotedSkuIds.SequenceEqual(skuComboArray);
                            if (areEqual)
                            {
                                var promotedSkuList = promotion.PromotedSkus;
                                var purchasedSkuList = cart.PurchasedSkus.Where(a => skuComboArray.Contains(a.SkuId));
                                // Get correponding list of Sku item entries from mock database collection
                                var skuRowList = SkuCollection.Where(a => skuComboArray.Contains(a.SkuId));

                                bool isPromoApplicable = true;

                                // Maintains combination of related Sku items which are promoted
                                List<string> skuList = new List<string>();

                                // Looping over a list of promoted Sku items
                                foreach (var promotedSku in promotedSkuList)
                                {
                                    // For current promoted Sku item extract purchased Sku item and Sku item from mock collection
                                    var purchasedSku = cart.PurchasedSkus.FirstOrDefault(a => a.SkuId == promotedSku.SkuId);
                                    var skuRow = SkuCollection.FirstOrDefault(a => a.SkuId == promotedSku.SkuId);

                                    // If purchased Sku item quantity is less than that of promotional sku item quantity break the promotion 
                                    if (purchasedSku.Quantity < promotedSku.Quantity)
                                    {
                                        isPromoApplicable = false;
                                        break;
                                    }
                                    // If purchased Sku item quantity is more than or same as that of promotional sku item quantity directly apply offer
                                    else
                                    {
                                        // Check if promotion and list of Sku item already exists in Dictionary
                                        if (PromotionAppliedWithPriceForSkus.Keys.Contains((promotion.PromotionId, skuList)))
                                        {
                                            // If exists remove the record to add updated record later
                                            PromotionAppliedWithPriceForSkus.Remove((promotion.PromotionId, skuList));
                                        }

                                        skuList.Add(purchasedSku.SkuId);
                                        PromotionAppliedWithPriceForSkus.Add((promotion.PromotionId, skuList), promotion.Offer);

                                        // If purchased Sku item quantity is more than that of promotional sku item quantity
                                        if (purchasedSku.Quantity > promotedSku.Quantity)
                                        {
                                            // If Sku item is applied promotion, the remaining quantity will be applied base price
                                            remainingSkuList.Add(new PurchasedSku { SkuId = purchasedSku.SkuId, Quantity = purchasedSku.Quantity - promotedSku.Quantity });
                                        }
                                    }
                                }

                                // If even a single Sku item cannot satisfy promotion condition, pricing of all the Sku items which are part of combination will be recalculated
                                if (!isPromoApplicable)
                                {
                                    // Undo promotion of dependent Sku items 
                                    PromotionAppliedWithPriceForSkus.Remove((promotion.PromotionId, skuList));
                                }
                            }
                        }
                    }
                }
            }

            List<string> promotionAppliedSkus = new List<string>();

            // Dictionary key consisting of PromotionId and List of Sku items
            var keys = PromotionAppliedWithPriceForSkus.Keys;
            foreach (var key in keys)
            {
                // Get all the Sku items which are applied in a promotion
                promotionAppliedSkus.AddRange(key.Item2);
            }
            // Distinct Sku items
            promotionAppliedSkus = promotionAppliedSkus.Distinct().ToList();

            // Get all the Sku items which are not part of any promotion
            var nonPromotedSkuList = purchasedSkuIds.Except(promotionAppliedSkus).ToList();
            foreach (var sku in nonPromotedSkuList)
            {
                var purchasedSku = cart.PurchasedSkus.FirstOrDefault(a => a.SkuId == sku);
                // Add non promoted sku item in remaining sku item list
                remainingSkuList.Add(new PurchasedSku { SkuId = purchasedSku.SkuId, Quantity = purchasedSku.Quantity });
            }

            foreach (var remainingSku in remainingSkuList)
            {
                var skuRow = SkuCollection.FirstOrDefault(a => a.SkuId == remainingSku.SkuId);
                // Apply base price of the Sku item
                PromotionAppliedWithPriceForSkus.Add((rnd.Next(-100, -1), new List<string> { remainingSku.SkuId }), remainingSku.Quantity * skuRow.Price);
            }

            cart.CartTotal = PromotionAppliedWithPriceForSkus.Sum(a => a.Value);
            return cart;
        }

        private List<string> GetAllCombinations(List<string> inputList)
        {
            List<string> outputList = new List<string>();
            string outstr = string.Empty;
            double count = Math.Pow(2, inputList.Count);
            for (int i = 1; i <= count - 1; i++)
            {
                string str = Convert.ToString(i, 2).PadLeft(inputList.Count, '0');
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                        outstr += inputList[j] + ",";
                    }
                }
                outputList.Add(outstr);
                outstr = string.Empty;
            }

            return outputList;
        }
    }
}
