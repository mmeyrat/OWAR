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
    public class RotateMenuTests : BasePlayModeTests
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

        #region RotateMenuTests 
        
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
        /// Test the fact that the menu can be rotated. 
        /// Only rotating from the right edge is tested.
        /// </summary>
        [UnityTest]
        public IEnumerator RotateMenuFromFront()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            float initialAngleX = backgroundMenu.transform.localRotation.y;
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();
            yield return new WaitForSeconds(2f);

            // Instanciate a hand to click on the sphere placed on an edge of the right side of the menu
            Vector3 firstPosition = new Vector3(0.17f, -0.15f, 0.6f);
            yield return rightHand.Show(firstPosition);

            yield return new WaitForSeconds(2f);

            // Grab the sphere and make a rotation of 90 degres
            Vector3 movedPosition = new Vector3(0.0f, 0.0f, 0.0f);
            yield return rightHand.GrabAndThrowAt(movedPosition, true, 100);

            yield return new WaitForSeconds(2f);

            // Getting the new rotation round y axis of the menu
            float rotatedAngleX = backgroundMenu.transform.localRotation.y + 90.0f;
            yield return rightHand.MoveTo(movedPosition);

            yield return new WaitForSeconds(2f);

            // Make sure the menu has been rotated
            Assert.IsTrue(rotatedAngleX > initialAngleX, "The menu hasn't been rotated");
            
            yield return new WaitForSeconds(5f);
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        /// <summary>
        /// Test the fact that the menu can be rotated from behind. 
        /// Only rotating from the left edge is tested.
        /// </summary>
        [UnityTest]
        public IEnumerator RotateMenuFromBehind()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            // Look at the menu from behind
            backgroundMenu.transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
            float initialAngleX = backgroundMenu.transform.localRotation.y;
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();
            yield return new WaitForSeconds(2f);

            // Instanciate a hand to click on the sphere placed on an edge of the right side of the menu
            Vector3 firstPosition = new Vector3(0.17f, -0.15f, 0.6f);
            yield return rightHand.Show(firstPosition);

            yield return new WaitForSeconds(2f);

            // Grab the sphere and make a rotation of 90 degres
            Vector3 movedPosition = new Vector3(0.0f, 0.0f, 0.0f);
            yield return rightHand.GrabAndThrowAt(movedPosition, true, 100);

            yield return new WaitForSeconds(2f);

            // Getting the new rotation round y axis of the menu
            float rotatedAngleX = backgroundMenu.transform.localRotation.y + 90.0f;
            yield return rightHand.MoveTo(movedPosition);

            yield return new WaitForSeconds(2f);

            // Make sure the menu has been rotated
            Assert.IsTrue(rotatedAngleX > initialAngleX, "The menu hasn't been rotated");
            
            yield return new WaitForSeconds(5f);
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        /**
        * Negatives cases 
        **/

        /// <summary>
        /// Test the fact that the menu can't be rotated if one sphere is not pinched. 
        /// Only rotating from the right edge is tested.
        /// </summary>
        [UnityTest]
        public IEnumerator RotateMenuWithoutPinch()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            float initialAngleX = backgroundMenu.transform.localRotation.y;
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();
            yield return new WaitForSeconds(2f);

            // Instanciate a hand to click on the sphere placed on an edge of the right side of the menu
            Vector3 firstPosition = new Vector3(0.17f, -0.15f, 0.6f);
            yield return rightHand.Show(firstPosition);

            yield return new WaitForSeconds(2f);

            // Grab the sphere and make a rotation of 90 degres
            Vector3 movedPosition = new Vector3(0.0f, 0.0f, 0.0f);

            yield return new WaitForSeconds(2f);

            // Getting the new rotation around y axis of the menu
            float rotatedAngleX = backgroundMenu.transform.localRotation.y;
            yield return rightHand.MoveTo(movedPosition);

            yield return new WaitForSeconds(2f);

            // Make sure the menu hasn't been rotated
            Assert.IsTrue(rotatedAngleX == initialAngleX, "The menu has been rotated");
            
            yield return new WaitForSeconds(5f);
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        /// <summary>
        /// Test the fact that the menu can't be rotated if one sphere pinched but not moved. 
        /// Only rotating from the right edge is tested.
        /// </summary>
        [UnityTest]
        public IEnumerator RotateMenuWithPinchAndNotMoving()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            float initialAngleX = backgroundMenu.transform.localRotation.y;
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();
            yield return new WaitForSeconds(2f);

            // Instanciate a hand to click on the sphere placed on an edge of the right side of the menu
            Vector3 firstPosition = new Vector3(0.17f, -0.15f, 0.6f);
            yield return rightHand.Show(firstPosition);

            yield return new WaitForSeconds(2f);

            // Grab the sphere but not moving
            yield return rightHand.GrabAndThrowAt(firstPosition, true, 100);

            yield return new WaitForSeconds(2f);

            // Getting the new rotation round y axis of the menu
            float rotatedAngleX = backgroundMenu.transform.localRotation.y;

            // Make sure the menu hasn't been rotated
            Assert.IsTrue(rotatedAngleX == initialAngleX, "The menu has been rotated");
            
            yield return new WaitForSeconds(5f);
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        /// <summary>
        /// Test the fact that the menu can't be rotated if one sphere pinched but not moved. 
        /// Only rotating from the right edge is tested.
        /// </summary>
        [UnityTest]
        public IEnumerator RotateMenuWithPinchAndNotMovingAndResize()
        {
            // Finding objects used for this test
            GameObject backgroundMenu = GameObject.Find("Menu");
            float initialAngleX = backgroundMenu.transform.localRotation.y;
            Assert.IsNotNull(backgroundMenu);

            TestUtilities.PlayspaceToOriginLookingForward();
            yield return new WaitForSeconds(2f);

            // Instanciate a hand to click on the sphere placed on an edge of the right side of the menu
            Vector3 firstPosition = new Vector3(0.17f, -0.15f, 0.6f);
            yield return rightHand.Show(firstPosition);

            yield return new WaitForSeconds(2f);

            // Grab the sphere but not moving
            yield return rightHand.GrabAndThrowAt(firstPosition, true, 100);

            yield return new WaitForSeconds(2f);
            
            // Move the hand to the upper right corner to resize the menu
            Vector3 cornerUpperRightPosition = new Vector3(0.17f, -0.082f, 0.6f);
            yield return rightHand.MoveTo(cornerUpperRightPosition);

            yield return new WaitForSeconds(2f);

            Vector3 movedPosition = new Vector3(0.45f, -0.05f, 0.6f);
            yield return rightHand.GrabAndThrowAt(movedPosition, true, 100);

            // Getting the new rotation round y axis of the menu
            float rotatedAngleX = backgroundMenu.transform.localRotation.y;

            // Make sure the menu hasn't been rotated
            Assert.IsTrue(rotatedAngleX == initialAngleX, "The menu has been rotated");
            
            yield return new WaitForSeconds(5f);
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        #endregion

    }
}
#endif
