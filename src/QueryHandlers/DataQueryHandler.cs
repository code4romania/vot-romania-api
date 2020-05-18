using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VotRomania.Models;
using VotRomania.Providers;
using VotRomania.Queries;

namespace VotRomania.QueryHandlers
{
    public class DataQueryHandler : IRequestHandler<GetData, ApplicationData>
    {
        private readonly IDataProvider _dataProvider;

        public DataQueryHandler(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<ApplicationData> Handle(GetData request, CancellationToken cancellationToken)
        {
            var data = new ApplicationData
            {
                StaticTexts = _dataProvider.LoadStaticData()
            };

            return await Task.FromResult(data);
        }
    }
}