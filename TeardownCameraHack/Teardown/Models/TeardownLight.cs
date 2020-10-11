using Squalr.Engine.Memory;

namespace TeardownCameraHack.Teardown.Models
{
    public class TeardownLight
    {
        private readonly ulong _address;

        public bool IsEnabled
        {
            // performing these truthy checks with the byte value because setting the type to `bool` results in an exception
            get => Reader.Default.Read<byte>(_address + 0x28, out _) != 0;
            set => Writer.Default.Write(_address + 0x28, value ? 1 : 0);
        }

        public float PositionX
        {
            get => Reader.Default.Read<float>(_address + 0x30, out _);
            set => Writer.Default.Write(_address + 0x30, value);
        }

        public float PositionY
        {
            get => Reader.Default.Read<float>(_address + 0x34, out _);
            set => Writer.Default.Write(_address + 0x34, value);
        }

        public float PositionZ
        {
            get => Reader.Default.Read<float>(_address + 0x38, out _);
            set => Writer.Default.Write(_address + 0x38, value);
        }

        public float Rotation1
        {
            get => Reader.Default.Read<float>(_address + 0x3C, out _);
            set => Writer.Default.Write(_address + 0x3C, value);
        }

        public float Rotation2
        {
            get => Reader.Default.Read<float>(_address + 0x40, out _);
            set => Writer.Default.Write(_address + 0x40, value);
        }

        public float Rotation3
        {
            get => Reader.Default.Read<float>(_address + 0x44, out _);
            set => Writer.Default.Write(_address + 0x44, value);
        }

        public float Rotation4
        {
            get => Reader.Default.Read<float>(_address + 0x48, out _);
            set => Writer.Default.Write(_address + 0x44, value);
        }

        public float Red
        {
            get => Reader.Default.Read<float>(_address + 0x4C, out _);
            set => Writer.Default.Write(_address + 0x4C, value);
        }

        public float Green
        {
            get => Reader.Default.Read<float>(_address + 0x50, out _);
            set => Writer.Default.Write(_address + 0x50, value);
        }

        public float Blue
        {
            get => Reader.Default.Read<float>(_address + 0x54, out _);
            set => Writer.Default.Write(_address + 0x54, value);
        }

        public TeardownLight(ulong address)
        {
            _address = address;
        }
    }
}
