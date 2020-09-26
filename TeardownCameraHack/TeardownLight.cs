using Squalr.Engine.Memory;

namespace TeardownCameraHack
{
    public class TeardownLight
    {
        private readonly ulong _pointer;

        public bool IsEnabled
        {
            // performing these truthy checks with the byte value because setting the type to `bool` results in an exception
            get => Reader.Default.Read<byte>(_pointer + 0x28, out _) != 0;
            set => Writer.Default.Write(_pointer + 0x28, value ? 1 : 0);
        }

        public float Red
        {
            get => Reader.Default.Read<float>(_pointer + 0x4C, out _);
            set => Writer.Default.Write(_pointer + 0x4C, value);
        }

        public float Green
        {
            get => Reader.Default.Read<float>(_pointer + 0x50, out _);
            set => Writer.Default.Write(_pointer + 0x50, value);
        }

        public float Blue
        {
            get => Reader.Default.Read<float>(_pointer + 0x54, out _);
            set => Writer.Default.Write(_pointer + 0x54, value);
        }

        public TeardownLight(ulong pointer)
        {
            _pointer = pointer;
        }
    }
}
