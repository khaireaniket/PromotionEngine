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

        public Engine()
        {
            // Seed mock database
            MockDatabase.Seed();

            // Get promotion and Sku collections from mock database
            PromotionCollection = MockDatabase.PromotionCollection;
            SkuCollection = MockDatabase.SkuCollection;
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

            return cart;
        }
    }
}
