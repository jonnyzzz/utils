using System;

namespace p4clientsCleaner
{
  public class Disposable : IDisposable
  {
    public delegate void Disposer();

    private readonly Disposer myDisposer;

    public Disposable(Disposer disposer)
    {
      myDisposer = disposer;
    }

    public void Dispose()
    {
      myDisposer();
    }
  }
}