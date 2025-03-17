using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.OpenAPI
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple =false)]
    public class SwaggerHeaderAttribute(string headerName, string description, bool isRequired, string defaultValue) : Attribute
    {
        public string HeaderName { get; set; } = headerName;
        public string Description { get; set; } = description;
        public bool IsRequired { get; set; } = isRequired;
        public string DefaultValue { get; set; } = defaultValue;
    }
}
