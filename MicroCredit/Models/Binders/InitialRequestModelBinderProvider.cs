using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MicroCredit.Interfaces;
using System;

namespace MicroCredit.Models.Binders
{
    public class InitialRequestModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(IPhaseRequest))
            {
                return new BinderTypeModelBinder(typeof(InitialRequestModelBinder));
            }

            return null;
        }
    }
}