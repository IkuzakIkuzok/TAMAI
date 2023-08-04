
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Win;

internal abstract class DisposableObjectBase : IDisposable
{
    private bool _disposed;

    public void Dispose()
        => Dispose(true);

    public void Dispose(bool disposing)
    {
        if (this._disposed) return;
        if (!disposing) return;

        Terminate();
        this._disposed = true;
    } // public void Dispose (bool)

    protected abstract void Terminate();
} // internal abstract class DisposableObjectBase : IDisposable
