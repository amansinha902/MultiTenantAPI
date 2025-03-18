using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Infrastructure.OpenAPI
{
    public class SwaggerHeaderAttributeProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var swaggerHeader = context.MethodInfo.GetCustomAttribute<SwaggerHeaderAttribute>();

            if (swaggerHeader != null)
            {
                var parameters = context.OperationDescription.Operation.Parameters;
                var existingParam = parameters
                    .FirstOrDefault(p => p.Kind == OpenApiParameterKind.Header && p.Name == swaggerHeader.HeaderName);

                if (existingParam != null)
                {
                    parameters.Remove(existingParam);
                }

                parameters.Add(new OpenApiParameter
                {
                    Name = swaggerHeader.HeaderName,
                    Kind = OpenApiParameterKind.Header,
                    Description = swaggerHeader.Description,
                    IsRequired = true,
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
