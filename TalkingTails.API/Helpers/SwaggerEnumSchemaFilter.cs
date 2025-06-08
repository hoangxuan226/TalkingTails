using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TalkingTails.API.Helpers
{
    public class SwaggerEnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();
                schema.Type = "string";
                schema.Format = null;

                foreach (var enumName in Enum.GetNames(context.Type))
                {
                    schema.Enum.Add(new OpenApiString(enumName));
                }
            }
        }
    }
}