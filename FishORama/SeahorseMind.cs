using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;              // Required to use XNA features.
using XNAMachinationisRatio;                // Required to use the XNA Machinationis Ratio Engine general features.
using XNAMachinationisRatio.AI;             // Required to use the XNA Machinationis Ratio general AI features.

namespace FishORama
{
    class SeahorseMind : BaseFishMind
    {
        #region Data Members

        private Vector3 startingPosition;

        private float distanceSwum; // Distance the fish has swum since the beginning of its special behaviour
        private float distanceToSwim; // Distance the fish should swim before ending any given behaviour
        
        private bool scared = false;
        private bool chickenLegActive;

        private float currentSin;
        private float sinIncrement;
        private float sinIntensity;

        #endregion

        #region Properties
        
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pToken">Token to be associated with the mind.</param>
        public SeahorseMind(X2DToken pToken, Random rand)
            : base(pToken, rand)
        {
            /* LEARNING PILL: associating a mind with a token
             * In order for a mind to control a token, it must be associated with the token.
             * This is done when the mind is constructed, using the method Possess inherited
             * from class AIPlayer.
             */
            mFacingDirectionY = -1;  
            
            edgeBouncingY = true;

            mSpeedX = mRand.Next(1, 6);
            mSpeedY = mSpeedX;

            distanceToSwim = 100;

            currentSin = 0;
            sinIntensity = 100f;
            sinIncrement = mSpeedX / 100;
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
            if (mAquarium.ChickenLeg != null) // If chicken leg is placed in the aquarium
            {
                if(!chickenLegActive) // For the first update before chickenLegActive is set to true
                {
                    scared = true;

                    edgeBouncingX = false;
                    edgeBouncingY = false;

                    chickenLegActive = true;

                    distanceSwum = 0; // Reset distance the fish has swum
                }
            }
            else if (chickenLegActive)
            {
                chickenLegActive = false;
            }

            if(scared)
            {
                Vector3 chickenLegPosition = mAquarium.ChickenLeg.Position;

                Vector2 tempPosition1 = new Vector2(tokenPosition.X - chickenLegPosition.X, tokenPosition.Y - chickenLegPosition.Y);
                tempPosition1 = Vector2.Normalize(tempPosition1);
                tempPosition1 *= mSpeedX + 5;
                
                if (distanceSwum >= distanceToSwim)
                {
                    SetRegularBehaviour();
                }
                else
                {
                    if (tempPosition1.X > 0)
                    {
                        mFacingDirectionX = 1; // Invert horizontal moving direction
                        PossessedToken.Orientation = new Vector3(mFacingDirectionX * -1,
                                                                      PossessedToken.Orientation.Y,
                                                                      PossessedToken.Orientation.Z);
                    }
                    else
                    {
                        mFacingDirectionX = -1; // Invert horizontal moving direction
                        PossessedToken.Orientation = new Vector3(mFacingDirectionX * -1,
                                                                      PossessedToken.Orientation.Y,
                                                                      PossessedToken.Orientation.Z);
                    }

                    tokenPosition += new Vector3(tempPosition1.X, tempPosition1.Y, 0);

                    distanceSwum += mSpeedX + 5;
                }
            }
            /* Disabled code for implementing sine wave movement behaviour
            else
            {
                tokenPosition = new Vector3(tokenPosition.X, startingPosition.Y + ((float)Math.Sin(currentSin) * sinIntensity), startingPosition.Z);

                currentSin += sinIncrement;
            }
            */
        }

        /// <summary>
        /// Returns the fish to its default movement behaviour, either when the program starts or after the fish stops running from the chicken leg
        /// </summary>
        private void SetRegularBehaviour()
        {
            scared = false;

            edgeBouncingX = true;
            edgeBouncingY = true;
            
            startingPosition = PossessedToken.Position;
        }

        /// <summary>
        /// Checks the current position of the fish, ensuring it doesn't leave the bounds of the aquarium
        /// </summary>
        private new void CheckPosition()
        {
            Vector3 aquariumRelativePosition = tokenPosition - mAquarium.Position;
            Vector3 startingRelativePosition = tokenPosition - startingPosition;

            if (Math.Abs(aquariumRelativePosition.X) + mSize.X >= (mAquarium.Width / 2)) // If token has passed either horizontal boundary of the aquarium
            {
                if (aquariumRelativePosition.X <= 0) // If the left edge of the screen was hit
                {
                    tokenPosition.X = ((mAquarium.Width / 2) - mSize.X) * -1; // Lock fish to left edge of screen
                }
                else if (aquariumRelativePosition.X > 0) // If the right edge of the screen was hit
                {
                    tokenPosition.X = (mAquarium.Width / 2) - mSize.X; // Lock fish to right edge of screen
                }

                if (edgeBouncingX) // If fish should bounce at this edge
                {
                    mFacingDirectionX *= -1; // Invert horizontal moving direction
                    PossessedToken.Orientation = new Vector3(mFacingDirectionX * mRenderDirection,
                                                                  PossessedToken.Orientation.Y,
                                                                  PossessedToken.Orientation.Z);
                }

                hitEdgeX = true;
            }
            else
            {
                hitEdgeX = false;
            }

            if ((Math.Abs(aquariumRelativePosition.Y) + mSize.Y >= (mAquarium.Height / 2))) // If token has passed either vertical boundary of the aquarium
            {
                if (aquariumRelativePosition.Y <= 0) // If the bottom edge of the screen was hit
                {
                    tokenPosition.Y = ((mAquarium.Height / 2) - mSize.Y) * -1; // Lock fish to bottom edge of screen
                }
                else if (aquariumRelativePosition.Y > 0) // If the top edge of the screen was hit
                {
                    tokenPosition.Y = (mAquarium.Height / 2) - mSize.Y; // Lock fish to top of screen
                }

                if (edgeBouncingY) // If fish should bounce at this edge
                {
                    mFacingDirectionY *= -1; // Invert vertical moving direction
                }

                hitEdgeY = true;
            }
            else if (Math.Abs(startingRelativePosition.Y) >= distanceToSwim) // If token has swum up or down far enough to turn around
            {
                mFacingDirectionY *= -1;
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
            if(firstUpdate)
            {
                SetRegularBehaviour();

                firstUpdate = false;
            }

            tokenPosition = PossessedToken.Position; // Store the current position of the fish

            if (!scared)
            {
                Move();
            }

            SpecialBehaviour();

            CheckPosition();

            PossessedToken.Position = tokenPosition; // Set the token's current position to the new one, after all movements

            Console.SetCursorPosition(0, 2 + (PossessedToken as SeahorseToken).Index);
            Console.Write($"{mFacingDirectionY}, {scared}                ");

            /* LEARNING PILL: This is a special method which gets called over and over again from somewhere within the FishORama framework based on Game time (A timer)
            *  the method in thoery is like a loop, but after each iteration unlike a look, it will allow you see what happened during that iteration
            *  this acts like frames of animation, so each iteration will be a new frame.  If we move the fish 1 pixel per iteration you will see that happen
            *  during 'game time'.
            */
        }


        #endregion
    }
}
