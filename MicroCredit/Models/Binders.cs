using System;
using System.IO;
using System.Threading.Tasks;
using MicroCredit.Factories;
using MicroCredit.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Newtonsoft.Json.Linq;

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

            var requestData = bindingContext.HttpContext.Request.Body;
            using (var reader = new StreamReader(requestData))
            {
                var body = await reader.ReadToEndAsync();
                var data = JObject.Parse(body);
                var request = PhaseRequestFactory.CreateRequest(data);
                bindingContext.Result = ModelBindingResult.Success(request);
            }
        }
    }

    public class PhaseRequestModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(IPhaseRequest))
            {
                return new BinderTypeModelBinder(typeof(PhaseRequestModelBinder));
            }

            return null;
        }
    }
}