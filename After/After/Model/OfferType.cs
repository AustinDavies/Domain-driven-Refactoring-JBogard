using System;

namespace After.Model
{
    public class OfferType
    {
        public DateTime? BeginDate { get; set; }
        public int DaysValid { get; set; }
        public ExpirationType ExpirationType { get; set; }
        public string Name { get; set; }

        public DateTime CalculateExpirationDate()
            => ExpirationType.CalculateExpirationDate(this);
    }
}
