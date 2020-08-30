using System;
using System.Collections.Generic;

namespace PromotionEngine.Models.DBModel
{
    public class PromotionCollection
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public bool IsActive { get; set; }
        public OfferType OfferType { get; set; }
        public decimal Offer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<PromotedSku> PromotedSkus { get; set; }
    }

    public enum OfferType
    {
        Percent,
        Amount
    }
}
