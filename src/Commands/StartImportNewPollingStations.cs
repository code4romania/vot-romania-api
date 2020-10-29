using System;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace VotRomania.Commands
{
    public class StartImportNewPollingStations : IRequest<Result<Guid>>
    {
        public IFormFile File { get; }

        public StartImportNewPollingStations(IFormFile file)
        {
            File = file;
        }
    }
}