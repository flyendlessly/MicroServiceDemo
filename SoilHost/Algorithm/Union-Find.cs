using System;
using System.Collections.Generic;
using System.Text;

namespace SoilHost.Algorithm
{
    public class Union_Find
    {
        int count; //连通分量数
        int[] id; //每个数所属的连通分量

        public Union_Find(int N)
        {
            count = N;
            id = new int[N];
            for (int i = 0; i < N; i++)
            {
                id[i] = i;
            }
        }

        //返回连通分量数
        public int getCount()
        {
            return this.count;
        }

        //查找x所属的连通分量
        public int find(int x)
        {
            while (x != id[x]) x = id[x];
            return x;
        }

        //将两个点进行合并
        public void union(int p, int q)
        {
            int pID = find(p);
            int qID = find(q);
            if (pID == qID) return;
            id[qID] = pID;
            count--;
        }

        //判断两个点是否连通
        public bool connected(int p, int q)
        {
            return find(p) == find(q);
        }

        public static void start()
        {
            Console.WriteLine("please input the count of total numbers");
            int.TryParse(Console.ReadLine(),out int N) ;
            Union_Find buf = new Union_Find(N);
            Console.WriteLine("please input pairs,input exit to end");
          
            while (true)
            {
                var next = Console.ReadLine();
                if (next == "exit")
                    break;
                var arr = next.Split("-");
                int.TryParse(arr[0],out int p);
                int.TryParse(arr[1], out int q);
                if (buf.connected(p, q)) continue;
                buf.union(p, q);
            }
            Console.WriteLine("总的连通分量数是:" + buf.getCount());
            Console.ReadKey();
        }
    }
}
