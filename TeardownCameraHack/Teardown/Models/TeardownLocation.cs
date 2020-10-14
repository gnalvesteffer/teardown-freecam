using System;
using System.Numerics;
using Squalr.Engine.Memory;
using TeardownCameraHack.Utilities;

namespace TeardownCameraHack.Teardown.Models
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
            -(float)Math.Cos(Yaw + Math.PI / 2),
            0.0f,
            -(float)Math.Sin(Yaw + Math.PI / 2)
        );

        public Vector3 Back => -Front;

        public Vector3 Right => new Vector3(
            (float)Math.Cos(Yaw),
            0.0f,
            (float)Math.Sin(Yaw)
        );

        public Vector3 Left => -Right;

        public Vector3 Up => new Vector3(
            0.0f,
            1.0f,
            0.0f
        );

        public Vector3 Down => -Up;

        public Quaternion Frame
        {
            get => new Quaternion(RotationX, RotationY, RotationZ, RotationW);
            set
            {
                RotationW = value.X;
                RotationX = value.Y;
                RotationY = value.Z;
                RotationZ = value.W;
            }
        }

        public float Yaw => MathUtility.WrapAngle((float)Math.Atan2(RotationZ, RotationX) * 2.0f);

        public float RotationW
        {
            get => Reader.Default.Read<float>(_address + 0x34, out _);
            set => Writer.Default.Write(_address + 0x34, value);
        }

        public float RotationX
        {
            get => Reader.Default.Read<float>(_address + 0x38, out _);
            set => Writer.Default.Write(_address + 0x38, value);
        }

        public float RotationY
        {
            get => Reader.Default.Read<float>(_address + 0x3C, out _);
            set => Writer.Default.Write(_address + 0x3C, value);
        }

        public float RotationZ
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
