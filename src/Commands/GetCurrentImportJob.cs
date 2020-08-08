using CSharpFunctionalExtensions;
using MediatR;
using VotRomania.Models;

namespace VotRomania.Commands
{
    public class GetCurrentImportJob : IRequest<Result<JobStatusModel>>
    {
     
    }
}