using Squalr.Engine.Memory;

namespace TeardownCameraHack.Teardown.Models
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

        public float RocketSpeed
        {
            get => Reader.Default.Read<float>(_teardownBaseAddress + 0x2EDF98, out _);
            set => Writer.Default.Write(_teardownBaseAddress + 0x2EDF98, value);
        }

        public float BulletDespawnDistance
        {
            get => Reader.Default.Read<float>(_teardownBaseAddress + 0x2F2BAC, out _);
            set => Writer.Default.Write(_teardownBaseAddress + 0x2F2BAC, value);
        }

        public TeardownProjectileType BulletType
        {
            get => (TeardownProjectileType)Reader.Default.Read<byte>(_teardownBaseAddress + 0x1F2A24, out _);
            set => Writer.Default.Write(_teardownBaseAddress + 0x1F2A24, (byte)value);
        }

        public TeardownSettings(ulong teardownBaseAddress)
        {
            _teardownBaseAddress = teardownBaseAddress;
        }
    }
}
