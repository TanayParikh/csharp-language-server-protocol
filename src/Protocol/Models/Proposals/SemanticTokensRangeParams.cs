﻿using System;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [Method(TextDocumentNames.SemanticTokensRange)]
    public class SemanticTokensRangeParams : IWorkDoneProgressParams, ITextDocumentIdentifierParams,
        IPartialItem<SemanticTokensPartialResult>, IRequest<SemanticTokens>
    {
        /// <summary>
        /// The text document.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The range the semantic tokens are requested for.
        /// </summary>
        public Range Range { get; set; }

        public ProgressToken WorkDoneToken { get; set; }
        public ProgressToken PartialResultToken { get; set; }
    }
}