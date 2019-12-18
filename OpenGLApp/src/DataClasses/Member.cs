using System;

#if HAD
#else
namespace ConsoleApp1.src.DataClasses
{

  public class Member : User
  {
    public Member(params string[] list)
    {
      if (list.Length < 6) {
        throw new ArgumentException("Error: Not enough parameters");
      }
    }

    public void Display()
        {

        }
  }

}
#endif