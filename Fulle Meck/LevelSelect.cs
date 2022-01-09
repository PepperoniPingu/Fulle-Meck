using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Fulle_Meck
{
    class LevelSelect
    {
        // Initialize method
        public static void initialize()
        {
            Game1.gameState = Game1.GameStates.levelSelect;
            menus.currentMenu = "levelSelect";

        }

        // Update method
        public static void update(MouseState mouse)
        {
            // Update menu
            menus.update(mouse);

        }

        // Draw method
        public static void draw(SpriteBatch spriteBatch, MouseState mouse)
        {
            // Make the background dark gray
            Texture2D background = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            background.SetData(new Color[] { new Color(20, 20, 20) });
            spriteBatch.Draw(background, new Rectangle(0, 0, Game1.window.ClientBounds.Width, Game1.window.ClientBounds.Height), Color.White);

            // Draw menu
            menus.draw(spriteBatch, mouse);

        }

    }
}
