using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Fulle_Meck
{
    class ShootemUpResult
    {
        private static Molijox _molijox;

        // Initialize method
        public static void initialize(bool win, Molijox molijox)
        {
            _molijox = molijox;

            Game1.gameState = Game1.GameStates.shootemUpResult;
            if (win)
            {
                menus.currentMenu = _molijox.unlocked ? "levelClearedMoj" : "levelCleared";
            } else menus.currentMenu = "gameOver";
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

            // Draw won molijox
            if (_molijox.unlocked) spriteBatch.Draw(_molijox.sprite8x, new Vector2(Game1.window.ClientBounds.Width / 2 - 128, Game1.window.ClientBounds.Height / 2 - 128), Color.White);

        }

    }
}
