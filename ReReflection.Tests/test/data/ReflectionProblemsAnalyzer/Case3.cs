using System;
using System.Reflection;

namespace N
{

 public class TestClass
        {
            public string TestPublicField;
            private string TestPrivateField;

            public int this[int index]
            {
                get
                {
                    return 0;
                }
            }

            public int this[int index, int index2]
            {
                get
                {
                    return 0;
                }
            }
        }

  class C
  {
    static void Main(string[] args)
    {
		typeof(Math).GetMethod("Min");
		typeof (object).GetMethod("GetHashCode", BindingFlags.Instance | BindingFlags.Public);
		var f = typeof(TestClass).GetField("TestPublicField", BindingFlags.Instance | BindingFlags.Public);  

        f = typeof(TestClass).GetField("TestPrivateField", BindingFlags.Static | BindingFlags.NonPublic); 




        var p = typeof(TestClass).GetProperty("TestPrivateField", BindingFlags.Instance | BindingFlags.NonPublic);
        p = typeof(TestClass).GetProperty("Item", BindingFlags.Instance | BindingFlags.NonPublic);
    }
  }
}
