using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoilHost
{
    public struct Unit
    {
        public static readonly Unit Value = default(Unit);

        public static readonly Task<Unit> Task = System.Threading.Tasks.Task.FromResult<Unit>(Value);

        public int CompareTo(Unit other)
        {
            return 0;
        }

        int CompareTo(object obj)
        {
            return 0;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public bool Equals(Unit other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        public static bool operator ==(Unit first, Unit second)
        {
            return true;
        }

        public static bool operator !=(Unit first, Unit second)
        {
            return false;
        }

        public override string ToString()
        {
            return "()";
        }
    }

    /// <summary>
    /// 利用这个委托，完成对Ioc容器的一层抽象。这样就可以对接任意你喜欢用的Ioc容器
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public delegate object ServiceFactory(Type serviceType);



    #region Mediator
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken));

        Task Publish(object notification, CancellationToken cancellationToken = default(CancellationToken));

        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification;
    }

    public class Mediator : IMediator
    {
        private readonly ServiceFactory _serviceFactory;

        private static readonly ConcurrentDictionary<Type, object> _requestHandlers = new ConcurrentDictionary<Type, object>();

        private static readonly ConcurrentDictionary<Type, NotificationHandlerWrapper> _notificationHandlers = new ConcurrentDictionary<Type, NotificationHandlerWrapper>();

        public Mediator(ServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            Type requestType = ((object)request).GetType();

            return default;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }
            //return PublishNotification((INotification)(object)notification, cancellationToken);
            return default;
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }
            INotification notification2;
            if ((notification2 = (notification as INotification)) != null)
            {
                //return PublishNotification(notification2, cancellationToken);
                return default;
            }
            throw new ArgumentException("notification does not implement $INotification");
        }
    }
    #endregion

    #region IRequest
    /// <summary>
    /// Allows for generic type constraints of objects implementing IRequest or IRequest{TResponse}
    /// </summary>
    public interface IBaseRequest { }

    /// <summary>
    /// 代表有返回值的请求
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    public interface IRequest<out TResponse> : IBaseRequest { }

    /// <summary>
    /// 代表无需返回值的请求
    /// </summary>
    public interface IRequest : IRequest<Unit>, IBaseRequest
    { }

    public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit> where TRequest : IRequest<Unit>
    {
    }

    #endregion

    #region INotification 多播

    public interface INotification
    {
    }

    public interface INotificationHandler<in TNotification> where TNotification : INotification
    {
        Task Handle(TNotification notification, CancellationToken cancellationToken);
    }


    #endregion

    public abstract class AsyncRequestHandler<TRequest> : IRequestHandler<TRequest> where TRequest : IRequest
    {
        async Task<Unit> IRequestHandler<TRequest, Unit>.Handle(TRequest request, CancellationToken cancellationToken)
        {
            await Handle(request, cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }

        protected abstract Task Handle(TRequest request, CancellationToken cancellationToken);
    }

    #region 行为管道接口
    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();


    public interface IPipelineBehavior<in TRequest, TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next);
    }


    public class RequestPreProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            return default;
        }
    }

    public class RequestPostProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            return default;
        }
    }


    internal abstract class RequestHandlerWrapper<TResponse>
    {
        public abstract Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken, ServiceFactory serviceFactory);
    }
    internal abstract class NotificationHandlerWrapper
    {
        public abstract Task Handle(INotification notification, CancellationToken cancellationToken, ServiceFactory serviceFactory, Func<IEnumerable<Func<Task>>, Task> publish);
    }


    internal class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse> where TRequest : IRequest<TResponse>
    {
        public override Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken, ServiceFactory serviceFactory)
        {

            //局部函数&&表达式形式的成员函数
            //相当于：
            //Task<TResponse> Handler()
            //{
            //    return GetHandler<IRequestHandler<TRequest, TResponse>>(serviceFactory).Handle((TRequest)request, cancellationToken);
            //};
            //Task<TResponse> Handler() => GetHandler<IRequestHandler<TRequest, TResponse>>(serviceFactory).Handle((TRequest)request, cancellationToken);


            //(RequestHandlerDelegate<TResponse>)Handler的意思是将函数调用转为委托
            //相当于下面写法：
            //RequestHandlerDelegate<TResponse>) handler = () => GetHandler<IRequestHandler<TRequest, TResponse>>(serviceFactory).Handle((TRequest)request, cancellationToken);

            //(next, pipeline) => () => pipeline.Handle((TRequest)request, cancellationToken, next)借助委托构造函数链，等价于combineFunc
            //相当于下面写法：
            //Func<RequestHandlerDelegate<TResponse>, IPipelineBehavior<TRequest, TResponse>, RequestHandlerDelegate<TResponse>> combineFunc = (delegater, behavior) =>
            //{
            //    RequestHandlerDelegate<TResponse> combineDelegate = () => behavior.Handle((TRequest)request, cancellationToken, delegater);
            //    //相当于下面的写法：
            //    //RequestHandlerDelegate<TResponse> combineDelegate = delegate { return behavior.Handle((TRequest)request, cancellationToken, delegater); };
            //    return combineDelegate;
            //};
            //return serviceFactory
            //    .GetInstances<IPipelineBehavior<TRequest, TResponse>>()
            //    .Reverse()
            //    .Aggregate(
            //        (RequestHandlerDelegate<TResponse>)Handler,
            //        (next, pipeline) => () => pipeline.Handle((TRequest)request, cancellationToken, next) //
            //        )//累加操作返回的是一个由委托构造而成的函数链
            //    ();//进行委托调用

            return default;
        }

    }
    #endregion

}
