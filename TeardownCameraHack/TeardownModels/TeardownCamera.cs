using System;
using Squalr.Engine.Memory;

namespace TeardownCameraHack.TeardownModels
{
    public class TeardownCamera
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

        public bool IsFogEnabled
        {
            get => !Reader.Default.Read<bool>(_address + 0x1C, out _);
            set => Writer.Default.Write(_address + 0x1C, !value);
        }

        public TeardownScene Scene
        {
            get
            {
                var sceneAddress = Reader.Default.Read<ulong>(_address + 0x40, out _);
                return new TeardownScene(sceneAddress);
            }
        }

        public float LifeTime
        {
            get => Reader.Default.Read<float>(_address + 0x14C, out _);
            set => Writer.Default.Write(_address + 0x14C, value);
        }

        public float Time
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

        public float PositionX
        {
            get => Reader.Default.Read<float>(_address + 0x1E4, out _);
            set => Writer.Default.Write(_address + 0x1E4, value);
        }

        public float PositionY
        {
            get => Reader.Default.Read<float>(_address + 0x1E8, out _);
            set => Writer.Default.Write(_address + 0x1E8, value);
        }

        public float PositionZ
        {
            get => Reader.Default.Read<float>(_address + 0x1EC, out _);
            set => Writer.Default.Write(_address + 0x1EC, value);
        }

        private float _rotationY;

        public float RotationY
        {
            get => _rotationY;
            set
            {
                _rotationY = value;
                Rotation1 = (float)Math.Cos(_rotationY);
                Rotation2 = Rotation1;
                Rotation3 = (float)Math.Sin(_rotationY);
                Rotation4 = -Rotation3;
            }
        }

        public float Rotation1
        {
            get => Reader.Default.Read<float>(_address + 0x1B4, out _);
            set => Writer.Default.Write(_address + 0x1B4, value);
        }

        public float Rotation2
        {
            get => Reader.Default.Read<float>(_address + 0x1DC, out _);
            set => Writer.Default.Write(_address + 0x1DC, value);
        }

        public float Rotation3
        {
            get => Reader.Default.Read<float>(_address + 0x1BC, out _);
            set => Writer.Default.Write(_address + 0x1BC, value);
        }

        public float Rotation4
        {
            get => Reader.Default.Read<float>(_address + 0x1D4, out _);
            set => Writer.Default.Write(_address + 0x1D4, value);
        }

        public float RotationX
        {
            get => Reader.Default.Read<float>(_address + 0x1D8, out _);
            set => Writer.Default.Write(_address + 0x1D8, value);
        }

        public TeardownCamera(ulong address)
        {
            _address = Reader.Default.Read<ulong>(address, out _);
        }
    }
}
