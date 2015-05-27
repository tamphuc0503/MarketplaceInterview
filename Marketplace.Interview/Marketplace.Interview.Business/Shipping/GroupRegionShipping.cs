using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Interview.Business.Shipping
{
    public class GroupRegionShipping : PerRegionShipping
    {
        public decimal DeductedAmount { get; private set; }

        public GroupRegionShipping()
            : this(0.5m)
        {
        }

        public GroupRegionShipping(decimal deductedAmount)
        {
            this.DeductedAmount = deductedAmount;
        }

        /// <summary>
        /// If there is at least one other item in the basket with same Shipping Option, same Supplier and Region,
        /// 50 pence should be deducted from the shipping.
        /// </summary>
        /// <param name="lineItem">The line item need to get amount.</param>
        /// <param name="basket">The basket contains all line items.</param>
        /// <returns>The actual shipping amount of line item.</returns>
        public override decimal GetAmount(Basket.LineItem lineItem, Basket.Basket basket)
        {
            var shippingAmount = base.GetAmount(lineItem, basket);
            var shouldDeductAmount = false;
            if (basket != null && basket.LineItems != null)
            {
                var groupLineItems = basket.LineItems.Where(x => x.DeliveryRegion == lineItem.DeliveryRegion && x.SupplierId == lineItem.SupplierId
                    && lineItem.Shipping.ToString() == x.Shipping.ToString()).ToList();
                if (groupLineItems != null && groupLineItems.Count > 0)
                {
                    //the first item will never be deducted.
                    if (lineItem != groupLineItems[0])
                        shouldDeductAmount = true;
                }
            }
            return shouldDeductAmount ? shippingAmount - DeductedAmount : shippingAmount;
        }

        public override string GetDescription(Basket.LineItem lineItem, Basket.Basket basket)
        {
            return string.Format("Group shipping to {0}", lineItem.DeliveryRegion);
        }
    }
}
