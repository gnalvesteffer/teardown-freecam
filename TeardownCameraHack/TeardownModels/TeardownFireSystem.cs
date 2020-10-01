using Squalr.Engine.Memory;

namespace TeardownCameraHack.TeardownModels
{
    public class TeardownFireSystem
    {
        private readonly ulong _address;

        public int TotalFires
        {
            get => Reader.Default.Read<int>(_address + 0x08, out _);
            set => Writer.Default.Write(_address + 0x08, value);
        }

        public int FireArraySize
        {
            get => Reader.Default.Read<int>(_address + 0x0C, out _);
            set => Writer.Default.Write(_address + 0x0C, value);
        }

        public TeardownFireSystem(ulong address)
        {
            _address = address;
        }
    }
}
