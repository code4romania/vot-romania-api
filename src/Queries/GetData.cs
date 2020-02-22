using MediatR;
using VotRomania.Models;

namespace VotRomania.Queries
{
    public class GetData:IRequest<ApplicationData[]>
    {
    }
}
