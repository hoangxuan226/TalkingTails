using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TalkingTails.API.Helpers
{
    public class SwaggerEnumParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            var type = context.ParameterInfo?.ParameterType ?? context.PropertyInfo?.PropertyType;

            if (type?.IsEnum == true)
            {
                parameter.Schema.Type = "string";
                parameter.Schema.Format = null;
                parameter.Schema.Enum.Clear();

                foreach (var enumName in Enum.GetNames(type))
                {
                    parameter.Schema.Enum.Add(new OpenApiString(enumName));
                }
            }
        }
    }
}