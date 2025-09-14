﻿using OpenMir2.NativeList.Abstracts;
using OpenMir2.NativeList.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace OpenMir2.NativeList.Utils
{
    public sealed unsafe class NativeList<TItem> : NativeStructureBase, IList<TItem> where TItem : unmanaged
    {
        /// <summary>
        /// Just to have an ability to LINQ this thing.
        /// </summary>
        private class NativeListEnumerator : IEnumerator<TItem>
        {
            private NativeList<TItem> _buffer;
            private int _index;

            public NativeListEnumerator(NativeList<TItem> owner)
            {
                _buffer = owner;
                Reset();
            }

            public TItem Current => _buffer[_index];

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                _index++;

                return _index < _buffer.Count;
            }

            public void Reset()
            {
                _index = -1;
            }

            public void Dispose()
            {
                Reset();
                _buffer = null;
            }
        }

        private int _bufferLength;

        public NativeList(int length = 1)
        {
            ArgumentsGuard.ThrowIfLessOrEqualZero(length);

            _bufferLength = NearestPowerOfTwo(length);

            Initialize(_bufferLength * sizeof(TItem));
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException"/>
        public TItem this[int index]
        {
            get
            {
                ThrowIfDisposed();

                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return GetAt(index);
            }
            set
            {
                ThrowIfDisposed();

                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                SetAt(index, value);
            }
        }

        private int _count;

        /// <inheritdoc/>
        public int Count => _count;

        /// <inheritdoc/>
        /// <remarks>
        /// Always false.
        /// </remarks>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException"/>
        public void Add(TItem item)
        {
            ThrowIfDisposed();

            TryIncreaseBufferCapacity();

            SetAt(Count, item);

            Interlocked.Increment(ref _count);
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException"/>
        public void Insert(int index, TItem item)
        {
            ThrowIfDisposed();

            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            TryIncreaseBufferCapacity();

            for (int i = Count; i > index; i--)
            {
                SetAt(i, GetAt(i - 1));
            }

            SetAt(index, item);

            Interlocked.Increment(ref _count);
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException"/>
        public bool Contains(TItem item)
        {
            return IndexOf(item) != -1;
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException"/>
        public int IndexOf(TItem item)
        {
            ThrowIfDisposed();

            for (int i = 0; i < Count; i++)
            {
                if (GetAt(i).Equals(item))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException"/>
        public void RemoveAt(int index)
        {
            ThrowIfDisposed();

            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            for (int i = index; i < Count; i++)
            {
                SetAt(i, GetAt(i + 1));
            }

            Interlocked.Decrement(ref _count);
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException"/>
        public bool Remove(TItem item)
        {
            int index = IndexOf(item);

            if (index == -1)
            {
                return false;
            }

            RemoveAt(index);

            return true;
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException"/>
        public void Clear()
        {
            ThrowIfDisposed();

            for (int i = 0; i < Count; i++)
            {
                SetAt(i, default);
            }

            Interlocked.Exchange(ref _count, 0);
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException"/>
        public IEnumerator<TItem> GetEnumerator()
        {
            ThrowIfDisposed();

            return new NativeListEnumerator(this);
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException"/>
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(array);
            ArgumentsGuard.ThrowIfLessZero(arrayIndex);
            ArgumentsGuard.ThrowIfGreaterOrEqual(arrayIndex, array.Length);

            Span<TItem> fromSpan = new Span<TItem>(UnsafeHandle.ToPointer(), Count);
            Span<TItem> destinationSpan = new Span<TItem>(array, arrayIndex, array.Length - arrayIndex);

            fromSpan.CopyTo(destinationSpan);
        }

        /// <summary>
        /// Ensures that the buffer is too small to contain new elements 
        /// and increases its size, new size equals previous size times two.
        /// </summary>
        /// <returns>Returns true if buffer was increased.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryIncreaseBufferCapacity()
        {
            if (Count >= _bufferLength)
            {
                _bufferLength *= 2;

                ReInitialize(_bufferLength * sizeof(TItem));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Simple wrapper on non-easy readable operations.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TItem GetAt(int index)
        {
            return *(((TItem*)UnsafeHandle.ToPointer()) + index);
        }

        /// <summary>
        /// Simple wrapper on non-easy readable operations.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetAt(int index, TItem value)
        {
            *(((TItem*)UnsafeHandle.ToPointer()) + index) = value;
        }

        /// <summary>
        /// Returns the nearest number that is a power of two.
        /// </summary>
        private static int NearestPowerOfTwo(int size)
        {
            if (size <= 1)
            {
                return 1;
            }

            int pow = 0;

            while (size > 0)
            {
                size <<= 1;
                pow++;
            }

            pow--;

            return (int)Math.Pow(2, pow);
        }

        protected override void InternalDispose(bool manual)
        {
            Interlocked.Exchange(ref _count, 0);
            _bufferLength = 0;

            base.InternalDispose(manual);
        }
    }
}