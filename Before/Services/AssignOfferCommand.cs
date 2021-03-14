using Before.Model;
using MediatR;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Before.Services
{
    public class AssignOfferCommand : IRequest
    {
        public Guid MemberId { get; set; }
        public Guid OfferTypeId { get; set; }

        class AssignOfferCommandHandler : AsyncRequestHandler<AssignOfferCommand>
        {
            private readonly AppDbContext _appDbContext;
            private readonly HttpClient _httpClient;

            public AssignOfferCommandHandler(AppDbContext appDbContext, HttpClient httpClient)
            {
                _appDbContext = appDbContext;
                _httpClient = httpClient;
            }

            protected override async Task Handle(AssignOfferCommand request, CancellationToken cancellationToken)
            {
                var member = await _appDbContext.Members.FindAsync(request.MemberId, cancellationToken);
                var offerType = await _appDbContext.OfferTypes.FindAsync(request.OfferTypeId, cancellationToken);

                // Calculate offer value
                var response = await _httpClient.GetAsync(
                    $"/calculate-offer-value?email={member.Email}&offerType={offerType.Name}",
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                await using var responseStream = await response.Content.ReadAsStreamAsync();

                var value = await JsonSerializer.DeserializeAsync<int>(responseStream, cancellationToken: cancellationToken);

                // Calculate expiration date
                DateTime dateExpiring;

                switch (offerType.ExpirationType)
                {
                    case ExpirationType.Assignment:
                        dateExpiring = DateTime.Today.AddDays(offerType.DaysValid);
                        break;
                    case ExpirationType.Fixed:
                        if (offerType.BeginDate != null)
                            dateExpiring =
                                offerType.BeginDate.Value.AddDays(offerType.DaysValid);
                        else
                            throw new InvalidOperationException();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // Assign Offer
                var offer = new Offer
                {
                    MemberAssigned = member,
                    Type = offerType,
                    Value = value,
                    DateExpiring = dateExpiring
                };
                member.AssignedOffers.Add(offer);
                member.NumberOfActiveOffers++;

                await _appDbContext.Offers.AddAsync(offer, cancellationToken);

                await _appDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
