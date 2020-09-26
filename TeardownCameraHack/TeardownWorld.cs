namespace TeardownCameraHack
{
    public class TeardownWorld
    {
        private readonly ulong _pointer;
        private readonly ulong _playerPointer;

        private TeardownPlayer _player;
        public TeardownPlayer Player
        {
            get
            {
                if (_player != null)
                {
                    return _player;
                }
                return _player = new TeardownPlayer(_playerPointer);
            }
        }

        public TeardownWorld(ulong pointer)
        {
            _pointer = pointer;
            _playerPointer = _pointer + 0xE8;
        }
    }
}
