using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Services;

namespace Tests {
  public class PlayerCharacterTesting {

    private GameObject go;

    private PlayerCharacter player;

    private UnityPhysics physics;

    private MovementSettings settings;

    private void SetupTest() {
      go = new GameObject();

      player = go.AddComponent<PlayerCharacter>();
      physics = go.AddComponent<UnityPhysics>();
      settings = go.AddComponent<MovementSettings>();

      
    }


  }
}