using System;
using System.Reflection;

namespace N
{
public class T
{
private string A1;
}

  class C
  {
public static Type _T
{
get; private set;
}

static C()
{
_T = typeof(T);
}

    static void Main(string[] args)
    {
      var property = _T.GetField("A");
    }
  }
}
