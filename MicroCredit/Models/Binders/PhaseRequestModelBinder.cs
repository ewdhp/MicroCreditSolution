using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using MicroCredit.Models;
using System;
using MicroCredit.Interfaces;
using System.IO;
using System.Text.Json;

namespace MicroCredit.ModelBinders
{
    public class PhaseRequestModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var request = bindingContext.HttpContext.Request;

            if (!request.ContentType.Contains("application/json"))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            using (var reader = new StreamReader(request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var model = JsonSerializer.Deserialize<PhaseRequest>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Add logging to verify the values
                Console.WriteLine($"Model Binder: Type: {model?.Type}, Action: {model?.Action}");

                if (model == null || string.IsNullOrEmpty(model.Type) || string.IsNullOrEmpty(model.Action))
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                bindingContext.Result = ModelBindingResult.Success(model);
            }
        }
    }

    public class PhaseRequest : IPhaseRequest
    {
        public string Type { get; set; }
        public string Action { get; set; }
    }
}