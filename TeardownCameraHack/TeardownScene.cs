using Squalr.Engine.Memory;

namespace TeardownCameraHack
{
    public class TeardownScene
    {
        private readonly ulong _pointer;

        private TeardownLight _light;

        public TeardownLight Light
        {
            get
            {
                if (_light != null)
                {
                    return _light;
                }
                return _light = new TeardownLight(Reader.Default.Read<ulong>(_pointer + 0xE8, out _));
            }
        }

        public TeardownScene(ulong pointer)
        {
            _pointer = pointer;
        }
    }
}
