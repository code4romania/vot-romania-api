using MediatR;
using VotRomania.Models;

namespace VotRomania.Commands
{
    public class DeleteApplicationContent : IRequest<(bool isSuccess, string errorMessage)>
    {
        public Language Language { get; }

        public DeleteApplicationContent(Language language)
        {
            Language = language;
        }
    }
}