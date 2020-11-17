using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotRomania.Models;

namespace VotRomania.Stores
{
    public class AddressBankRepository : IAddressBankRepository
    {
        private readonly VotRomaniaContext _context;
        private readonly ILogger<AddressBankRepository> _logger;

        public AddressBankRepository(VotRomaniaContext context, ILogger<AddressBankRepository> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<Result<ResolvedAddressModel>> GetAddress(string county, string locality, string address)
        {
            var resolvedAddress = await _context.ResolvedAddresses
                .Where(x => x.County == county)
                .Where(x => x.Locality == locality)
                .Where(x => x.Address == address)
                .FirstOrDefaultAsync();

            if (resolvedAddress == null)
            {
                return Result.Failure<ResolvedAddressModel>($"Could not find address {county} {locality} {resolvedAddress}");
            }

            return Result.Success(new ResolvedAddressModel()
            {
                County = county,
                Locality = locality,
                Street = address,
                Latitude = resolvedAddress.Latitude,
                Longitude = resolvedAddress.Longitude
            });
        }

        public async Task<Result> StoreAddress(string county, string locality, string address, double latitude, double longitude)
        {
            return await Result.Try(async () =>
             {
                 await _context.ResolvedAddresses.AddAsync(new Entities.ResolvedAddressEntity()
                 {
                     County = county,
                     Locality = locality,
                     Address = address,
                     Latitude = latitude,
                     Longitude = longitude
                 });

                 await _context.SaveChangesAsync();
             }, (e) =>
             {
                 _logger.LogError(e, "Error occurred");
                 return "Error occured";
             });
        }
    }
}