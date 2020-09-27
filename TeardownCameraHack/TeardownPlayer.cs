using Squalr.Engine.Memory;

namespace TeardownCameraHack
{
    public class TeardownPlayer
    {
        private readonly ulong _address;

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

        public TeardownPlayer(ulong address)
        {
            _address = address;
        }
    }
}
