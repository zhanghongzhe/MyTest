using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTest.Collection
{
    public class EntityCollection
    {
        //利用GetEnumerator方法加上yield关键字，构建迭代器实现循环
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < 10; i++)
            {
                yield return i;
            }
        }
    }
}
