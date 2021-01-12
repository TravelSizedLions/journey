using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders.Tests {
  public static class TestingTools {

    public static PlayerCharacter ConstructPlayer() {
      GameObject go = new GameObject();
      PlayerCharacter player = go.AddComponent<PlayerCharacter>();
      Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
      go.AddComponent<MovementSettings>();
      go.AddComponent<PowersSettings>();
      go.AddComponent<EffectsSettings>();
      go.AddComponent<PlayerInventory>();
      go.AddComponent<FiniteStateMachine>();
      go.AddComponent<Animator>();
      player.Physics = go.AddComponent<PhysicsComponent>();
      player.Physics.Inject(rb);
      player.PlayerInput = new VirtualInput();
      player.Sprite = go.AddComponent<SpriteRenderer>();

      return player;
    }

  }

}