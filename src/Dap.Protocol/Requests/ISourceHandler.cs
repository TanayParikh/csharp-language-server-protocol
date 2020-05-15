using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Source)]
    public interface ISourceHandler : IJsonRpcRequestHandler<SourceArguments, SourceResponse>
    {
    }

    public abstract class SourceHandler : ISourceHandler
    {
        public abstract Task<SourceResponse> Handle(SourceArguments request, CancellationToken cancellationToken);
    }

    public static class SourceHandlerExtensions
    {
        public static IDisposable OnSource(this IDebugAdapterRegistry registry,
            Func<SourceArguments, CancellationToken, Task<SourceResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Source, RequestHandler.For(handler));
        }

        public static IDisposable OnSource(this IDebugAdapterRegistry registry,
            Func<SourceArguments, Task<SourceResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Source, RequestHandler.For(handler));
        }
    }
}
