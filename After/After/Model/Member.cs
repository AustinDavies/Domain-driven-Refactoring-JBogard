using After.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace After.Model
{
    public class Member : Entity
    {
        private List<Offer> _assignedOffers = new List<Offer>();
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int NumberOfActiveOffers { get; private set; }

        public IEnumerable<Offer> AssignedOffers => _assignedOffers;

        public async Task<Offer> AssignOffer(OfferType offerType,
            IOfferValueCalculator offerValueCalculator,
            CancellationToken cancellationToken)
        {
            var value = await offerValueCalculator.Calculate(this, offerType,
                    cancellationToken);

            var dateExpiring = offerType.CalculateExpirationDate();

            var offer = new Offer
            {
                MemberAssigned = this,
                Type = offerType,
                Value = value,
                DateExpiring = dateExpiring
            };

            _assignedOffers.Add(offer);
            NumberOfActiveOffers++;

            return offer;
        }
    }
}
