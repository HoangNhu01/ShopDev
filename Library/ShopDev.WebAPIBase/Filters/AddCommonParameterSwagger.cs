using ShopDev.ApplicationBase.Localization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ShopDev.WebAPIBase.Filters
{
    public class AddCommonParameterSwagger : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();
            var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor != null)
            {
                operation.Parameters.Add(
                    new OpenApiParameter()
                    {
                        Name = "local",
                        In = ParameterLocation.Query,
                        Description = "Localization",
                        Required = false,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Enum = LocalizationNames
                                .All.Select(o => new OpenApiString(o))
                                .Cast<IOpenApiAny>()
                                .ToList()
                        }
                    }
                );
            }
        }
    }
}
