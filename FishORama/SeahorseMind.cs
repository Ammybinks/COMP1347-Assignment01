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
        
        private float distanceSwum; // Distance the fish has swum since the beginning of its special behaviour
        private float distanceToSwim; // Distance the fish should swim before ending any given behaviour
        private float defaultDistanceToSwim;
        
        private bool scared = false;
        private bool chickenLegActive;
        
        #endregion

        #region Properties
        
        public int Speed
        {
            set
            {
                mSpeedX = value;
                mSpeedY = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pToken">Token to be associated with the mind.</param>
        public SeahorseMind(X2DToken pToken)
            :base(pToken)
        {
            /* LEARNING PILL: associating a mind with a token
             * In order for a mind to control a token, it must be associated with the token.
             * This is done when the mind is constructed, using the method Possess inherited
             * from class AIPlayer.
             */
            mFacingDirectionY = -1;  
            
            edgeBouncingY = true;
            
            defaultDistanceToSwim = 200;

            SetRegularBehaviour();
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
            if(mAquarium.ChickenLeg != null)
            {
                if(!chickenLegActive)
                {
                    scared = true;

                    edgeBouncingX = false;
                    edgeBouncingY = false;

                    chickenLegActive = true;

                    distanceSwum = 0;
                    distanceToSwim = 100;

                    Console.SetCursorPosition(0, 6 + (PossessedToken as SeahorseToken).Index);
                    Console.Write("Scared       ");
                }
            }
            else
            {
                if(chickenLegActive)
                {
                    chickenLegActive = false;

                    if(scared)
                    {
                        SetRegularBehaviour();
                    }
                }
            }
            
            if(scared)
            {
                Vector3 chickenLegPosition = mAquarium.ChickenLeg.Position;

                Vector2 tempPosition1 = new Vector2(tokenPosition.X - chickenLegPosition.X, tokenPosition.Y - chickenLegPosition.Y);
                Vector2 tempPosition2 = tempPosition1;
                tempPosition1 = Vector2.Normalize(tempPosition1);
                tempPosition1 *= mSpeedX;
                
                if (distanceSwum >= distanceToSwim)
                {
                    SetRegularBehaviour();
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

                    distanceSwum += mSpeedX;
                }

            }
            else
            {
                if (hitEdgeY)
                {
                    distanceToSwim = distanceSwum - (mSpeedY * 2);

                    distanceSwum = 0;
                }
                else if (distanceSwum >= distanceToSwim)
                {
                    distanceSwum = 0;
                    distanceToSwim = defaultDistanceToSwim;

                    mFacingDirectionY *= -1;
                }
                else
                {
                    distanceSwum += mSpeedY;
                }
            }
        }

        private void SetRegularBehaviour()
        {
            scared = false;

            edgeBouncingX = true;
            edgeBouncingY = true;
            
            distanceSwum = 100 + (mSpeedY);
            distanceToSwim = defaultDistanceToSwim;

            Console.SetCursorPosition(0, 6 + (PossessedToken as SeahorseToken).Index);
            Console.Write("Regular         ");
        }
        
        /// <summary>
        /// AI Update method.
        /// </summary>
        /// <param name="pGameTime">Game time</param>
        public override void Update(ref GameTime pGameTime)
        {
            tokenPosition = PossessedToken.Position; // Store the current position of the fish

            if (!scared)
            {
                Move();
            }

            SpecialBehaviour();

            CheckPosition();

            PossessedToken.Position = tokenPosition; // Set the token's current position to the new one, after all movements

            Console.SetCursorPosition(0, 6 + (PossessedToken as SeahorseToken).Index);
            Console.Write($"{mFacingDirectionY}, {distanceSwum}, {distanceToSwim}, {hitEdgeY}         ");

            /* LEARNING PILL: This is a special method which gets called over and over again from somewhere within the FishORama framework based on Game time (A timer)
            *  the method in thoery is like a loop, but after each iteration unlike a look, it will allow you see what happened during that iteration
            *  this acts like frames of animation, so each iteration will be a new frame.  If we move the fish 1 pixel per iteration you will see that happen
            *  during 'game time'.
            */
        }


        #endregion
    }
}
