using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public interface IWindowLanguageClient : IResponseRouter
    {
        IWindowProgressLanguageClient Progress { get; }
    }
}
