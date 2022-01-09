using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Fulle_Meck
{
    // Base class for all items with position, texture and hitbox
    public class BaseItem
    {
        public Texture2D sprite;
        public Texture2D sprite2x;
        public Texture2D sprite4x;
        public Texture2D sprite8x;
        public Rectangle rect;
        public Vector2 Pos
        {
            get => new Vector2(rect.X, rect.Y);
            set
            {
                rect.X = (int)value.X;
                rect.Y = (int)value.Y;
            }
        }
    }

    // Class for mokajänger and mojiloxer a.k.a. components
    public class Molijox : BaseItem
    {
        // Enumerate different type of components
        public enum Type
        {
            motor, tank, top
        }

        public Type type;
        public float fuelConsumption;
        public float power; // Generic metric for how good the component is
        public bool grabbed;
        public bool onRocket;
        public bool unlocked;

        public Texture2D sprite2xMulle;

        // Generic constructor for placeholders
        public Molijox()
        {
            grabbed = false;
            onRocket = false;
            unlocked = false;
        }

        // Constructor for when you want to add a component
        public Molijox(Rectangle rect, Texture2D sprite2x, Texture2D sprite8x, Type type, float fuelConsumption, float power, bool unlocked)
        {
            this.rect = rect;
            this.sprite2x = sprite2x;
            this.sprite8x = sprite8x;
            this.type = type;
            grabbed = false;
            onRocket = false;
            this.fuelConsumption = fuelConsumption;
            this.power = power;
            this.unlocked = unlocked;
        }
        // Constructor like previous but contains special texture with Mulle
        public Molijox(Rectangle rect, Texture2D sprite2x, Texture2D sprite2xMulle, Texture2D sprite8x, Type type, float fuelConsumption, float power, bool unlocked) : this(rect, sprite2x, sprite8x, type, fuelConsumption, power, unlocked)
        {
            this.sprite2xMulle = sprite2xMulle;
        }
    }

    // Class for stones
    public class Stone : BaseItem
    {
        public int size;
        public float speed;
        public Texture2D currentSprite;

        // Used for debouncing of collision
        public bool collided;

        public Stone(int level)
        {
            collided = false;

            // Set the sprites
            sprite = ShootemUp.stoneSprite;
            sprite2x = ShootemUp.stoneSprite2x;
            sprite4x = ShootemUp.stoneSprite4x;
            sprite8x = ShootemUp.stoneSprite8x;

            // Randomize the properties of the stone created
            Random rnd = new Random();
            speed = rnd.Next(3, level + 4);
            Pos = new Vector2(rnd.Next(0, 1920), rnd.Next(-Game1.window.ClientBounds.Height * 2, 0));
            size = rnd.Next(0, level);


            // Determine the sprite to actually use and size of the hitbox depending on the size
            switch (size)
            {
                case 0:
                    currentSprite = sprite;

                    rect.Width = sprite.Width;
                    rect.Height = sprite.Height;
                    break;

                case 1:
                    currentSprite = sprite2x;

                    rect.Width = sprite2x.Width;
                    rect.Height = sprite2x.Height;
                    break;

                case 2:
                    currentSprite = sprite4x;

                    rect.Width = sprite4x.Width;
                    rect.Height = sprite4x.Height;
                    break;

                case 3:
                    currentSprite = sprite8x;

                    rect.Width = sprite8x.Width;
                    rect.Height = sprite8x.Height;
                    break;

                default:
                    currentSprite = sprite;

                    rect.Width = sprite.Width;
                    rect.Height = sprite.Height;
                    break;
            }
        }
    }

    // Class for pickable fuel in shootemup
    public class FuelItem : BaseItem
    {
        public float speed;

        public FuelItem(int level)
        {
            sprite2x = ShootemUp.fuelSprite2x;

            // Randomize the properties of the stone created
            Random rnd = new Random();
            rect = new Rectangle(rnd.Next(0, 1920), rnd.Next(-Game1.window.ClientBounds.Height * 2, 0), sprite2x.Width, sprite2x.Height);
            speed = rnd.Next(3, level + 4);
        }
    }
}
