using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;
using Squalr.Engine.Memory;
using Squalr.Engine.OS;
using TeardownCameraHack.Teardown;
using TeardownCameraHack.Teardown.Models;
using TeardownCameraHack.Utilities;

namespace TeardownCameraHack
{
    public class Hack
    {
        private static readonly float TickRate = 1.0f / 60.0f;
        private static readonly float NormalCameraSpeed = 5.0f;
        private static readonly float FastCameraSpeed = 25.0f;
        //private static readonly float TurnSpeed = (float)Math.PI * 0.05f;
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
            Console.WriteLine("Special thanks to the Teardown community for all of their support.");
            Console.WriteLine();
            Console.WriteLine("Controls:");
            Console.WriteLine("Use WASD/QE/Shift to move.");
            Console.WriteLine("Click and drag the Right Mouse Button to look around.");
            Console.WriteLine("Up/Down arrows to change fire size, and Shift+Down to reset fire size.");
            Console.WriteLine("Delete to remove all fires");
            Console.WriteLine("1,2,3,4,5,6 to change the projectile type.");
            Console.WriteLine("SHIFT+1,2,3,4,5,6 to change the flashlight color.");
            Console.WriteLine("0 to toggle fog.");
            Console.WriteLine("Capslock to toggle autoclicker.");
            Console.WriteLine("CTRL+R to restart level.");
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
            var lastMousePositionY = input.MouseWindowPositionY;
            var cameraRotationX = 0.0f;
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

                if (Processes.Default.OpenedProcess.HasExited)
                {
                    break;
                }
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
                var currentMousePositionY = input.MouseWindowPositionY;
                var cameraSensitivity = game.Sensitivity * 0.001f * (float)Math.PI;

                var location = game.Scene.Locations.Length >= 2
                    ? game.Scene.Locations[game.Scene.Locations.Length - 2]
                    : null;
                if (location != null)
                {
                    // camera rotation
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.RBUTTON))
                    {
                        cameraRotationX += (currentMousePositionY - lastMousePositionY) * cameraSensitivity * deltaTime;
                        cameraRotationY -= (currentMousePositionX - lastMousePositionX) * cameraSensitivity * deltaTime;
                    }
                    location.Frame =
                        Quaternion.CreateFromAxisAngle(location.Left, cameraRotationX) *
                        Quaternion.CreateFromAxisAngle(location.Up, cameraRotationY);

                    // camera position
                    var requestedCameraMovementAmount = new Vector3();
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_S))
                    {
                        requestedCameraMovementAmount += location.Back;
                    }
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_W))
                    {
                        requestedCameraMovementAmount += location.Front;
                    }
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_A))
                    {
                        requestedCameraMovementAmount += location.Left;
                    }
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_D))
                    {
                        requestedCameraMovementAmount += location.Right;
                    }
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_Q))
                    {
                        requestedCameraMovementAmount += location.Down;
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

                // restart level
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.CONTROL) && _inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_R))
                {
                    game.NextState = TeardownGameState.Level;
                    Console.Beep(900, 200);
                }

                // toggle fog
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_0))
                {
                    game.IsFogEnabled = !game.IsFogEnabled;
                    Console.Beep(700, 200);
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

                // delete all fires
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.DELETE))
                {
                    game.Scene.FireSystem.TotalFires = 0;
                    Console.Beep(600, 200);
                }

                // flashlight color
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.SHIFT))
                {
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
                }
                else // change projectile type
                {
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_1))
                    {
                        settings.ProjectileType = TeardownProjectileType.RegularBullet;
                        Console.Beep(420, 200);
                    }
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_2))
                    {
                        settings.ProjectileType = TeardownProjectileType.SmallBullet;
                        Console.Beep(440, 200);
                    }
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_3))
                    {
                        settings.ProjectileType = TeardownProjectileType.LargeBullet;
                        Console.Beep(460, 200);
                    }
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_4))
                    {
                        settings.ProjectileType = TeardownProjectileType.Rocket;
                        Console.Beep(480, 200);
                    }
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_5))
                    {
                        settings.ProjectileType = TeardownProjectileType.Bomb;
                        Console.Beep(500, 200);
                    }
                    if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_6))
                    {
                        settings.ProjectileType = TeardownProjectileType.Force;
                        Console.Beep(520, 200);
                    }
                }

                lastMousePositionX = currentMousePositionX;
                lastMousePositionY = currentMousePositionY;
            }
        }
    }
}
