// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: HealthCheck.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace GrpcProtocol
{
  public static partial class Health
  {
    static readonly string __ServiceName = "GrpcProtocol.Health";

    static readonly grpc::Marshaller<global::GrpcProtocol.HealthCheckRequest> __Marshaller_GrpcProtocol_HealthCheckRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::GrpcProtocol.HealthCheckRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::GrpcProtocol.HealthCheckResponse> __Marshaller_GrpcProtocol_HealthCheckResponse = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::GrpcProtocol.HealthCheckResponse.Parser.ParseFrom);

    static readonly grpc::Method<global::GrpcProtocol.HealthCheckRequest, global::GrpcProtocol.HealthCheckResponse> __Method_Check = new grpc::Method<global::GrpcProtocol.HealthCheckRequest, global::GrpcProtocol.HealthCheckResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Check",
        __Marshaller_GrpcProtocol_HealthCheckRequest,
        __Marshaller_GrpcProtocol_HealthCheckResponse);

    static readonly grpc::Method<global::GrpcProtocol.HealthCheckRequest, global::GrpcProtocol.HealthCheckResponse> __Method_Watch = new grpc::Method<global::GrpcProtocol.HealthCheckRequest, global::GrpcProtocol.HealthCheckResponse>(
        grpc::MethodType.ServerStreaming,
        __ServiceName,
        "Watch",
        __Marshaller_GrpcProtocol_HealthCheckRequest,
        __Marshaller_GrpcProtocol_HealthCheckResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::GrpcProtocol.HealthCheckReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of Health</summary>
    [grpc::BindServiceMethod(typeof(Health), "BindService")]
    public abstract partial class HealthBase
    {
      public virtual global::System.Threading.Tasks.Task<global::GrpcProtocol.HealthCheckResponse> Check(global::GrpcProtocol.HealthCheckRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task Watch(global::GrpcProtocol.HealthCheckRequest request, grpc::IServerStreamWriter<global::GrpcProtocol.HealthCheckResponse> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for Health</summary>
    public partial class HealthClient : grpc::ClientBase<HealthClient>
    {
      /// <summary>Creates a new client for Health</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public HealthClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for Health that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public HealthClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected HealthClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected HealthClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual global::GrpcProtocol.HealthCheckResponse Check(global::GrpcProtocol.HealthCheckRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Check(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::GrpcProtocol.HealthCheckResponse Check(global::GrpcProtocol.HealthCheckRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Check, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::GrpcProtocol.HealthCheckResponse> CheckAsync(global::GrpcProtocol.HealthCheckRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return CheckAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::GrpcProtocol.HealthCheckResponse> CheckAsync(global::GrpcProtocol.HealthCheckRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Check, null, options, request);
      }
      public virtual grpc::AsyncServerStreamingCall<global::GrpcProtocol.HealthCheckResponse> Watch(global::GrpcProtocol.HealthCheckRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Watch(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncServerStreamingCall<global::GrpcProtocol.HealthCheckResponse> Watch(global::GrpcProtocol.HealthCheckRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncServerStreamingCall(__Method_Watch, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override HealthClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new HealthClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(HealthBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Check, serviceImpl.Check)
          .AddMethod(__Method_Watch, serviceImpl.Watch).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, HealthBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_Check, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::GrpcProtocol.HealthCheckRequest, global::GrpcProtocol.HealthCheckResponse>(serviceImpl.Check));
      serviceBinder.AddMethod(__Method_Watch, serviceImpl == null ? null : new grpc::ServerStreamingServerMethod<global::GrpcProtocol.HealthCheckRequest, global::GrpcProtocol.HealthCheckResponse>(serviceImpl.Watch));
    }

  }
}
#endregion
