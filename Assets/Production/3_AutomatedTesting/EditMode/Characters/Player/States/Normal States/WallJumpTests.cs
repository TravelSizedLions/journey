using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Components;
using Storm.Cameras;

namespace Tests.Characters.Player {
  public class WallJumpTests : PlayerStateTest<WallJump> {
    [Test]
    public void WallJump_Can_Rise() {
      SetupTest();
      
      state.OnWallJumpFinished();
      
      AssertStateChange<SingleJumpRise>();
    }

    [Test]
    public void WallJump_Right() {
      SetupTest();
      
      settings.WallJump = new Vector2(10, 15);
      state.OnStateAdded();
      
      player.IsTouchingLeftWall().Returns(true);
      state.OnStateExit();

      Assert.AreEqual(10, physics.Vx);
      Assert.AreEqual(15, physics.Vy);
    }

    [Test]
    public void WallJump_Left() {
      SetupTest();

      settings.WallJump = new Vector2(10, 15);
      state.OnStateAdded();

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(true);

      state.OnStateExit();

      Assert.AreEqual(-10, physics.Vx);
      Assert.AreEqual(15, physics.Vy);
    }
  }
}