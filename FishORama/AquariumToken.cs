using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;              // Required to use XNA features.
using XNAMachinationisRatio;                // Required to use the XNA Machinationis Ratio Engine.
using XNAMachinationisRatio.Resource;       // Required to use the XNA Machinationis Ratio resource management features.

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

/*LEARNING PILL: Game Tokens in the XNA Machinationis Ratio Engine.
 * The XNA Machinationis Ratio engine models games as systems
 * comprising entities called tokens. Tokens have a visual representation
 * and interactive behaviors. Tokens are implemented in C# as instances
 * of the class Token or one of its derived classes.
 * 
 * Tokens have attributes that allow implementing simulation features.
 * Tokens can be associated to a 'mind' object, in order to implement their behaviors.
 */

/* LEARNING PILL: virtual world space and graphics in FishORama
 * In the Machinationis Ratio engine every object has graphic a position in the
 * virtual world expressed through a 3D vector (represented via a Vector3 object).
 * In 2D simulations the first coordinate of the vector is the horizontal (X)
 * position, the second coordinate (Y) represents the vertical position, and 
 * the third coordinate (Z) represents the depth. All simulation features are
 * based on world coordinates.
 * 
 * At any time, a portion of the scene is visible through a camera object (in FishO'Rama 
 * this is created, initialized and referenced through the Kernel class). For the purpose
 * of visualization, coordinates may also be expressed relative to the camera
 * origin (i.e. the center of the camera). In FishO'Rama the camera is centered on the
 * origin of the world, which makes camera coordinates coinciding with world coordinates.
 * This greatly simplifies all the operations.
 * 
 * The third coordinate of the world position of an object represents the depth, i.e.
 * how close an object is to the camera. This defines which objects are in front of others 
 * (for instance, an object with Z=3 will always be drawn in front of an object with
 * Z=2).
 */

/* LEARNING PILL: placing tokens in a scene.
 * In order to be managed by the Machinationis Ratio engine, tokens must be placed
 * in a scene.
 * 
 * In FishORama the procedure for the creation and placement of tokens that must be
 * placed in the scene at startup is carried out byn the method LoadContent of
 * class Kernel.
 * 
 * Tokens can also be created in runtime by any method of any class, provided that
 * the method has access to the simulation scene object encapsulated in class Kernel.
 * This object can be accessed through the property Scene of class Kernel.
 */

namespace FishORama
{
    /// <summary>
    /// Abstraction to represent the Aquarium used in the simulation. The aquarium
    /// is a container for other objects representing creatures that inhabit it and
    /// more. Hence, the aquarium object serves to store references to these objects,
    /// and can be used to mediate their interactions (for instance, an object A
    /// in the aquarium could "ask" to the aquarium to obtain access to another
    /// object B, in order to interact with it).
    /// 
    /// This class is derived from class X2DToken. In the XNA Machinationis Ratio engine
    /// class X2DToken is a base class for all classes representing objects which
    /// have a visual representation and interactive behaviors in a 2D simulation.
    /// X2DToken implements a number of functionalities that make it easy for developers
    /// to add interactivity to objects minimizing the amount of coded required.
    /// 
    /// Hence, whenever we want to create a new type of object, we must create a new
    /// class derived from X2DToken.
    /// </summary>
    /// 
    class AquariumToken : X2DToken
    {
        #region Data Members
        
        // Reference to the simulation kernel (orchestrator of the whole application).
        private Kernel mKernel;             
        
        // Reference to the mind of the aquarium.
        private AquariumMind mMind;         
        
        /*
         * Attributes of the aquarium.
         */
        private int mWidth;                 // Aquarium width.
        private int mHeight;                // Aquarium height.
        
        /*
         * Useful references to entities populating the aquarium.
         */

        // Reference to the chicken leg. Required to allow the piranha
        // reaching it.
        private ChickenLegToken mChickenLeg = null;

        public OrangeFishToken[] mOrangeFish = new OrangeFishToken[10];

        #endregion

        #region Properties

        /// <summary>
        /// Get reference to aquarium width.
        /// </summary>
        public int Width
        {
            get { return mWidth; }
        }

        /// <summary>
        /// Get reference to aquarium height.
        /// </summary>
        public int Height
        {
            get { return mHeight; }
        }

