// ${COMPLETE_ITEM:Tan}
// ${SHOW_SIGNATURES:false}
// ${CASE_SENSITIVE_COMPLETION:false}
// ${COMPLETION_TYPE:Replace}

using System;
using System.Reflection;
using System.Linq;

namespace N
{
class Test<T>
{
public void M(T t)
{
}

public void M(T t, T t1)
{
}

}

  class C
  {
    static void Main(string[] args)
    {
	var t = typeof(Test<>).MakeGenericType(typeof(int));
      t.GetMethod({caret}
    }
  }
}
