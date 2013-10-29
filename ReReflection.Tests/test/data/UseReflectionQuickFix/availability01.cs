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
      string s = A.M{caret};
    }
  }
}
