using System;
using Squalr.Engine.Memory;

namespace TeardownCameraHack.Teardown.Models
{
    public class TeardownGame
    {
        private readonly ulong _address;

        public int ScreenWidth
        {
            get => Reader.Default.Read<int>(_address + 0x0, out _);
            set => Writer.Default.Write(_address + 0x0, value);
        }

        public int ScreenHeight
        {
            get => Reader.Default.Read<int>(_address + 0x04, out _);
            set => Writer.Default.Write(_address + 0x04, value);
        }

        public TeardownGameState State
        {
            get => (TeardownGameState)Reader.Default.Read<int>(_address + 0x08, out _);
            set => Writer.Default.Write(_address + 0x08, (int)value);
        }

        public TeardownGameState NextState
        {
            get => (TeardownGameState)Reader.Default.Read<int>(_address + 0x0C, out _);
            set => Writer.Default.Write(_address + 0x0C, (int)value);
        }

        public bool IsFogEnabled
        {
            get => Reader.Default.Read<byte>(_address + 0x1C, out _) == 0;
            set => Writer.Default.Write(_address + 0x1C, value ? 0 : 1);
        }

        public TeardownScene Scene
        {
            get
            {
                var address = Reader.Default.Read<ulong>(_address + 0x40, out _);
                return new TeardownScene(address);
            }
        }

        public TeardownHud IngameHud
        {
            get
            {
                var address = Reader.Default.Read<ulong>(_address + 0x78, out _);
                return new TeardownHud(address);
            }
        }

        public TeardownScriptingEngine ScriptingEngine
        {
            get
            {
                var address = Reader.Default.Read<ulong>(_address + 0xA8, out _);
                return new TeardownScriptingEngine(address);
            }
        }

        public string LevelName
        {
            get => Reader.Default.Read<string>(_address + 0x108, out _);
            set => Writer.Default.Write(_address + 0x108, value);
        }

        public float SimulationTimeStep
        {
            get => Reader.Default.Read<float>(_address + 0x144, out _);
            set => Writer.Default.Write(_address + 0x144, value);
        }

        public float ActualTime
        {
            get => Reader.Default.Read<float>(_address + 0x14C, out _);
            set => Writer.Default.Write(_address + 0x14C, value);
        }

        public float SimulationTime
        {
            get => Reader.Default.Read<float>(_address + 0x150, out _);
            set => Writer.Default.Write(_address + 0x150, value);
        }

        public float DrawDistance
        {
            // valid range -1.0 through 1.0
            get => Reader.Default.Read<float>(_address + 0x1AC, out _);
            set => Writer.Default.Write(_address + 0x1AC, value);
        }

        public float CameraPositionX
        {
            get => Reader.Default.Read<float>(_address + 0x1E4, out _);
            set => Writer.Default.Write(_address + 0x1E4, value);
        }

        public float CameraPositionY
        {
            get => Reader.Default.Read<float>(_address + 0x1E8, out _);
            set => Writer.Default.Write(_address + 0x1E8, value);
        }

        public float CameraPositionZ
        {
            get => Reader.Default.Read<float>(_address + 0x1EC, out _);
            set => Writer.Default.Write(_address + 0x1EC, value);
        }

        private float _cameraRotationY;

        public float CameraRotationY
        {
            get => _cameraRotationY;
            set
            {
                _cameraRotationY = value;
                CameraRotation1 = (float)Math.Cos(_cameraRotationY);
                CameraRotation2 = CameraRotation1;
                CameraRotation3 = (float)Math.Sin(_cameraRotationY);
                CameraRotation4 = -CameraRotation3;
            }
        }

        public float CameraRotation1
        {
            get => Reader.Default.Read<float>(_address + 0x1B4, out _);
            set => Writer.Default.Write(_address + 0x1B4, value);
        }

        public float CameraRotation2
        {
            get => Reader.Default.Read<float>(_address + 0x1DC, out _);
            set => Writer.Default.Write(_address + 0x1DC, value);
        }

        public float CameraRotation3
        {
            get => Reader.Default.Read<float>(_address + 0x1BC, out _);
            set => Writer.Default.Write(_address + 0x1BC, value);
        }

        public float CameraRotation4
        {
            get => Reader.Default.Read<float>(_address + 0x1D4, out _);
            set => Writer.Default.Write(_address + 0x1D4, value);
        }

        public float CameraRotationX
        {
            get => Reader.Default.Read<float>(_address + 0x1D8, out _);
            set => Writer.Default.Write(_address + 0x1D8, value);
        }

        public TeardownGame(ulong address)
        {
            _address = Reader.Default.Read<ulong>(address, out _);
        }
    }
}
