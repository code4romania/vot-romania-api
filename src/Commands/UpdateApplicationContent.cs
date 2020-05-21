using MediatR;
using VotRomania.Models;

namespace VotRomania.Commands
{
    public class UpdateApplicationContent : IRequest<(bool isSuccess, string errorMessage)>
    {
        public ApplicationContent Content { get; }

        public UpdateApplicationContent(ApplicationContent content)
        {
            Content = content;
        }
    }
}