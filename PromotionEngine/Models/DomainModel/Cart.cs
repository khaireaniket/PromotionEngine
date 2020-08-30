using System;
using System.Collections.Generic;

namespace PromotionEngine.Models.DomainModel
{
    public class Cart
    {
        public Guid CartId { get; private set; }

        public List<PurchasedSku> PurchasedSkus { get; set; }

        public decimal CartTotal { get; set; }

        public Cart()
        {
            CartId = Guid.NewGuid();
            PurchasedSkus = new List<PurchasedSku>();
        }
    }
}
