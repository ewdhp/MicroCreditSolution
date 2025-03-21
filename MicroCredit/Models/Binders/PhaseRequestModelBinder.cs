using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using MicroCredit.Models;
using System;
using MicroCredit.Interfaces;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace MicroCredit.ModelBinders
{
    public class PhaseRequestModelBinder : IModelBinder
    {
        private readonly ILogger<PhaseRequestModelBinder> _logger;
        private readonly ILoanService _loanService;

        public PhaseRequestModelBinder(ILogger<PhaseRequestModelBinder> logger, ILoanService loanService)
        {
            _logger = logger;
            _loanService = loanService;
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

                    var loan = await _loanService.GetCurrentLoanAsync();

                    CStatus status = CStatus.Initial;
                    if (loan != null)
                    {
                        status = loan.Status;
                    }

                    _logger.LogInformation($"model binder status: {status}");
                    IPhaseRequest model = status switch
                    {
                        CStatus.Initial => JsonSerializer.Deserialize<InitialRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                        CStatus.Pending => JsonSerializer.Deserialize<ApprovalRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                        CStatus.Rejected => JsonSerializer.Deserialize<ApprovalRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                        CStatus.Active => JsonSerializer.Deserialize<PayRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                        _ => throw new InvalidOperationException("Unknown status type")
                    };

                    bindingContext.Result = ModelBindingResult.Success(model);
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