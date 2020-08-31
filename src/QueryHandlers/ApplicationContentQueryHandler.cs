using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using VotRomania.Models;
using VotRomania.Queries;
using VotRomania.Stores;

namespace VotRomania.QueryHandlers
{
    public class ApplicationContentQueryHandler : IRequestHandler<GetApplicationContent, (bool isSuccess, string errorMessage, ApplicationContentModel[] data)>
    {
        private readonly IApplicationContentRepository _repository;
        private readonly ILogger<ApplicationContentQueryHandler> _logger;

        public ApplicationContentQueryHandler(IApplicationContentRepository repository, ILogger<ApplicationContentQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<(bool isSuccess, string errorMessage, ApplicationContentModel[] data)> Handle(GetApplicationContent request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _repository.GetApplicationContentAsync(request.Language);

                return (true, string.Empty, data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"error retrieving for language = {request.Language}");
                return (true, e.Message, null);

            }
        }
    }
}