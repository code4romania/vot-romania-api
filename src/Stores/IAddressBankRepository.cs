using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using VotRomania.Models;

namespace VotRomania.Stores
{
    public interface IAddressBankRepository
    {
        Task<Result<ResolvedAddressModel>> GetAddress(string county, string locality, string address);
        Task<Result> StoreAddress(string county, string locality, string address, double latitude, double longitude);
    }
}