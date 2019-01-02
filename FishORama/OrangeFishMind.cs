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
    class OrangeFishMind : BaseFishMind
    {
        #region Data Members
        
        private float maxSpeedX = 12; // Defines the absolute maximum speed the fish will travel horizontally, after any behaviour modifiers
        private float maxSpeedY = 12; // Defines the absolute maximum speed the fish will travel vertically, after any behaviour modifiers

        private float minSpeedX = 2; // Defines the absolute minimum speed the fish will travel horizontally, after any behaviour modifiers
        private float minSpeedY = 2; // Defines the absolute minimum speed the fish will travel vertically, after any behaviour modifiers
        
        private enum Action // Enumeration of actions that can be taken by the fish
        {
            None,
            Dashing,
            Accelerating,
            Hungry,
            Sinking
        }
        private Action currentAction = Action.None; // Holds the current special action the fish is taking; holds 'None' while fish is using regular behaviour

        private int timer;

        private float distanceSwum; // Distance the fish has swum since the beginning of its special behaviour
        private float distanceToSwim; // Distance the fish should swim before ending any given behaviour

        #endregion

        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pToken">Token to be associated with the mind.</param>
        public OrangeFishMind(X2DToken pToken, Random rand)
            : base(pToken, rand)
        {
            mRenderDirection = 1;
            
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
        /// Changes the fishes behaviour based on randomised timers
        /// </summary>
        private void SpecialBehaviour()
        {
            switch(currentAction)
            {
                case Action.Dashing:
                    distanceSwum += mSpeedX; // Increment distance swum by the amount the fish has moved since the last update

                    if (distanceSwum >= distanceToSwim)
                    {
                        mSpeedX = minSpeedX; // Set the fishes speed to its minimum
                        
                        currentAction = Action.None; // End behaviour
                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine("End Dashing                ");
                    }
                    break;
                case Action.Accelerating:

                    if(GetCurrentTime() >= timer) // If the timer has been reached, and the fish should be decelerating
                    {
                        if(mSpeedX > minSpeedX) // If speed has not yet reached its minimum
                        {
                            mSpeedX -= 0.02f; // Decrease speed slightly
                        }
                        else
                        {
                            currentAction = Action.None; // End behaviour
                            Console.SetCursorPosition(0, 0);
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
                            
                            currentAction = Action.None; // End behaviour
                            Console.SetCursorPosition(0, 0);
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

                    if (GetCurrentTime() >= timer)
                    {
                        mSpeedY = 0; // Stop the fish moving upwards

                        edgeBouncingX = true; // Allow the fish to bounce off the edge of the screen again
                        
                        currentAction = Action.None; // End behaviour
                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine("End Hungry - end of time                ");
                    }
                    break;
                case Action.Sinking:
                    distanceSwum += mSpeedY; // Increment distance swum by the amount the fish has moved since the last update

                    if(distanceSwum >= distanceToSwim)
                    {
                        mSpeedY = 0; // Stop fish moving downwards
                        
                        currentAction = Action.None; // End behaviour
                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine("End Sinking                ");
                    }
                    break;
                case Action.None:
                    int action = mRand.Next(1, 2001);
                    if (action >= 1 && action <= 10)
                    {
                        currentAction = Action.Sinking; // Begin the sinking behaviour, starting from the next update

                        // Start the fish moving downwards
                        mSpeedY = 2;
                        mFacingDirectionY = -1;

                        distanceSwum = 0; // Reset distance swum counter
                        distanceToSwim = mRand.Next(50, 151); // Randomise distance the fish will swim before the behaviour ends

                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine("Begin Sinking                ");
                    }
                    else if(action >= 11 && action <= 15)
                    {
                        currentAction = Action.Accelerating; // Begin the accelerating behaviour, starting from the next update

                        timer = GetCurrentTime() + 15; // Set the amount of time before the fish decelerates

                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine("Begin Accelerating                ");
                    }
                    else if(action >= 16 && action <= 20)
                    {
                        currentAction = Action.Hungry; // Begin the hungry behaviour, starting from the next update

                        // Start the fish moving upwards
                        mSpeedY = 1;
                        mFacingDirectionY = 1;

                        edgeBouncingX = false; // Prevent the fish from bouncing off the edge of the screen

                        distanceSwum = 0; // Reset distance swum counter
                        distanceToSwim = 150; // Set distance fish will swim before turning

                        timer = GetCurrentTime() + 5; // Set maximum time behaviour will be active

                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine("Begin Hungry                ");
                    }
                    else if (action >= 21 && action <= 25)
                    {
                        currentAction = Action.Dashing; // Begin the dashing behaviour, starting from the next update

                        mSpeedX = maxSpeedX; // Set the fishes speed to its maximum

                        distanceSwum = 0; // Reset distance swum counter
                        distanceToSwim = 250; // Set the distance the fish will swim before the behaviour ends

                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine("Begin Dashing                ");
                    }
                    break;
            }
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

            Console.SetCursorPosition(0, 1);
            Console.WriteLine($"{timer - GetCurrentTime()}                ");

            /* LEARNING PILL: This is a special method which gets called over and over again from somewhere within the FishORama framework based on Game time (A timer)
            *  the method in thoery is like a loop, but after each iteration unlike a look, it will allow you see what happened during that iteration
            *  this acts like frames of animation, so each iteration will be a new frame.  If we move the fish 1 pixel per iteration you will see that happen
            *  during 'game time'.
            */
        }


        #endregion
    }
}
        