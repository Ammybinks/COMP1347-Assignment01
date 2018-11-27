using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAMachinationisRatio;        // Required to use the XNA Machinationis Ratio Engine.

namespace FishORama
{
    class BaseFishToken : X2DToken
    {
        #region Data members

        // This token needs to interact with the aquarium to swim in it (it needs information
        // regarding the aquarium's boundaries). Hence, it needs a "link" to the aquarium,
        // which is why it stores in an instance variable a reference to its aquarium.
        private AquariumToken mAquarium;    // Reference to the aquarium in which the creature lives.

        private BaseFishMind mMind;       // Explicit reference to the mind the token is using to enact its behaviors.

        private Vector3 mSize; // Size of the fishes visible dimensions, for collisions

        #endregion

        #region Properties

        /// <summary>
        /// Get aquarium in which the creature lives.
        /// </summary>
        public AquariumToken Aquarium
        {
            get { return mAquarium; }
        }
        
        #endregion

        #region Constructors

        /// Constructor for the orange fish.
        /// Uses base class to initialize the token name, and adds code to
        /// initialize custom members.
        /// <param name="pTokenName">Name of the token.</param>
        /// <param name="pAquarium">Reference to the aquarium in which the token lives.</param>
        public BaseFishToken(String pTokenName, AquariumToken pAquarium)
            : base(pTokenName) {
            mAquarium = pAquarium;          // Store reference to aquarium in which the creature is living.
            mMind.Aquarium = mAquarium;     // Provide to the mind a reference to the aquarium, required to swim appropriately.
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
        protected override void DefaultProperties()
        {

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

            BaseFishMind myMind = new BaseFishMind(this);   // Create mind, implicitly associating it to the token.


            mMind = myMind;     // Store explicit reference to mind being used.
            mMind.Aquarium = mAquarium;   // Provide to mind explicit reference to Aquarium.

            mSize = new Vector3(65, 65, 0);
            mMind.Size = mSize; // Provide to mind the dimensions of the token
        }

        #endregion

    }
}
