using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;      // Required to use XNA features.
using XNAMachinationisRatio;        // Required to use the XNA Machinationis Ratio Engine.
using XNAMachinationisRatio.AI;     // Required to use the XNA Machinationis Ratio general AI features

/* LERNING PILL: XNAMachinationisRatio Engine
 * XNAMachinationisRatio is an engine that allows implementing
 * simulations and games based on MonoGame, simplifying the use of MonoGame
 * and adding features not directly available in MonoGame.
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
 * related functionalities can be accessed from any of your MonoGame project
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
    /// Abstraction to represent the orange fish which moves peacefully in the aquarium.
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
    class OrangeFishToken: BaseFishToken
    {
        #region Data members

        #endregion

        #region Properties

        #endregion

        #region Constructors

        /// Constructor for the orange fish.
        /// Uses base class to initialize the token name, and adds code to
        /// initialize custom members.
        /// <param name="pTokenName">Name of the token.</param>
        /// <param name="pAquarium">Reference to the aquarium in which the token lives.</param>
        /// <param name="rand">Reference to the global Random object</param>
        public OrangeFishToken(String pTokenName, AquariumToken pAquarium, Random rand)
            : base(pTokenName, pAquarium, rand) {
            mAquarium = pAquarium;          // Store reference to aquarium in which the creature is living.
            mMind.Aquarium = mAquarium;     // Provide to the mind a reference to the aquarium, required to swim appropriately.

            mRand = rand; // Store reference to random number generator, to be sent to the mind
        }

        #endregion

        #region Methods 

        /* LEARNING PILL: XNA Machinationis Ration token properties.
         * All tokens created through the XNA Machinationis Ratio engine have standard
         * attributes that define their behavior in a simulation. These standard
         * attributes can be initialized in a very efficient and simple way using
         * the DeafultProperties() method.
         */

        /// <summary>
        /// Setup default properties of the token.
        /// </summary>
        protected override void DefaultProperties() {

            // Specify which image should be associated to this token, assigning
            // the name of the graphic asset to be used ("OrangeFishVisuals" in this case)
            // to the property 'GraphicProperties.AssetID' of the token.
            this.GraphicProperties.AssetID = "OrangeFishVisuals";

            // Specify mass of the fish. This can be used by
            // physics-based behaviors (work in progress, not functional yet).
            this.PhysicsProperties.Mass = 3;

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

            OrangeFishMind myMind = new OrangeFishMind(this, mRand);   // Create mind, implicitly associating it to the token.
            

            mMind = myMind;     // Store explicit reference to mind being used.
            mMind.Aquarium = mAquarium;   // Provide to mind explicit reference to Aquarium.

            mSize = new Vector3(64, 42, 0);
            mMind.Size = mSize; // Provide to mind the dimensions of the token
        }

        #endregion

    }
}
