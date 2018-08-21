using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesMovie.Filters
{
    public class SamplePageFilter : IPageFilter
    {
        public SamplePageFilter() { }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        { }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        { }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        { }
    }
}
