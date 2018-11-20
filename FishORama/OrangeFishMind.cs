using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;              // Required to use XNA features.
using XNAMachinationisRatio;                // Required to use the XNA Machinationis Ratio Engine general features.
using XNAMachinationisRatio.AI;             // Required to use the XNA Machinationis Ratio general AI features.


/* LERNING PILL: XNAMachinationisRatio Engine
 * XNAMachinationisRatio is an engine that allows implementing
 * simulations and games based on XNA, simplifying the use of XNA
 * and adding features not directly available in XNA.
 * XNAMachinationisRatio is a work in progress.
 * The engine works "under the hood", taking care of many features
 * of an interactive simulation automatically, thus minimizing
 * the amount of code that developers have to write.
 * 
 * In order to use the engine, the application main class (Kernel, in the
 * case of FishO'Rama) creates, initializes and stores
 * an instance of class Engine in one of its data members.
 * 
 * The classes comprised in the  XNA Machinationis Ratio engine and the
 * related functionalities can be accessed from any of your XNA project
 * source code files by adding appropriate 'using' statements at the beginning of
 * the file. 
 * 
 */

namespace FishORama
{
    /* LEARNING PILL: Token behaviors in the XNA Machinationis Ratio engine
     * Some simulation tokens may need to enact specific behaviors in order to
     * participate in the simulation. The XNA Machinationis Ratio engine
     * allows a token to enact a behavior by associating an artificial intelligence
     * mind to it. Mind objects are created from subclasses of the class AIPlayer
     * included in the engine. In order to associate a mind to a token, a new
     * mind object must be created, passing to the constructor of the mind a reference
     * of the object that must be associated with the mind. This must be done in
     * the DefaultProperties method of the token.
     * 
     * Hence, every time a new tipe of AI mind is required, a new class derived from
     * AIPlayer must be created, and an instance of it must be associated to the
     * token classes that need it.
     * 
     * Mind objects enact behaviors through the method Update (see below for further details). 
     */
    class OrangeFishMind : AIPlayer
    {
        #region Data Members

        // This mind needs to interact with the token which it possesses, 
        // since it needs to know where are the aquarium's boundaries.
        // Hence, the mind needs a "link" to the aquarium, which is why it stores in
        // an instance variable a reference to its aquarium.
        private AquariumToken mAquarium;        // Reference to the aquarium in which the creature lives.

        private Vector3 tokenPosition; // Stores the temporary position of the fish

        private float mFacingDirectionX;         // Horizontal direction the fish is facing (1: right; -1: left).
        private float mFacingDirectionY;         // Vertical direction the fish is facing (1: up; -1: down).

        private float mSpeedX = 5; // Defines horizontal movement speed of the fish
        private float mSpeedY = 0; // Defines vertical movement speed of the fish

        private enum Action // Enumeration of actions that can be taken by the fish
        {
            None,
            Dashing,
            Accelerating,
            Hungry,
            Sinking
        }
        private Action currentAction = Action.None; // Holds the current special action the fish is taking; holds 'None' while fish is using regular behaviour

        private Random rand = new Random(); // Initialise global random number generator

        private int startTime; // Stores the time the program started

        // Initialise timers for each individual behaviour
        private int dashingTimer = 0;
        private int acceleratingTimer = 0;
        private int hungryTimer = 0;
        private int sinkingTimer = 0;

