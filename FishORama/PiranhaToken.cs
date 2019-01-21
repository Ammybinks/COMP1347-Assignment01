using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAMachinationisRatio;        // Required to use the XNA Machinationis Ratio Engine.

namespace FishORama
{
    class PiranhaToken : BaseFishToken
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
        public PiranhaToken(String pTokenName, AquariumToken pAquarium, Random rand)
            : base(pTokenName, pAquarium, rand)
        {
            Orientation = new Vector3(-1, Orientation.Y, Orientation.Z); // Change default facing direction of the fish

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
        protected override void DefaultProperties()
        {

            // Specify which image should be associated to this token, assigning
            // the name of the graphic asset to be used ("PiranhaVisuals" in this case)
            // to the property 'GraphicProperties.AssetID' of the token.
            this.GraphicProperties.AssetID = "PiranhaVisuals1";

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

            PiranhaMind myMind = new PiranhaMind(this, mRand);   // Create mind, implicitly associating it to the token.


            mMind = myMind;     // Store explicit reference to mind being used.
            mMind.Aquarium = mAquarium;   // Provide to mind explicit reference to Aquarium.

            mSize = new Vector3(65, 65, 0);
            mMind.Size = mSize; // Provide to mind the dimensions of the token
        }

        #endregion

    }
}
