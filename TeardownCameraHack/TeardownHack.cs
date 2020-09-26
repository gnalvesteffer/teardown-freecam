using System;
using System.Diagnostics;
using WindowsInput;
using WindowsInput.Native;
using Squalr.Engine.Memory;
using Squalr.Engine.OS;

namespace TeardownCameraHack
{
    public class TeardownHack
    {
        private static readonly float NormalCameraSpeed = 0.01f;
        private static readonly float FastCameraSpeed = 0.05f;
        private static readonly float TurnSpeed = 0.001f;

        private readonly InputSimulator _inputSimulator;
        private readonly ulong _teardownBaseAddress;

        public TeardownHack(Process teardownProcess)
        {
            _inputSimulator = new InputSimulator();
            _teardownBaseAddress = (ulong)teardownProcess.MainModule.BaseAddress;
            Processes.Default.OpenedProcess = teardownProcess;
        }

        public void Start()
        {
            DisplayInstructions();
            ApplyPatches();
            ControlLoop();
        }

        private void DisplayInstructions()
        {
            Console.WriteLine("Teardown Camera Hack by Xorberax");
            Console.WriteLine("Use WASD/QE/Shift to move.");
        }

        private void ControlLoop()
        {
            var world = new TeardownWorld(this._teardownBaseAddress + 0x3E8B60);
            var camera = new TeardownCamera(this._teardownBaseAddress + 0x003E2528);
            var scene = new TeardownScene(Reader.Default.Read<ulong>(Reader.Default.Read<ulong>(this._teardownBaseAddress + 0x3E8B60, out _), out _));
            var stopwatch = Stopwatch.StartNew();
            while (true)
            {
                var deltaTime = Math.Max(stopwatch.ElapsedMilliseconds / 1000.0f, 1.0f / 60.0f);
                stopwatch.Restart();

                var shouldUseFastCameraSpeed = _inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.SHIFT);
                var cameraMovementSpeed = shouldUseFastCameraSpeed ? FastCameraSpeed : NormalCameraSpeed;

                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_W))
                {
                    camera.PositionZ += cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_S))
                {
                    camera.PositionZ -= cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_A))
                {
                    camera.PositionX += cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_D))
                {
                    camera.PositionX -= cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_Q))
                {
                    camera.PositionY += cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_E))
                {
                    camera.PositionY -= cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_Z))
                {
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_C))
                {
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_1))
                {
                    scene.Light.Red -= deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_2))
                {
                    scene.Light.Red += deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_3))
                {
                    scene.Light.Green -= deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_4))
                {
                    scene.Light.Green += deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_5))
                {
                    scene.Light.Blue -= deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_6))
                {
                    scene.Light.Blue += deltaTime;
                }

                // ToDo: sync player with camera
                // world.Player.PositionX = camera.PositionX;
                // world.Player.PositionY = camera.PositionY;
                // world.Player.PositionZ = camera.PositionZ;
            }
        }

        private void ApplyPatches()
        {
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x312D1, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }); // pause camera path
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x2E750, new byte[] { 0x90, 0x90, 0x90, 0x90 }); // prevent camera position assignment
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x2E73C, new byte[] { 0x90, 0x90, 0x90, 0x90 }); // prevent camera rotation assignment
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x2E74C, new byte[] { 0x90, 0x90, 0x90, 0x90 }); // prevent camera rotation assignment
            // Writer.Default.WriteBytes(_teardownBaseAddress + 0xC6989, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }); // prevent player position assignment
            // Writer.Default.WriteBytes(_teardownBaseAddress + 0xC698E, new byte[] { 0x90, 0x90, 0x90 }); // prevent player position assignment
        }
    }
}
