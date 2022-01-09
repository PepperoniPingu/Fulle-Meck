using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Fulle_Meck
{
    class Launching
    {
        // The position of ground
        private const int groundHeight = 850;

        // Where space begins
        private const int spaceHeight = -3000;

        // Background image
        public static Texture2D launchBkg;
        // Rocket exhaust
        public static Texture2D rocketFire;

        // The screen needs to scroll therefore create a position for the window/camera
        private static Vector2 cameraPos = new Vector2(0, 0);

        // Class to address the built rocket
        public static class builtRocket
        {
            public static Molijox motor = new Molijox();
            public static Molijox tank = new Molijox();
            public static Molijox top = new Molijox();
            public static float speed;
            public static float fuel = 1f;

            // A list to address the built rocket in a different way, sometimes more convenient
            public static List<Molijox> list()
            {
                List<Molijox> _list = new List<Molijox>();
                _list.Add(motor);
                _list.Add(tank);
                _list.Add(top);
                return _list;
            }

            // A global position that can go outside the screen limits is needed since the screen scrolls
            public static Vector2 globalPos { get; set; }

            // Global rectangle for the whole rocket
            public static Rectangle globalRect
            {
                get => new Rectangle(Convert.ToInt32(globalPos.X), Convert.ToInt32(globalPos.Y), 64, 192);
                set => globalPos = new Vector2(value.X, value.Y);
            }

        }

        // Method for initializing flying
        public static void initialize()
        {
            Game1.gameState = Game1.GameStates.launch;
            menus.currentMenu = "flying";

            // Reset speed and positions
            cameraPos = new Vector2();
            builtRocket.speed = 0;
            builtRocket.globalPos = new Vector2(Game1.window.ClientBounds.Width / 2 - 32, groundHeight - builtRocket.globalRect.Height);

            foreach (Molijox moj in Game1.molijoxer.ToList())
            {
                // Define wich parts make up the built rocket
                if (moj.onRocket)
                {
                    switch (moj.type)
                    {
                        case Molijox.Type.motor:
                            builtRocket.motor = moj;
                            break;

                        case Molijox.Type.tank:
                            builtRocket.tank = moj;
                            break;

                        case Molijox.Type.top:
                            builtRocket.top = moj;
                            break;

                        default:
                            break;
                    }
                }
            }

            // Fill the fuel according to the tank capacity
            builtRocket.fuel = builtRocket.tank.power;
        }

        // Update routine
        public static void update(KeyboardState keyboard, MouseState mouse)
        {
            // Increase speed if up key is pressed
            if (keyboard.IsKeyDown(Keys.Up) && builtRocket.fuel > 0)
            {
                builtRocket.speed -= builtRocket.motor.power;
                // Decrease fuel
                builtRocket.fuel -= builtRocket.motor.fuelConsumption;

                // Play sound
                if (Game1.motorSoundInstance.State == SoundState.Stopped) Game1.motorSoundInstance.Play();
            } else
            {
                Game1.motorSoundInstance.Stop();
            }

            // If above ground, gravity is applied
            if (builtRocket.globalRect.Bottom + builtRocket.speed + gravity(builtRocket.globalRect.Center.Y) < groundHeight)
            {
                builtRocket.speed += gravity(builtRocket.globalRect.Center.Y);
            }
            else if (builtRocket.speed > 0) // Otherwise, and if the speed is downwords, reset the speed and position
            {
                builtRocket.speed = 0;
                builtRocket.globalPos = new Vector2(builtRocket.globalRect.X, groundHeight - builtRocket.globalRect.Height);
            }

            // Change to shootemup if in "space" adn stop music
            if (builtRocket.globalPos.Y < spaceHeight)
            {
                Game1.motorSoundInstance.Stop();
                LevelSelect.initialize();
            }

            // Apply the speed on the position
            builtRocket.globalPos = new Vector2(builtRocket.globalRect.X, Convert.ToInt32(builtRocket.globalRect.Y + builtRocket.speed));

            // Shift the window position if the rocket is outside the camera bounds
            if (builtRocket.globalRect.Top < cameraBound().Top)
            {
                cameraPos.Y -= (cameraBound().Top - builtRocket.globalRect.Top);

            }
            else if (builtRocket.globalRect.Bottom > cameraBound().Bottom)
            {
                cameraPos.Y += (builtRocket.globalRect.Bottom - cameraBound().Bottom);
            }

            // Update menu
            menus.update(mouse);
        }

        // Drawing routine for flying game mode
        public static void draw(SpriteBatch spriteBatch, KeyboardState keyboard, MouseState mouse)
        {
            // Make the screen black
            Texture2D background = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            background.SetData(new Color[] { Color.Black });
            spriteBatch.Draw(background, new Rectangle(0, 0, Game1.window.ClientBounds.Width, Game1.window.ClientBounds.Height), Color.White);
            // Draw background
            spriteBatch.Draw(launchBkg, new Vector2(cameraPos.X, -cameraPos.Y - 3240), Color.White);

            // Draw all the parts of the rocket
            spriteBatch.Draw(builtRocket.motor.sprite2x, new Vector2(builtRocket.globalRect.X - cameraPos.X, builtRocket.globalRect.Y + 128 - cameraPos.Y), Color.White);
            spriteBatch.Draw(builtRocket.tank.sprite2x, new Vector2(builtRocket.globalRect.X - cameraPos.X, builtRocket.globalRect.Y + 64 - cameraPos.Y), Color.White);
            spriteBatch.Draw(builtRocket.top.sprite2xMulle, new Vector2(builtRocket.globalRect.X - cameraPos.X, builtRocket.globalRect.Y - cameraPos.Y), Color.White);

            // Draw rocket exhaust
            if (keyboard.IsKeyDown(Keys.Up) && builtRocket.fuel > 0) spriteBatch.Draw(rocketFire, new Vector2(builtRocket.globalRect.X - cameraPos.X, builtRocket.globalRect.Bottom - cameraPos.Y), Color.White);

            // Draw a rectangle for the fuel bar background
            Texture2D fuelBackground = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            fuelBackground.SetData(new Color[] { Color.Black });
            spriteBatch.Draw(fuelBackground, new Rectangle(50, 170, 410, 50), Color.White);
            // Draw a slightly smaller rectangle for the fuel bar
            Texture2D fuelBar = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            // If there is no fuel left, make the whole bar red to indicate
            if (builtRocket.fuel > 0)
            {
                fuelBar.SetData(new Color[] { Color.GreenYellow });
                spriteBatch.Draw(fuelBar, new Rectangle(55, 175, Convert.ToInt32(400 * (builtRocket.fuel / builtRocket.tank.power)), 40), Color.White);
            }
            else
            {
                fuelBar.SetData(new Color[] { Color.Red });
                spriteBatch.Draw(fuelBar, new Rectangle(55, 175, 400, 40), Color.White);
            }

            // Draw menu
            menus.draw(spriteBatch, mouse);
        }


        // Returns the gravity at given height
        private static float gravity(float height)
        {
            const int atmosphere = 5000;
            return (atmosphere - (-height + Game1.window.ClientBounds.Height)) / 10000;
        }

        // Returns a rectangle for where the rocket can be wihout the camera moving
        private static Rectangle cameraBound()
        {
            return new Rectangle(
                Convert.ToInt32(Game1.window.ClientBounds.Width * 0.25 + cameraPos.X),
                Convert.ToInt32(Game1.window.ClientBounds.Height * 0.2 + cameraPos.Y),
                Convert.ToInt32(Game1.window.ClientBounds.Width * 0.5),
                Convert.ToInt32(Game1.window.ClientBounds.Height * 0.6)
                );
        }
    }
}
