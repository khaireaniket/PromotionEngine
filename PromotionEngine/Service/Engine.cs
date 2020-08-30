﻿using PromotionEngine.Data;
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


            // Looping over all the filterd promotions
            foreach (var promotion in promotionsWithPurchasedSku)
            {
                // If the iterated promotion has only one Sku item
                if (promotion.PromotedSkus.Count == 1) 
                {
                    // If purchased Sku item quantity is less than that of promotional sku item quantity


                    // If purchased Sku item quantity is same as that of promotional sku item quantity


                    // If purchased Sku item quantity is more than that of promotional sku item quantity

                }
                // If the iterated promotion has more than one Sku item
                else if (promotion.PromotedSkus.Count > 1) 
                { 
                    
                }
            }

            return cart;
        }
    }
}
