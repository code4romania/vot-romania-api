using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VotRomania.Stores.Entities;
using VotRomania.Options;

namespace VotRomania.Services.Location.HereMaps
{
    public class HereAddressLocationSearchService : IAddressLocationSearchService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<HereAddressLocationSearchService> _logger;
        private readonly HttpClient _client;
        private readonly string _apiKey;
        public HereAddressLocationSearchService(IOptions<HereMapsOptions> hereOptions, IMemoryCache memoryCache, ILogger<HereAddressLocationSearchService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;

            _apiKey = hereOptions.Value?.Token;

            _client = new HttpClient
            {
                BaseAddress = new Uri("https://geocode.search.hereapi.com")
            };
        }

        public async Task<LocationSearchResult> FindCoordinates(string county, string fullAddress)
        {
            LocationSearchResult? locationSearchResult;
            try
            {
                if (_memoryCache.TryGetValue(fullAddress, out locationSearchResult))
                {
                    return locationSearchResult;
                }

                using (var response = await _client.GetAsync($"/v1/geocode?q={county} {fullAddress}&apiKey={_apiKey}&countryCode:ROU"))
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var geocodeResponse = JsonConvert.DeserializeObject<HereGeocodeResponse>(responseString);

                        // TODO: should we handle multiple responses ?
                        if (geocodeResponse.Items.Length > 1)
                        {
                            _logger.LogWarning($"Found {geocodeResponse.Items.Length} locations for address '{county} {fullAddress}'");
                        }

                        if (geocodeResponse.Items.Any())
                        {
                            locationSearchResult = new LocationSearchResult
                            {
                                OperationStatus = ResolvedAddressStatusType.Success,
                                Latitude = geocodeResponse.Items.First().Position.Lat,
                                Longitude = geocodeResponse.Items.First().Position.Lng
                            };

                        }
                        else
                        {
                            var errorMessage = $"Could not find address: '{county} {fullAddress}'";
                            _logger.LogWarning(errorMessage);
                            locationSearchResult = new LocationSearchResult
                            {
                                ErrorMessage = errorMessage,
                                OperationStatus = ResolvedAddressStatusType.NotFound,
                            };
                        }
                    }
                    else
                    {
                        locationSearchResult = new LocationSearchResult()
                        {
                            ErrorMessage = responseString,
                            OperationStatus = ResolvedAddressStatusType.Failed
                        };
                    }
                }
            }
            catch (Exception e)
            {
                locationSearchResult = new LocationSearchResult
                {
                    ErrorMessage = e.Message,
                    OperationStatus = ResolvedAddressStatusType.Failed
                };
            }

            _memoryCache.Set(fullAddress, locationSearchResult);

            return locationSearchResult;
        }
    }
}