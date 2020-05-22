using MediatR;
using VotRomania.Models;

namespace VotRomania.Commands
{
    public class AddOrUpdateApplicationContent : IRequest<(bool isSuccess, string errorMessage)>
    {
        public Language Language { get; }
        public ApplicationContentModel Content { get; }

        public AddOrUpdateApplicationContent(Language language, ApplicationContentModel content)
        {
            Language = language;
            Content = content;
        }
    }
}