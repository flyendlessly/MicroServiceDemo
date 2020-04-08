using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Application.MiddleWare
{
    #region 扩展方法
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Distinct<T, K>(this IEnumerable<T> source, Func<T, K> predicate)
        {
            HashSet<K> sets = new HashSet<K>();
            foreach (var item in source)
            {
                if (sets.Add(predicate(item)))
                {
                    yield return item;
                }
            }
        }
    }
    #endregion

    #region 属性
    public class DiagnosticNameAttribute : Attribute
    {
        public string Name { get; }

        public DiagnosticNameAttribute(string name)
        {
            Name = name;
        }
    }
    public interface IParameterResolver
    {
        object Resolve(object value);
    }
    public abstract class ParameterResolverAttribute : Attribute, IParameterResolver
    {
        public abstract object Resolve(object value);
    }
    public class ObjectAttribute : ParameterResolverAttribute
    {
        public Type TargetType { get; set; }

        public override object Resolve(object value)
        {
            if (TargetType == null || value == null)
            {
                return value;
            }

            if (TargetType == value.GetType())
            {
                return value;
            }

            if (TargetType.IsInstanceOfType(value))
            {
                return value;
            }

            return null;
        }
    }

    public class PropertyAttribute : ParameterResolverAttribute
    {
        public string Name { get; set; }

        public override object Resolve(object value)
        {
            if (value == null || Name == null)
            {
                return null;
            }

            var property = value.GetType().GetProperty(Name);
            return property?.GetReflector()?.GetValue(value);
        }
    }
    public class NullParameterResolver : IParameterResolver
    {
        public object Resolve(object value)
        {
            return null;
        }
    }
    #endregion

    /// <summary>
    /// DiagnosticSource事件发布,DiagnosticSource相当于被观察者，发送消息给观察者们
    /// </summary>
    public class LogDiagnostic
    {
        private static readonly DiagnosticSource testDiagnosticListener = new DiagnosticListener("TestDiagnosticListener");

        public static void SendLog()
        {
            if (testDiagnosticListener.IsEnabled("RequestStart"))
            {
                testDiagnosticListener.Write("RequestStart", "hello world");
            }
        }
    }

    /// <summary>
    /// DiagnosticListener 事件消费处理接口
    /// </summary>
    public interface IDiagnosticProcessor
    {
        string ListenerName { get; }
    }
    /// <summary>
    /// 定义诊断器名为 TestDiagnosticListener 的 DiagnosticListener 事件消费处理逻辑
    /// 实现类中的 ListenerName 必须与对应 DiagnosticListener 的诊断器名一致
    /// </summary>
    public class TestDiagnosticProcessor : IDiagnosticProcessor
    {
        public string ListenerName { get; } = "TestDiagnosticListener";

        //携带数据对象写入诊断器 DiagnosticSource 中
        [DiagnosticName("RequestStart")]
        public void RequestStart([Object]string name)
        {
            Console.WriteLine(name);
        }
    }

    /// <summary>
    /// 观察者,继承订阅所有类DiagnosticListener
    /// </summary>
    public class DiagnosticListenerObserver : IObserver<DiagnosticListener>
    {
        private readonly List<IDisposable> _subscriptions;
        private readonly IEnumerable<IDiagnosticProcessor> _diagnosticProcessors;
        public DiagnosticListenerObserver(IEnumerable<IDiagnosticProcessor> diagnosticProcessors)
        {
            _subscriptions = new List<IDisposable>();
            _diagnosticProcessors = diagnosticProcessors;
        }
        public void OnCompleted()
        {
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions.Clear();
        }

        public void OnError(Exception error)
        {
            // Method intentionally left empty.
        }

        public void OnNext(DiagnosticListener value)
        {
            //获取当前的诊断器名
            var diagnosticProcessor = _diagnosticProcessors?.FirstOrDefault(_ => _.ListenerName == value.Name);
            if (diagnosticProcessor == null) return;
            //订阅
            value.Subscribe(new DiagnosticEventObserver(diagnosticProcessor));
        }
    }

    public class DiagnosticEvent
    {
        private readonly IDiagnosticProcessor _diagnosticProcessor;
        private readonly IParameterResolver[] _parameterResolvers;
        private readonly MethodReflector _reflector;

        public DiagnosticEvent(IDiagnosticProcessor diagnosticProcessor, MethodInfo method)
        {
            _diagnosticProcessor = diagnosticProcessor;
            _reflector = method.GetReflector();
            _parameterResolvers = GetParameterResolvers(method).ToArray();
        }

        public void Invoke(object value)
        {
            var args = new object[_parameterResolvers.Length];
            for (var i = 0; i < _parameterResolvers.Length; i++)
            {
                args[i] = _parameterResolvers[i].Resolve(value);
            }

            _reflector.Invoke(_diagnosticProcessor, args);
        }

        private static IEnumerable<IParameterResolver> GetParameterResolvers(MethodInfo methodInfo)
        {
            foreach (var parameter in methodInfo.GetParameters())
            {
                var binder = parameter.GetCustomAttribute<ParameterResolverAttribute>();
                if (binder != null)
                {
                    if (binder is ObjectAttribute objectBinder)
                    {
                        if (objectBinder.TargetType == null)
                        {
                            objectBinder.TargetType = parameter.ParameterType;
                        }
                    }
                    if (binder is PropertyAttribute propertyBinder)
                    {
                        if (propertyBinder.Name == null)
                        {
                            propertyBinder.Name = parameter.Name;
                        }
                    }
                    yield return binder;
                }
                else
                {
                    yield return new NullParameterResolver();
                }
            }
        }
    }
    public class DiagnosticEventCollection
    {
        private readonly Dictionary<string, DiagnosticEvent> _eventDict = new Dictionary<string, DiagnosticEvent>();

        public DiagnosticEventCollection(IDiagnosticProcessor diagnosticProcessor)
        {
            foreach (var method in diagnosticProcessor.GetType().GetMethods())
            {
                var diagnosticName = method.GetCustomAttribute<DiagnosticNameAttribute>();
                if (diagnosticName == null)
                    continue;
                _eventDict.Add(diagnosticName.Name, new DiagnosticEvent(diagnosticProcessor, method));
            }
        }

        public DiagnosticEvent GetDiagnosticEvent(string name)
        {
            if (_eventDict.ContainsKey(name))
            {
                return _eventDict[name];
            }
            return null;
        }
    }
    /// <summary>
    /// 根据触发的事件名（value.Key）和已订阅的事件处理集合（_eventCollection）进行匹对查找，匹配上的通过反射执行对应的消费方法
    /// </summary>
    public class DiagnosticEventObserver : IObserver<KeyValuePair<string, object>>
    {
        private readonly DiagnosticEventCollection _eventCollection;

        public DiagnosticEventObserver(IDiagnosticProcessor diagnosticProcessor)
        {
            _eventCollection = new DiagnosticEventCollection(diagnosticProcessor);
        }

        public void OnCompleted()
        {
            // Method intentionally left empty.
        }

        public void OnError(Exception error)
        {
            // Method intentionally left empty.
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            var diagnosticEvent = _eventCollection.GetDiagnosticEvent(value.Key);
            if (diagnosticEvent == null) return;

            try
            {
                diagnosticEvent.Invoke(value.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
