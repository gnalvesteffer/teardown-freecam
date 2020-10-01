using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;
using Squalr.Engine.Memory;
using Squalr.Engine.OS;
using TeardownCameraHack.TeardownModels;
using TeardownCameraHack.Utilities;

namespace TeardownCameraHack
{
    public class Hack
    {
        private static readonly float TickRate = 1.0f / 60.0f;
        private static readonly float NormalCameraSpeed = 5.0f;
        private static readonly float FastCameraSpeed = 25.0f;
        private static readonly float TurnSpeed = (float)Math.PI * 0.05f;
        private static readonly float LightColorChangeAmount = 25.0f;
        private static readonly float FireSizeChangeAmount = 1.0f;
        private static readonly float DrawDistanceChangeAmount = 0.1f;

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
            WaitUntilGameIsReady();
            DisplayInstructions();
            ApplyPatches();
            MainLoop();
        }

        private void WaitUntilGameIsReady()
        {
            Console.WriteLine("Waiting for level to load...");
            var game = new TeardownGame(_teardownBaseAddress + 0x003E2528);
            while (true)
            {
                if (game.ActualTime > 0.0f && (int)game.State == (int)TeardownGameState.Level)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
            Console.Clear();
        }

        private void DisplayInstructions()
        {
            Console.WriteLine("Teardown Camera Hack by Xorberax");
            Console.WriteLine("Special thanks to Danyadd and TheOwlOfLife for their contributions!");
            Console.WriteLine();
            Console.WriteLine("Controls:");
            Console.WriteLine("Use WASD/QE/Shift to move.");
            Console.WriteLine("Click and drag the Right Mouse Button to turn.");
            Console.WriteLine("Up/Down arrows to change fire size, and Shift+Down to reset fire size.");
            Console.WriteLine("1,2,3,4,5,6 to change the flashlight color.");
            Console.WriteLine("7 to change the projectile type.");
            Console.WriteLine("Capslock to toggle autoclicker.");
        }

        private void ApplyPatches()
        {
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x1F2533, new byte[] { 0xEB }); // prevent mission from ending after 60 seconds
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x1F2798, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }); // allow player to shoot after 60 seconds
        }

        private void MainLoop()
        {
            var settings = new TeardownSettings(_teardownBaseAddress);
            var input = new TeardownInput(Reader.Default.Read<ulong>(_teardownBaseAddress + 0x3E8E10, out _));
            var game = new TeardownGame(_teardownBaseAddress + 0x003E2528);

            var lastMousePositionX = input.MouseWindowPositionX;
            var cameraRotationY = 0.0f;
            var stopwatch = Stopwatch.StartNew();
            while (true)
            {
                var deltaTime = stopwatch.ElapsedMilliseconds / 1000.0f;
                if (deltaTime < TickRate)
                {
                    continue;
                }
                stopwatch.Restart();

                if (game.State != TeardownGameState.Level)
                {
                    continue;
                }

                if (game.SimulationTime < 60.0f) // skip to end of level so that the last location of the camera path is always attempting to be reached
                {
                    game.SimulationTime = 60.0f;
                }

                var shouldUseFastCameraSpeed = _inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.SHIFT);
                var cameraMovementSpeed = shouldUseFastCameraSpeed ? FastCameraSpeed : NormalCameraSpeed;
                var currentMousePositionX = input.MouseWindowPositionX;

                var location = game.Scene.Locations.Length >= 2
                    ? game.Scene.Locations[game.Scene.Locations.Length - 2]
                    : null;
                if (location != null)
                {
                    // camera rotation
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.RBUTTON))
                    {
                        cameraRotationY -= (currentMousePositionX - lastMousePositionX) * TurnSpeed * deltaTime;
                    }
                    location.RotationY = cameraRotationY;

                    // camera position
                    var requestedCameraMovementAmount = new Vector3();
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_S))
                    {
                        requestedCameraMovementAmount -= location.Front;
                    }
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_W))
                    {
                        requestedCameraMovementAmount += location.Front;
                    }
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_A))
                    {
                        requestedCameraMovementAmount -= location.Right;
                    }
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_D))
                    {
                        requestedCameraMovementAmount += location.Right;
                    }
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_Q))
                    {
                        requestedCameraMovementAmount -= location.Up;
                    }
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_E))
                    {
                        requestedCameraMovementAmount += location.Up;
                    }

                    // apply camera movement
                    var cameraMovementAmount = requestedCameraMovementAmount.Normalized() * cameraMovementSpeed * deltaTime;
                    location.PositionX += cameraMovementAmount.X;
                    location.PositionY += cameraMovementAmount.Y;
                    location.PositionZ += cameraMovementAmount.Z;
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

                // settings
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.UP))
                {
                    settings.FireSize += FireSizeChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.DOWN))
                {
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.SHIFT))
                    {
                        settings.FireSize = 0.4f;
                        Console.Beep(400, 200);
                    }
                    else
                    {
                        settings.FireSize -= FireSizeChangeAmount * deltaTime;
                    }
                }
                settings.FireSize = Math.Max(settings.FireSize, 0.0f);

                // flashlight color
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_1))
                {
                    game.Scene.FlashLight.Red -= LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_2))
                {
                    game.Scene.FlashLight.Red += LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_3))
                {
                    game.Scene.FlashLight.Green -= LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_4))
                {
                    game.Scene.FlashLight.Green += LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_5))
                {
                    game.Scene.FlashLight.Blue -= LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_6))
                {
                    game.Scene.FlashLight.Blue += LightColorChangeAmount * deltaTime;
                }

                // change projectile type
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_7))
                {
                    Console.Beep(500, 200); // HACK: utilize the beep to notify the player that the type changed, and to delay the keystrokes, preventing the types from cycling quickly -- replace this with a keypress/key-up check instead
                    settings.BulletType = (TeardownProjectileType)(((byte)settings.BulletType + 1) % Enum.GetValues(typeof(TeardownProjectileType)).Length);
                }

                lastMousePositionX = currentMousePositionX;
            }
        }
    }
}
