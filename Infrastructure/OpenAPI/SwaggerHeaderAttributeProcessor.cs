using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.OpenAPI
{
    public class SwaggerHeaderAttributeProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            if(context.MethodInfo.GetCustomAttributes(typeof(SwaggerHeaderAttribute)) is SwaggerHeaderAttribute swaggerHeader)
            {
                var parameters = context.OperationDescription.Operation.Parameters;
                var existingParam = parameters.
                    FirstOrDefault(p => p.Name == swaggerHeader.HeaderName &&  p.Kind == OpenApiParameterKind.Header);
                if (existingParam != null)
                {
                    parameters.Remove(existingParam);
                }
                parameters.Add(new OpenApiParameter
                {
                    Name = swaggerHeader.HeaderName,
                    Kind = OpenApiParameterKind.Header,
                    Description = swaggerHeader.Description,
                    IsRequired = swaggerHeader.IsRequired,
                    Default = swaggerHeader.DefaultValue,
                    Schema = new JsonSchema
                    {
                        Type = JsonObjectType.String,
                        Default = swaggerHeader.DefaultValue
                    }
                });
            }
            return true;
        }
    }
}
