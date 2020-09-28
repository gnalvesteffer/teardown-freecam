using Squalr.Engine.Memory;

namespace TeardownCameraHack
{
    public class TeardownProjectile
    {
        public static readonly int StructureSize = 0x24;

        private readonly ulong _address;

        public float PositionX
        {
            get => Reader.Default.Read<float>(_address + 0x00, out _);
            set => Writer.Default.Write(_address + 0x00, value);
        }

        public float PositionY
        {
            get => Reader.Default.Read<float>(_address + 0x04, out _);
            set => Writer.Default.Write(_address + 0x04, value);
        }

        public float PositionZ
        {
            get => Reader.Default.Read<float>(_address + 0x08, out _);
            set => Writer.Default.Write(_address + 0x08, value);
        }

        public float RotationX
        {
            get => Reader.Default.Read<float>(_address + 0x0C, out _);
            set => Writer.Default.Write(_address + 0x0C, value);
        }

        public float RotationY
        {
            get => Reader.Default.Read<float>(_address + 0x14, out _);
            set => Writer.Default.Write(_address + 0x14, value);
        }

        public float RotationZ
        {
            get => Reader.Default.Read<float>(_address + 0x18, out _);
            set => Writer.Default.Write(_address + 0x18, value);
        }

        public TeardownProjectile(ulong address)
        {
            _address = address;
        }
    }
}
