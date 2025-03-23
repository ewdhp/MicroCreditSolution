using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MicroCredit.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace MicroCredit.Models.Binders
{
    public class MyBinderAttribute : ModelBinderAttribute
    {
        public MyBinderAttribute() : base(typeof(PhaseRequestModelBinder))
        {
        }
    }

    public class PhaseRequestModelBinder : IModelBinder
    {
        private readonly ILogger<PhaseRequestModelBinder> _logger;

        public PhaseRequestModelBinder(ILogger<PhaseRequestModelBinder> logger)
        {
            _logger = logger;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            _logger.LogInformation("Starting model binding for {ModelName}", bindingContext.ModelName);

            using (var reader = new StreamReader(bindingContext.HttpContext.Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                if (string.IsNullOrEmpty(body))
                {
                    _logger.LogWarning("Empty request body for {ModelName}", bindingContext.ModelName);
                    return;
                }

                try
                {
                    _logger.LogInformation("Parsing JSON value for {ModelName}", bindingContext.ModelName);
                    var jsonObject = JObject.Parse(body);
                    var discriminator = jsonObject["discriminator"]?.Value<string>();

                    if (string.IsNullOrEmpty(discriminator))
                    {
                        _logger.LogError("Discriminator value is missing for {ModelName}", bindingContext.ModelName);
                        throw new JsonException("Discriminator value is missing");
                    }

                    _logger.LogInformation("Discriminator value: {Discriminator}", discriminator);
                    var type = Assembly.GetExecutingAssembly().GetTypes()
                        .FirstOrDefault(t => typeof(IPhaseRequest).IsAssignableFrom(t) && t.Name.Equals(discriminator, StringComparison.OrdinalIgnoreCase));

                    if (type == null)
                    {
                        _logger.LogError("Unknown discriminator value: {Discriminator}", discriminator);
                        throw new JsonException("Unknown discriminator value");
                    }

                    _logger.LogInformation("Deserializing JSON to type {Type}", type.Name);
                    var result = (IPhaseRequest)JsonConvert.DeserializeObject(jsonObject["data"].ToString(), type);
                    bindingContext.Result = ModelBindingResult.Success(result);
                    _logger.LogInformation("Model binding successful for {ModelName}", bindingContext.ModelName);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing JSON for {ModelName}", bindingContext.ModelName);
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Invalid IPhaseRequest: {ex.Message}");
                }
            }
        }
    }
}
