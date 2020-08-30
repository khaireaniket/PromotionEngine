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
                    List<string> combinedSkuIds = new List<string>();

                    // Looping over all the combinations
                    foreach (string skuCombo in combinedSkuIds) 
                    {
                        // Looping over a list of promoted Sku items

                        // If purchased Sku item quantity is less than that of promotional sku item quantity

                        // If purchased Sku item quantity is same as that of promotional sku item quantity

                        // If purchased Sku item quantity is more than that of promotional sku item quantity
                    }
                }
            }

            // List for holding purchased Sku items which are not applicable for promotion
            List<PurchasedSku> remainingSkuList = new List<PurchasedSku>();

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
    }
}
