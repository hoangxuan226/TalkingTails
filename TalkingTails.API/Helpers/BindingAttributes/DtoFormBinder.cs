using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TalkingTails.API.Helpers.BindingAttributes
{
    public class DtoFormBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            {
                // Fetch the value of the argument by name and set it to the model state
                var modelName = bindingContext.ModelName;
                var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

                bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

                var objRepresentation = valueProviderResult.FirstValue;
                if (objRepresentation is null)
                {
                    return Task.CompletedTask;
                }

                try
                {
                    // Deserialize the provided value and set the binding result
                    var result = JsonSerializer.Deserialize(
                        objRepresentation,
                        bindingContext.ModelType,
                        new JsonSerializerOptions()
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    bindingContext.Result = ModelBindingResult.Success(result);
                }
                catch (Exception e)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                }

                return Task.CompletedTask;
            }
        }
    }
}