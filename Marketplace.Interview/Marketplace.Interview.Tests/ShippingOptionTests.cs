using System.Collections.Generic;
using NUnit.Framework;
using Marketplace.Interview.Business.Basket;
using Marketplace.Interview.Business.Shipping;

namespace Marketplace.Interview.Tests
{
    [TestFixture]
    public class ShippingOptionTests
    {
        [Test]
        public void FlatRateShippingOptionTest()
        {
            var flatRateShippingOption = new FlatRateShipping { FlatRate = 1.5m };
            var shippingAmount = flatRateShippingOption.GetAmount(new LineItem(), new Basket());

            Assert.That(shippingAmount, Is.EqualTo(1.5m), "Flat rate shipping not correct.");
        }

        [Test]
        public void PerRegionShippingOptionTest()
        {
            var perRegionShippingOption = new PerRegionShipping()
            {
                PerRegionCosts = new[]
                                                                       {
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.UK,
                                                                                   Amount = .75m
                                                                               },
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.Europe,
                                                                                   Amount = 1.5m
                                                                               }
                                                                       },
            };

            var shippingAmount = perRegionShippingOption.GetAmount(new LineItem() { DeliveryRegion = RegionShippingCost.Regions.Europe }, new Basket());
            Assert.That(shippingAmount, Is.EqualTo(1.5m));

