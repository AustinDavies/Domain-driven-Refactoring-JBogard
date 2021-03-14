using After.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace After.Services
{
    public interface IOfferValueCalculator
    {
        Task<int> Calculate(Member member, OfferType offerType,
            CancellationToken cancellationToken);
    }

    public class OfferValueCalculator : IOfferValueCalculator
    {
        private readonly HttpClient _httpClient;

        public OfferValueCalculator(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> Calculate(Member member, OfferType offerType, 
            CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(
                    $"/calculate-offer-value?email={member.Email}&offerType={offerType.Name}",
                    cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<int>(responseStream, cancellationToken: cancellationToken);
        }
    }
}
