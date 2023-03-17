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
    public class ResizeMenuTests : BasePlayModeTests
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

        #region ResizeMenuTests 
        
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
        /// Test the fact that the menu is resizable. 
        /// Only the upper right corner is selected.
        /// </summary>
        [UnityTest]
        public IEnumerator ResizeMenuFromFront()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            float initialSizeX = backgroundMenu.GetComponent<BoxCollider>().size.x;
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();
            yield return new WaitForSeconds(2f);

            // Instanciate a hand to click on the upper right corner
            Vector3 firstPosition = new Vector3(0.17f, -0.083f, 0.6f);
            yield return rightHand.Show(firstPosition);

            yield return new WaitForSeconds(2f);

            // Grab the upper right corner menu and move it at a new position to resize menu
            Vector3 movedPosition = new Vector3(0.5f, -0.1f, 0.6f);
            yield return rightHand.Click();
            yield return rightHand.GrabAndThrowAt(movedPosition, true, 100);

            yield return new WaitForSeconds(2f);

            // Getting the new dimension in x of the menu
            float resizeSizeX = backgroundMenu.GetComponent<BoxCollider>().size.x + (movedPosition.x - firstPosition.x);
            yield return rightHand.MoveTo(movedPosition);

            yield return new WaitForSeconds(2f);

            // Make sure the menu has been moved
            Assert.IsTrue(resizeSizeX > initialSizeX, "The menu hasn't been correctly resized");
            
            yield return new WaitForSeconds(5f);
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        /// <summary>
        /// Test the fact that the menu is resizable from behind. 
        /// Only the upper right corner is selected.
        /// </summary>
        [UnityTest]
        public IEnumerator ResizeMenuFromBehind()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            // Look at the menu from behind
            backgroundMenu.transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
            float initialSizeX = backgroundMenu.GetComponent<BoxCollider>().size.x;
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();
            yield return new WaitForSeconds(2f);

            // Instanciate a hand to click on the upper right corner
            Vector3 firstPosition = new Vector3(0.17f, -0.083f, 0.6f);
            yield return rightHand.Show(firstPosition);

            yield return new WaitForSeconds(2f);

            // Grab the upper right corner menu and move it at a new position to resize menu
            Vector3 movedPosition = new Vector3(0.5f, -0.1f, 0.6f);
            yield return rightHand.Click();
            yield return rightHand.GrabAndThrowAt(movedPosition, true, 100);

            yield return new WaitForSeconds(2f);

            // Getting the new dimension in x of the menu
            float resizeSizeX = backgroundMenu.GetComponent<BoxCollider>().size.x + (movedPosition.x - firstPosition.x);
            yield return rightHand.MoveTo(movedPosition);

            yield return new WaitForSeconds(2f);

            // Make sure the menu has been moved
            Assert.IsTrue(resizeSizeX > initialSizeX, "The menu hasn't been correctly resized");
            
            yield return new WaitForSeconds(5f);
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        /**
        * Negatives cases 
        **/

        /// <summary>
        /// Test the fact that the menu can't be resize if one corner is not pinched. 
        /// Only the upper right corner is selected. 
        /// </summary>
        [UnityTest]
        public IEnumerator ResizeMenuWithoutPinch()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            float initialSizeX = backgroundMenu.GetComponent<BoxCollider>().size.x;
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();
            yield return new WaitForSeconds(2f);

            // Instanciate a hand to click on the upper right corner
            Vector3 firstPosition = new Vector3(0.17f, -0.083f, 0.6f);
            yield return rightHand.Show(firstPosition);

            yield return new WaitForSeconds(2f);

            // Grab the upper right corner menu and don't pinch it
            Vector3 movedPosition = new Vector3(0.5f, -0.1f, 0.6f);

            // Getting the new dimension in x of the menu
            float resizeSizeX = backgroundMenu.GetComponent<BoxCollider>().size.x;
            yield return rightHand.MoveTo(movedPosition);

            yield return new WaitForSeconds(2f);

            // Make sure the menu has been moved
            Assert.IsTrue(resizeSizeX == initialSizeX, "The menu has been resized");
            
            yield return new WaitForSeconds(5f);
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        #endregion

    }
}
#endif
