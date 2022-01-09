using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Fulle_Meck
{
    class Building
    {
        // Textures for the outlines of where molijoxer can be placed
        public static Texture2D motorStencil;
        public static Texture2D tankStencil;
        public static Texture2D topStencil;
        // Positions for the outlines
        private static Vector2 motorStencilPos;
        private static Vector2 tankStencilPos;
        private static Vector2 topStencilPos;
        // Rectangles for the outlines
        private static Rectangle motorStencilRect;
        private static Rectangle tankStencilRect;
        private static Rectangle topStencilRect;

        // Flag for if an item is grabbed
        private static bool grabbed;
        // Reference to grabbed item
        private static Molijox grabbedMoj;
        // Remember the grabbed items position in relation to the mouse cursor
        private static Vector2 mouseOffset;

        // The built rocket
        private static class builtRocket
        {
            public static Molijox motor = new Molijox();
            public static Molijox tank = new Molijox();
            public static Molijox top = new Molijox();
        }


        // Function that initializes values before launching the rocket. Sets components positions
        public static void initialize()
        {
            Game1.gameState = Game1.GameStates.build;
            menus.currentMenu = "building";
            grabbedMoj = new Molijox();
            grabbed = false;

            // Initialize the position for all of the stencils
            motorStencilPos = new Vector2(
                Game1.window.ClientBounds.Width / 2 - 128,
                Convert.ToInt32(Game1.window.ClientBounds.Height * 0.6)
                );

            tankStencilPos = new Vector2(
                Game1.window.ClientBounds.Width / 2 - 128,
                 Convert.ToInt32(Game1.window.ClientBounds.Height * 0.6 - 256)
                );

            topStencilPos = new Vector2(
                motorStencilPos.X,
                motorStencilPos.Y - 256 * 2
                );

            // Initialize the rectangles for all stencils used for intersection detection
            motorStencilRect = new Rectangle(Convert.ToInt32(motorStencilPos.X), Convert.ToInt32(motorStencilPos.Y), 256, 256);
            tankStencilRect = new Rectangle(Convert.ToInt32(tankStencilPos.X), Convert.ToInt32(tankStencilPos.Y), 256, 256);
            topStencilRect = new Rectangle(Convert.ToInt32(topStencilPos.X), Convert.ToInt32(topStencilPos.Y), 256, 256);

            foreach (Molijox moj in Game1.molijoxer.ToList())
            {
                // If the component is part of the built rocket, make sure to define it as part of the built rocket
                if (moj.onRocket)
                {
                    switch (moj.type)
                    {
                        case Molijox.Type.motor:
                            moj.Pos = motorStencilPos;
                            break;

                        case Molijox.Type.tank:
                            moj.Pos = tankStencilPos;
                            break;

                        case Molijox.Type.top:
                            moj.Pos = topStencilPos;
                            break;

                        default:
                            break;
                    }
                }
                else // Otherwize, just randomize the positions if component is not used
                {
                    Random rnd = new Random();
                    moj.Pos = new Vector2(
                        rnd.Next(Game1.window.ClientBounds.Width - moj.sprite8x.Width),
                        rnd.Next(Game1.window.ClientBounds.Height - moj.sprite8x.Height)
                        );
                }
            }
        }

        // Update routine for building game mode
        public static void update(MouseState mouse)
        {
            // If no item is grabbed and the mouse button is pressed, try to grab an item
            if (mouse.LeftButton == ButtonState.Pressed && !grabbed)
            {
                foreach (Molijox moj in Game1.molijoxer)
                {
                    moj.rect = new Rectangle(moj.rect.X, moj.rect.Y, 256, 256);
                    if (moj.rect.Contains(mouse.Position) && moj.unlocked)
                    {
                        grabbed = true;
                        grabbedMoj = moj;
                        mouseOffset.X = mouse.X - moj.Pos.X;
                        mouseOffset.Y = mouse.Y - moj.Pos.Y;
                    }
                }
            }
            // If an item is being dropped
            else if (mouse.LeftButton == ButtonState.Released && grabbed)
            {
                // Set the flag
                grabbed = false;
                // Create a rectangle for the dropped 
                grabbedMoj.rect = new Rectangle(grabbedMoj.rect.X, grabbedMoj.rect.Y, 256, 256);

                // Depending on the dropped items type snap it into different stencil positions
                switch (grabbedMoj.type) {
                    case Molijox.Type.motor:
                        if (motorStencilRect.Contains(grabbedMoj.rect.Center))
                        {
                            // Randomize the previous snapped items position
                            Random rnd = new Random();
                            if (builtRocket.motor.onRocket)
                            {
                                while (motorStencilRect.Intersects(builtRocket.motor.rect))builtRocket.motor.Pos = new Vector2(
                                   rnd.Next(Game1.window.ClientBounds.Width - 256),
                                   rnd.Next(Game1.window.ClientBounds.Height - 256)
                                   );
                                builtRocket.motor.onRocket = false;
                            }
                            // Snap the position
                            grabbedMoj.Pos = new Vector2(motorStencilRect.X, motorStencilRect.Y);
                            // Mark the item as on the rocket
                            grabbedMoj.onRocket = true;
                            builtRocket.motor = grabbedMoj;
                            // Play snap sound
                            Game1.snapSoundInstance.Play();
                        }
                        else grabbedMoj.onRocket = false; 
                        break;

                    case Molijox.Type.tank:
                        if (tankStencilRect.Contains(grabbedMoj.rect.Center))
                        {
                            // Randomize the previous snapped items position
                            Random rnd = new Random();
                            if (builtRocket.tank.onRocket)
                            {
                                while (tankStencilRect.Intersects(builtRocket.tank.rect)) builtRocket.tank.Pos = new Vector2(
                                    rnd.Next(Game1.window.ClientBounds.Width - 256),
                                    rnd.Next(Game1.window.ClientBounds.Height - 256)
                                    );
                                builtRocket.tank.onRocket = false;
                            }
                            // Snap the position
                            grabbedMoj.Pos = new Vector2(tankStencilRect.X, tankStencilRect.Y);
                            // Mark the item as on the rocket
                            grabbedMoj.onRocket = true;
                            builtRocket.tank = grabbedMoj;
                            // Play snap sound
                            Game1.snapSoundInstance.Play();
                        }
                        else grabbedMoj.onRocket = false;
                        break;

                    case Molijox.Type.top:
                        if (topStencilRect.Contains(grabbedMoj.rect.Center))
                        {
                            // Randomize the previous snapped items position
                            Random rnd = new Random();
                            if (builtRocket.top.onRocket)
                            {
                                while (topStencilRect.Intersects(builtRocket.top.rect)) builtRocket.top.Pos = new Vector2(
                                    rnd.Next(Game1.window.ClientBounds.Width - 256),
                                    rnd.Next(Game1.window.ClientBounds.Height - 256)
                                    );
                                builtRocket.top.onRocket = false;
                            }
                            // Snap the position
                            grabbedMoj.Pos = new Vector2(topStencilRect.X, topStencilRect.Y);
                            // Mark the item as on the rocket
                            grabbedMoj.onRocket = true;
                            builtRocket.top = grabbedMoj;
                            // Play snap sound
                            Game1.snapSoundInstance.Play();
                        }
                        else grabbedMoj.onRocket = false;
                        break;

                    default:
                        break;
                }
            }

            // If an item is grabbed, move it
            if (grabbed)
            {
                grabbedMoj.Pos = new Vector2(mouse.X - mouseOffset.X, mouse.Y - mouseOffset.Y);
            }

            // Update menus
            menus.update(mouse);

        }

        // Drawing routine for building game mode
        public static void draw(SpriteBatch spriteBatch, MouseState mouse)
        {
            // Draw outlines
            spriteBatch.Draw(motorStencil, motorStencilPos, Color.White);
            spriteBatch.Draw(tankStencil, tankStencilPos, Color.White);
            spriteBatch.Draw(topStencil, topStencilPos, Color.White);

            // Draw all unlocked molijoxer in order

            // Draw the ones on the rocket first
            foreach (Molijox moj in Game1.molijoxer.ToList())
            {
                if (moj.unlocked && moj.onRocket)
                {
                    spriteBatch.Draw(moj.sprite8x, moj.Pos, Color.White);
                }
            }
            // Then draw the ones floating around
            foreach (Molijox moj in Game1.molijoxer.ToList())
            {
                if (moj.unlocked && !moj.onRocket)
                {
                    spriteBatch.Draw(moj.sprite8x, moj.Pos, Color.White);
                }
            }
            // Lastly, draw the grabbed one
            if (grabbed) spriteBatch.Draw(grabbedMoj.sprite8x, grabbedMoj.Pos, Color.White);

            // Draw menu
            menus.draw(spriteBatch, mouse);
        }
    }
}