        /// <summary>
        /// Get/set reference to chicken leg.
        /// </summary>
        public ChickenLegToken ChickenLeg
        {
            get { return mChickenLeg; }
            set { mChickenLeg = value; }
        }

        /// <summary>
        /// Get reference to Kernel.
        /// </summary>
        public Kernel Kernel
        {
            get { return mKernel; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for the aquarium.
        /// Uses base class to initialize the token name, and adds code to
        /// initialize custom members.
        /// </summary
        /// <param name="pTokenName">Name of the token.</param>
        /// <param name="pKernel">Reference to the simulation kernel.</param>
        /// <param name="pWidth">Width of the aquarium.</param>
        /// <param name="pHeight">Height of the aquarium.</param>
        public AquariumToken(String pTokenName, Kernel pKernel, int pWidth, int pHeight)
            : base(pTokenName)
        {
            mKernel = pKernel;      // Store reference to kernel.
            mHeight = pHeight;      // Height of the aquarium.
            mWidth = pWidth;        // Width of the aquarium.
        }

        #endregion
        
        #region Methods

        /* LEARNING PILL: Token default properties.
         * In the XNA Machinationis Ratio engine tokens have properties that define
         * how the behave and are visualized. Using the DefaultProperties method 
         *  it is possible to assign deafult values to the token's properties,
         * after the token has been created.
         */

        /// <summary>
        /// Setup default values for this token's porperties.
        /// </summary>
        protected override void DefaultProperties()
        {
            // Specify which image should be associated to this token, assigning
            // the name of the graphic asset to be used ("AquariumVisuals" in this case)
            // to the property 'GraphicProperties.AssetID' of the token.
            this.GraphicProperties.AssetID = "AquariumVisuals";

            /* LEARNING PILL: Token behaviors in the XNA Machinationis Ratio engine
             * Some simulation tokens may need to enact specific behaviors in order to
             * participate in the simulation. The XNA Machinationis Ratio engine
             * allows a token to enact a behavior by associating an artificial intelligence
             * mind to it. Mind objects are created from subclasses of the class AIPlayer
             * included in the engine. In order to associate a mind to a token, a new
             * mind object must be created, passing to the constructor of the mind a reference
             * of the object that must be associated with the mind. This must be done in
             * the DefaultProperties method of the token. Upon creation of the mind, XNA
             * Machinationis Ratio automatically "injects" its into the token, establishing
             * a link which is not visible to the programmer (but it there!)
             * 
             * In this case, instances of the class OrangeFishToken can enact a simple swimming
             * behavior. The behavior is implemented through the class SimpleSwimMind.
             */

            AquariumMind myMind = new AquariumMind(this);   // Create mind, implicitly associating it to the token.


            mMind = myMind;         // Store explicit reference to mind being used.
            mMind.Aquarium = this;  // Provide to mind explicit reference to Aquarium.
        }

        /// <summary>
        /// Indicate whether a game object in the aquarium has reached the aquarium's
        /// horizontal boundaries.
        /// </summary>
        /// <param name="pObject">Object to check.</param>
        /// <returns>Result of the check.</returns>
        public bool ReachedHorizontalBoundary(GameObject pObject)
        {
            // Check if the absolute value of the horizontal distance between the center of
            // aquarium and the object is greater than half the width of the aquarium,
            // which means that center of the object has reached the horizontal boundary
            // of the aquarium.
            if (Math.Abs(pObject.Position.X - this.Position.X) >= (this.mWidth / 2))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Indicate whether a game object in the aquarium has reached the aquarium's
        /// horizontal boundaries.
        /// </summary>
        /// <param name="pObject">Object to check.</param>
        /// <returns>Result of the check.</returns>
        public bool ReachedVerticalBoundary(GameObject pObject)
        {
            // Check if the absolute value of the horizontal distance between the center of
            // aquarium and the object is greater than half the width of the aquarium,
            // which means that center of the object has reached the horizontal boundary
            // of the aquarium.
            if (Math.Abs(pObject.Position.Y - this.Position.Y) >= (this.mHeight / 2))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Remove chicken leg from aquarium.
        /// </summary>
        public void RemoveChickenLeg()
        {
            this.mKernel.Scene.Remove(mChickenLeg);
            mChickenLeg = null;
        }

        #endregion
    }
}