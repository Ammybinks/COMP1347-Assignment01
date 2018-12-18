using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;              // Required to use XNA features.
using XNAMachinationisRatio;                // Required to use the XNA Machinationis Ratio Engine general features.
using XNAMachinationisRatio.AI;             // Required to use the XNA Machinationis Ratio general AI features.

namespace FishORama
{
    class PiranhaMind : BaseFishMind
    {
        #region Data Members
        
        private bool feeding = false;
        
        private Vector3 chickenLegPosition;
        
        #endregion

        #region Properties
        
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pToken">Token to be associated with the mind.</param>
        public PiranhaMind(X2DToken pToken)
            :base(pToken)
        {
            mSpeedX = 5;

            Console.SetCursorPosition(0, 9);
            Console.Write("Regular        ");
        }

        #endregion

        #region Methods


        /* LEARNING PILL: The AI update method.
         * Mind objects enact behaviors through the method Update. This method is
         * automatically invoked by the engine, periodically, 'under the hood'. This can be
         * be better understood that the engine asks to all the available AI-based tokens:
         * "Would you like to do anything at all?" And this 'asking' is done through invoking
         * the Update method of each mind available in the system. The response is the execution
         * of the Update method of each mind , and all the methods possibly triggered by Update.
         * 
         * Although the Update method could invoke other methods if needed, EVERY
         * BEHAVIOR STARTS from Update. If a behavior is not directly coded in Updated, or in
         * a method invoked by Update, then it is IGNORED.
         * 
         */

        private void SpecialBehaviour()
        {
            if (mAquarium.ChickenLeg != null)
            {
                if(!feeding)
                {
                    feeding = true;

                    (PossessedToken as PiranhaToken).SwitchSprite();

                    Console.SetCursorPosition(0, 9);
                    Console.Write("Hungry        ");
                }

                chickenLegPosition = mAquarium.ChickenLeg.Position;

                Vector2 tempPosition1 = new Vector2(chickenLegPosition.X - tokenPosition.X, chickenLegPosition.Y - tokenPosition.Y);
                Vector2 tempPosition2 = tempPosition1;
                tempPosition1 = Vector2.Normalize(tempPosition1);
                tempPosition1 *= mSpeedX;

                if(Math.Abs(tempPosition1.X) > Math.Abs(tempPosition2.X) && Math.Abs(tempPosition1.Y) > Math.Abs(tempPosition2.Y))
                {
                    mAquarium.RemoveChickenLeg();

                    feeding = false;

                    Console.SetCursorPosition(0, 9);
                    Console.Write("Regular        ");
                }
                else
                {
                    if (tempPosition1.X > 0)
                    {
                        mFacingDirectionX = 1; // Invert horizontal moving direction
                        this.PossessedToken.Orientation = new Vector3(mFacingDirectionX * -1,
                                                                      this.PossessedToken.Orientation.Y,
                                                                      this.PossessedToken.Orientation.Z);
                    }
                    else
                    {
                        mFacingDirectionX = -1; // Invert horizontal moving direction
                        this.PossessedToken.Orientation = new Vector3(mFacingDirectionX * -1,
                                                                      this.PossessedToken.Orientation.Y,
                                                                      this.PossessedToken.Orientation.Z);
                    }

                    tokenPosition += new Vector3(tempPosition1.X, tempPosition1.Y, 0);
                }
            }
        }

        /// <summary>
        /// Adds the speed of the fish in both movement axes to the fishes current position
        /// </summary>
        private void Move()
        {
            if(!feeding)
            {
                tokenPosition.X += mSpeedX * mFacingDirectionX;
                tokenPosition.Y += mSpeedY * mFacingDirectionY;
            }
        }

        /// <summary>
        /// Checks the current position of the fish, ensuring it doesn't leave the bounds of the aquarium
        /// </summary>
        private void CheckPosition()
        {
            Vector3 relativePosition = tokenPosition - mAquarium.Position;

            if (Math.Abs(relativePosition.X) + mSize.X >= (mAquarium.Width / 2)) // If token has passed either horizontal boundary of the aquarium
            {
                if (relativePosition.X <= 0) // If the left edge of the screen was hit
                {
                    tokenPosition.X = ((mAquarium.Width / 2) - mSize.X) * -1; // Lock fish to left edge of screen
                }
                else if (relativePosition.X > 0) // If the right edge of the screen was hit
                {
                    tokenPosition.X = (mAquarium.Width / 2) - mSize.X; // Lock fish to right edge of screen
                }

                if (edgeBouncingX) // If fish should bounce at this edge
                {
                    mFacingDirectionX *= -1; // Invert horizontal moving direction
                    this.PossessedToken.Orientation = new Vector3(mFacingDirectionX * -1,
                                                                  this.PossessedToken.Orientation.Y,
                                                                  this.PossessedToken.Orientation.Z);
                }

                hitEdgeX = true;
            }
            else
            {
                hitEdgeX = false;
            }

            if (Math.Abs(relativePosition.Y) + mSize.Y >= (mAquarium.Height / 2)) // If token has passed either vertical boundary of the aquarium
            {
                if (relativePosition.Y <= 0) // If the bottom edge of the screen was hit
                {
                    tokenPosition.Y = ((mAquarium.Height / 2) - mSize.Y) * -1; // Lock fish to bottom edge of screen
                }
                else if (relativePosition.Y > 0) // If the top edge of the screen was hit
                {
                    tokenPosition.Y = (mAquarium.Height / 2) - mSize.Y; // Lock fish to top of screen
                }

                if (edgeBouncingY) // If fish should bounce at this edge
                {
                    mFacingDirectionY *= -1; // Invert vertical moving direction
                }

                hitEdgeY = true;
            }
            else
            {
                hitEdgeY = false;
            }
        }

        /// <summary>
        /// AI Update method.
        /// </summary>
        /// <param name="pGameTime">Game time</param>
        public override void Update(ref GameTime pGameTime)
        {
            if (firstUpdate)
            {
                firstUpdate = false;
            }

            tokenPosition = PossessedToken.Position; // Store the current position of the fish

            SpecialBehaviour();
            Move();
            CheckPosition();

            PossessedToken.Position = tokenPosition; // Set the token's current position to the new one, after all movements


            /* LEARNING PILL: This is a special method which gets called over and over again from somewhere within the FishORama framework based on Game time (A timer)
            *  the method in thoery is like a loop, but after each iteration unlike a look, it will allow you see what happened during that iteration
            *  this acts like frames of animation, so each iteration will be a new frame.  If we move the fish 1 pixel per iteration you will see that happen
            *  during 'game time'.
            */
        }


        #endregion
    }
}
