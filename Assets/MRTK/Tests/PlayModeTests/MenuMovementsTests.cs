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
    public class MenuMovementsTests : BasePlayModeTests
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

        #region MoveMenuTests 
        
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
        
        /**
        * Positives cases 
        **/

        /// <summary>
        /// Test the fact that the menu is movable. 
        /// The menu is grabbable and can be placed everywhere.
        /// </summary>
        [UnityTest]
        public IEnumerator TestMenuMovementFromFront()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            Vector3 initialPoseMenu = backgroundMenu.transform.position;
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();
            yield return new WaitForSeconds(2f);

            // Instanciate a hand to click on button  
            Vector3 firstPosition = new Vector3(0.081f, -0.15f, 0.6f);
            yield return rightHand.Show(firstPosition);

            yield return new WaitForSeconds(2f);

            // Grab the menu and move it at a new position
            Vector3 movedPosition = new Vector3(0.5f, -0.1f, 0.6f);
            yield return rightHand.Click();
            yield return rightHand.GrabAndThrowAt(movedPosition, true, 100);

            yield return new WaitForSeconds(2f);

            yield return rightHand.MoveTo(movedPosition);

            yield return new WaitForSeconds(2f);

            // Make sure the menu has been moved
            Assert.IsTrue(movedPosition.x > initialPoseMenu.x, "The menu hasn't been moved");
            
            yield return new WaitForSeconds(5f);
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        /// <summary>
        /// Test the fact that the menu is movable from behind. 
        /// The menu is grabbable and can be placed everywhere.
        /// </summary>
        [UnityTest]
        public IEnumerator TestMenuMovementFromBehind()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            // Look at the menu from behind
            backgroundMenu.transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
            Vector3 initialPoseMenu = backgroundMenu.transform.position;
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();
            yield return new WaitForSeconds(2f);

            // Instanciate a hand to click on button  
            Vector3 firstPosition = new Vector3(0.081f, -0.15f, 0.6f);
            yield return rightHand.Show(firstPosition);

            yield return new WaitForSeconds(2f);

            // Grab the menu and move it at a new position
            Vector3 movedPosition = new Vector3(0.5f, -0.1f, 0.6f);
            yield return rightHand.Click();
            yield return rightHand.GrabAndThrowAt(movedPosition, true, 100);

            yield return new WaitForSeconds(2f);

            yield return rightHand.MoveTo(movedPosition);

            yield return new WaitForSeconds(2f);

            // Make sure the menu has been moved
            Assert.IsTrue(movedPosition.x > initialPoseMenu.x, "The menu hasn't been moved");
            
            yield return new WaitForSeconds(5f);
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        /**
        * Negatives cases 
        **/

        /// <summary>
        /// Test the fact that the menu can't move if it's not pinched. 
        /// </summary>
        [UnityTest]
        public IEnumerator TestMenuMovementWithoutPinch()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            Vector3 initialPoseMenu = backgroundMenu.transform.position;
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();
            yield return new WaitForSeconds(2f);

            // Instanciate a hand and place it on the menu's background
            Vector3 firstPosition = new Vector3(0.081f, -0.15f, 0.6f);
            yield return rightHand.Show(firstPosition);

            yield return new WaitForSeconds(2f);

            // Only clicking on the menu and try to move it at a new position
            Vector3 movedPosition = new Vector3(0.5f, -0.1f, 0.6f);
            yield return rightHand.MoveTo(movedPosition);

            yield return new WaitForSeconds(2f);

            // Make sure the menu hasn't been moved
            Assert.IsTrue(initialPoseMenu.x == 0, "The menu has been moved");
            
            yield return new WaitForSeconds(5f);
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        /// <summary>
        /// Test the fact that the menu hasn't been moved if the correct gesture is applied but not on the menu. 
        /// </summary>
        [UnityTest]
        public IEnumerator TestMenuMovementWithPinchNextToItAndMove()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            Vector3 initialPoseMenu = backgroundMenu.transform.position;
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();
            yield return new WaitForSeconds(2f);

            // Instanciate a hand to grab nothing 
            Vector3 firstPosition = new Vector3(0.25f, -0.15f, 0.6f);
            yield return rightHand.Show(firstPosition);

            yield return new WaitForSeconds(2f);

            // Grab nothing and move it at a new position
            Vector3 movedPosition = new Vector3(0.5f, -0.1f, 0.6f);
            yield return rightHand.Click();
            yield return rightHand.GrabAndThrowAt(movedPosition, true, 100);

            yield return new WaitForSeconds(2f);

            yield return rightHand.MoveTo(movedPosition);

            yield return new WaitForSeconds(2f);

            // Make sure the menu hasn't been moved
            Assert.IsTrue(initialPoseMenu.x == 0, "The menu has been moved");
            
            yield return new WaitForSeconds(5f);
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        /// <summary>
        /// Test the fact that the menu can't move if it's not pinched. 
        /// </summary>
        [UnityTest]
        public IEnumerator TestMenuMovementWithPinchOnlyOnIt()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            Vector3 initialPoseMenu = backgroundMenu.transform.position;
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();
            yield return new WaitForSeconds(2f);

            // Instanciate a hand and place it on the menu's background
            Vector3 firstPosition = new Vector3(0.081f, -0.15f, 0.6f);
            yield return rightHand.Show(firstPosition);

            yield return new WaitForSeconds(2f);

            // Only clicking on the menu and try to move it at a new position
            Vector3 movedPosition = new Vector3(0.5f, -0.1f, 0.6f);
            yield return rightHand.Click();

            yield return new WaitForSeconds(2f);

            yield return rightHand.MoveTo(movedPosition);

            yield return new WaitForSeconds(2f);

            // Make sure the menu hasn't been moved
            Assert.IsTrue(initialPoseMenu.x == 0, "The menu has been moved");
            
            yield return new WaitForSeconds(5f);
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        #endregion

    }
}
#endif
