using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    [Parallel, Method(TextDocumentNames.DidClose)]
    public interface IDidCloseTextDocumentHandler : IJsonRpcNotificationHandler<DidCloseTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions>, ICapability<SynchronizationCapability> { }

    public abstract class DidCloseTextDocumentHandler : IDidCloseTextDocumentHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public DidCloseTextDocumentHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; }
    }

    public static class DidCloseTextDocumentHandlerExtensions
    {
        public static IDisposable OnDidCloseTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidCloseTextDocumentParams, CancellationToken, Task<Unit>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<SynchronizationCapability> setCapability = null)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DidCloseTextDocumentHandler
        {
            private readonly Func<DidCloseTextDocumentParams, CancellationToken, Task<Unit>> _handler;
            private readonly Action<SynchronizationCapability> _setCapability;

            public DelegatingHandler(
                Func<DidCloseTextDocumentParams, CancellationToken, Task<Unit>> handler,
                Action<SynchronizationCapability> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(SynchronizationCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
