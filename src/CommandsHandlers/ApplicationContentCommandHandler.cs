using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using VotRomania.Commands;
using VotRomania.Stores;

namespace VotRomania.CommandsHandlers
{
    public class ApplicationContentCommandHandler : IRequestHandler<AddOrUpdateApplicationContent, (bool isSuccess, string errorMessage)>,
        IRequestHandler<DeleteApplicationContent, (bool isSuccess, string errorMessage)>
    {
        private readonly IApplicationContentRepository _repository;
        private readonly ILogger<ApplicationContentCommandHandler> _logger;

        public ApplicationContentCommandHandler(IApplicationContentRepository repository, ILogger<ApplicationContentCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<(bool isSuccess, string errorMessage)> Handle(AddOrUpdateApplicationContent request, CancellationToken cancellationToken)
        {
            var languageContent = await _repository.GetApplicationContentAsync(request.Content.Language);
            if (languageContent != null && languageContent.Length > 0)
            {
                return await _repository.UpdateApplicationContentAsync(request.Content);
            }

            return await _repository.AddApplicationContentAsync(request.Content);
        }

        public async Task<(bool isSuccess, string errorMessage)> Handle(DeleteApplicationContent request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteApplicationContentAsync(request.Language);

        }
    }
}