using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
namespace SoilHost.Emit
{
    public class EmitDemo
    {
        public static void Demo()
        {
            #region 1.构建程序集
            //代表程序集的名称
            var asmName = new AssemblyName("Emitdemo");

            //.net framework
            //var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
            //.net core
            //Run 可以执行但无法保存该动态程序集。
            //RunAndCollect 当动态程序集不再可供访问时，将自动卸载该程序集，并回收其内存。
            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);
            #endregion

            #region 2.创建模块
            var mdlBldr = asmBuilder.DefineDynamicModule("Main");
            #endregion

            #region 3.定义类
            //DefineType还可以设置要定义的类的基类，要实现的接口等等。
            var typeBldr = mdlBldr.DefineType("Hello", TypeAttributes.Public);
            #endregion

            #region 4.定义类成员（方法，属性等等）
            //该方法的原型为public void SayHello();
            var methodBldr = typeBldr.DefineMethod(
                                "SayHello",
                                MethodAttributes.Public,
                                null,//return type
                                null);//parameter type );
            #endregion

            #region 5.实现签名方法
            //方法签名已经生成好了，但方法还缺少实现。在生成方法的实现前，必须提及一个很重要的概念：evaluation stack。
            //在.Net下基本所有的操作都是通过入栈出栈完成的。
            //这个栈就是evaluation stack。比如要计算两个数(a,b)的和，首先要将a放入evaluation stack中，
            //再将b也放入栈中，最后执行加法时将弹出栈顶的两个元素也就是a和b，相加再将结果推送至栈顶。
            Console.WriteLine("Hello,World 1");//可以用Emit这样生成：
            var il = methodBldr.GetILGenerator();//获取il生成器
            il.Emit(OpCodes.Ldstr, "Hello, World 2");
            il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
            il.Emit(OpCodes.Ret);
            #endregion

            //一个类型好像就已经完成了。事实上却还没有，最后我们还必须显示的调用CreateType来完成类型的创建。
            typeBldr.CreateType();

            //这样一个完整的类就算完成了。但为了能用reflector查看我们创建的动态程序集，我们选择将这个程序集保存下来。
            //asmBuilder.Save("Main.dll");
        }


    }
}
