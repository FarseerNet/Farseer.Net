using System;
using System.Buffers;

namespace FS.Log
{
  internal sealed class ArrayBufferWriter<T> : IBufferWriter<T>
  {
    private T[] _buffer;
    private int _index;
    private const int DefaultInitialBufferSize = 256;

    public ArrayBufferWriter()
    {
      this._buffer = Array.Empty<T>();
      this._index = 0;
    }

    public ArrayBufferWriter(int initialCapacity)
    {
      this._buffer = initialCapacity > 0 ? new T[initialCapacity] : throw new ArgumentException((string) null, nameof (initialCapacity));
      this._index = 0;
    }

    public ReadOnlyMemory<T> WrittenMemory => (ReadOnlyMemory<T>) this._buffer.AsMemory<T>(0, this._index);

    public ReadOnlySpan<T> WrittenSpan => (ReadOnlySpan<T>) this._buffer.AsSpan<T>(0, this._index);

    public int WrittenCount => this._index;

    public int Capacity => this._buffer.Length;

    public int FreeCapacity => this._buffer.Length - this._index;

    public void Clear()
    {
      this._buffer.AsSpan<T>(0, this._index).Clear();
      this._index = 0;
    }

    public void Advance(int count)
    {
      if (count < 0)
        throw new ArgumentException((string) null, nameof (count));
      if (this._index > this._buffer.Length - count)
        throw new InvalidOperationException(this._buffer.Length.ToString());
      this._index += count;
    }

    public Memory<T> GetMemory(int sizeHint = 0)
    {
      this.CheckAndResizeBuffer(sizeHint);
      return this._buffer.AsMemory<T>(this._index);
    }

    public Span<T> GetSpan(int sizeHint = 0)
    {
      this.CheckAndResizeBuffer(sizeHint);
      return this._buffer.AsSpan<T>(this._index);
    }

    private void CheckAndResizeBuffer(int sizeHint)
    {
      if (sizeHint < 0)
        throw new ArgumentException(nameof (sizeHint));
      if (sizeHint == 0)
        sizeHint = 1;
      if (sizeHint <= this.FreeCapacity)
        return;
      int length = this._buffer.Length;
      int val1 = Math.Max(sizeHint, length);
      if (length == 0)
        val1 = Math.Max(val1, 256);
      int newSize = length + val1;
      if ((uint) newSize > (uint) int.MaxValue)
      {
        newSize = length + sizeHint;
        if ((uint) newSize > (uint) int.MaxValue)
          throw new OutOfMemoryException(newSize.ToString());
      }
      Array.Resize<T>(ref this._buffer, newSize);
    }
  }
}
