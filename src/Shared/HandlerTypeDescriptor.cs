﻿using System;
using System.Linq;
using System.Reflection;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    class HandlerTypeDescriptor : IHandlerTypeDescriptor
    {
        public HandlerTypeDescriptor(Type handlerType)
        {
            Method = handlerType.GetCustomAttribute<MethodAttribute>().Method;
            HandlerType = handlerType;
            InterfaceType = HandlerTypeHelpers.GetHandlerInterface(handlerType);
            ParamsType = handlerType.IsGenericType ? handlerType.GetGenericArguments()[0] : typeof(EmptyRequest);
            HasParamsType = ParamsType != typeof(EmptyRequest);
            IsNotification = typeof(IJsonRpcNotificationHandler).IsAssignableFrom(handlerType) || typeof(IJsonRpcNotificationHandler<>).MakeGenericType(ParamsType).IsAssignableFrom(handlerType);
            IsRequest = !IsNotification;

            var requestInterface = ParamsType?
                .GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));
            if (requestInterface != null)
                ResponseType = requestInterface.GetGenericArguments()[0];
            HasResponseType = ResponseType != null;
            RegistrationType = HandlerTypeHelpers.UnwrapGenericType(typeof(IRegistration<>), handlerType);
            HasRegistration = RegistrationType != null;
            CapabilityType = HandlerTypeHelpers.UnwrapGenericType(typeof(ICapability<>), handlerType);
            HasCapability = CapabilityType != null;
            IsDynamicCapability = typeof(IDynamicCapability).GetTypeInfo().IsAssignableFrom(CapabilityType);
            RequestProcessType = HandlerType
                .GetCustomAttributes(true)
                .Concat(HandlerType.GetCustomAttributes(true))
                .Concat(InterfaceType.GetInterfaces().SelectMany(x => x.GetCustomAttributes(true)))
                .Concat(HandlerType.GetInterfaces().SelectMany(x => x.GetCustomAttributes(true)))
                .OfType<ProcessAttribute>()
                .FirstOrDefault()?.Type;
        }

        public string Method { get; }
        public RequestProcessType? RequestProcessType { get; }
        public bool IsRequest { get; }
        public Type HandlerType { get; }
        public Type InterfaceType { get; }
        public bool IsNotification { get; }
        public bool HasParamsType { get; }
        public Type ParamsType { get; }
        public bool HasResponseType { get; }
        public Type ResponseType { get; }
        public bool HasRegistration { get; }
        public Type RegistrationType { get; }
        public bool HasCapability { get; }
        public Type CapabilityType { get; }
        public bool IsDynamicCapability { get; }
    }
}