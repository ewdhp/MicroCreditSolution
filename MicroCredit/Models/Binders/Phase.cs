using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MicroCredit.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MicroCredit.Models.Binders
{
    public class IPhaseRequestModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            var value = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            try
            {
                var jsonObject = JObject.Parse(value);
                var discriminator = jsonObject["discriminator"]?.Value<string>();

                if (string.IsNullOrEmpty(discriminator))
                {
                    throw new JsonException("Discriminator value is missing");
                }

                var type = Assembly.GetExecutingAssembly().GetTypes()
                    .FirstOrDefault(t => typeof(IPhaseRequest).IsAssignableFrom(t) && t.Name.EndsWith(discriminator));

                if (type == null)
                {
                    throw new JsonException("Unknown discriminator value");
                }

                var result = (IPhaseRequest)JsonConvert.DeserializeObject(value, type);
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (JsonException ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Invalid IPhaseRequest: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }

    public class IPhaseRequestModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(IPhaseRequest))
            {
                return new BinderTypeModelBinder(typeof(IPhaseRequestModelBinder));
            }

            return null;
        }
    }

    public class IPhaseResponseModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            var value = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            try
            {
                var jsonObject = JObject.Parse(value);
                var discriminator = jsonObject["discriminator"]?.Value<string>();

                if (string.IsNullOrEmpty(discriminator))
                {
                    throw new JsonException("Discriminator value is missing");
                }

                var type = Assembly.GetExecutingAssembly().GetTypes()
                    .FirstOrDefault(t => typeof(IPhaseResponse).IsAssignableFrom(t) && t.Name.EndsWith(discriminator));

                if (type == null)
                {
                    throw new JsonException("Unknown discriminator value");
                }

                var result = (IPhaseResponse)JsonConvert.DeserializeObject(value, type);
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (JsonException ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Invalid IPhaseResponse: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }

    public class IPhaseResponseModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(IPhaseResponse))
            {
                return new BinderTypeModelBinder(typeof(IPhaseResponseModelBinder));
            }

            return null;
        }
    }
}