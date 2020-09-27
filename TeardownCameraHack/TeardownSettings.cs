using Squalr.Engine.Memory;

namespace TeardownCameraHack
{
    public class TeardownSettings
    {
        private readonly ulong _teardownBaseAddress;

        public float FireSize
        {
            get => Reader.Default.Read<float>(_teardownBaseAddress + 0x2EE110, out _);
            set => Writer.Default.Write(_teardownBaseAddress + 0x2EE110, value);
        }

        public float BulletSpeed
        {
            get => Reader.Default.Read<float>(_teardownBaseAddress + 0x2EE638, out _);
            set => Writer.Default.Write(_teardownBaseAddress + 0x2EE638, value);
        }

        public TeardownSettings(ulong teardownBaseAddress)
        {
            _teardownBaseAddress = teardownBaseAddress;
        }
    }
}
