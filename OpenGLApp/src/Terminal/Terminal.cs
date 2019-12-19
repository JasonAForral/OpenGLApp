using System;
using System.Collections;
using static System.Console;

namespace OpenGLApp.src.Terminal
{
  public class Terminal
  {
    readonly Stack breadcrumbs;

    public Terminal()
    {
      WriteLine("Authorized Access Only!");
      breadcrumbs = new Stack();
      breadcrumbs.Push("Terminal");
    }

    public void Start()
    {
      PrintBreadcrumbs();
      WriteLine();
      breadcrumbs.Push("Provider");
      PrintBreadcrumbs();
      WriteLine();
      breadcrumbs.Push("CheckedIn");
      PrintBreadcrumbs();
      WriteLine();
      breadcrumbs.Pop();
      PrintBreadcrumbs();
      WriteLine();
      breadcrumbs.Pop();
      PrintBreadcrumbs();
      WriteLine();

    }

    public void PrintBreadcrumbs()
    {
      if (breadcrumbs.Count == 0)
      {
        Write("$ ");
        return;
      }
      Object[] crumbs = breadcrumbs.ToArray();
      for (int i = breadcrumbs.Count - 1; i >= 0; --i)
      {
        Write(crumbs[i] + " > ");
      }
    }
  }
}
