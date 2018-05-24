using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace RestNet.App_Start
{
    public class AttachRouteNameFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            {
                string routeName = apiDescription
                    ?.GetControllerAndActionAttributes<RouteAttribute>()
                    ?.FirstOrDefault()
                    ?.Name;

                operation.summary = string.Join(" - ", new[] { routeName, operation.summary }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
            }
        }
    }
}