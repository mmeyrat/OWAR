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

        /// <summary>
        /// Test solver system's ability to change target types at runtime
        /// </summary>
        // TODO osef tier
        [UnityTest]
        public IEnumerator TestTargetTypes()
        {
            Vector3 rightHandPos = Vector3.right * 50.0f;
            Vector3 leftHandPos = -rightHandPos;
            Vector3 customTransformPos = Vector3.up * 50.0f;

            var transformOverride = new GameObject("Override");
            transformOverride.transform.position = customTransformPos;

            var testObjects = InstantiateTestSolver<Orbital>();
            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();

            // Test orbital around right hand
            {
                testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
                yield return TestHandSolver(testObjects, inputSimulationService, rightHandPos, Handedness.Right);
            }

            // Test orbital around left hand line pointer
            {
                testObjects.handler.TrackedTargetType = TrackedObjectType.ControllerRay;
                testObjects.handler.TrackedHandedness = Handedness.Left;

                yield return TestHandSolver(testObjects, inputSimulationService, leftHandPos, Handedness.Left);
            }

            // Test orbital around head
            {
                testObjects.handler.TrackedTargetType = TrackedObjectType.Head;

                yield return WaitForFrames(2);

                TestUtilities.AssertLessOrEqual(Vector3.Distance(testObjects.target.transform.position, CameraCache.Main.transform.position), DistanceThreshold);
            }

            // Test orbital around custom override
            {
                testObjects.handler.TrackedTargetType = TrackedObjectType.CustomOverride;
                testObjects.handler.TransformOverride = transformOverride.transform;

                yield return WaitForFrames(2);

                TestUtilities.AssertLessOrEqual(Vector3.Distance(testObjects.target.transform.position, customTransformPos), DistanceThreshold);

                yield return WaitForFrames(2);
            }
        }

        /// <summary>
        /// Tests solver handler's ability to switch hands
        /// </summary>
        // TODO osef tier
        [UnityTest]
        public IEnumerator TestHandModality()
        {
            var testObjects = InstantiateTestSolver<Orbital>();

            // Set solver handler to track hands
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;

            // Set and save relevant positions
            Vector3 rightHandPos = Vector3.right * 20.0f;
            Vector3 leftHandPos = Vector3.right * -20.0f;

            yield return WaitForFrames(2);

            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();

            // Test orbital around right hand
            yield return TestHandSolver(testObjects, inputSimulationService, rightHandPos, Handedness.Right);

            // Test orbital around left hand
            yield return TestHandSolver(testObjects, inputSimulationService, leftHandPos, Handedness.Left);

            // Test orbital with both hands visible
            yield return PlayModeTestUtilities.ShowHand(Handedness.Left, inputSimulationService, ArticulatedHandPose.GestureId.Open, leftHandPos);
            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService, ArticulatedHandPose.GestureId.Open, rightHandPos);
            
            // Give time for cube to float to hand
            yield return WaitForFrames(2);

            Vector3 handOrbitalPos = testObjects.target.transform.position;
            TestUtilities.AssertLessOrEqual(Vector3.Distance(handOrbitalPos, leftHandPos), DistanceThreshold);
        }

        /// <summary>
        /// Test Surface Magnetism against "wall" and that attached object falls head direction
        /// </summary>
        // TODO osef tier
        [UnityTest]
        public IEnumerator TestSurfaceMagnetism()
        {
            // Reset view to origin
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

            // Build wall to collide against
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.localScale = new Vector3(25.0f, 25.0f, 0.2f);
            wall.transform.Rotate(Vector3.up, 180.0f); // Rotate wall so forward faces camera
            wall.transform.position = Vector3.forward * 10.0f;

            yield return WaitForFrames(2);

            // Instantiate our test GameObject with solver. 
            // Set layer to ignore raycast so solver doesn't raycast itself (i.e BoxCollider)
            var testObjects = InstantiateTestSolver<SurfaceMagnetism>();
            testObjects.target.layer = LayerMask.NameToLayer("Ignore Raycast");
            SurfaceMagnetism surfaceMag = testObjects.solver as SurfaceMagnetism;

            var targetTransform = testObjects.target.transform;
            var cameraTransform = CameraCache.Main.transform;

            yield return WaitForFrames(2);

            // Confirm that the surfacemagnetic cube is about on the wall straight ahead
            TestUtilities.AssertLessOrEqual(Vector3.Distance(targetTransform.position, wall.transform.position), DistanceThreshold);

            // Rotate the camera
            Vector3 cameraDir = Vector3.forward + Vector3.right;
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(cameraDir);
            });

            // Calculate where our camera hits the wall
            RaycastHit hitInfo;
            Assert.IsTrue(UnityEngine.Physics.Raycast(Vector3.zero, cameraDir, out hitInfo), "Raycast from camera did not hit wall");

            // Let SurfaceMagnetism update
            yield return WaitForFrames(2);

            // Confirm that the surfacemagnetic cube is on the wall with camera rotated
            TestUtilities.AssertLessOrEqual(Vector3.Distance(targetTransform.position, hitInfo.point), DistanceThreshold);

            // Default orientation mode is TrackedTarget, test object should be facing camera
            Assert.IsTrue(Mathf.Approximately(-1.0f, Vector3.Dot(targetTransform.forward.normalized, cameraTransform.forward.normalized)));

            // Change default orientation mode to surface normal
            surfaceMag.CurrentOrientationMode = SurfaceMagnetism.OrientationMode.SurfaceNormal;

            yield return WaitForFrames(2);

            // Test object should now be facing into the wall (i.e Z axis)
            Assert.IsTrue(Mathf.Approximately(1.0f, Vector3.Dot(targetTransform.forward.normalized, Vector3.forward)));
        }

        /// <summary>
        /// Test solver system's ability to change target types at runtime
        /// </summary>
        // TODO osef tier
        [UnityTest]
        public IEnumerator TestInBetween()
        {
            // Build "posts" to put solved object between
            var leftPost = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftPost.transform.position = Vector3.forward * 10.0f - Vector3.right * 10.0f;

            var rightPost = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightPost.transform.position = Vector3.forward * 10.0f + Vector3.right * 10.0f;

            // Instantiate our test GameObject with solver. 
            var testObjects = InstantiateTestSolver<InBetween>();

            testObjects.handler.TrackedTargetType = TrackedObjectType.CustomOverride;
            testObjects.handler.TransformOverride = leftPost.transform;

            InBetween inBetween = testObjects.solver as InBetween;
            Assert.IsNotNull(inBetween, "Solver cast to InBetween is null");

            inBetween.SecondTrackedObjectType = TrackedObjectType.CustomOverride;
            inBetween.SecondTransformOverride = rightPost.transform;

            // Let InBetween update
            yield return WaitForFrames(2);

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.forward * 10.0f, "InBetween solver did not place object in middle of posts");

            inBetween.PartwayOffset = 0.0f;

            // Let InBetween update
            yield return WaitForFrames(2);

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, rightPost.transform.position, "InBetween solver did not move to the left post");
        }

        /// <summary>
        /// Test the Overlap solver and make sure it tracks the left simulated hand exactly
        /// </summary>
        // TODO osef tier
        [UnityTest]
        public IEnumerator TestOverlap()
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Overlap>();
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            var targetTransform = testObjects.target.transform;

            TestUtilities.AssertAboutEqual(targetTransform.position, Vector3.zero, "Overlap not at original position");
            TestUtilities.AssertAboutEqual(targetTransform.rotation, Quaternion.identity, "Overlap not at original rotation");

            // Test that the solver flies to the position of the left hand
            var handPosition = Vector3.forward - Vector3.right;
            var handRotation = Quaternion.LookRotation(handPosition);
            var leftHand = new TestHand(Handedness.Left);
            yield return leftHand.Show(handPosition);
            yield return leftHand.SetRotation(handRotation);

            yield return WaitForFrames(2);
            var hand = PlayModeTestUtilities.GetInputSimulationService().GetControllerDevice(Handedness.Left) as SimulatedHand;
            Assert.IsNotNull(hand);
            Assert.IsTrue(hand.TryGetJoint(TrackedHandJoint.Palm, out MixedRealityPose pose));

            TestUtilities.AssertAboutEqual(targetTransform.position, pose.Position, "Overlap solver is not at the same position as the left hand.");
            Assert.IsTrue(Quaternion.Angle(targetTransform.rotation, pose.Rotation) < 2.0f);

            // Make sure the solver did not move when hand was hidden
            yield return leftHand.Hide();
            yield return WaitForFrames(2);
            TestUtilities.AssertAboutEqual(targetTransform.position, pose.Position, "Overlap solver moved when the hand was hidden.");
            Assert.IsTrue(Quaternion.Angle(targetTransform.rotation, pose.Rotation) < 2.0f);
        }

        /// <summary>
        /// Test solver system's ability to add multiple solvers at runtime and switch between them.
        /// </summary>
        // TODO osef tier
        [UnityTest]
        public IEnumerator TestSolverSwap()
        {
            // Reset view to origin
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

            // Instantiate and setup RadialView to place object in the view center.
            var testObjects = InstantiateTestSolver<RadialView>();
            RadialView radialViewSolver = (RadialView)testObjects.solver;
            radialViewSolver.MinDistance = 2.0f;
            radialViewSolver.MaxDistance = 2.0f;
            radialViewSolver.MinViewDegrees = 0.0f;
            radialViewSolver.MaxViewDegrees = 0.0f;

            // Let RadialView update the target object
            yield return WaitForFrames(2);

            // Make sure Radial View is placing object in center of View, so we can later check that a solver swap actually moved the target object.
            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.forward * 2.0f, "RadialView does not place object in center of view");

            // Disable the old solver
            radialViewSolver.enabled = false;

            // Add a another solver during runtime, give him a specific location to check whether the new solver updates the target object.
            Orbital orbitalSolver = AddSolverComponent<Orbital>(testObjects.target);
            orbitalSolver.WorldOffset = Vector3.zero;
            orbitalSolver.LocalOffset = Vector3.down * 2.0f;

            // Let Orbital update the target object
            yield return WaitForFrames(2);

            // Make sure Orbital is now updating the target object
            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.down * 2.0f, "Orbital solver did not place object below origin");

            // Swap solvers once again during runtime
            radialViewSolver.enabled = true;
            orbitalSolver.enabled = false;

            // Let RadialView update the target object
            yield return WaitForFrames(2);

            // Make sure Radial View is now updating the target object once again.
            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.forward * 2.0f, "RadialView solver did not place object in center of view");
        }

        // TODO rename region and clean code
        #region MYTESTS 
        
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
        public IEnumerator TestMenuAnchorAndMovingRight()
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
        public IEnumerator TestMenuAnchorAndMovingLeft()
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
        public IEnumerator TestMenuAnchorAndMovingUp()
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
        public IEnumerator TestMenuAnchorAndMovingDown()
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
        public IEnumerator TestMenuAnchorEnableDisable()
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
        public IEnumerator TestMenuAnchorEnableDisableAfterMovement()
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

            // Get anchored menu position to test later
            Vector3 menuPosition = backgroundMenu.transform.position;

            // Move and click again on anchor button
            Vector3 movedUpPosition = new Vector3(0.15f, 0.68f, 0.6f);
            yield return rightHand.Show(movedUpPosition);        

            yield return rightHand.Click();

            // Check if anchor is disable
            Assert.IsFalse(backgroundMenu.GetComponent<SolverHandler>().isActiveAndEnabled);

            // Move the playspace to simulate head movement again
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.left;
                p.LookAt(Vector3.left);
            });

            // Check if the menu does not move
            Assert.Equals(backgroundMenu.transform.position, menuPosition);

            yield return new WaitForFixedUpdate();
            yield return null;
        }


        #endregion

        #region TapToPlace Tests

        /// <summary>
        /// Test the default behavior for Tap to Place.  The default behavior has the target object following the head.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTapToPlaceOnClickHead()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // Create a cube with Tap to Place attached and Head (default) as the TrackedTargetType 
            var tapToPlaceObj = InstantiateTestSolver<TapToPlace>();
            tapToPlaceObj.target.transform.position = Vector3.forward;
            TapToPlace tapToPlace = tapToPlaceObj.solver as TapToPlace;

            // Set hand position 
            Vector3 handStartPosition = new Vector3(-0.055f, -0.1f, 0.5f);
            var leftHand = new TestHand(Handedness.Left);
            yield return leftHand.Show(handStartPosition);
            
            // Select Tap to Place Obj
            yield return leftHand.Click();

            // Make sure the object is being placed
            Assert.True(tapToPlace.IsBeingPlaced);

            // Move the playspace to simulate head movement
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.left * 1.5f;
            });

            yield return new WaitForFixedUpdate();
            yield return null;

            // Make sure the target obj has followed the head
            Assert.AreEqual(CameraCache.Main.transform.position.x, tapToPlaceObj.target.transform.position.x, 1.0e-5f, "The tap to place object position.x does not match the camera position.x");

            // Tap to place has a 0.5 sec timer between clicks to make sure a double click does not get registered
            // We need to wait at least 0.5 secs until another click is called or tap to place will ignore the action
            yield return new WaitForSeconds(0.5f);

            // Click object to stop placement
            yield return leftHand.Click();

            // Make sure the object is not being placed after the click
            Assert.False(tapToPlace.IsBeingPlaced);

            // Move the playspace to simulate head movement again
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.right;
            });

            yield return new WaitForFixedUpdate();
            yield return null;

            // Make sure the target obj is NOT following the head
            Assert.AreNotEqual(CameraCache.Main.transform.position.x, tapToPlaceObj.target.transform.position.x, "The tap to place object position.x matches camera position.x, when it should not");
        }

        #endregion

        #region Follow Tests

        /// <summary>
        /// Test the Follow solver distance clamp options
        /// </summary>
        [UnityTest]
        public IEnumerator TestFollowDistance()
        {
            const float followWaitTime = 0.1f;

            // Reset view to origin
            TestUtilities.PlayspaceToOriginLookingForward();

            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Follow>();
            var followSolver = (Follow)testObjects.solver;
            testObjects.handler.TrackedTargetType = TrackedObjectType.Head;
            var targetTransform = testObjects.target.transform;

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(followWaitTime);

            // Test distance remains within min/max bounds
            float distanceToHead = Vector3.Distance(targetTransform.position, CameraCache.Main.transform.position);
            TestUtilities.AssertLessOrEqual(distanceToHead, followSolver.MaxDistance, "Follow exceeded max distance");
            TestUtilities.AssertGreaterOrEqual(distanceToHead, followSolver.MinDistance, "Follow subceeded min distance");

            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.back * 2;
            });

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(followWaitTime);

            distanceToHead = Vector3.Distance(targetTransform.position, CameraCache.Main.transform.position);
            TestUtilities.AssertLessOrEqual(distanceToHead, followSolver.MaxDistance, "Follow exceeded max distance");
            TestUtilities.AssertGreaterOrEqual(distanceToHead, followSolver.MinDistance, "Follow subceeded min distance");

            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.forward * 4;
            });

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(followWaitTime);

            distanceToHead = Vector3.Distance(targetTransform.position, CameraCache.Main.transform.position);
            TestUtilities.AssertLessOrEqual(distanceToHead, followSolver.MaxDistance, "Follow exceeded max distance");
            TestUtilities.AssertGreaterOrEqual(distanceToHead, followSolver.MinDistance, "Follow subceeded min distance");

            // Test VerticalMaxDistance
            followSolver.VerticalMaxDistance = 0.1f;
            targetTransform.position = Vector3.forward;
            targetTransform.rotation = Quaternion.identity;
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(followWaitTime);

            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.LookAt(Vector3.forward + Vector3.up);
            });

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(followWaitTime);

            float yDistance = targetTransform.position.y - CameraCache.Main.transform.position.y;
            Assert.AreEqual(followSolver.VerticalMaxDistance, yDistance);

            followSolver.VerticalMaxDistance = 0f;
        }

        /// <summary>
        /// Test the Follow solver orientation options
        /// </summary>
        [UnityTest]
        public IEnumerator TestFollowOrientation()
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Follow>();
            var followSolver = (Follow)testObjects.solver;
            testObjects.handler.TrackedTargetType = TrackedObjectType.Head;
            var targetTransform = testObjects.target.transform;

            // Test orientation deadzone
            followSolver.OrientToControllerDeadzoneDegrees = 70;
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.back;
                p.LookAt(Vector3.forward);
            });

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.AreEqual(targetTransform.rotation, Quaternion.identity, "Target rotated before we moved beyond the deadzone");

            MixedRealityPlayspace.PerformTransformation(p => p.RotateAround(Vector3.zero, Vector3.up, 90));
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.AreNotEqual(targetTransform.rotation, Quaternion.identity, "Target did not rotate after we moved beyond the deadzone");

            // Test FaceUserDefinedTargetTransform
            var hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.forward + Vector3.right);
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            followSolver.FaceUserDefinedTargetTransform = true;
            followSolver.TargetToFace = CameraCache.Main.transform;

            TestUtilities.AssertAboutEqual(Quaternion.LookRotation(targetTransform.position - CameraCache.Main.transform.position), targetTransform.rotation, "Target expected to be facing camera.");

            yield return hand.MoveTo(Vector3.forward + Vector3.left, 1);
            yield return null;

            TestUtilities.AssertAboutEqual(Quaternion.LookRotation(targetTransform.position - CameraCache.Main.transform.position), targetTransform.rotation, "Target expected to be facing camera.");
        }

        /// <summary>
        /// Test the Follow solver angular clamp options
        /// </summary>
        [UnityTest]
        public IEnumerator TestFollowDirection()
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Follow>();
            var followSolver = (Follow)testObjects.solver;
            testObjects.handler.TrackedTargetType = TrackedObjectType.Head;
            var targetTransform = testObjects.target.transform;

            // variables and lambdas to test direction remains within bounds
            var maxXAngle = followSolver.MaxViewHorizontalDegrees / 2;
            var maxYAngle = followSolver.MaxViewVerticalDegrees / 2;
            Vector3 directionToHead() => CameraCache.Main.transform.position - targetTransform.position;
            float xAngle() => (Mathf.Acos(Vector3.Dot(directionToHead(), targetTransform.right)) * Mathf.Rad2Deg) - 90;
            float yAngle() => 90 - (Mathf.Acos(Vector3.Dot(directionToHead(), targetTransform.up)) * Mathf.Rad2Deg);

            // Test without rotation
            TestUtilities.PlayspaceToOriginLookingForward();

            yield return new WaitForFixedUpdate();
            yield return null;

            TestUtilities.AssertLessOrEqual(Mathf.Abs(xAngle()), maxXAngle, "Follow exceeded the max horizontal angular bounds");
            TestUtilities.AssertLessOrEqual(Mathf.Abs(yAngle()), maxYAngle, "Follow exceeded the max vertical angular bounds");

            // Test y axis rotation
            MixedRealityPlayspace.PerformTransformation(p => p.Rotate(Vector3.up, 45));
            yield return new WaitForFixedUpdate();
            yield return null;

            TestUtilities.AssertLessOrEqual(Mathf.Abs(xAngle()), maxXAngle, "Follow exceeded the max horizontal angular bounds");
            TestUtilities.AssertLessOrEqual(Mathf.Abs(yAngle()), maxYAngle, "Follow exceeded the max vertical angular bounds");

            // Test x axis rotation
            MixedRealityPlayspace.PerformTransformation(p => p.Rotate(Vector3.right, 45));
            yield return new WaitForFixedUpdate();
            yield return null;

            TestUtilities.AssertLessOrEqual(Mathf.Abs(xAngle()), maxXAngle, "Follow exceeded the max horizontal angular bounds");
            TestUtilities.AssertLessOrEqual(Mathf.Abs(yAngle()), maxYAngle, "Follow exceeded the max vertical angular bounds");

            // Test translation
            MixedRealityPlayspace.PerformTransformation(p => p.Translate(Vector3.back, Space.World));
            yield return new WaitForFixedUpdate();
            yield return null;

            TestUtilities.AssertLessOrEqual(Mathf.Abs(xAngle()), maxXAngle, "Follow exceeded the max horizontal angular bounds");
            TestUtilities.AssertLessOrEqual(Mathf.Abs(yAngle()), maxYAngle, "Follow exceeded the max vertical angular bounds");

            // Test renderer bounds clamp mode.
            followSolver.AngularClampMode = Follow.AngularClampType.RendererBounds;
            MixedRealityPlayspace.PerformTransformation(p => p.Rotate(Vector3.up, 180));
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.Greater(Vector3.Dot(targetTransform.position - CameraCache.Main.transform.position, CameraCache.Main.transform.forward), 0.0f, "Follow did not clamp angle when using AngularClampType.RendererBounds.");

            // Test collider bounds clamp mode.
            followSolver.AngularClampMode = Follow.AngularClampType.ColliderBounds;
            MixedRealityPlayspace.PerformTransformation(p => p.Rotate(Vector3.up, 0.0f));
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.Greater(Vector3.Dot(targetTransform.position - CameraCache.Main.transform.position, CameraCache.Main.transform.forward), 0.0f, "Follow did not clamp angle when using AngularClampType.ColliderBounds.");
        }

        /// <summary>
        /// Test the Follow solver angular clamp options
        /// </summary>
        [UnityTest]
        public IEnumerator TestFollowStuckBehind()
        {
            const float followWaitTime = 0.1f;

            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Follow>();
            var followSolver = (Follow)testObjects.solver;
            testObjects.handler.TrackedTargetType = TrackedObjectType.Head;
            var targetTransform = testObjects.target.transform;

            // variables and lambdas to test direction remains within bounds
            Vector3 toTarget() => targetTransform.position - CameraCache.Main.transform.position;

            // Test without rotation
            TestUtilities.PlayspaceToOriginLookingForward();

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(followWaitTime);

            Assert.Greater(Vector3.Dot(CameraCache.Main.transform.forward, toTarget()), 0, "Follow behind the player");

            // Test y axis rotation
            MixedRealityPlayspace.PerformTransformation(p => p.Rotate(Vector3.up, 180));
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(followWaitTime);

            Assert.Greater(Vector3.Dot(CameraCache.Main.transform.forward, toTarget()), 0, "Follow behind the player");
        }

        #endregion

        #region Test Helpers

        private IEnumerator TestHandSolver(SetupData testData, InputSimulationService inputSimulationService, Vector3 handPos, Handedness hand)
        {
            Assert.IsTrue(testData.handler.TrackedTargetType == TrackedObjectType.ControllerRay
                || testData.handler.TrackedTargetType == TrackedObjectType.HandJoint, "TestHandSolver supports on ControllerRay and HandJoint tracked target types");

            yield return PlayModeTestUtilities.ShowHand(hand, inputSimulationService, ArticulatedHandPose.GestureId.Open, handPos);

            // Give time for cube to float to hand
            yield return WaitForFrames(2);

            Vector3 handOrbitalPos = testData.target.transform.position;
            TestUtilities.AssertLessOrEqual(Vector3.Distance(handOrbitalPos, handPos), DistanceThreshold);

            Transform expectedTransform = null;
            if (testData.handler.TrackedTargetType == TrackedObjectType.ControllerRay)
            {
                LinePointer pointer = PointerUtils.GetPointer<LinePointer>(hand);
                expectedTransform = (pointer != null) ? pointer.transform : null;
            }
            else
            {
                var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
                expectedTransform = handJointService.RequestJointTransform(testData.handler.TrackedHandJoint, hand);
            }

            Assert.AreEqual(testData.handler.CurrentTrackedHandedness, hand);
            Assert.IsNotNull(expectedTransform);

            // SolverHandler creates a dummy GameObject to provide a transform for tracking so it can be managed (allocated/deleted)
            // Look at the parent to compare transform equality for what we should be tracking against
            Assert.AreEqual(testData.handler.TransformTarget.parent, expectedTransform);

            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);

            yield return WaitForFrames(2);
        }

        private SetupData InstantiateTestSolver<T>(bool setGameObjectActive = true) where T : Solver
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = typeof(T).Name;
            cube.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);
            cube.SetActive(setGameObjectActive);

            Solver solver = AddSolverComponent<T>(cube);

            SolverHandler handler = cube.GetComponent<SolverHandler>();
            Assert.IsNotNull(handler, "GetComponent<SolverHandler>() returned null");

            var setupData = new SetupData()
            {
                handler = handler,
                solver = solver,
                target = cube
            };

            setupDataList.Add(setupData);

            return setupData;
        }

        private T AddSolverComponent<T>(GameObject target) where T : Solver
        {
            T solver = target.AddComponent<T>();
            Assert.IsNotNull(solver, "AddComponent<T>() returned null");

            // Set Solver lerp times to 0 so we can process tests faster instead of waiting for transforms to update/apply
            solver.MoveLerpTime = 0.0f;
            solver.RotateLerpTime = 0.0f;
            solver.ScaleLerpTime = 0.0f;

            return solver;
        }

        private IEnumerator WaitForFrames(int frames)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }
        }

        /// <summary>
        /// A generalized testing functionality for the HandConstraintPalmUp script that takes in a safezone and target handedness configuration
        /// and then tests it (using those configurations to generate a target test hand placement for activation)
        /// </summary>
        /// <param name="safeZone"> The safezone tested against for this test</param>
        /// <param name="targetHandedness">The target handedness tested against for these activation tests</param>=
        private IEnumerator TestHandConstraintPalmUpGazeActivationByZoneAndHand(HandConstraint.SolverSafeZone safeZone, Handedness targetHandedness)
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<HandConstraintPalmUp>();
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            testObjects.handler.TrackedHandedness = Handedness.Both;

            var handConstraintSolver = (HandConstraintPalmUp)testObjects.solver;
            handConstraintSolver.FollowHandUntilFacingCamera = true;
            handConstraintSolver.UseGazeActivation = true;

            handConstraintSolver.SafeZone = safeZone;
            testObjects.solver.Smoothing = false;

            // Ensure that FacingCameraTrackingThreshold is greater than FollowHandCameraFacingThresholdAngle
            Assert.AreEqual(handConstraintSolver.FacingCameraTrackingThreshold - handConstraintSolver.FollowHandCameraFacingThresholdAngle > 0, true);

            yield return null;

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.zero, "HandConstraintPalmUp solver did not start at the origin");

            var cameraTransform = CameraCache.Main.transform;
            // Place hand 1 meter in front of user, and near the activation zone
            var handTestPos = cameraTransform.position + cameraTransform.forward + DetermineHandOriginPositionOffset(safeZone, targetHandedness);

            // Generate hand rotation with hand palm facing camera
            var cameraLookVector = (handTestPos - cameraTransform.position).normalized;
            var handRotation = Quaternion.LookRotation(cameraTransform.up, cameraLookVector);

            // Add a hand based on the passed in handedness.
            var hand = new TestHand(targetHandedness);
            yield return hand.Show(handTestPos);
            yield return hand.SetRotation(handRotation);
            yield return null;

            // Ensure Activation occurred by making sure the testObjects position isn't still Vector3.zero
            Assert.AreNotEqual(testObjects.target.transform.position, Vector3.zero);

            var palmConstraint = testObjects.solver as HandConstraint;
            // Test forward offset 
            palmConstraint.ForwardOffset = -0.6f;
            yield return null;
            for (float forwardOffset = -0.5f; forwardOffset < 0; forwardOffset += 0.1f)
            {
                Vector3 prevPosition = testObjects.target.transform.position;
                palmConstraint.ForwardOffset = forwardOffset;
                yield return null;
                Vector3 curPosition = testObjects.target.transform.position;
                Vector3 deltaPos = curPosition - prevPosition;
                float actual = Vector3.Dot(deltaPos, CameraCache.Main.transform.forward);
                string debugStr = $"forwardOffset: {palmConstraint.ForwardOffset} prevPosition: {prevPosition.ToString("0.0000")} curPosition: {curPosition.ToString("0.0000")}, {actual}";
                Assert.True(actual < 0, $"Increasing forward offset is expected to move object toward camera. {debugStr}");
            }

            palmConstraint.ForwardOffset = 0;
            palmConstraint.SafeZoneAngleOffset = 0;
            yield return null;
            int delta = 30;
            for (int angle = delta; angle <= 90; angle += delta)
            {
                Vector3 prevPalmToObj = testObjects.target.transform.position - handTestPos;
                palmConstraint.SafeZoneAngleOffset = angle;
                yield return null;
                Vector3 curPalmToObj = testObjects.target.transform.position - handTestPos;
                Vector3 rotationAxis = -cameraTransform.forward;
                if (safeZone == HandConstraint.SolverSafeZone.AtopPalm)
                {
                    HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, targetHandedness, out MixedRealityPose palmPose);
                    rotationAxis = -palmPose.Forward;
                }
                float signedAngle = Vector3.SignedAngle(prevPalmToObj, curPalmToObj, rotationAxis);

                if (targetHandedness == Handedness.Right)
                {
                    signedAngle *= -1;
                }
                Assert.True(signedAngle < 0, $"Increasing SolverSafeZoneAngleOffset should move menu in clockwise direction in left hand, anti-clockwise in right hand {signedAngle}");
            }


            yield return hand.Hide();
        }

        /// <summary>
        /// Based on the type of handconstraint solver safe zone and handedness, returns the offset that the tested hand should apply initially.
        /// </summary>
        /// <param name="safeZone">The target safezone type that's used to determine the position calculations done</param>
        /// <param name="targetHandedness"> The target handedness that's used to calculate the initial activation position</param>
        /// <returns>The Vector3 representing where the hand should be positioned to during the test to trigger the activation</returns>
        private Vector3 DetermineHandOriginPositionOffset(HandConstraint.SolverSafeZone safeZone, Handedness targetHandedness)
        {
            switch (safeZone)
            {
                case HandConstraint.SolverSafeZone.RadialSide:
                    if (targetHandedness == Handedness.Left)
                    {
                        return Vector3.left * RadialUlnarTestActivationPointModifier;
                    }
                    else
                    {
                        return Vector3.right * RadialUlnarTestActivationPointModifier;
                    }

                case HandConstraint.SolverSafeZone.BelowWrist:
                    return Vector3.up * WristTestActivationPointModifier;

                // AtopPalm uses the same test zone as AboveFingerTips because
                // the hand must move to a similar position to activate.
                case HandConstraint.SolverSafeZone.AtopPalm:
                case HandConstraint.SolverSafeZone.AboveFingerTips:
                    return Vector3.down * AboveFingerTipsTestActivationPointModifier;

                default:
                case HandConstraint.SolverSafeZone.UlnarSide:
                    if (targetHandedness == Handedness.Left)
                    {
                        return Vector3.right * RadialUlnarTestActivationPointModifier;
                    }
                    else
                    {
                        return Vector3.left * RadialUlnarTestActivationPointModifier;
                    }
            }
        }

        #endregion
    }
}
#endif
