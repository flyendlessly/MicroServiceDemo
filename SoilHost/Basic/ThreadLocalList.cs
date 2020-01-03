using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace SoilHost.Basic
{
    public class ConcurrBagDemo<T>
    {
        [Serializable]
        // 构造了双向链表的节点
        internal class Node
        {
            public Node(T value)
            {
                m_value = value;
            }
            public readonly T m_value;
            public Node m_next;
            public Node m_prev;
        }

        /// <summary>
        /// 集合操作类型
        /// </summary>
        internal enum ListOperation
        {
            None,
            Add,
            Take
        };

        /// <summary>
        /// 线程锁定的类
        /// </summary>
        internal class ThreadLocalList
        {
            // 双向链表的头结点 如果为null那么表示链表为空
            internal volatile Node m_head;

            // 双向链表的尾节点
            private volatile Node m_tail;

            // 定义当前对List进行操作的种类 
            // 与前面的 ListOperation 相对应
            internal volatile int m_currentOp;

            // 这个列表元素的计数
            private int m_count;

            // The stealing count
            // 这个不是特别理解 好像是在本地列表中 删除某个Node 以后的计数
            internal int m_stealCount;

            // 下一个列表 可能会在其它线程中
            internal volatile ThreadLocalList m_nextList;

            // 设定锁定是否已进行
            internal bool m_lockTaken;

            // The owner thread for this list
            internal Thread m_ownerThread;

            // 列表的版本，只有当列表从空变为非空统计是底层
            internal volatile int m_version;

            /// <summary>
            /// ThreadLocalList 构造器
            /// </summary>
            /// <param name="ownerThread">拥有这个集合的线程</param>
            internal ThreadLocalList(Thread ownerThread)
            {
                m_ownerThread = ownerThread;
            }
            /// <summary>
            /// 添加一个新的item到链表首部
            /// </summary>
            /// <param name="item">The item to add.</param>
            /// <param name="updateCount">是否更新计数.</param>
            internal void Add(T item, bool updateCount)
            {
                checked
                {
                    m_count++;
                }
                Node node = new Node(item);
                if (m_head == null)
                {
                    Debug.Assert(m_tail == null);
                    m_head = node;
                    m_tail = node;
                    m_version++; // 因为进行初始化了，所以将空状态改为非空状态
                }
                else
                {
                    // 使用头插法 将新的元素插入链表
                    node.m_next = m_head;
                    m_head.m_prev = node;
                    m_head = node;
                }
                if (updateCount) // 更新计数以避免此添加同步时溢出
                {
                    m_count = m_count - m_stealCount;
                    m_stealCount = 0;
                }
            }

            /// <summary>
            /// 从列表的头部删除一个item
            /// </summary>
            /// <param name="result">The removed item</param>
            internal void Remove(out T result)
            {
                // 双向链表删除头结点数据的流程
                Debug.Assert(m_head != null);
                Node head = m_head;
                m_head = m_head.m_next;
                if (m_head != null)
                {
                    m_head.m_prev = null;
                }
                else
                {
                    m_tail = null;
                }
                m_count--;
                result = head.m_value;

            }

            /// <summary>
            /// 返回列表头部的元素
            /// </summary>
            /// <param name="result">the peeked item</param>
            /// <returns>True if succeeded, false otherwise</returns>
            internal bool Peek(out T result)
            {
                Node head = m_head;
                if (head != null)
                {
                    result = head.m_value;
                    return true;
                }
                result = default(T);
                return false;
            }

            /// <summary>
            /// 从列表的尾部获取一个item
            /// </summary>
            /// <param name="result">the removed item</param>
            /// <param name="remove">remove or peek flag</param>
            internal void Steal(out T result, bool remove)
            {
                Node tail = m_tail;
                Debug.Assert(tail != null);
                if (remove) // Take operation
                {
                    m_tail = m_tail.m_prev;
                    if (m_tail != null)
                    {
                        m_tail.m_next = null;
                    }
                    else
                    {
                        m_head = null;
                    }
                    // Increment the steal count
                    m_stealCount++;
                }
                result = tail.m_value;
            }


            /// <summary>
            /// 获取总计列表计数, 它不是线程安全的, 如果同时调用它, 则可能提供不正确的计数
            /// </summary>
            internal int Count
            {
                get
                {
                    return m_count - m_stealCount;
                }
            }
        }
    }

}
