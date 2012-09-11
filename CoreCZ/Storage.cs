using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CoreCZ
{
    /// <summary>
    /// Simple 2D array which allows negative indexes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Storage<T>
    {

        public Storage(uint capacity)
        {
            mCapacity = 2*(int)capacity;
            mArray = new T[mCapacity*mCapacity];
        }

        public Storage(uint capacity, T defaultValue)
            : this(capacity)
        {
            Fill(defaultValue);
        }

        public T this[int i, int j]
        {
            get
            {
                return mArray[Convert(i) * mCapacity + Convert(j)];
            }
            set
            {
                mArray[Convert(i) * mCapacity + Convert(j)] = value;
            }
        }

        public void Fill(T value)
        {
            for (int i = 0; i < mCapacity; ++i)
            {
                for (int j = 0; j < mCapacity; ++j)
                {
                    mArray[i * mCapacity + j] = value;
                }
            }
        }

        private int Convert(int x)
        {
            return mCapacity/2 + x;
        }

        private T[] mArray;
        private int mCapacity;
    }
}