            shippingAmount = perRegionShippingOption.GetAmount(new LineItem() { DeliveryRegion = RegionShippingCost.Regions.UK }, new Basket());
            Assert.That(shippingAmount, Is.EqualTo(.75m));
        }

        [Test]
        public void BasketShippingTotalTest()
        {
            var perRegionShippingOption = new PerRegionShipping()
            {
                PerRegionCosts = new[]
                                                                       {
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.UK,
                                                                                   Amount = .75m
                                                                               },
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.Europe,
                                                                                   Amount = 1.5m
                                                                               }
                                                                       },
            };

            var flatRateShippingOption = new FlatRateShipping { FlatRate = 1.1m };

            var basket = new Basket()
            {
                LineItems = new List<LineItem>
                                                 {
                                                     new LineItem()
                                                         {
                                                             DeliveryRegion = RegionShippingCost.Regions.UK,
                                                             Shipping = perRegionShippingOption
                                                         },
                                                     new LineItem()
                                                         {
                                                             DeliveryRegion = RegionShippingCost.Regions.Europe,
                                                             Shipping = perRegionShippingOption
                                                         },
                                                     new LineItem() {Shipping = flatRateShippingOption},
                                                 }
            };

            var calculator = new ShippingCalculator();

            decimal basketShipping = calculator.CalculateShipping(basket);

            Assert.That(basketShipping, Is.EqualTo(3.35m));
        }

        [Test]
        public void GroupRegionShippingOptionTest()
        {
            var lineItemIdentity = 1;
            var groupRegionShippingOption = new GroupRegionShipping(0.3m)
            {
                PerRegionCosts = new[]
                                                                       {
                                                                           new RegionShippingCost()
                                                                               {                                                                                   
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.UK,
                                                                                   Amount = .75m
                                                                               },
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.Europe,
                                                                                   Amount = 1.5m
                                                                               },
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.RestOfTheWorld,
                                                                                   Amount = 2.5m
                                                                               },  
                                                                       },
            };
            var basket = new Basket
            {
                LineItems = new List<LineItem>
                {
                       new LineItem()
                            {
                                Id = lineItemIdentity++,
                                DeliveryRegion = RegionShippingCost.Regions.Europe,
                                Shipping = groupRegionShippingOption
                            },                      
                       new LineItem() 
                            {
                                Id = lineItemIdentity++,
                                DeliveryRegion = RegionShippingCost.Regions.UK,
                                Shipping = groupRegionShippingOption
                            }
                }
            };

            var shippingAmount = groupRegionShippingOption.GetAmount(new LineItem()
            {
                Id = lineItemIdentity++,
                DeliveryRegion = RegionShippingCost.Regions.Europe,
                Shipping = groupRegionShippingOption
            }, basket);

            Assert.That(shippingAmount, Is.EqualTo(1.2m));

            shippingAmount = groupRegionShippingOption.GetAmount(new LineItem()
            {
                Id = lineItemIdentity++,
                DeliveryRegion = RegionShippingCost.Regions.UK,
                Shipping = groupRegionShippingOption
            }, basket);

            Assert.That(shippingAmount, Is.EqualTo(0.45m));

            shippingAmount = groupRegionShippingOption.GetAmount(new LineItem()
            {
                Id = lineItemIdentity++,
                DeliveryRegion = RegionShippingCost.Regions.RestOfTheWorld,
                Shipping = groupRegionShippingOption
            }, basket);

            Assert.That(shippingAmount, Is.EqualTo(2.5m));
        }

        [Test]
        public void BasketShippingTotalTestWithGroupRegionShippingOption()
        {
            var lineItemIdentity = 1;
            var perRegionCosts = new[]
            {
                new RegionShippingCost()
                    {
                        DestinationRegion =
                            RegionShippingCost.Regions.UK,
                        Amount = .76m
                    },
                new RegionShippingCost()
                    {
                        DestinationRegion =
                            RegionShippingCost.Regions.Europe,
                        Amount = 1.67m
                    },
                new RegionShippingCost()
                    {
                        DestinationRegion = RegionShippingCost.Regions.RestOfTheWorld,
                        Amount = 2.75m
                    }
            };
            var perRegionShippingOption = new PerRegionShipping()
            {
                PerRegionCosts = perRegionCosts
            };

            var groupRegionShippingOption = new GroupRegionShipping(0.4m)
            {
                PerRegionCosts = perRegionCosts
            };

            var flatRateShippingOption = new FlatRateShipping { FlatRate = 1.1m };

            var basket = new Basket()
            {
                LineItems = new List<LineItem>
                                                 {
                                                     new LineItem()
                                                         {
                                                             DeliveryRegion = RegionShippingCost.Regions.UK,
                                                             Shipping = perRegionShippingOption
                                                         },
                                                     new LineItem()
                                                         {
                                                             DeliveryRegion = RegionShippingCost.Regions.Europe,
                                                             Shipping = perRegionShippingOption
                                                         },
                                                     new LineItem() {Shipping = flatRateShippingOption},
                                                     new LineItem()
                                                        {
                                                            Id = lineItemIdentity++,
                                                            DeliveryRegion = RegionShippingCost.Regions.RestOfTheWorld,
                                                            Shipping = groupRegionShippingOption
                                                        },
                                                     new LineItem()
                                                        {
                                                            Id = lineItemIdentity++,
                                                            DeliveryRegion = RegionShippingCost.Regions.RestOfTheWorld,
                                                            Shipping = groupRegionShippingOption
                                                        },
                                                     new LineItem()
                                                        {
                                                            Id = lineItemIdentity++,
                                                            DeliveryRegion = RegionShippingCost.Regions.UK,
                                                            Shipping = groupRegionShippingOption,
                                                        },
                                                     new LineItem()
                                                        {
                                                            Id = lineItemIdentity++,
                                                            DeliveryRegion = RegionShippingCost.Regions.UK,
                                                            Shipping = groupRegionShippingOption
                                                        },
                                                     new LineItem()
                                                        {
                                                            Id = lineItemIdentity++,
                                                            DeliveryRegion = RegionShippingCost.Regions.Europe,
                                                            Shipping = groupRegionShippingOption,
                                                        }
                                                 }
            };

            var calculator = new ShippingCalculator();

            decimal basketShipping = calculator.CalculateShipping(basket);

            Assert.That(basketShipping, Is.EqualTo(11.42m));
        }
    }
}