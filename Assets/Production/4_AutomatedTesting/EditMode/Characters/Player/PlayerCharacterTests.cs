using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using HumanBuilders;



namespace HumanBuilders.Tests {
  public class PlayerCharacterTesting {

    private GameObject go;

    private PlayerCharacter player;

    private PhysicsComponent physics;

    private MovementSettings settings;

    private void SetupTest() {
      go = new GameObject();

      player = go.AddComponent<PlayerCharacter>();
      physics = go.AddComponent<PhysicsComponent>();
      settings = go.AddComponent<MovementSettings>();
    }
  }
}