using System.Collections.Generic;

namespace Before.Model
{
    public class Member : Entity
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int NumberOfActiveOffers { get; set; }
        public List<Offer> AssignedOffers { get; set; } = new List<Offer>();
    }
}
