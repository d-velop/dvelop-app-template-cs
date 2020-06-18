using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;

namespace Dvelop.Remote.Constraints
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ProducesConstraintAttribute :ActionMethodSelectorAttribute
    {
        public ProducesConstraintAttribute(string contentType, params string[] otherContentTypes)
        {
            if (contentType == null)
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            // We want to ensure that the given provided content types are valid values, so
            // we validate them using the semantics of MediaTypeHeaderValue.
            MediaTypeHeaderValue.Parse(contentType);

            foreach (var t in otherContentTypes)
            {
                MediaTypeHeaderValue.Parse(t);
            }

            ContentTypes = GetContentTypes(contentType, otherContentTypes);
        }

        public MediaTypeCollection ContentTypes { get; set; }

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            var requestHeaders = new RequestHeaders(routeContext.HttpContext.Request.Headers);
                
            var actionConstrains = action.ActionConstraints
                .Where(constraintMetadata => constraintMetadata.GetType() == typeof(ProducesConstraintAttribute))
                .Cast<ProducesConstraintAttribute>();
            
            var any = actionConstrains.Any(constraintAttribute => constraintAttribute.ContentTypes.Any(
                producesType => requestHeaders.Accept?.Any(acceptFromHeader => acceptFromHeader.MediaType.Value == producesType) ?? false)
            );
            
            return any;
        }

        private MediaTypeCollection GetContentTypes(string firstArg, string[] args)
        {
            var completeArgs = new List<string>
            {
                firstArg
            };
            completeArgs.AddRange(args);
            var contentTypes = new MediaTypeCollection();
            foreach (var arg in completeArgs)
            {
                var mediaType = new MediaType(arg);
                if (mediaType.MatchesAllSubTypes ||
                    mediaType.MatchesAllTypes)
                {
                    throw new InvalidOperationException(
                        $"{arg}");
                }

                contentTypes.Add(arg);
            }

            return contentTypes;
        }

    }
}
