using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using MicroCredit.Interfaces;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            _logger.LogInformation("Starting model binding.");

            var request = bindingContext.HttpContext.Request;
            if (!request.Body.CanRead)
            {
                _logger.LogWarning("Request body cannot be read.");
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            using (var reader = new StreamReader(request.Body))
            {
                var body = await reader.ReadToEndAsync();
                if (string.IsNullOrEmpty(body))
                {
                    _logger.LogWarning("Request body is empty.");
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                _logger.LogInformation("Request body read successfully.");

                try
                {
                    var jsonObject = JsonConvert.DeserializeObject<dynamic>(body);
                    if (jsonObject.Status != null)
                    {

                        int n = (int)jsonObject.Status;
                        _logger.LogInformation("PROCESSING STATUS IN BINDER: {Status}", n);

                        IPhaseRequest phaseRequest = n switch
                        {
                            0 => JsonConvert.DeserializeObject<InitialRequest>(body),
                            1 => JsonConvert.DeserializeObject<CreateRequest>(body),
                            2 => JsonConvert.DeserializeObject<ApprovalRequest>(body),
                            _ => throw new ArgumentOutOfRangeException(nameof(n), n, null)
                        };

                        _logger.LogInformation("Model binding successful.");
                        bindingContext.Result = ModelBindingResult.Success(phaseRequest);
                        return;
                    }
                    else
                    {
                        _logger.LogWarning("Data or Status is missing in the request body.");
                        bindingContext.Result = ModelBindingResult.Failed();
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing JSON.");
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex, bindingContext.ModelMetadata);
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }
        }
    }
}