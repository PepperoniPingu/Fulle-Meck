using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Fulle_Meck
{
    class ShootemUp
    {
        // How long before the level is cleared in seconds
        private const int levelDuration = 20;

        // Textures
        public static Texture2D stoneSprite;
        public static Texture2D stoneSprite2x;
        public static Texture2D stoneSprite4x;
        public static Texture2D stoneSprite8x;
        public static Texture2D fuelSprite2x;
        public static Texture2D heartSprite;
        public static Texture2D rocketFire;
        public static Texture2D rocketFire2x;

        // Frame counter
        private static long frames;

        // List for flying stones
        private static List<Stone> stones;
        // List for flying fuel cans
        private static List<FuelItem> fuelItems;

        // Current level
        private static int level;

        // Class for the molijox possible to unlock
        private static class unlockableMolijox
        {
            public static Molijox molijox;
            // Flag for if there are any molijoxer left to unlock
            public static bool available;
            // Speed
            public static float speed;
        }

        // Object for the rocket
        public static class builtRocket
        {
            private const int width = 64;
            private const int height = 192;

            public static Vector2 pos;

            public static int health;
            public static float fuel;

            public static Rectangle Rect
            {
                get => new Rectangle((int)pos.X, (int)pos.Y, width, height);
                set => pos = new Vector2(value.X, value.Y);
            }

            public static Molijox motor = new Molijox();
            public static Molijox tank = new Molijox();
            public static Molijox top = new Molijox();
        }

        // Method to initialize
        public static void initialize(int _level)
        {
            frames = 0;

            Game1.gameState = Game1.GameStates.shootemUp;
            menus.currentMenu = "flying";

            builtRocket.pos = new Vector2(Game1.window.ClientBounds.Width / 2, Game1.window.ClientBounds.Height * 0.7f);

            builtRocket.health = 3;

            level = _level;

            // Document the parts of the built rocket
            builtRocket.motor = Launching.builtRocket.motor;
            builtRocket.tank = Launching.builtRocket.tank;
            builtRocket.top = Launching.builtRocket.top;
            builtRocket.fuel = Launching.builtRocket.fuel;

            // List for items
            stones = new List<Stone>();
            fuelItems = new List<FuelItem>();

            // Select a locked molijox at random
            List<Molijox> tempUnlockableMolijoxer = new List<Molijox>();
            foreach (Molijox moj in Game1.molijoxer)
            {
                if (!moj.unlocked) tempUnlockableMolijoxer.Add(moj);
            }
            if (tempUnlockableMolijoxer.Count > 0)
            {
                Random rnd = new Random();
                unlockableMolijox.molijox = tempUnlockableMolijoxer[rnd.Next(0, tempUnlockableMolijoxer.Count)];
                unlockableMolijox.available = true;
                unlockableMolijox.speed = rnd.Next(3, 5);
                unlockableMolijox.molijox.rect = new Rectangle(rnd.Next(0, 1920), rnd.Next(-Game1.window.ClientBounds.Height * 2, 0), 64, 64);
            }
            else
            {
                unlockableMolijox.molijox = new Molijox();
                unlockableMolijox.available = false;
            }
            
        }

        // Update routine
        public static void update(KeyboardState keyboard, MouseState mouse)
        {
            // Count the frames
            frames++;

            // Check collision with stones
            foreach (Stone stn in stones.ToList())
            {
                if (builtRocket.Rect.Intersects(stn.rect) && !stn.collided)
                {
                    builtRocket.health--;
                    stn.collided = true;
                }
                else if (!builtRocket.Rect.Intersects(stn.rect)) stn.collided = false;
            }
            // Check collision with fuel
            foreach (FuelItem fl in fuelItems.ToList())
            {
                if (builtRocket.Rect.Intersects(fl.rect))
                {
                    if (builtRocket.fuel + 0.5f > 1f)
                    {
                        builtRocket.fuel = 1f;
                    } else builtRocket.fuel += 0.5f;
                    fuelItems.Remove(fl);
                }
            }

            // Move the rocket in all 4 directions if fuel is present
            bool flag = false;
            if (builtRocket.fuel > 0f)
            {
                if (keyboard.IsKeyDown(Keys.Left) && builtRocket.Rect.Left > Game1.window.ClientBounds.Left)
                {
                    builtRocket.pos.X -= 7;
                    builtRocket.fuel -= builtRocket.top.fuelConsumption;
                    if (Game1.motorSoundInstance.State == SoundState.Stopped) Game1.motorSoundInstance.Play();
                    flag = true;
                }
                if (keyboard.IsKeyDown(Keys.Right) && builtRocket.Rect.Right < Game1.window.ClientBounds.Right)
                {
                    builtRocket.pos.X += 7;
                    builtRocket.fuel -= builtRocket.top.fuelConsumption;
                    if (Game1.motorSoundInstance.State == SoundState.Stopped) Game1.motorSoundInstance.Play();
                    flag = true;
                }
                if (keyboard.IsKeyDown(Keys.Up) && builtRocket.Rect.Top > Game1.window.ClientBounds.Top)
                {
                    builtRocket.pos.Y -= 7;
                    builtRocket.fuel -= builtRocket.motor.fuelConsumption;
                    if (Game1.motorSoundInstance.State == SoundState.Stopped) Game1.motorSoundInstance.Play();
                    flag = true;
                }
                if (keyboard.IsKeyDown(Keys.Down) && builtRocket.Rect.Bottom < Game1.window.ClientBounds.Bottom)
                {
                    builtRocket.pos.Y += 7;
                    builtRocket.fuel -= builtRocket.motor.fuelConsumption;
                    if (Game1.motorSoundInstance.State == SoundState.Stopped) Game1.motorSoundInstance.Play();
                    flag = true;
                }
            }
            if (!flag) Game1.motorSoundInstance.Stop();

            // Fill the list with stones. Amount of stones determined by the level played
            while (stones.Count <= level * 15)
            {
                stones.Add(new Stone(level));
            }

            // Update the position of the stones and delete the ones outside of the screen
            foreach(Stone stn in stones.ToList())
            {
                if (stn.Pos.Y >= Game1.window.ClientBounds.Height)
                {
                    stones.Remove(stn);
                } else
                {
                    stn.Pos = new Vector2(stn.Pos.X, stn.Pos.Y + stn.speed);
                }
            }

            // Fill the list with stones. Amount of stones determined by the level played
            while (stones.Count <= level * 15)
            {
                stones.Add(new Stone(level));
            }

            // Update the position of the stones and delete the ones outside of the screen
            foreach (FuelItem fl in fuelItems.ToList())
            {
                if (fl.Pos.Y >= Game1.window.ClientBounds.Height)
                {
                    fuelItems.Remove(fl);
                }
                else
                {
                    fl.Pos = new Vector2(fl.Pos.X, fl.Pos.Y + fl.speed);
                }
            }

            // Spawn pickable fuel cans. Amount determined by current level
            while (fuelItems.Count <= 4 - level)
            {
                fuelItems.Add(new FuelItem(level));
            }

            // Move the unlockable molijox and check collision
            if (frames/60 > 5 && unlockableMolijox.available)
            {
                unlockableMolijox.molijox.Pos = new Vector2(unlockableMolijox.molijox.Pos.X, unlockableMolijox.molijox.Pos.Y + unlockableMolijox.speed);

                if (builtRocket.Rect.Intersects(unlockableMolijox.molijox.rect))
                {
                    unlockableMolijox.available = false;
                    unlockableMolijox.molijox.unlocked = true;
                }
            }

            // Check the health and decide if it's game over
            if (builtRocket.health <= 0)
            {
                Game1.motorSoundInstance.Stop();
                ShootemUpResult.initialize(false, new Molijox());
                unlockableMolijox.molijox.unlocked = false; 
            }

            // Check if the level was cleared
            if (frames / 60 > levelDuration)
            {
                // Unlock next level
                Game1.motorSoundInstance.Stop();
                if (level != 3) Game1.levelsUnlocked[level].var = true;
                ShootemUpResult.initialize(true, unlockableMolijox.molijox.unlocked ? unlockableMolijox.molijox : new Molijox());
            }

            // Update menu
            menus.update(mouse);
        }


        // Drawing routine
        public static void draw(SpriteBatch spriteBatch, KeyboardState keyboard, MouseState mouse)
        {
            // Make the background dark gray
            Texture2D background = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            background.SetData(new Color[] { new Color(20, 20, 20) });
            spriteBatch.Draw(background, new Rectangle(0, 0, Game1.window.ClientBounds.Width, Game1.window.ClientBounds.Height), Color.White);

            // Draw the unlockable molijox if there is one
            if (unlockableMolijox.available) spriteBatch.Draw(unlockableMolijox.molijox.sprite2x, unlockableMolijox.molijox.Pos, Color.White);

            // Draw the fuel cans
            foreach (FuelItem fl in fuelItems)
            {
                spriteBatch.Draw(fl.sprite2x, fl.Pos, Color.White);
            }

            // Draw all the parts of the rocket
            spriteBatch.Draw(builtRocket.motor.sprite2x, new Vector2(builtRocket.pos.X, builtRocket.pos.Y + 128), Color.White);
            spriteBatch.Draw(builtRocket.tank.sprite2x, new Vector2(builtRocket.pos.X, builtRocket.pos.Y + 64), Color.White);
            spriteBatch.Draw(builtRocket.top.sprite2xMulle, builtRocket.pos, Color.White);

            // Draw the rocket exhaust if key is pressed
            if (builtRocket.fuel > 0f)
            {
                if (keyboard.IsKeyDown(Keys.Left) && builtRocket.Rect.Left > Game1.window.ClientBounds.Left)
                {
                    spriteBatch.Draw(rocketFire, new Vector2(builtRocket.Rect.Right + 32, builtRocket.pos.Y + 32), null, Color.White, (float)(Math.PI * 0.5f), new Vector2(), 1f, SpriteEffects.None, 0);
                }
                if (keyboard.IsKeyDown(Keys.Right) && builtRocket.Rect.Right < Game1.window.ClientBounds.Right)
                {
                    spriteBatch.Draw(rocketFire, new Vector2(builtRocket.Rect.Left - 32, builtRocket.pos.Y + 64), null, Color.White, (float)(Math.PI * -0.5f), new Vector2(), 1f, SpriteEffects.None, 0);
                }
                if (keyboard.IsKeyDown(Keys.Up) && builtRocket.Rect.Top > Game1.window.ClientBounds.Top)
                {
                    spriteBatch.Draw(rocketFire2x, new Vector2(builtRocket.Rect.Left, builtRocket.Rect.Bottom), Color.White);
                }
                if (keyboard.IsKeyDown(Keys.Down) && builtRocket.Rect.Bottom < Game1.window.ClientBounds.Bottom)
                {
                    spriteBatch.Draw(rocketFire, new Vector2(builtRocket.Rect.Left + 48, builtRocket.Rect.Top), null, Color.White, (float)(-Math.PI), new Vector2(), 1f, SpriteEffects.None, 0);
                }
            }

            // Draw the stones
            foreach (Stone stn in stones) 
            {
                spriteBatch.Draw(stn.currentSprite, stn.Pos, Color.White);
            }

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

            // Draw the health
            for(int i = 0; i < builtRocket.health; i++)
            {
                spriteBatch.Draw(heartSprite, new Vector2(55 + i * 35, 230), Color.White);
            }

            // Draw menu
            menus.draw(spriteBatch, mouse);
        }
    }
}
