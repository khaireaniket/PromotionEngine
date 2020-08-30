using PromotionEngine.Models.DomainModel;
using PromotionEngine.Service;
using Xunit;

namespace PromotionEngine.Tests.Service
{
    public class EngineTest
    {
        [Fact]
        public void Test_PromotionEngine()
        {
            // Arrange
            Cart cart = new Cart();
            Engine engine = new Engine();
            decimal expectedCartTotal = 0;

            // Act
            engine.Run(cart);

            // Assert
            Assert.Equal(expectedCartTotal, cart.CartTotal);
        }
    }
}
