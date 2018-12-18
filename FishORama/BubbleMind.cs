using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;              // Required to use XNA features.
using XNAMachinationisRatio;                // Required to use the XNA Machinationis Ratio Engine general features.
using XNAMachinationisRatio.AI;             // Required to use the XNA Machinationis Ratio general AI features.


namespace FishORama
{
    class BubbleMind : BaseFishMind
    {
        #region Data Members

        private float distanceFloated;
        private float distanceToFloat;

        #endregion

        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pToken">Token to be associated with the mind.</param>
        public BubbleMind(X2DToken pToken)
            : base(pToken)
        {
            mFacingDirectionY = 1;
            mSpeedY = 3;
            mSpeedX = 0;

            distanceFloated = 0;
            distanceToFloat = 150;
        }

        #endregion

        #region Methods

        private void SpecialBehaviour()
        {
            distanceFloated += mSpeedY;

            if(distanceFloated >= distanceToFloat)
            {
                tokenPosition = (PossessedToken as BubbleToken).AttatchedFishPosition;

                mSpeedY = mRand.Next(3, 6);

                distanceFloated = 0;
            }

            
        }

        new private void CheckPosition()
        {

        }


        /// <summary>
        /// AI Update method.
        /// </summary>
        /// <param name="pGameTime">Game time</param>
        public override void Update(ref GameTime pGameTime)
        {
            CheckFirstUpdate();

            tokenPosition = PossessedToken.Position; // Store the current position of the fish

            Move();

            SpecialBehaviour();

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