        private float distanceToSwim; // Distance the fish should swim before ending any given behaviour
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
        public OrangeFishMind(X2DToken pToken)
        {
            /* LEARNING PILL: associating a mind with a token
             * In order for a mind to control a token, it must be associated with the token.
             * This is done when the mind is constructed, using the method Possess inherited
             * from class AIPlayer.
             */
            this.Possess(pToken);       // Possess token.
            mFacingDirectionX = 1;       // Current direction the fish is facing.  

            startTime = (DateTime.Now.Second + DateTime.Now.Minute * 60);

            dashingTimer = rand.Next(10, 31);
            acceleratingTimer = rand.Next(10, 31);
            hungryTimer = rand.Next(10, 31);
            sinkingTimer = rand.Next(10, 31);
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
            switch(currentAction)
            {
                case Action.Dashing:
                    dashingTimer = 100;
                    currentAction = Action.None;
                    break;
                case Action.Accelerating:
                    acceleratingTimer = 100;
                    currentAction = Action.None;
                    break;
                case Action.Hungry:
                    hungryTimer = 100;
                    currentAction = Action.None;
                    break;
                case Action.Sinking:
                    sinkingTimer = 100;
                    currentAction = Action.None;
                    break;
                case Action.None:
                    if(dashingTimer <= 0)
                    {
                        currentAction = Action.Dashing;
                    }
                    else if(acceleratingTimer <= 0)
                    {
                        currentAction = Action.Accelerating;
                    }
                    else if(hungryTimer <= 0)
                    {
                        currentAction = Action.Hungry;
                    }
                    else if(sinkingTimer <= 0)
                    {
                        currentAction = Action.Sinking;
                        distanceToSwim = rand.Next(50, 151);
                    }
                    break;
            }
        }

        /// <summary>
        /// Adds the speed of the fish in both movement axes to the fishes current position
        /// </summary>
        /// <param name="tokenPosition">Current position of the fish</param>
        /// <returns>The fishes new position</returns>
        private void Move()
        {
            tokenPosition.X += mSpeedX * mFacingDirectionX;
            tokenPosition.Y += mSpeedY * mFacingDirectionY;
        }

        /// <summary>
        /// Checks the current position of the fish, ensuring it doesn't leave the bounds of the aquarium
        /// </summary>
        /// <param name="tokenPosition">Current position of the fish</param>
        /// <returns>The fishes new position</returns>
        private void CheckPosition()
        {
            Vector3 relativePosition = tokenPosition - mAquarium.Position;

            if (Math.Abs(relativePosition.X) >= (mAquarium.Width / 2)) // If token has passed either horizontal boundary of the aquarium
            {
                if (relativePosition.X <= 0) // If the left edge of the screen was hit
                {
                    tokenPosition.X = (mAquarium.Width / 2) * -1; // Lock fish to left edge of screen
                }
                else if (relativePosition.X > 0) // If the right edge of the screen was hit
                {
                    tokenPosition.X = (mAquarium.Width / 2); // Lock fish to right edge of screen
                }

                mFacingDirectionX *= -1;

                this.PossessedToken.Orientation = new Vector3(mFacingDirectionX,
                                                              this.PossessedToken.Orientation.Y,
                                                              this.PossessedToken.Orientation.Z);
            }
            if (Math.Abs(relativePosition.Y) >= (mAquarium.Height / 2)) // If token has passed either vertical boundary of the aquarium
            {
                if (relativePosition.Y <= 0) // If the bottom edge of the screen was hit
                {
                    tokenPosition.Y = (mAquarium.Height / 2) * -1; // Lock fish to bottom edge of screen
                }
                else if (relativePosition.Y > 0) // If the top edge of the screen was hit
                {
                    tokenPosition.Y = (mAquarium.Height / 2); // Lock fish to top of screen
                }
            }
        }

        private void CheckTime()
        {
            dashingTimer = (DateTime.Now.Second + DateTime.Now.Minute * 60) + startTime;
            acceleratingTimer = (DateTime.Now.Second + DateTime.Now.Minute * 60) + startTime;
            hungryTimer = (DateTime.Now.Second + DateTime.Now.Minute * 60) + startTime;
            sinkingTimer = (DateTime.Now.Second + DateTime.Now.Minute * 60) + startTime;
        }

        /// <summary>
        /// AI Update method.
        /// </summary>
        /// <param name="pGameTime">Game time</param>
        public override void Update(ref GameTime pGameTime)
        {
            tokenPosition = PossessedToken.Position; // Store the current position of the fish

            SpecialBehaviour();
            Move();
            CheckPosition();

            CheckTime();

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
        