using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTest.Collection
{
    public class CollectionDemo
    {
        public void Test1()
        {
            var entityCollection = new EntityCollection();
            foreach (int entity in entityCollection)
            {
                Console.WriteLine(entity);
            }
        }
    }
}
