namespace TeardownCameraHack
{
    public class TeardownWorld
    {
        private readonly ulong _worldBaseAddress;
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

        public TeardownWorld(ulong teardownBaseAddress)
        {
            _worldBaseAddress = teardownBaseAddress + 0x3E8B60;
            _playerPointer = _worldBaseAddress + 0xE8;
        }
    }
}
