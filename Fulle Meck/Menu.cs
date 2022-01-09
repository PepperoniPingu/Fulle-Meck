using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Fulle_Meck
{
    // Class for a menu
    public class Menu
    {
        public List<Element> elements = new List<Element>();

        public class Element
        {
            public Rectangle rect;
            public Vector2 Pos()
            {
                return new Vector2(rect.X, rect.Y);
            }
            public string text;
            public Vector2 textPos;
            public Color color;
            // The buttons need to do something therefore create a delegate
            public delegate void handler();
            public handler method;
            // Menus can be locked
            public Unlocked unlocked;

            // The different types of menus
            public enum Type
            {
                text, button
            }

            public Type type;

            // Constructor for when you create a text element
            public Element(Vector2 _pos, string _text, Type _type)
            {
                type = _type;
                text = _text;
                textPos = new Vector2(
                    _pos.X - (Convert.ToInt32(Game1.gameFont.MeasureString(text).X)) / 2, 
                    _pos.Y - (Convert.ToInt32(Game1.gameFont.MeasureString(text).Y)) / 2
                    );
            }
            // Constructor for when you create a button
            public Element(Rectangle _rect, string _text, Type _type, handler _handler, Color _color)
            {
                rect = new Rectangle(_rect.X - _rect.Width / 2, _rect.Y - _rect.Height / 2, _rect.Width, _rect.Height);
                type = _type;
                text = _text;
                textPos = new Vector2(
                    rect.X + (rect.Width - Convert.ToInt32(Game1.gameFont.MeasureString(text).X)) / 2,
                    rect.Y + (rect.Height - Convert.ToInt32(Game1.gameFont.MeasureString(text).Y)) / 2
                    );
                method = _handler;
                color = _color;
                unlocked = new Unlocked(true);
            }
            // Constructor for when you create a locked button
            public Element(Rectangle _rect, string _text, Type _type, handler _handler, Color _color, Unlocked _unlocked)
            {
                rect = new Rectangle(_rect.X - _rect.Width / 2, _rect.Y - _rect.Height / 2, _rect.Width, _rect.Height);
                type = _type;
                text = _text;
                textPos = new Vector2(
                    rect.X + (rect.Width - Convert.ToInt32(Game1.gameFont.MeasureString(text).X)) / 2,
                    rect.Y + (rect.Height - Convert.ToInt32(Game1.gameFont.MeasureString(text).Y)) / 2
                    );
                method = _handler;
                color = _color;
                unlocked = _unlocked;
            }
        }
    }

    public static class menus
    {
        // Dictionary of all menus
        public static Dictionary<string, Menu> dictionary = new Dictionary<string, Menu>();

        // The menu displayed at the moment
        public static string currentMenu = "start";

        // Method for creating menus
        public static void createMenu(string name, List<Menu.Element> _elements)
        {
            Menu menu = new Menu();

            foreach (Menu.Element element in _elements.ToList())
            {
                menu.elements.Add(element);
            }

            dictionary.Add(name, menu);
        }

        // Update method
        public static void update(MouseState mouse)
        {
            // Check if the current menus has a any buttons that are being clicked
            foreach (Menu.Element element in menus.dictionary[menus.currentMenu].elements.ToList())
            {
                if (element.rect.Contains(mouse.X, mouse.Y) && Game1.mouseRealesedEvent(mouse) && element.type == Menu.Element.Type.button && element.unlocked.var)
                {
                    Game1.motorSoundInstance.Stop();
                    // Execute the handler
                    element.method();
                }
            }
        }

        // Draw method for current menu
        public static void draw(SpriteBatch spriteBatch, MouseState mouse)
        {
            foreach (Menu.Element element in dictionary[currentMenu].elements.ToList())
            {
                // Draw buttons
                if (element.type == Menu.Element.Type.button)
                {
                    Texture2D _texture;
                    _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                    _texture.SetData(new Color[] { Color.Black });
                    spriteBatch.Draw(_texture, element.rect, Color.White);

                    // Determine if the button should be darker if it is pressed
                    Color mixColor = new Color((mouse.LeftButton == ButtonState.Pressed && element.rect.Contains(mouse.Position.ToVector2()) || !element.unlocked.var) ? Color.Gray : Color.White, 1f);

                    _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                    _texture.SetData(new Color[] { element.color });
                    spriteBatch.Draw(_texture, new Rectangle(element.rect.X + 5, element.rect.Y + 5, element.rect.Width - 10, element.rect.Height - 10), mixColor);

                    spriteBatch.DrawString(Game1.gameFont, element.text, element.textPos, mixColor);

                } else if (element.type == Menu.Element.Type.text)
                {
                    spriteBatch.DrawString(Game1.gameFont, element.text, element.textPos, Color.White);
                }

            }
        }
    }
}
