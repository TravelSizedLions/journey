using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.ResetSystem;

namespace Storm.LevelMechanics.Platforms {

    public class CrumblingPlatform : Resetting
    {

        public bool canReset;

        public float resetTime;

        public float decayTime;

        public int states;

        private bool resetting;

        private float decayTimer;

        private float resetTimer;

        private int curState;

        private float timeBetweenStates;

        private List<CrumblingBlock> blocks;

        public void Start() {
            blocks = new List<CrumblingBlock>(GetComponentsInChildren<CrumblingBlock>());
            decayTimer = 0f;
            curState = 0;
            resetting = false;
            timeBetweenStates = decayTime/states;
            Debug.Log(timeBetweenStates);
        }

        public void Update() {
            if (IsDeteriorating()) {
                decayTimer += Time.deltaTime;

                if (decayTimer > decayTime) {
                    resetTimer = 0;
                    resetting = true;
                    DisableBlocks();
                    SetDeteriorating(false);
                } else if (decayTimer > curState*timeBetweenStates) {
                    Debug.Log("Changing!");
                    curState++;
                    SetStates(curState);
                }
            } else if (canReset && resetting) {
                resetTimer += Time.deltaTime;

                if (resetTimer > resetTime) {
                    resetting = false;

                    decayTimer = 0;
                    curState = 0;
                    SetStates(curState);
                    EnableBlocks();
                    EnableTriggers();
                }
            }
        }

        public void DisableBlocks() {
            foreach (var b in blocks) {
                b.physicsCol.enabled = false;
                b.sprite.enabled = false;
            }
        }

        public void EnableBlocks() {
            foreach(var b in blocks) {
                b.physicsCol.enabled = true;
                b.sprite.enabled = true;
            }
        }

        public void DisableTriggers() {
            foreach (var b in blocks) {
                b.triggerCol.enabled = false;
            }
        }

        public void EnableTriggers() {
            foreach(var b in blocks) {
                b.triggerCol.enabled = true;
            }
        }

        public void SetStates(int state) {
            foreach(var b in blocks) {
                b.anim.SetInteger("State", state);
            }
        }

        public void SetDeteriorating(bool deteriorating) {
            foreach(var b in blocks) {
                b.deteriorating = false;
            }
        }

        public bool IsDeteriorating() {
            foreach(var b in blocks) {
                if(b.deteriorating) {
                    return true;
                }
            }
            return false;
        }

        public override void Reset() {
            resetTimer = 0;
            decayTimer = 0;
            EnableBlocks();
            EnableTriggers();
            resetting = false;
            curState = 0;
            SetStates(curState);
        }

    }

}
