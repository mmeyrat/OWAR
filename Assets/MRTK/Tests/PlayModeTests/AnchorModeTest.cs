// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class AnchorModeTest : BasePlayModeTests
    {

        #region Utilities

        private const float DistanceThreshold = 1.5f;
        private const float HandDistanceThreshold = 0.5f;
        private const float SolverUpdateWaitTime = 1.0f; // in seconds
        private const float RadialUlnarTestActivationPointModifier = .03f;
        private const float AboveFingerTipsTestActivationPointModifier = .06f;
        private const float WristTestActivationPointModifier = .05f;

        /// <summary>
        /// Internal class used to store data for setup
        /// </summary>
        protected class SetupData
        {
            public SolverHandler handler;
            public Solver solver;
            public GameObject target;
        }

        private List<SetupData> setupDataList = new List<SetupData>();

        [UnityTearDown]
        public override IEnumerator TearDown()
        {
            Scene scene = SceneManager.GetSceneByName("MainScene");
            yield return null;
        }

        #endregion

        private TestHand rightHand;
        private InputSimulationService inputSimulationService;

        /// <summary>
        /// Setup needed (scene and hand to point) for our tests. 
        /// </summary>
        [UnitySetUp]
        public IEnumerator Setup()
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
            loadOp.allowSceneActivation = true;
            while (!loadOp.isDone)
            {
                yield return null;
            }
            yield return true;
            
            // Initialize the PlaySpace
            TestUtilities.InitializePlayspace();

            // Disable user input during tests
            inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            inputSimulationService.UserInputEnabled = false;

            // Initialize anything required for input simulation
            rightHand = new TestHand(Handedness.Right);
        }

        /// <summary>
        /// Testing the anchor for the menu when we are moving the head to the right.
        /// It supposed to be on the left side of our screen. 
        /// </summary>
        [UnityTest]
        public IEnumerator TestAnchorAndMovingRight()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();

            // Get Menu transformation to test later
            Vector3 menuBasicPosition = backgroundMenu.transform.position;

            // Move the hand to anchor button  
            Vector3 anchorButtonPosition = new Vector3(0.11f, -0.22f, 0.3f);
            yield return rightHand.Show(anchorButtonPosition);

            yield return new WaitForSeconds(4f);

            // Click on anchor button
            yield return rightHand.Click();

            // Check if anchor is activated 
            Assert.IsTrue(backgroundMenu.GetComponent<SolverHandler>().isActiveAndEnabled);

            // Moving the hand now 
            Vector3 movedPosition = new Vector3(0.1f, -0.07f, 0.6f);
            yield return rightHand.MoveTo(movedPosition);

            // Move the playspace to simulate head movement again
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.right;
                p.LookAt(Vector3.right);
            });

            // Check if the menu has really move        
            Assert.AreNotEqual(backgroundMenu.transform.position, menuBasicPosition, "The menu isn't correctly anchored");

            yield return new WaitForFixedUpdate();
            yield return null;
        }

        /// <summary>
        /// Test the anchor for the menu when we are moving the head to the left. 
        /// It's supposed to be on the right side of our screen.
        /// </summary>
        [UnityTest]
        public IEnumerator TestAnchorAndMovingLeft()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();            

            // Get Menu transformation to test later
            Vector3 menuBasicPosition = backgroundMenu.transform.position;                  

            // Move the hand to anchor button  
            Vector3 anchorButtonPosition = new Vector3(0.11f, -0.22f, 0.3f);
            yield return rightHand.Show(anchorButtonPosition);

            yield return new WaitForSeconds(4f);

            // Click on anchor button
            yield return rightHand.Click();

            // Check if anchor is activated 
            Assert.IsTrue(backgroundMenu.GetComponent<SolverHandler>().isActiveAndEnabled);         

            // Moving the hand now 
            Vector3 movedPosition = new Vector3(0.1f, -0.07f, 0.6f);
            yield return rightHand.MoveTo(movedPosition);          

            // Move the playspace to simulate head movement again
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.left;
                p.LookAt(Vector3.left);
            });

            // Check if the menu has really move        
            Assert.AreNotEqual(backgroundMenu.transform.position, menuBasicPosition, "The menu isn't correctly anchored");                    
            
            yield return new WaitForFixedUpdate();
            yield return null;
        }
        
        /// <summary>
        /// Test the anchor for the menu when we are moving the look to the sky. 
        /// It's supposed to be on the bottom of our screen and a little bit sloping.
        /// </summary>
        [UnityTest]
        public IEnumerator TestAnchorAndMovingUp()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();

            // Get Menu transformation to test later
            Vector3 menuBasicPosition = backgroundMenu.transform.position;

            // Move the hand to anchor button  
            Vector3 anchorButtonPosition = new Vector3(0.11f, -0.22f, 0.3f);
            yield return rightHand.Show(anchorButtonPosition);

            yield return new WaitForSeconds(4f);

            // Click on anchor button
            yield return rightHand.Click();

            // Check if anchor is activated 
            Assert.IsTrue(backgroundMenu.GetComponent<SolverHandler>().isActiveAndEnabled);

            // Moving the hand now 
            Vector3 movedPosition = new Vector3(0.1f, -0.07f, 0.6f);
            yield return rightHand.MoveTo(movedPosition);

            // Move the playspace to simulate head movement again
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.up;
                p.LookAt(Vector3.up);
            });

            // Check if the menu has really move        
            Assert.AreNotEqual(backgroundMenu.transform.position, menuBasicPosition, "The menu isn't correctly anchored");

            yield return new WaitForFixedUpdate();
            yield return null;
        }

        /// <summary>
        /// Test the anchor for teh menu when we are moving the look to the ground. 
        /// It's supposed to be on the top of our screen and a little bit sloping.
        /// </summary>
        [UnityTest]
        public IEnumerator TestAnchorAndMovingDown()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();

            // Get Menu transformation to test later
            Vector3 menuBasicPosition = backgroundMenu.transform.position;

            // Move the hand to anchor button  
            Vector3 anchorButtonPosition = new Vector3(0.11f, -0.22f, 0.3f);
            yield return rightHand.Show(anchorButtonPosition);

            yield return new WaitForSeconds(4f);

            // Click on anchor button
            yield return rightHand.Click();

            // Check if anchor is activated 
            Assert.IsTrue(backgroundMenu.GetComponent<SolverHandler>().isActiveAndEnabled);

            // Moving the hand now 
            Vector3 movedPosition = new Vector3(0.1f, -0.07f, 0.6f);
            yield return rightHand.MoveTo(movedPosition);

            // Move the playspace to simulate head movement again
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.down;
                p.LookAt(Vector3.down);
            });

            // Check if the menu has really move        
            Assert.AreNotEqual(backgroundMenu.transform.position, menuBasicPosition, "The menu isn't correctly anchored");

            yield return new WaitForFixedUpdate();
            yield return null;
        }

        /// <summary>
        /// Test the anchor enable and disable.
        /// </summary>
        [UnityTest]
        public IEnumerator TestAnchorEnableDisable()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();

            // Move the hand to anchor button  
            Vector3 anchorButtonPosition = new Vector3(0.11f, -0.22f, 0.3f);
            yield return rightHand.Show(anchorButtonPosition);

            // Check if anchor is disbale
            Assert.IsFalse(backgroundMenu.GetComponent<SolverHandler>().isActiveAndEnabled);

            // Click on anchor button
            yield return rightHand.Click();

            // Moving the hand now 
            Vector3 movedPosition = new Vector3(0.1f, -0.07f, 0.6f);
            yield return rightHand.MoveTo(movedPosition);

            // Check if anchor is activated 
            Assert.IsTrue(backgroundMenu.GetComponent<SolverHandler>().isActiveAndEnabled);

            // Move and click again on anchor button
            yield return rightHand.Show(anchorButtonPosition);
            yield return rightHand.Click();

            // Check if anchor is disable
            Assert.IsFalse(backgroundMenu.GetComponent<SolverHandler>().isActiveAndEnabled);

            yield return new WaitForFixedUpdate();
            yield return null;
        }

        /// <summary>
        /// Test the anchor enable and disable after movement.
        /// </summary>
        [UnityTest]
        public IEnumerator TestAnchorEnableDisableAfterMovement()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();

            // Move the hand to anchor button  
            Vector3 anchorButtonPosition = new Vector3(0.11f, -0.22f, 0.3f);
            yield return rightHand.Show(anchorButtonPosition);

            // Check if anchor is disbale
            Assert.IsFalse(backgroundMenu.GetComponent<SolverHandler>().isActiveAndEnabled);

            // Click on anchor button
            yield return rightHand.Click();

            // Moving the hand now 
            Vector3 movedPosition = new Vector3(0.1f, -0.07f, 0.6f);
            yield return rightHand.MoveTo(movedPosition);

            // Check if anchor is activated 
            Assert.IsTrue(backgroundMenu.GetComponent<SolverHandler>().isActiveAndEnabled);            

            // Move the playspace to simulate head movement
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.up;
                p.LookAt(Vector3.up);
            });            

            // Move and click again on anchor button
            Vector3 movedUpPosition = new Vector3(0.15f, 0.68f, 0.6f);
            yield return rightHand.Show(movedUpPosition);

            yield return new WaitForSeconds(2f); // The component can take some time to update

            yield return rightHand.Click();

            // Get anchored menu position to test later
            Vector3 menuPosition = backgroundMenu.transform.position;

            yield return new WaitForSeconds(2f); // The component can take some time to update

            // Check if anchor is disable            
            Assert.IsFalse(backgroundMenu.GetComponent<SolverHandler>().isActiveAndEnabled);

            // Move the playspace to simulate head movement again
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.left;
                p.LookAt(Vector3.left);
            });

            // Check if the menu does not move
            Assert.AreEqual(backgroundMenu.transform.position, menuPosition, "the menu anchore don't stop");

            yield return new WaitForFixedUpdate();
            yield return null;
        }

    }
}
#endif
