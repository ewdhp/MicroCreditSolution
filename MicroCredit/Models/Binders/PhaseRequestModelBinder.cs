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
                var phaseRequest = JsonSerializer.Deserialize<PhaseRequest>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Add logging to verify the values
                Console.WriteLine($"Model Binder: Type: {phaseRequest?.Type}, Action: {phaseRequest?.Action}");

                if (phaseRequest == null || string.IsNullOrEmpty(phaseRequest.Type) || string.IsNullOrEmpty(phaseRequest.Action))
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                IPhaseReq model = phaseRequest.Type switch
                {
                    "Initial" => JsonSerializer.Deserialize<InitialRequest>
                    (body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),

                    "Pending" => JsonSerializer.Deserialize<PendingRequest>
                    (body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),

                    "Approved" => JsonSerializer.Deserialize<ApprovalRequest>
                    (body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),

                    _ => null
                };

                if (model == null)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                bindingContext.Result = ModelBindingResult.Success(model);
            }
        }
    }

    public class PhaseRequest : IPhaseReq
    {
        public string Type { get; set; }
        public string Action { get; set; }
    }
}