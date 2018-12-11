using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;              // Required to use XNA features.
using XNAMachinationisRatio;                // Required to use the XNA Machinationis Ratio Engine general features.
using XNAMachinationisRatio.AI;             // Required to use the XNA Machinationis Ratio general AI features.

namespace FishORama
{
    class PiranhaMind : AIPlayer
    {
        #region Data Members

        // This mind needs to interact with the token which it possesses, 
        // since it needs to know where are the aquarium's boundaries.
        // Hence, the mind needs a "link" to the aquarium, which is why it stores in
        // an instance variable a reference to its aquarium.
        private AquariumToken mAquarium;        // Reference to the aquarium in which the creature lives.

        private Vector3 mSize; // Size of the possessed tokens' visible dimensions, for collisions

        private Vector3 tokenPosition; // Stores the temporary position of the fish

        private float mFacingDirectionX;         // Horizontal direction the fish is facing (1: right; -1: left).
        private float mFacingDirectionY;         // Vertical direction the fish is facing (1: up; -1: down).

        private float mSpeedX = 5; // Defines horizontal movement speed of the fish
        private float mSpeedY = 0; // Defines vertical movement speed of the fish

        private bool edgeBouncingX; // Determines whether the fish will bounce off the edge of the left & right hand sides of the screen
        private bool edgeBouncingY; // Determines whether the fish will bounce off the edge of the top & bottom sides of the screen

        private bool hitEdgeX; // Set to true if the fish is currently hitting the left or right bounds of the screen
        private bool hitEdgeY; // Set to true if the fish is currently hitting the top or bottom bounds of the screen

        private bool feeding = false;

        private Random mRand; // Store refence to global random number generator

        private Vector3 chickenLegPosition;

        private bool firstUpdate = true;

        #endregion

        #region Properties

        /// <summary>
        /// Set Aquarium in which the mind's behavior should be enacted.
        /// </summary>
        public AquariumToken Aquarium
        {
            set { mAquarium = value; }
        }

        public Vector3 Size
        {
            set { mSize = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pToken">Token to be associated with the mind.</param>
        public PiranhaMind(X2DToken pToken)
        {
            /* LEARNING PILL: associating a mind with a token
             * In order for a mind to control a token, it must be associated with the token.
             * This is done when the mind is constructed, using the method Possess inherited
             * from class AIPlayer.
             */
            this.Possess(pToken);       // Possess token.
            mFacingDirectionX = 1;       // Current direction the fish is facing.   

            edgeBouncingX = true; // Set the fish to bounce off the edges of the screen        

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
                mRand = (PossessedToken as PiranhaToken).Rand;

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
