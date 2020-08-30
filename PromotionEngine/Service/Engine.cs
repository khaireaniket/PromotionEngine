using PromotionEngine.Data;
using PromotionEngine.Models.DBModel;
using System.Collections.Generic;

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
    }
}
