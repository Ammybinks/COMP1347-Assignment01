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

        private Vector3 mSize; // Size of the possessed tokens' visible dimensions, for collisions

        private Vector3 tokenPosition; // Stores the temporary position of the fish

        private float mFacingDirectionX;         // Horizontal direction the fish is facing (1: right; -1: left).
        private float mFacingDirectionY = 1;         // Vertical direction the fish is facing (1: up; -1: down).

        private float mSpeedX = 2; // Defines horizontal movement speed of the fish
        private float mSpeedY = 5; // Defines vertical movement speed of the fish

        private float maxSpeedX = 12; // Defines the absolute maximum speed the fish will travel horizontally, after any behaviour modifiers
        private float maxSpeedY = 12; // Defines the absolute maximum speed the fish will travel vertically, after any behaviour modifiers

        private float minSpeedX = 2; // Defines the absolute minimum speed the fish will travel horizontally, after any behaviour modifiers
        private float minSpeedY = 2; // Defines the absolute minimum speed the fish will travel vertically, after any behaviour modifiers

        private bool edgeBouncingX; // Determines whether the fish will bounce off the edge of the left & right hand sides of the screen
        private bool edgeBouncingY; // Determines whether the fish will bounce off the edge of the top & bottom sides of the screen

        private bool hitEdgeX; // Set to true if the fish is currently hitting the left or right bounds of the screen
        private bool hitEdgeY; // Set to true if the fish is currently hitting the top or bottom bounds of the screen

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
        
        private int currentTime; // Stores the current time, captured every update. Checked against each timer to determine time remaining before each behaviour starts

        // Initialise timers for each individual behaviour
        private int dashingTimer;
        private int acceleratingTimer;
        private int hungryTimer;
        private int sinkingTimer;

        private float distanceSwum; // Distance the fish has swum since the beginning of its special behaviour
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
        public OrangeFishMind(X2DToken pToken)
        {
            /* LEARNING PILL: associating a mind with a token
             * In order for a mind to control a token, it must be associated with the token.
             * This is done when the mind is constructed, using the method Possess inherited
             * from class AIPlayer.
             */
            this.Possess(pToken);       // Possess token.
            mFacingDirectionX = 1;       // Current direction the fish is facing.  

            edgeBouncingX = true; // The fish should bounce at the left & right hand edges of the screen
            
            // Randomise the time before each behaviour starts
            dashingTimer = GetCurrentTime() + rand.Next(0, 26);
            acceleratingTimer = GetCurrentTime() + rand.Next(0, 26);
            hungryTimer = GetCurrentTime() + rand.Next(0, 26);
            sinkingTimer = GetCurrentTime() + rand.Next(0, 26);
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
                    distanceSwum += mSpeedX; // Increment distance swum by the amount the fish has moved since the last update

                    if (distanceSwum >= distanceToSwim)
                    {
                        mSpeedX = minSpeedX; // Set the fishes speed to its minimum

                        dashingTimer = GetCurrentTime() + rand.Next(5, 31);  // Randomise the time before the fish dashes next

                        currentAction = Action.None; // End behaviour
                        Console.WriteLine("End Dashing                ");
                    }
                    break;
                case Action.Accelerating:

                    if(currentTime >= acceleratingTimer) // If the timer has been reached, and the fish should be decelerating
                    {
                        if(mSpeedX > minSpeedX) // If speed has not yet reached its minimum
                        {
                            mSpeedX -= 0.02f; // Decrease speed slightly
                        }
                        else
                        {
                            acceleratingTimer = GetCurrentTime() + rand.Next(5, 31); // Randomise the time before the fish accelerates next

                            currentAction = Action.None; // End behaviour
                            Console.WriteLine("End Accelerating                ");
                        }
                    }
                    else // Else the fish should be accelerating
                    {
                        if(mSpeedX < maxSpeedX) // If speed has not yet reached its maximum
                        {
                            mSpeedX += 0.02f; // Increase speed slightly
                        }
                    }
                    break;
                case Action.Hungry:
                    distanceSwum += mSpeedX; // Increment distance swum by the amount the fish has moved since the last update

                    if(distanceSwum >= distanceToSwim)
                    {
                        if(hitEdgeY) // If fish is at the top of screen
                        {
                            mSpeedY = 0; // Stop the fish moving upwards

                            edgeBouncingX = true; // Allow the fish to bounce off the edge of the screen again

                            hungryTimer = GetCurrentTime() + rand.Next(5, 31); // Randomise the time before the fish becomes hungry next

                            currentAction = Action.None; // End behaviour
                            Console.WriteLine("End Hungry - top of screen                ");
                        }
                        else
                        {
                            // Invert the fishes direction
                            mFacingDirectionX *= -1;
                            this.PossessedToken.Orientation = new Vector3(mFacingDirectionX,
                                                                          this.PossessedToken.Orientation.Y,
                                                                          this.PossessedToken.Orientation.Z);

                            distanceSwum = 0; // Reset the distance the fish has swum
                        }
                    }

                    if (currentTime >= hungryTimer)
                    {
                        mSpeedY = 0; // Stop the fish moving upwards

                        edgeBouncingX = true; // Allow the fish to bounce off the edge of the screen again

                        hungryTimer = GetCurrentTime() + rand.Next(5, 31); // Randomise the time before the fish becomes hungry next

                        currentAction = Action.None; // End behaviour
                        Console.WriteLine("End Hungry - end of time                ");
                    }
                    break;
                case Action.Sinking:
                    distanceSwum += mSpeedY; // Increment distance swum by the amount the fish has moved since the last update

                    if(distanceSwum >= distanceToSwim)
                    {
                        mSpeedY = 0; // Stop fish moving downwards

                        sinkingTimer = GetCurrentTime() + rand.Next(5, 31); // Randomise the time before the fish sinks next

                        currentAction = Action.None; // End behaviour
                        Console.WriteLine("End Sinking                ");
                    }
                    break;
                case Action.None:
                    if (currentTime >= sinkingTimer)
                    {
                        currentAction = Action.Sinking; // Begin the sinking behaviour, starting from the next update

                        // Start the fish moving downwards
                        mSpeedY = 2;
                        mFacingDirectionY = -1;

                        distanceSwum = 0; // Reset distance swum counter
                        distanceToSwim = rand.Next(50, 151); // Randomise distance the fish will swim before the behaviour ends

                        Console.WriteLine("Begin Sinking                ");
                    }
                    else if(currentTime >= acceleratingTimer)
                    {
                        currentAction = Action.Accelerating; // Begin the accelerating behaviour, starting from the next update

                        acceleratingTimer = GetCurrentTime() + 15; // Set the amount of time before the fish decelerates

                        Console.WriteLine("Begin Accelerating                ");
                    }
                    else if(currentTime >= hungryTimer)
                    {
                        currentAction = Action.Hungry; // Begin the hungry behaviour, starting from the next update

                        // Start the fish moving upwards
                        mSpeedY = 1;
                        mFacingDirectionY = 1;

                        edgeBouncingX = false; // Prevent the fish from bouncing off the edge of the screen

                        distanceSwum = 0; // Reset distance swum counter
                        distanceToSwim = 150; // Set distance fish will swim before turning

                        hungryTimer = GetCurrentTime() + 5; // Set maximum time behaviour will be active

                        Console.WriteLine("Begin Hungry                ");
                    }
                    else if (currentTime >= dashingTimer)
                    {
                        currentAction = Action.Dashing; // Begin the dashing behaviour, starting from the next update

                        mSpeedX = maxSpeedX; // Set the fishes speed to its maximum

                        distanceSwum = 0; // Reset distance swum counter
                        distanceToSwim = 250; // Set the distance the fish will swim before the behaviour ends

                        Console.WriteLine("Begin Dashing                ");
                    }
                    break;
            }

            Console.SetCursorPosition(0, 0);
            Console.Write($"{currentAction}     \nSinking: {sinkingTimer - currentTime}     \nAccelerating: {acceleratingTimer - currentTime}     \nHungry: {hungryTimer - currentTime}     \nDashing: {dashingTimer - currentTime}     \n");
        }

        /// <summary>
        /// Adds the speed of the fish in both movement axes to the fishes current position
        /// </summary>
        private void Move()
        {
            tokenPosition.X += mSpeedX * mFacingDirectionX;
            tokenPosition.Y += mSpeedY * mFacingDirectionY;
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
                    this.PossessedToken.Orientation = new Vector3(mFacingDirectionX,
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

        /// <returns>The current system time in seconds</returns>
        private int GetCurrentTime()
        {
            return DateTime.Now.Second + DateTime.Now.Minute * 60;
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

            PossessedToken.Position = tokenPosition; // Set the token's current position to the new one, after all movements

            currentTime = GetCurrentTime();


            /* LEARNING PILL: This is a special method which gets called over and over again from somewhere within the FishORama framework based on Game time (A timer)
            *  the method in thoery is like a loop, but after each iteration unlike a look, it will allow you see what happened during that iteration
            *  this acts like frames of animation, so each iteration will be a new frame.  If we move the fish 1 pixel per iteration you will see that happen
            *  during 'game time'.
            */
        }


        #endregion
    }
}
        