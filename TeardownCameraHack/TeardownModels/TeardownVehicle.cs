using Squalr.Engine.Memory;

namespace TeardownCameraHack.TeardownModels
{
    public class TeardownVehicle
    {
        private readonly ulong _address;

        public string Name
        {
            get => Reader.Default.Read<string>(_address + 0xF8, out _);
            set => Writer.Default.Write(_address + 0xF8, value);
        }

        public bool IsPlayerControlled
        {
            get => Reader.Default.Read<byte>(_address + 0xDC, out _) > 0;
            set => Writer.Default.Write(_address + 0xDC, value ? 1 : 0);
        }

        public TeardownVehicle(ulong address)
        {
            _address = address;
        }
    }
}
