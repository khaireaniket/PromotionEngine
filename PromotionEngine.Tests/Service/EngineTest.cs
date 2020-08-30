using PromotionEngine.Models.DomainModel;
using PromotionEngine.Service;
using System.Collections.Generic;
using Xunit;

namespace PromotionEngine.Tests.Service
{
    public class EngineTest
    {
        [Fact]
        public void Test_PromotionEngine_Scenario_1()
        {
            // Arrange
            Cart cart = new Cart
            {
                PurchasedSkus = new List<PurchasedSku>
                                    {
                                        new PurchasedSku { SkuId = "A", Quantity = 1 },
                                        new PurchasedSku { SkuId = "B", Quantity = 1 },
                                        new PurchasedSku { SkuId = "C", Quantity = 1 }
                                    }
            };

            Engine engine = new Engine();
            decimal expectedCartTotal = 100;

            // Act
            engine.Run(cart);

            // Assert
            Assert.Equal(expectedCartTotal, cart.CartTotal);
        }

        [Fact]
        public void Test_PromotionEngine_Scenario_2()
        {
            // Arrange
            Cart cart = new Cart
            {
                PurchasedSkus = new List<PurchasedSku>
                                    {
                                        new PurchasedSku { SkuId = "A", Quantity = 5 },
                                        new PurchasedSku { SkuId = "B", Quantity = 5 },
                                        new PurchasedSku { SkuId = "C", Quantity = 1 }
                                    }
            };

            Engine engine = new Engine();
            decimal expectedCartTotal = 370;

            // Act
            engine.Run(cart);

            // Assert
            Assert.Equal(expectedCartTotal, cart.CartTotal);
        }

        [Fact]
        public void Test_PromotionEngine_Scenario_3()
        {
            // Arrange
            Cart cart = new Cart
            {
                PurchasedSkus = new List<PurchasedSku>
                                    {
                                        new PurchasedSku { SkuId = "A", Quantity = 3 },
                                        new PurchasedSku { SkuId = "B", Quantity = 5 },
                                        new PurchasedSku { SkuId = "C", Quantity = 1 },
                                        new PurchasedSku { SkuId = "D", Quantity = 1 }
                                    }
            };

            Engine engine = new Engine();
            decimal expectedCartTotal = 280;

            // Act
            engine.Run(cart);

            // Assert
            Assert.Equal(expectedCartTotal, cart.CartTotal);
        }
    }
}
