using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Components;

namespace Tests.Characters.Player {
  public class CrouchEndTests : PlayerStateTest<CrouchEnd> {
    

    [Test]
    public void CrouchEnd_Can_Run() {
      SetupTest();

      player.TryingToMove().Returns(true);

      state.OnUpdate();

      AssertStateChange<Running>();
    }


    [Test]
    public void CrouchEnd_Can_Recrouch() {
      SetupTest();

      player.HoldingDown().Returns(true);

      state.OnUpdate();

      AssertStateChange<Crouching>();
    }


    [Test]
    public void CrouchEnd_Can_Jump() {
      SetupTest();

      player.HoldingJump().Returns(true);

      state.OnUpdate();

      AssertStateChange<SingleJumpStart>();
    }


    [Test]
    public void CrouchEnd_No_Op() {
      SetupTest();

      player.HoldingJump().Returns(false);
      player.HoldingDown().Returns(false);
      player.HoldingJump().Returns(false);

      AssertNoStateChange<SingleJumpStart>();
      AssertNoStateChange<CrouchStart>();
      AssertNoStateChange<Running>();
    }

    [Test]
    public void CrouchEnd_Finish_Animation() {
      SetupTest();

      state.OnCrouchEndFinished();

      AssertStateChange<Idle>();
    }
  }
}