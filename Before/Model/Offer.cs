using System;

namespace Before.Model
{
    public class Offer : Entity
    {
        public DateTime DateExpiring { get; set; }
        public int Value { get; set; }

        public OfferType Type { get; set; }
        public Member MemberAssigned { get; set; }
    }
}
