namespace N
{
  class A
  {
	private static string M;
  }
  
  class C
  {
    static void Main(string[] args)
    {
      A.M{caret} = string.Empty;
    }
  }
}
