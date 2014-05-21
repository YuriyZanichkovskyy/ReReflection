namespace N
{
  class C
  {
    static void Main(string[] args)
    {
      var method = typeof(object).GetMethod("GetHash");
	  var property = typeof(object).GetProperty("Test");
	  var member = typeof(object).GetMember("_Test");
    }
  }
}
