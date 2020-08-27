using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;

namespace SoilHost.AopDemo
{
    public class User
    {
        public string Name { set; get; }
        public string PassWord { set; get; }
    }

    #region 1、定义特性方便使用
    public class LogHandlerAttribute : HandlerAttribute
    {
        public string LogInfo { set; get; }
        public int Order { get; set; }
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new LogHandler() { Order = this.Order, LogInfo = this.LogInfo };
        }
    }
    #endregion

    #region 2、注册对需要的Handler拦截请求
    public class LogHandler : ICallHandler
    {
        public int Order { get; set; }
        public string LogInfo { set; get; }

        //这个方法就是拦截的方法，可以规定在执行方法之前和之后的拦截
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            Console.WriteLine("LogInfo内容" + LogInfo);
            //0.解析参数
            var arrInputs = input.Inputs;
            if (arrInputs.Count > 0)
            {
                var oUserTest1 = arrInputs[0] as User;
            }
            //1.执行方法之前的拦截
            Console.WriteLine("方法执行前拦截到了");
            //2.执行方法
            var messagereturn = getNext()(input, getNext);

            //3.执行方法之后的拦截
            Console.WriteLine("方法执行后拦截到了");
            return messagereturn;
        }
    }
    #endregion

    #region 3、用户定义接口和实现
    public interface IUserOperation
    {
        void Test(User oUser);
        void Test2(User oUser, User oUser2);
    }


    //这里必须要继承这个类MarshalByRefObject，否则报错
    public class UserOperation : MarshalByRefObject, IUserOperation
    {

        private static UserOperation oUserOpertion = null;
        public UserOperation()
        {
            //oUserOpertion = PolicyInjection.Create<UserOperation>();
        }

        //定义单例模式将PolicyInjection.Create<UserOperation>()产生的这个对象传出去，这样就避免了在调用处写这些东西
        public static UserOperation GetInstance()
        {
            //if (oUserOpertion == null)
            //    oUserOpertion = PolicyInjection.Create<UserOperation>();

            return oUserOpertion;
        }
        //调用属性也会拦截
        public string Name { set; get; }

        //[LogHandler]，在方法上面加这个特性，只对此方法拦截
        [LogHandler(LogInfo = "Test的日志为aaaaa")]
        public void Test(User oUser)
        {
            Console.WriteLine("Test方法执行了");
        }

        [LogHandler(LogInfo = "Test2的日志为bbbbb")]
        public void Test2(User oUser, User oUser2)
        {
            Console.WriteLine("Test2方法执行了");
        }
    }
    #endregion
}
