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
        
        private enum state
        {
            patrolling,
            feeding,
            returning,
            full
        }
        private state currentState = state.patrolling;

        private float baseSpeed = 5;
        private float fullSpeed = 1;

        private Vector2 direction;

        private float startingDirection;

        private Vector3 startingPosition;
        private Vector3 chickenLegPosition;

        private int fullTimer;

        #endregion

        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pToken">Token to be associated with the mind.</param>
        public PiranhaMind(X2DToken pToken, Random rand)
            : base(pToken, rand)
        {
            mSpeedX = baseSpeed;

            Console.SetCursorPosition(0, 5);
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
            if ((mAquarium.ChickenLeg != null && currentState != state.full) || (currentState == state.feeding))
            {
                FeedingBehaviour();
            }

            if(currentState == state.returning)
            {
                ReturningBehaviour();
            }

            if(currentState == state.full)
            {
                FullBehaviour();
            }
        }

        private void FeedingBehaviour()
        {
            if (currentState != state.feeding)
            {
                if (currentState == state.patrolling)
                {
                    startingDirection = mFacingDirectionX;

                    startingPosition = tokenPosition;
                }

                chickenLegPosition = mAquarium.ChickenLeg.Position;

                direction = new Vector2(chickenLegPosition.X - tokenPosition.X, chickenLegPosition.Y - tokenPosition.Y);

                if (direction.X > 0)
                {
                    mFacingDirectionX = 1; // Invert horizontal moving direction
                    this.PossessedToken.Orientation = new Vector3(mFacingDirectionX * -1,
                                                                    this.PossessedToken.Orientation.Y,
                                                                    this.PossessedToken.Orientation.Z);
                }
                else if (direction.X < 0)
                {
                    mFacingDirectionX = -1; // Invert horizontal moving direction
                    this.PossessedToken.Orientation = new Vector3(mFacingDirectionX * -1,
                                                                  this.PossessedToken.Orientation.Y,
                                                                  this.PossessedToken.Orientation.Z);
                }

                currentState = state.feeding;

                Console.SetCursorPosition(0, 5);
                Console.Write("Hungry        ");
            }


            direction = new Vector2(chickenLegPosition.X - tokenPosition.X, chickenLegPosition.Y - tokenPosition.Y);
            direction = Vector2.Normalize(direction);
            direction *= mSpeedX;

            Vector3 relativePosition = chickenLegPosition - tokenPosition;

            if ((Math.Abs(relativePosition.X) <= mSize.X / 2) && (Math.Abs(relativePosition.Y) <= mSize.Y / 2)) // If token has passed either horizontal boundary of the aquarium
            {
                mAquarium.RemoveChickenLeg();

                currentState = state.returning;

                mFacingDirectionX *= -1; // Invert horizontal moving direction
                this.PossessedToken.Orientation = new Vector3(mFacingDirectionX * -1,
                                                                this.PossessedToken.Orientation.Y,
                                                                this.PossessedToken.Orientation.Z);

                Console.SetCursorPosition(0, 5);
                Console.Write("Returning        ");
            }
            else
            {
                tokenPosition += new Vector3(direction.X, direction.Y, 0);
            }
        }

        private void ReturningBehaviour()
        {
            Vector2 direction = new Vector2(startingPosition.X - tokenPosition.X, startingPosition.Y - tokenPosition.Y);
            Vector2 direction2 = direction;
            direction = Vector2.Normalize(direction);
            direction *= mSpeedX;

            Vector3 relativePosition = startingPosition - tokenPosition;

            if ((Math.Abs(direction2.X) < mSpeedX) && (Math.Abs(direction2.Y) < mSpeedX)) // If token has passed either horizontal boundary of the aquarium
            {
                currentState = state.full;

                mSpeedX = fullSpeed;

                fullTimer = GetCurrentTime() + 5;

                mFacingDirectionX = startingDirection; // Invert horizontal moving direction
                this.PossessedToken.Orientation = new Vector3(mFacingDirectionX * -1,
                                                                this.PossessedToken.Orientation.Y,
                                                                this.PossessedToken.Orientation.Z);

                Console.SetCursorPosition(0, 5);
                Console.Write("Full        ");
            }
            else
            {
                tokenPosition += new Vector3(direction.X, direction.Y, 0);
            }
        }

        private void FullBehaviour()
        {
            if (GetCurrentTime() > fullTimer)
            {
                mSpeedX = baseSpeed;

                currentState = state.patrolling;

                Console.SetCursorPosition(0, 5);
                Console.Write("Patrolling        ");
            }
            else
            {
                Console.SetCursorPosition(0, 5);
                Console.Write($"Full {fullTimer - GetCurrentTime()}        ");
            }
        }

        private void PatrollingBehaviour()
        {

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
            if((currentState == state.patrolling) || (currentState == state.full))
            {
                Move();
                CheckPosition();
            }

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
