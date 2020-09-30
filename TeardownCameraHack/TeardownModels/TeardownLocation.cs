using System;
using System.Numerics;
using Squalr.Engine.Memory;
using TeardownCameraHack.Utilities;

namespace TeardownCameraHack.TeardownModels
{
    public class TeardownLocation
    {
        private readonly ulong _address;

        public float PositionX
        {
            get => Reader.Default.Read<float>(_address + 0x28, out _);
            set => Writer.Default.Write(_address + 0x28, value);
        }

        public float PositionY
        {
            get => Reader.Default.Read<float>(_address + 0x2C, out _);
            set => Writer.Default.Write(_address + 0x2C, value);
        }

        public float PositionZ
        {
            get => Reader.Default.Read<float>(_address + 0x30, out _);
            set => Writer.Default.Write(_address + 0x30, value);
        }

        public Vector3 Front => new Vector3(
            -(float)Math.Cos(RotationY + Math.PI / 2),
            0.0f,
            -(float)Math.Sin(RotationY + Math.PI / 2)
        );

        public Vector3 Right => new Vector3(
            (float)Math.Cos(RotationY),
            0.0f,
            (float)Math.Sin(RotationY)
        );

        public Vector3 Up => new Vector3(
            0.0f,
            1.0f,
            0.0f
        );

        public float RotationY
        {
            get { return MathUtility.WrapAngle((float)Math.Atan2(Rotation4, Rotation2) * 2.0f); }
            set
            {
                Rotation1 = 0.0f;
                Rotation2 = (float)Math.Sin(value);
                Rotation3 = 0.0f;
                Rotation4 = (float)Math.Cos(value);
            }
        }

        public float Rotation1
        {
            get => Reader.Default.Read<float>(_address + 0x34, out _);
            set => Writer.Default.Write(_address + 0x34, value);
        }

        public float Rotation2
        {
            get => Reader.Default.Read<float>(_address + 0x38, out _);
            set => Writer.Default.Write(_address + 0x38, value);
        }

        public float Rotation3
        {
            get => Reader.Default.Read<float>(_address + 0x3C, out _);
            set => Writer.Default.Write(_address + 0x3C, value);
        }

        public float Rotation4
        {
            get => Reader.Default.Read<float>(_address + 0x40, out _);
            set => Writer.Default.Write(_address + 0x40, value);
        }

        public TeardownLocation(ulong address)
        {
            _address = address;
        }
    }
}
