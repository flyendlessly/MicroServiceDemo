using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Class1.Interface
{
    public interface IHello : IGrainWithIntegerKey
    {
        Task<string> SayHello(string hellostr);

        Task<string> DelayedMsg(string hellomsg);
    }
}
