// ${COMPLETE_ITEM:Tan}
// ${SHOW_SIGNATURES:false}
// ${CASE_SENSITIVE_COMPLETION:false}
// ${COMPLETION_TYPE:Replace}

using System;
using System.Reflection;
using System.Linq;

namespace N
{
  class C
  {
    static void Main(string[] args)
    {
	var t = typeof(Math);
      t.GetMethod({caret}
    }
  }
}
