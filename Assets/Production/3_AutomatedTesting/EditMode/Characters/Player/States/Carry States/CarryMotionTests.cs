using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;
using HumanBuilders;


namespace HumanBuilders.Tests {

  // Using CarryRun class, since CarryMotion is abstract and therfore can't be
  // added as a component.
  public class CarryMotionTests : PlayerStateTest<CarryRun> {

    [Test]
    public void Can_Buffered_Jump() {
      SetupTest();

      settings.GroundJumpBuffer = 1;

      state.OnStateAdded();
      
      player.DistanceToGround().Returns(0.5f);
      state.TryBufferedJump();
      
      AssertStateChange<CarryJumpStart>();
    }

    [Test]
    public void Buffered_Jump_Too_High() {
      SetupTest();
      
      settings.GroundJumpBuffer = 0.25f;

      state.OnStateAdded();

      player.DistanceToGround().Returns(0.5f);
      state.TryBufferedJump();
      
      AssertNoStateChange<CarryJumpStart>();
    }
  }
}