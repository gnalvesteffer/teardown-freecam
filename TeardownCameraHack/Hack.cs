using System;
using System.Diagnostics;
using System.Numerics;
using WindowsInput;
using WindowsInput.Native;
using Squalr.Engine.Memory;
using Squalr.Engine.OS;
using TeardownCameraHack.TeardownModels;

namespace TeardownCameraHack
{
    public class Hack
    {
        private static readonly float TickRate = 1.0f / 60.0f;
        private static readonly float NormalCameraSpeed = 5.0f;
        private static readonly float FastCameraSpeed = 25.0f;
        private static readonly float TurnSpeed = (float)Math.PI * 0.1f;
        private static readonly float LightColorChangeAmount = 25.0f;
        private static readonly float FireSizeChangeAmount = 1.0f;

        private readonly InputSimulator _inputSimulator;
        private readonly ulong _teardownBaseAddress;

        public Hack(Process teardownProcess)
        {
            _inputSimulator = new InputSimulator();
            _teardownBaseAddress = (ulong)teardownProcess.MainModule.BaseAddress;
            Processes.Default.OpenedProcess = teardownProcess;
        }

        public void Start()
        {
            DisplayInstructions();
            ApplyPatches();
            MainLoop();
        }

        private void DisplayInstructions()
        {
            Console.WriteLine("Teardown Camera Hack by Xorberax");
            Console.WriteLine("Special thanks to Danyadd and TheOwlOfLife for their contributions!");
            Console.WriteLine();
            Console.WriteLine("Controls:");
            Console.WriteLine("Use WASD/QE/Shift to move.");
            Console.WriteLine("Click and drag the Right Mouse Button to turn.");
            Console.WriteLine("Up/Down arrows to change fire size.");
            Console.WriteLine("1,2,3,4,5,6 to change the flashlight color.");
            Console.WriteLine("7 to change the projectile type.");
            Console.WriteLine("Capslock to toggle autoclicker.");
        }

        private void ApplyPatches()
        {
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x1F2533, new byte[] { 0xEB }); // prevent mission from ending after 60 seconds
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x1F2798, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }); // allow player to shoot after 60 seconds
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x2E750, new byte[] { 0x90, 0x90, 0x90, 0x90 }); // prevent camera position assignment
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x2E73C, new byte[] { 0x90, 0x90, 0x90, 0x90 }); // prevent camera rotation assignment
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x2E74C, new byte[] { 0x90, 0x90, 0x90, 0x90 }); // prevent camera rotation assignment
            //Writer.Default.WriteBytes(_teardownBaseAddress + 0x312D1, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }); // pause time
            // Writer.Default.WriteBytes(_teardownBaseAddress + 0xC6989, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }); // prevent light position assignment
            // Writer.Default.WriteBytes(_teardownBaseAddress + 0xC698E, new byte[] { 0x90, 0x90, 0x90 }); // prevent light position assignment
            // Writer.Default.WriteBytes(_teardownBaseAddress + 0xC6989, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }); // prevent player position assignment
            // Writer.Default.WriteBytes(_teardownBaseAddress + 0xC698E, new byte[] { 0x90, 0x90, 0x90 }); // prevent player position assignment
        }

        private void MainLoop()
        {
            var settings = new TeardownSettings(_teardownBaseAddress);
            var input = new TeardownInput(Reader.Default.Read<ulong>(_teardownBaseAddress + 0x3E8E10, out _));
            var scene = new TeardownScene(Reader.Default.Read<ulong>(Reader.Default.Read<ulong>(_teardownBaseAddress + 0x3E8B60, out _), out _));
            var camera = new TeardownCamera(_teardownBaseAddress + 0x003E2528);

            // camera rotation vars
            var virtualCameraPosition = new Vector3();
            var previousMousePositionX = 0;
            var lastMousePositionX = (float)input.MouseWindowPositionX;

            var stopwatch = Stopwatch.StartNew();
            while (true)
            {
                var deltaTime = stopwatch.ElapsedMilliseconds / 1000.0f;
                if (deltaTime < TickRate)
                {
                    continue;
                }
                stopwatch.Restart();

                var shouldUseFastCameraSpeed = _inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.SHIFT);
                var cameraMovementSpeed = shouldUseFastCameraSpeed ? FastCameraSpeed : NormalCameraSpeed;

                // camera position
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_W))
                {
                    virtualCameraPosition.X += (float)Math.Sin(camera.RotationY) * cameraMovementSpeed * deltaTime;
                    virtualCameraPosition.Z += (float)Math.Cos(camera.RotationY) * cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_S))
                {
                    virtualCameraPosition.X -= (float)Math.Sin(camera.RotationY) * cameraMovementSpeed * deltaTime;
                    virtualCameraPosition.Z -= (float)Math.Cos(camera.RotationY) * cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_A))
                {
                    virtualCameraPosition.X += (float)Math.Cos(camera.RotationY) * cameraMovementSpeed * deltaTime;
                    virtualCameraPosition.Z -= (float)Math.Sin(camera.RotationY) * cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_D))
                {
                    virtualCameraPosition.X -= (float)Math.Cos(camera.RotationY) * cameraMovementSpeed * deltaTime;
                    virtualCameraPosition.Z += (float)Math.Sin(camera.RotationY) * cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_Q))
                {
                    virtualCameraPosition.Y += cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_E))
                {
                    virtualCameraPosition.Y -= cameraMovementSpeed * deltaTime;
                }

                // autoclicker
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.CAPITAL))
                {
                    Console.Beep(700, 200);
                }
                if (_inputSimulator.InputDeviceState.IsTogglingKeyInEffect(VirtualKeyCode.CAPITAL) && _inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.LBUTTON))
                {
                    _inputSimulator.Mouse.LeftButtonDown();
                }

                // camera rotation
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.RBUTTON))
                {
                    lastMousePositionX += (previousMousePositionX - input.MouseWindowPositionX) * TurnSpeed * deltaTime;
                }
                camera.RotationY = lastMousePositionX;
                camera.PositionX = virtualCameraPosition.X * (float)Math.Cos(camera.RotationY) - virtualCameraPosition.Z * (float)Math.Sin(camera.RotationY);
                camera.PositionY = virtualCameraPosition.Y;
                camera.PositionZ = virtualCameraPosition.X * (float)Math.Sin(camera.RotationY) + virtualCameraPosition.Z * (float)Math.Cos(camera.RotationY);

                // settings
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.UP))
                {
                    settings.FireSize += FireSizeChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.DOWN))
                {
                    settings.FireSize -= FireSizeChangeAmount * deltaTime;
                }

                // light color
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_1))
                {
                    scene.FlashLight.Red -= LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_2))
                {
                    scene.FlashLight.Red += LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_3))
                {
                    scene.FlashLight.Green -= LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_4))
                {
                    scene.FlashLight.Green += LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_5))
                {
                    scene.FlashLight.Blue -= LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_6))
                {
                    scene.FlashLight.Blue += LightColorChangeAmount * deltaTime;
                }

                // change projectile type
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_7))
                {
                    Console.Beep(500, 200); // HACK: utilize the beep to notify the player that the type changed, and to delay the keystrokes, preventing the types from cycling quickly -- replace this with a keypress/key-up check instead
                    settings.BulletType = (TeardownProjectileType)(((byte)settings.BulletType + 1) % Enum.GetValues(typeof(TeardownProjectileType)).Length);
                }

                previousMousePositionX = input.MouseWindowPositionX;
            }
        }
    }
}
