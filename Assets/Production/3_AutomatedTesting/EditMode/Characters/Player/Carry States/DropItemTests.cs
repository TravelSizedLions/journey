using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using NSubstitute;
using Storm.Characters.Player;
using Storm.Flexible;
using Storm.Characters;
using Storm.Components;

namespace Tests.Characters.Player {
  public class DropItemTests : PlayerStateTest<DropItem> {
    [Test]
    public void Can_Pick_Up_Item() {
      SetupTest();
      
      GameObject g = new GameObject();
      g.AddComponent<Carriable>();
      
      state.OnSignal(g);

      AssertStateChange<PickUpItem>();
    }

    [Test]
    public void Can_Idle() {
      SetupTest();
      
      state.OnDropItemFinished();
      
      AssertStateChange<Idle>();
    }

    [Test]
    public void Can_Apply_Drop_Force_Left() {
      SetupTest();

      carrySettings.DropForce = Vector2.one;

      Carriable c = BuildCarriable();
      player.CarriedItem = c;
      
      player.Physics.Velocity = Vector2.zero;
      player.Facing.Returns(Facing.Left);

      state.OnStateEnter();

      Assert.AreEqual(new Vector2(-1, 1), c.Physics.Velocity);
    }

    [Test]
    public void Can_Apply_Drop_Force_Right() {
      SetupTest();

      carrySettings.DropForce = Vector2.one;

      state.OnStateAdded();

      Carriable c = BuildCarriable();
      player.CarriedItem = c;
      
      player.Physics.Velocity = Vector2.zero;
      player.Facing.Returns(Facing.Right);

      state.OnStateEnter();

      Assert.AreEqual(new Vector2(1, 1), c.Physics.Velocity);
    }
  }
}