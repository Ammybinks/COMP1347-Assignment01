using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;              // Required to use XNA features.
using XNAMachinationisRatio;                // Required to use the XNA Machinationis Ratio Engine general features.
using XNAMachinationisRatio.AI;             // Required to use the XNA Machinationis Ratio general AI features.

namespace FishORama
{
    class BaseFishMind :AIPlayer
    {
        #region Data Members

        // This mind needs to interact with the token which it possesses, 
        // since it needs to know where are the aquarium's boundaries.
        // Hence, the mind needs a "link" to the aquarium, which is why it stores in
        // an instance variable a reference to its aquarium.
        private AquariumToken mAquarium;        // Reference to the aquarium in which the creature lives.

        private float mFacingDirectionX;         // Horizontal direction the fish is facing (1: right; -1: left).
        private float mFacingDirectionY;         // Vertical direction the fish is facing (1: up; -1: down).

        private float mSpeedX = 2; // Defines horizontal movement speed of the fish
        private float mSpeedY = 2; // Defines vertical movement speed of the fish

        private enum Action
        {
            None,
            Dashing,
            Accelerating,
            Hungry,
            Sinking
        }
        private Action currentAction = Action.None; // Holds the current special action the fish is taking; holds 'None' while fish is using regular behaviour

        #endregion

        #region Properties

        /// <summary>
        /// Set Aquarium in which the mind's behavior should be enacted.
        /// </summary>
        public AquariumToken Aquarium
        {
            set { mAquarium = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pToken">Token to be associated with the mind.</param>
        public BaseFishMind(X2DToken pToken)
        {
            /* LEARNING PILL: associating a mind with a token
             * In order for a mind to control a token, it must be associated with the token.
             * This is done when the mind is constructed, using the method Possess inherited
             * from class AIPlayer.
             */
            this.Possess(pToken);       // Possess token.
            mFacingDirectionX = 1;       // Current direction the fish is facing.            
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

        /// <summary>
        /// Adds the speed of the fish in both movement axes to the fishes current position
        /// </summary>
        /// <param name="tokenPosition">Current position of the fish</param>
        /// <returns>The fishes new position</returns>
        private Vector3 Move(Vector3 tokenPosition)
        {
            tokenPosition.X += mSpeedX;
            tokenPosition.Y += mSpeedY;

            return tokenPosition;
        }

        /// <summary>
        /// Clamps fish to horizontal edge of screen
        /// </summary>
        /// <param name="amount">The distance the fish is beyond the respective edge</param>
        /// <param name="direction">Which direction the fish should be moved</param>
        /// <param name="tokenPosition">Current position of the fish</param>
        /// <returns>The fishes new position</returns>
        private Vector3 ClampToScreenX(float amount, int direction, Vector3 tokenPosition)
        {
            tokenPosition.X += amount * direction;

            return tokenPosition;
        }
        /// <summary>
        /// Clamps fish to horizontal edge of screen
        /// </summary>
        /// <param name="amount">The distance the fish is beyond the respective edge</param>
        /// <param name="direction">Which direction the fish should be moved</param>
        /// <param name="tokenPosition">Current position of the fish</param>
        /// <returns>The fishes new position</returns>
        private Vector3 ClampToScreenY(float amount, int direction, Vector3 tokenPosition)
        {
            tokenPosition.Y += amount * direction;

            return tokenPosition;
        }

        /// <summary>
        /// Checks the current position of the fish, ensuring it doesn't leave the bounds of the aquarium
        /// </summary>
        /// <param name="tokenPosition">Current position of the fish</param>
        /// <returns>The fishes new position</returns>
        private Vector3 CheckPosition(Vector3 tokenPosition)
        {
            if (Math.Abs(tokenPosition.X - mAquarium.Position.X) >= (mAquarium.Width / 2)) // If token has passed either horizontal boundary of the aquarium
            {
                float extraPosition = Math.Abs(tokenPosition.X - mAquarium.Position.X) - mAquarium.Width / 2; // Store the distance the fish has swum past the boundary
                int direction = 1;

                if (tokenPosition.X < 0) // If the left edge of the screen was hit
                {
                    direction = 1; // Set which direction to move the fish to keep it on the screen
                }
                else if (tokenPosition.X > 0) // If the right edge of the screen was hit
                {
                    direction = -1; // Set which direction to move the fish to keep it on the screen
                }

                tokenPosition = ClampToScreenX(extraPosition, direction, tokenPosition);
            }
            if (Math.Abs(tokenPosition.Y - mAquarium.Position.Y) >= (mAquarium.Height / 2)) // If token has passed either vertical boundary of the aquarium
            {
                float extraPosition = Math.Abs(tokenPosition.Y - mAquarium.Position.Y) - mAquarium.Height / 2; // Store the distance the fish has swum past the boundary
                int direction = 1;

                if (tokenPosition.Y < 0) // If the bottom edge of the screen was hit
                {
                    direction = 1; // Set which direction to move the fish to keep it on the screen
                }
                else if (tokenPosition.Y > 0) // If the top edge of the screen was hit
                {
                    direction = -1; // Set which direction to move the fish to keep it on the screen
                }

                tokenPosition = ClampToScreenY(extraPosition, direction, tokenPosition);
            }

            return tokenPosition;
        }

        /// <summary>
        /// AI Update method.
        /// </summary>
        /// <param name="pGameTime">Game time</param>
        public override void Update(ref GameTime pGameTime)
        {
            Vector3 tokenPosition = PossessedToken.Position; // Store the current position of the fish

            tokenPosition = Move(tokenPosition);
            tokenPosition = CheckPosition(tokenPosition);

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
