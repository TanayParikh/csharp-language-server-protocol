using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    [Parallel, Method(WorkspaceNames.WorkspaceSymbol)]
    public interface IWorkspaceSymbolsHandler : IJsonRpcRequestHandler<WorkspaceSymbolParams, Container<SymbolInformation>>, ICapability<WorkspaceSymbolCapability>, IRegistration<WorkspaceSymbolRegistrationOptions> { }

    public abstract class WorkspaceSymbolsHandler : IWorkspaceSymbolsHandler
    {
        protected WorkspaceSymbolCapability Capability { get; private set; }
        private readonly WorkspaceSymbolRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }
        public WorkspaceSymbolsHandler(WorkspaceSymbolRegistrationOptions registrationOptions, IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public WorkspaceSymbolRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<SymbolInformation>> Handle(WorkspaceSymbolParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(WorkspaceSymbolCapability capability) => Capability = capability;
    }

    public static class WorkspaceSymbolsHandlerExtensions
    {
        public static IDisposable OnWorkspaceSymbols(
            this ILanguageServerRegistry registry,
            Func<WorkspaceSymbolParams, CancellationToken, Task<Container<SymbolInformation>>> handler,
            Action<WorkspaceSymbolCapability> setCapability = null,
            WorkspaceSymbolRegistrationOptions registrationOptions = null)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : WorkspaceSymbolsHandler
        {
            private readonly Func<WorkspaceSymbolParams, CancellationToken, Task<Container<SymbolInformation>>> _handler;
            private readonly Action<WorkspaceSymbolCapability> _setCapability;

            public DelegatingHandler(
                Func<WorkspaceSymbolParams, CancellationToken, Task<Container<SymbolInformation>>> handler,
                IWorkDoneProgressManager progressManager,
                Action<WorkspaceSymbolCapability> setCapability,
                WorkspaceSymbolRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<SymbolInformation>> Handle(WorkspaceSymbolParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(WorkspaceSymbolCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
