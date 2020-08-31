using MediatR;
using VotRomania.Models;

namespace VotRomania.Queries
{
    public class GetApplicationContent : IRequest<(bool isSuccess, string errorMessage, ApplicationContentModel[] data)>
    {
        public Language? Language { get; }

        public GetApplicationContent(Language? language = null)
        {
            Language = language;
        }
    }
}