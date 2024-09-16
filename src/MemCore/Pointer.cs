using System;

namespace MemCore
{
    public unsafe class MemPointer
    {
        private MemHandler memHandler;
        public IntPtr BaseAddress { get => (IntPtr)_baseAddress; }
        public IntPtr Address { get => (IntPtr)_address;}

        private readonly int* _baseAddress;
        private int _address;
        private int[] offsets;
        public bool IsNullPointer => _address == 0;

        public MemPointer(MemHandler memHandler, IntPtr baseAddress) : this(memHandler, (int*)baseAddress.ToPointer()) { }
        public MemPointer(MemHandler memHandler, IntPtr baseAddress, params int[] offsets) : this(memHandler, (int*)baseAddress.ToPointer(), offsets) { }

        public MemPointer(MemHandler memHandler, int* baseAddress)
        {
            this._address = 0;
            this.memHandler = memHandler;
            this._baseAddress = baseAddress;
            this.offsets = null;
            UpdatePointers();
        }

        public MemPointer(MemHandler memHandler, int* baseAddress, params int[] offsets)
        {
            this._address = 0;
            this.memHandler = memHandler;
            this._baseAddress = baseAddress;
            this.offsets = offsets;
            UpdatePointers();
        }

        public unsafe void UpdatePointers()
        {
            fixed (int* p = &_address)
                memHandler.TryGetIntAt(_baseAddress, p);
            
            if (_address == 0)
                return;

            if (offsets != null)
            {
                foreach (int offset in offsets)
                {
                    fixed (int* p = &_address)
                        memHandler.TryGetIntAt((int*)(_address + offset), p);
                    
                    if (_address == 0)
                        return;
                }
            }
        }
    }

    public bool TryDeref<T>(int offset, ref T result) where T : unmanaged
        => !IsNullPointer && this.memHandler.TryGetAt(IntPtr.Add(Address, offset), ref result);
    public bool TryDeref<T>(int offset, T* result) where T : unmanaged
        => !IsNullPointer && result != (T*)0 && this.memHandler.TryGetAt<T>((int*)(_address + offset), result);

    public bool TryDerefSByte(int offset, ref sbyte result)
        => !IsNullPointer && this.memHandler.TryGetSByteAt(IntPtr.Add(Address, offset), ref result);
    public bool TryDerefSByte(int offset, sbyte* result)
        => !IsNullPointer && result != (sbyte*)0 && this.memHandler.TryGetSByteAt((int*)(_address + offset), result);

    public bool TryDerefByte(int offset, ref byte result)
        => !IsNullPointer && this.memHandler.TryGetByteAt(IntPtr.Add(Address, offset), ref result);
    public bool TryDerefByte(int offset, byte* result)
        => !IsNullPointer && result != (byte*)0 && this.memHandler.TryGetByteAt((int*)(_address + offset), ref result);

    public bool TryDerefByteArray(int offset, int size, IntPtr result)
        => !IsNullPointer && result != IntPtr.Zero && this.memHandler.TryGetByteArrayAt(IntPtr.Add(Address, offset), size, result);
    public bool TryDerefByteArray(int offset, int size, byte* result)
        => !IsNullPointer && result != (byte*)0 && this.memHandler.TryGetByteArrayAt((int*)(_address + offset), size, result);

    public bool TryDerefShort(int offset, ref short result)
        => !IsNullPointer && this.memHandler.TryGetShortAt(IntPtr.Add(Address, offset), ref result);
    public bool TryDerefShort(int offset, short* result)
        => !IsNullPointer && result != (short*)0 && this.memHandler.TryGetShortAt((int*)(_address + offset), result);

    public bool TryDerefUShort(int offset, ref ushort result)
        => !IsNullPointer && this.memHandler.TryGetUShortAt(IntPtr.Add(Address, offset), ref result);
    public bool TryDerefUShort(int offset, ushort* result)
        => !IsNullPointer && result != (ushort*)0 && this.memHandler.TryGetUShortAt((int*)(_address + offset), result);

    public bool TryDerefInt(int offset, ref int result)
        => !IsNullPointer && this.memHandler.TryGetIntAt(IntPtr.Add(Address, offset), ref result);
    public bool TryDerefInt(int offset, int* result)
        => !IsNullPointer && result != (int*)0 && this.memHandler.TryGetIntAt((int*)(_address + offset), result);

    public bool TryDerefUInt(int offset, ref uint result)
        => !IsNullPointer && this.memHandler.TryGetUIntAt(IntPtr.Add(Address, offset), ref result);
    public bool TryDerefUInt(int offset, uint* result)
        => !IsNullPointer && result != (uint*)0 && this.memHandler.TryGetUIntAt((int*)(_address + offset), result);

    public bool TryDerefLong(int offset, ref long result)
        => !IsNullPointer && this.memHandler.TryGetLongAt(IntPtr.Add(Address, offset), ref result);
    public bool TryDerefLong(int offset, long* result)
        => !IsNullPointer && result != (long*)0 && this.memHandler.TryGetLongAt((int*)(_address + offset), result);

    public bool TryDerefULong(int offset, ref ulong result)
        => !IsNullPointer && this.memHandler.TryGetULongAt(IntPtr.Add(Address, offset), ref result);
    public bool TryDerefULong(int offset, ulong* result)
        => !IsNullPointer && result != (ulong*)0 && this.memHandler.TryGetULongAt((int*)(_address + offset), result);

    public bool TryDerefFloat(int offset, ref float result)
        => !IsNullPointer && this.memHandler.TryGetFloatAt(IntPtr.Add(Address, offset), ref result);
    public bool TryDerefFloat(int offset, float* result)
        => !IsNullPointer && result != (float*)0 && this.memHandler.TryGetFloatAt((int*)(_address + offset), result);

    public bool TryDerefDouble(int offset, ref double result)
        => !IsNullPointer && this.memHandler.TryGetDoubleAt(IntPtr.Add(Address, offset), ref result);
    public bool TryDerefDouble(int offset, double* result)
        => !IsNullPointer && result != (double*)0 && this.memHandler.TryGetDoubleAt((int*)(_address + offset), result);

    public bool TryDerefUnicodeString(int offset, int size, ref string result)
        => !IsNullPointer && this.memHandler.TryGetUnicodeStringAt(IntPtr.Add(Address, offset), size, ref result);
    public bool TryDerefUnicodeString(int offset, int size, string* result)
        => !IsNullPointer && result != (string*)0 && this.memHandler.TryGetUnicodeStringAt((int*)(_address + offset), size, result);

    public bool TryDerefASCIIString(int offset, int size, ref string result)
        => !IsNullPointer && this.memHandler.TryGetASCIIStringAt(IntPtr.Add(Address, offset), size, ref result);
    public bool TryDerefASCIIString(int offset, int size, string* result)
        => !IsNullPointer && result != (string*)0 && this.memHandler.TryGetASCIIStringAt((int*)(_address + offset), size, result);
}