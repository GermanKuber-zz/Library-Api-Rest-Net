using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Library.Api.Helper
{
    //TODO : 03 - Arego un objectresult que retorne Code 422
    public class UnprocessableEntityObjectResult : ObjectResult
    {
        public UnprocessableEntityObjectResult(ModelStateDictionary modelState)
            : base(new SerializableError(modelState))
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }
            //422 - Unprocessable Entity
            StatusCode = 422;
        }
    }
}
