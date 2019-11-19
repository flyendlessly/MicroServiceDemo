using System;
using System.Threading;

public delegate void DoSomething();
public delegate int DoSomethingReturn();
public delegate void DoMore(int age, string name);
public delegate int DoMoreReturn(int age, string name);

public class CommonDelegate
{
    public static void DoSomethingMethod()
    {
        Console.WriteLine("Sub-Start【ThreadId=" + Thread.CurrentThread.ManagedThreadId + "】：" + DateTime.Now);
        Thread.Sleep(3000);
        Console.WriteLine("Sub-End【ThreadId=" + Thread.CurrentThread.ManagedThreadId + "】：" + DateTime.Now);
    }

    public static void DoSomethingMethod2()
    {
        Console.WriteLine("Sub-Start2【ThreadId=" + Thread.CurrentThread.ManagedThreadId + "】：" + DateTime.Now);
        Thread.Sleep(1000);
        Console.WriteLine("Sub-End2【ThreadId=" + Thread.CurrentThread.ManagedThreadId + "】：" + DateTime.Now);
    }

    public static int DoSomethingReturnMethod()
    {
        Console.WriteLine("Sub-Start【ThreadId=" + Thread.CurrentThread.ManagedThreadId + "】：" + DateTime.Now);
        Thread.Sleep(3000);
        Console.WriteLine("Sub-End【ThreadId=" + Thread.CurrentThread.ManagedThreadId + "】：" + DateTime.Now);
        return 99;
    }

    public static void DoMoreMethod(int age, string name)
    {
        Console.WriteLine("Sub-Start【ThreadId=" + Thread.CurrentThread.ManagedThreadId + "】：" + DateTime.Now);
        Console.WriteLine("age={0},name={1}", age, name);
        Thread.Sleep(3000);
        Console.WriteLine("Sub-End【ThreadId=" + Thread.CurrentThread.ManagedThreadId + "】：" + DateTime.Now);
    }

    public static int DoMoreReturnMethod(int age, string name)
    {
        Console.WriteLine("Sub-Start【ThreadId=" + Thread.CurrentThread.ManagedThreadId + "】：" + DateTime.Now);
        Console.WriteLine("age={0},name={1}", age, name);
        Thread.Sleep(3000);
        Console.WriteLine("Sub-End【ThreadId=" + Thread.CurrentThread.ManagedThreadId + "】：" + DateTime.Now);
        return 99;
    }


}
