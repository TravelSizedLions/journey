using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;
using Storm.Characters.Player;

namespace Tests.Characters.Player {
  public class CarryMotionTests : PlayerStateTest<CarryMotion> {

    [Test]
    public void Can_Buffered_Jump() {
      SetupTest();

      movementSettings.GroundJumpBuffer = 1;

      state.OnStateAdded();
      
      player.DistanceToGround().Returns(0.5f);
      state.TryBufferedJump();
      
      AssertStateChange<CarryJumpStart>();
    }

    [Test]
    public void Buffered_Jump_Too_High() {
      SetupTest();
      
      movementSettings.GroundJumpBuffer = 0.25f;

      state.OnStateAdded();

      player.DistanceToGround().Returns(0.5f);
      state.TryBufferedJump();
      
      AssertNoStateChange<CarryJumpStart>();
    }
  }
}