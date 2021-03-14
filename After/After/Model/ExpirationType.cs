using Ardalis.SmartEnum;
using System;

namespace After.Model
{
    public abstract class ExpirationType : SmartEnum<ExpirationType>
    {
        protected ExpirationType(string name, int value)
            :base(name, value)
        {
        }

        public static readonly ExpirationType Assignment = new AssignmentType();
        public static readonly ExpirationType Fixed = new FixedType();

        private class AssignmentType : ExpirationType
        {
            public AssignmentType()
                : base(nameof(Assignment), 1)
            {
            }

            public override DateTime CalculateExpirationDate(OfferType offerType)
            => DateTime.Today.AddDays(offerType.DaysValid);
        }

        private class FixedType : ExpirationType
        {
            public FixedType()
                : base(nameof(Fixed), 2)
            {
            }

            public override DateTime CalculateExpirationDate(OfferType offerType)
                => offerType.BeginDate?.AddDays(offerType.DaysValid)
                ?? throw new InvalidOperationException();
        }

        public abstract DateTime CalculateExpirationDate(OfferType offerType);
    }
}
