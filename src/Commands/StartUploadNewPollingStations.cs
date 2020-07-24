using System;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace VotRomania.Commands
{
    public class StartUploadNewPollingStations : IRequest<Result<Guid>>
    {
        public IFormFile File { get; }

        public StartUploadNewPollingStations(IFormFile file)
        {
            File = file;
        }
    }
}