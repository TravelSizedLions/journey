using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.LevelMechanics.Platforms {

    public class MovingPlatformRig : MonoBehaviour {
        public MovingPlatformDestination pointA;
        public MovingPlatformDestination pointB;
        public MovingPlatformDestination currentDestination;

        public Rigidbody2D platformRB;

        public float topSpeed = 5;

        public float acceleration = 0.5f;

        public float stationaryTime = 3; // seconds

        // How close the platform needs to be to the destination to trigger a change in direction.
        private float triggerDistance = 0.01f;

        public Vector2 currentVelocity;

        // Start is called before the first frame update
        void Start() {
            var points = gameObject.GetComponentsInChildren<MovingPlatformDestination>();
            pointA = points[0];
            pointB = points[1];

            platformRB.transform.position = pointA.transform.position;
            currentDestination = pointB;
        }

        // Update is called once per frame
        void FixedUpdate() {
            var curPos = platformRB.transform.position;
            var futurePos = (acceleration)*curPos + (1-acceleration)*currentDestination.transform.position;

            // check that the platform isn't travelling too fast.
            currentVelocity = futurePos-curPos;
            if (currentVelocity.magnitude > topSpeed) {
                var direction = (futurePos-curPos).normalized;
                futurePos = curPos+direction*topSpeed;
            }

            platformRB.transform.position = futurePos;

            var distance = (currentDestination.transform.position - futurePos).magnitude;
            if (distance < triggerDistance) {
                SwitchDirections();
            }
        }


        public void SwitchDirections() {
            if (currentDestination == pointA) {
                currentDestination = pointB;
            } else {
                currentDestination = pointA;
            }
        }
    }
}