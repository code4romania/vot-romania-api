using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VotRomania.Extensions
{
    public class OrderDefinitionsAlphabeticallyDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument document, DocumentFilterContext context)
        {
            if (document?.Components?.Schemas != null)
            {
                var order = document.Components.Schemas.OrderBy(c => c.Key);
                document.Components.Schemas = order.ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }
    }
}