using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Http
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class WithParameterRoutingExtensions
    {
        public static T RouteValue<T>(this HttpContext http, string name)
        {
            return http.Request.RouteValues.Get<T>(name).Value;
        }

        public static void Map<T>(this IApplicationBuilder builder, string urlPrefix, Func<T> withParameter, Action<WithParameterEndpointRouteBuilder<T>> configuration)
            => Map<T>(builder, urlPrefix, _ => withParameter(), configuration);

        public static void Map<T>(this IApplicationBuilder builder, string urlPrefix, Func<HttpContext, T> withParameter, Action<WithParameterEndpointRouteBuilder<T>> configuration)
        {
            builder.Map(urlPrefix, innerBuilder =>
            {
                innerBuilder.UseRouting();
                innerBuilder.UseEndpoints(endpointsBuilder =>
                {
                    var contextualBuilder = new WithParameterEndpointRouteBuilder<T>(endpointsBuilder, withParameter);
                    configuration(contextualBuilder);
                });
            });
        }
    }

    public class WithParameterEndpointRouteBuilder<T>
    {
        private readonly IEndpointRouteBuilder builder;
        private readonly Func<HttpContext, T> withParameter;

        public WithParameterEndpointRouteBuilder(IEndpointRouteBuilder builder, Func<HttpContext, T> withContext)
        {
            this.builder = builder;
            this.withParameter = withContext;
        }

        private readonly static string[] GetMethods = new[] { "get" };
        private readonly static string[] PostMethods = new[] { "post" };
        private readonly static string[] PutMethods = new[] { "put" };
        private readonly static string[] DeleteMethods = new[] { "delete" };

        public IEndpointConventionBuilder MapGet(string pattern, WithParameterRequestDelegate<T> requestDelegate)
            => MapMethods(pattern, GetMethods, requestDelegate);

        public IEndpointConventionBuilder MapPost(string pattern, WithParameterRequestDelegate<T> requestDelegate)
            => MapMethods(pattern, PostMethods, requestDelegate);

        public IEndpointConventionBuilder MapPut(string pattern, WithParameterRequestDelegate<T> requestDelegate)
            => MapMethods(pattern, PutMethods, requestDelegate);

        public IEndpointConventionBuilder MapDelete(string pattern, WithParameterRequestDelegate<T> requestDelegate)
            => MapMethods(pattern, DeleteMethods, requestDelegate);

        public IEndpointConventionBuilder MapGet(string pattern, WithHttpParameterRequestDelegate<T> requestDelegate)
            => MapMethods(pattern, GetMethods, requestDelegate);

        public IEndpointConventionBuilder MapPost(string pattern, WithHttpParameterRequestDelegate<T> requestDelegate)
            => MapMethods(pattern, PostMethods, requestDelegate);

        public IEndpointConventionBuilder MapPut(string pattern, WithHttpParameterRequestDelegate<T> requestDelegate)
            => MapMethods(pattern, PutMethods, requestDelegate);

        public IEndpointConventionBuilder MapDelete(string pattern, WithHttpParameterRequestDelegate<T> requestDelegate)
            => MapMethods(pattern, DeleteMethods, requestDelegate);

        public IEndpointConventionBuilder MapMethods(string pattern, IEnumerable<string> httpMethods, WithParameterRequestDelegate<T> requestDelegate)
        {
            return builder.MapMethods(pattern, httpMethods, httpContext =>
            {
                var parameterValue = withParameter(httpContext);
                try
                {
                    return requestDelegate(parameterValue);
                }
                finally
                {
                    (parameterValue as IDisposable)?.Dispose();
                }
            });
        }

        public IEndpointConventionBuilder MapMethods(string pattern, IEnumerable<string> httpMethods, WithHttpParameterRequestDelegate<T> requestDelegate)
        {
            return builder.MapMethods(pattern, httpMethods, httpContext =>
            {
                var parameterValue = withParameter(httpContext);
                try
                {
                    return requestDelegate(httpContext, parameterValue);
                }
                finally
                {
                    (parameterValue as IDisposable)?.Dispose();
                }
            });
        }
    }

    public delegate Task WithParameterRequestDelegate<T>(T parameterValue);
    public delegate Task WithHttpParameterRequestDelegate<T>(HttpContext http, T parameterValue);

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
