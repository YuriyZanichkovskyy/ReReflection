using System;

namespace N
{
  class C
  {
    static void Main(string[] args)
    {
	  typeof(object).MakeGenericType();
	  var a1 = typeof(Action<,>).MakeGenericType();
	  typeof(Action<,>).MakeGenericType(typeof(object), typeof(object), typeof(object));
    }
  }
}
