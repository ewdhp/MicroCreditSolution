using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using MicroCredit.Models;
using System;
using MicroCredit.Interfaces;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MicroCredit.ModelBinders
{
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

            var request = bindingContext.HttpContext.Request;
            if (!request.Body.CanRead)
            {
                _logger.LogError("Request body is not readable.");
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            using (var reader = new StreamReader(request.Body))
            {
                var body = await reader.ReadToEndAsync();
                _logger.LogInformation($"Request body: {body}");

                if (string.IsNullOrWhiteSpace(body))
                {
                    _logger.LogError("Request body is empty.");
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                try
                {
                    var jsonDocument = JsonDocument.Parse(body);
                    var rootElement = jsonDocument.RootElement;

                    if (rootElement.TryGetProperty("Status", out var statusElement))
                    {
                        var status = statusElement.GetInt32();
                        IPhaseReq model = status switch
                        {
                            (int)CStatus.Initial => JsonSerializer.Deserialize<InitialRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                            (int)CStatus.Pending => JsonSerializer.Deserialize<PendingRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                            (int)CStatus.Approved => JsonSerializer.Deserialize<ApprovalRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                            _ => throw new InvalidOperationException("Unknown status type")
                        };

                        bindingContext.Result = ModelBindingResult.Success(model);
                    }
                    else
                    {
                        _logger.LogError("Request body does not contain 'Status' property.");
                        bindingContext.Result = ModelBindingResult.Failed();
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"JSON deserialization error: {ex.Message}");
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }
        }
    }
}