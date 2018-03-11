using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealEstate.Rentals
{
    public class PriceAdjustment
    {
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public string Reason { get; set; }

        public PriceAdjustment(AdjustPrice adjustPrice, decimal oldPrice)
        {
            NewPrice = adjustPrice.NewPrice;
            Reason = adjustPrice.Reason;
            OldPrice = oldPrice;
        }

        public string Describe()
        {
            return $"{OldPrice} -> {NewPrice} : {Reason}";
        }
    }
}