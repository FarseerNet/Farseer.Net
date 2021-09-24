using System;
using System.Buffers;

namespace FS.Log
{
    internal sealed class ArrayBufferWriter<T> : IBufferWriter<T>
    {
        private const int DefaultInitialBufferSize = 256;
        private       T[] _buffer;

        public ArrayBufferWriter()
        {
            _buffer      = Array.Empty<T>();
            WrittenCount = 0;
        }

        public ArrayBufferWriter(int initialCapacity)
        {
            _buffer      = initialCapacity > 0 ? new T[initialCapacity] : throw new ArgumentException(message: null, paramName: nameof(initialCapacity));
            WrittenCount = 0;
        }

        public ReadOnlyMemory<T> WrittenMemory => _buffer.AsMemory(start: 0, length: WrittenCount);

        public ReadOnlySpan<T> WrittenSpan => _buffer.AsSpan(start: 0, length: WrittenCount);

        public int WrittenCount { get; private set; }

        public int Capacity => _buffer.Length;

        public int FreeCapacity => _buffer.Length - WrittenCount;

        public void Advance(int count)
        {
            if (count        < 0) throw new ArgumentException(message: null, paramName: nameof(count));
            if (WrittenCount > _buffer.Length - count) throw new InvalidOperationException(message: _buffer.Length.ToString());
            WrittenCount += count;
        }

        public Memory<T> GetMemory(int sizeHint = 0)
        {
            CheckAndResizeBuffer(sizeHint: sizeHint);
            return _buffer.AsMemory(start: WrittenCount);
        }

        public Span<T> GetSpan(int sizeHint = 0)
        {
            CheckAndResizeBuffer(sizeHint: sizeHint);
            return _buffer.AsSpan(start: WrittenCount);
        }

        public void Clear()
        {
            _buffer.AsSpan(start: 0, length: WrittenCount).Clear();
            WrittenCount = 0;
        }

        private void CheckAndResizeBuffer(int sizeHint)
        {
            if (sizeHint < 0) throw new ArgumentException(message: nameof(sizeHint));
            if (sizeHint == 0) sizeHint = 1;
            if (sizeHint <= FreeCapacity) return;
            var length            = _buffer.Length;
            var val1              = Math.Max(val1: sizeHint, val2: length);
            if (length == 0) val1 = Math.Max(val1: val1, val2: 256);
            var newSize           = length + val1;
            if ((uint)newSize > int.MaxValue)
            {
                newSize = length + sizeHint;
                if ((uint)newSize > int.MaxValue) throw new OutOfMemoryException(message: newSize.ToString());
            }

            Array.Resize(array: ref _buffer, newSize: newSize);
        }
    }
}