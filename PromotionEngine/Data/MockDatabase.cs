using Newtonsoft.Json;
using PromotionEngine.Models.DBModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace PromotionEngine.Data
{
    class MockDatabase
    {
        public static List<PromotionCollection> PromotionCollection { get; set; }
        public static List<SkuCollection> SkuCollection { get; set; }

        public static void Seed()
        {
            PromotionCollection = JsonConvert.DeserializeObject<List<PromotionCollection>>(File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/MockData/MockPromotions.json"));
            SkuCollection = JsonConvert.DeserializeObject<List<SkuCollection>>(File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/MockData/MockSku.json"));
        }
    }
}
