using Squalr.Engine.Memory;

namespace TeardownCameraHack
{
    public class TeardownVehicle
    {
        private readonly ulong _address;

        public string Name
        {
            get => Reader.Default.Read<string>(_address + 0xF8, out _);
            set => Writer.Default.Write(_address + 0xF8, value);
        }

        public TeardownVehicle(ulong address)
        {
            _address = address;
        }
    }
}
