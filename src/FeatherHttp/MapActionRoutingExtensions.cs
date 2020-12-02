using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Http
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class MapActionRoutingExtensions
    {
        private static async Task InvokeViaReflection(HttpContext httpContext, MulticastDelegate action)
        {
            async ValueTask<object> GetParameterValue(Type parameterType, string parameterName)
            {
                if (parameterType == typeof(HttpContext))
                {
                    return httpContext;
                }

                if (httpContext.Request.RouteValues.TryGetValue(parameterName, out var routeValue))
                {
                    return Convert.ChangeType(routeValue, parameterType);
                }

                var service = httpContext.RequestServices.GetService(parameterType);
                if (service != null)
                {
                    return service;
                }

                return await httpContext.Request.ReadFromJsonAsync(parameterType);
            }

            // Just AsTask it for simplicity here
            var argsAsync = action.Method.GetParameters().Select(p => GetParameterValue(p.ParameterType, p.Name).AsTask());
            var args = await Task.WhenAll(argsAsync);
            await (Task)action.DynamicInvoke(args);
        }

        public static void MapAction<T0>(this IEndpointRouteBuilder builder, string method, string pattern, Func<T0, Task> action)
            => builder.MapMethods(pattern, new[] { method }, httpContext => InvokeViaReflection(httpContext, action));

        public static void MapAction<T0, T1>(this IEndpointRouteBuilder builder, string method, string pattern, Func<T0, T1, Task> action)
            => builder.MapMethods(pattern, new[] { method }, httpContext => InvokeViaReflection(httpContext, action));

        public static void MapAction<T0, T1, T2>(this IEndpointRouteBuilder builder, string method, string pattern, Func<T0, T1, T2, Task> action)
            => builder.MapMethods(pattern, new[] { method }, httpContext => InvokeViaReflection(httpContext, action));

        public static void MapAction<T0, T1, T2, T3>(this IEndpointRouteBuilder builder, string method, string pattern, Func<T0, T1, T2, T3, Task> action)
            => builder.MapMethods(pattern, new[] { method }, httpContext => InvokeViaReflection(httpContext, action));
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
