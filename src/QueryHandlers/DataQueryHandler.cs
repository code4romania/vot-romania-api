using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VotRomania.Models;
using VotRomania.Providers;
using VotRomania.Queries;

namespace VotRomania.QueryHandlers
{
    public class DataQueryHandler : IRequestHandler<GetData, ApplicationContent>
    {
        private readonly IDataProvider _dataProvider;

        public DataQueryHandler(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<ApplicationContent> Handle(GetData request, CancellationToken cancellationToken)
        {
            var data = new ApplicationContent
            {
                StaticTexts = _dataProvider.LoadStaticData()
            };

            return await Task.FromResult(data);
        }
    }
}