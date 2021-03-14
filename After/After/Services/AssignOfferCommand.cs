using After.Model;
using MediatR;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace After.Services
{
    public class AssignOfferCommand : IRequest
    {
        public Guid MemberId { get; set; }
        public Guid OfferTypeId { get; set; }

        class AssignOfferCommandHandler : AsyncRequestHandler<AssignOfferCommand>
        {
            private readonly AppDbContext _appDbContext;
            private readonly IOfferValueCalculator _offerValueCalculator;
            protected override async Task Handle(AssignOfferCommand request, CancellationToken cancellationToken)
            {
                var member = await _appDbContext.Members.FindAsync(request.MemberId, cancellationToken);
                
                var offerType = await _appDbContext.OfferTypes.FindAsync(request.OfferTypeId, cancellationToken);

                var offer = await member.AssignOffer(offerType, _offerValueCalculator, cancellationToken);

                await SaveOffer(offer, cancellationToken);
            }

            private async Task SaveOffer(Offer offer, CancellationToken cancellationToken)
            {
                await _appDbContext.Offers.AddAsync(offer, cancellationToken);

                await _appDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
