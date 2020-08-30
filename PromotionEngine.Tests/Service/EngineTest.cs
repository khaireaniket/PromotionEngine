using PromotionEngine.Models.DomainModel;
using PromotionEngine.Service;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace PromotionEngine.Tests.Service
{
    public class EngineTest
    {
        [Theory]
        [ClassData(typeof(CartTestData))]
        public void Test_PromotionEngine(Cart cart, decimal expectedCartTotal)
        {
            // Arrange
            Engine engine = new Engine();

            // Act
            engine.Run(cart);

            // Assert
            Assert.Equal(expectedCartTotal, cart.CartTotal);
        }
    }

    public class CartTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new Cart {
                    PurchasedSkus = new List<PurchasedSku>
                                    {
                                        new PurchasedSku { SkuId = "A", Quantity = 1 },
                                        new PurchasedSku { SkuId = "B", Quantity = 1 },
                                        new PurchasedSku { SkuId = "C", Quantity = 1 }
                                    }
                },
                100
            };
            yield return new object[]
            {
                new Cart {
                    PurchasedSkus = new List<PurchasedSku>
                                    {
                                        new PurchasedSku { SkuId = "A", Quantity = 5 },
                                        new PurchasedSku { SkuId = "B", Quantity = 5 },
                                        new PurchasedSku { SkuId = "C", Quantity = 1 }
                                    }
                },
                370
            };
            yield return new object[]
            {
                new Cart {
                    PurchasedSkus = new List<PurchasedSku>
                                    {
                                        new PurchasedSku { SkuId = "A", Quantity = 3 },
                                        new PurchasedSku { SkuId = "B", Quantity = 5 },
                                        new PurchasedSku { SkuId = "C", Quantity = 1 },
                                        new PurchasedSku { SkuId = "D", Quantity = 1 }
                                    }
                },
                280
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}