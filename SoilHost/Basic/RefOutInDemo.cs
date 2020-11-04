using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoilHost.Basic
{
    public struct NormalStruct
    {
        public decimal Number1 { get; set; }
        public decimal Number2 { get; set; }
        //...
        public decimal Number30 { get; set; }
    }

    public readonly struct ReadOnlyStruct
    {
        //public readonly decimal Number1 { get; }
        public decimal Number1 { get; }
        public decimal Number2 { get; }
        //...
        public decimal Number30 { get; }
    }

    public class BenchmarkClass
    {
        const int loops = 50000000;
        NormalStruct normalInstance = new NormalStruct();
        ReadOnlyStruct readOnlyInstance = new ReadOnlyStruct();

        [Benchmark(Baseline = true)]
        public decimal DoNormalLoop()
        {
            decimal result = 0M;
            for (int i = 0; i < loops; i++)
            {
                result = Compute(normalInstance);
            }
            return result;
        }

        [Benchmark]
        public decimal DoNormalLoopByIn()
        {
            decimal result = 0M;
            for (int i = 0; i < loops; i++)
            {
                result = ComputeIn(in normalInstance);
            }
            return result;
        }

        [Benchmark]
        public decimal DoReadOnlyLoopByIn()
        {
            decimal result = 0M;
            for (int i = 0; i < loops; i++)
            {
                result = ComputeIn(in readOnlyInstance);
            }
            return result;
        }

        public decimal Compute(NormalStruct s)
        {
            //业务逻辑
            return 0M;
        }

        public decimal ComputeIn(in NormalStruct s)
        {
            //业务逻辑
            return 0M;
        }

        public decimal ComputeIn(in ReadOnlyStruct s)
        {
            //业务逻辑
            return 0M;
        }
    }


    struct MyNormalStruct
    {
        public int Value { get; set; }

        public void UpdateValue(int value)
        {
            Value = value;
        }
    }

    class RefOutInDemo
    {
        static void UpdateMyNormalStruct(MyNormalStruct myStruct)
        {
            myStruct.UpdateValue(8);
        }

        public static void start()
        {
            MyNormalStruct myStruct = new MyNormalStruct();
            myStruct.UpdateValue(2);
            UpdateMyNormalStruct(myStruct);
            Console.WriteLine(myStruct.Value);
        }
    }
}
