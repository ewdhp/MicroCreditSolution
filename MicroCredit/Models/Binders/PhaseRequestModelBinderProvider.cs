using MicroCredit.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MicroCredit.ModelBinders
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class PhaseRequestModelBinderAttribute : Attribute, IBindingSourceMetadata, IModelNameProvider
    {
        public BindingSource BindingSource => BindingSource.Custom;
        public string Name { get; set; }

        public PhaseRequestModelBinderAttribute(string name = null)
        {
            Name = name;
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

            if (context.Metadata.IsComplexType && context.Metadata.ModelType == typeof(IPhaseRequest))
            {
                return new BinderTypeModelBinder(typeof(PhaseRequestModelBinder));
            }

            return null;
        }
    }
}