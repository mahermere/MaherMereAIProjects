using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace Triple_S_AEP_API
{
    public class RequireHylandHeadersOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
                operation.parameters = new List<Parameter>();

            operation.parameters.Add(new Parameter
            {
                name = "Hyland-Username",
                @in = "header",
                type = "string",
                required = true,
                description = "Hyland Username"
            });
            operation.parameters.Add(new Parameter
            {
                name = "Hyland-Password",
                @in = "header",
                type = "string",
                required = true,
                description = "Hyland Password"
            });
        }
    }
}
