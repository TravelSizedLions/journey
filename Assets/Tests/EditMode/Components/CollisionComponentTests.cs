using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Components;

namespace Tests.Components {
  public class CollisionComponentTests {

    private GameObject go;

    private GameObject otherGo;

    private CollisionComponent Collisions;

    private BoxCollider2D otherCollider;

    private void SetupTest() {
      go = new GameObject();
      Collisions = new CollisionComponent();

      otherGo = new GameObject();
      otherCollider = otherGo.AddComponent<BoxCollider2D>();
    }

    // A Test behaves as an ordinary method
    [Test]
    public void Collisions_Doesnt_Hit_TriggerCollider() {
      SetupTest();

      otherCollider.isTrigger = true;
      otherCollider.tag = "Ground";

      Vector2 hitNormal = Vector2.up;
      Vector2 checkNormal = Vector2.up;
      Assert.False(Collisions.IsHit(otherCollider, hitNormal, checkNormal));
    }

    [Test]
    public void Collisions_Only_Hits_Ground() {
      SetupTest();

      otherCollider.isTrigger = false;
      otherCollider.tag = "Untagged";

      Vector2 hitNormal = Vector2.up;
      Vector2 checkNormal = Vector2.up;
      Assert.False(Collisions.IsHit(otherCollider, hitNormal, checkNormal));
    }

    [Test]
    public void Collisions_Hits_Ground() {
      SetupTest();

      otherCollider.isTrigger = false;
      otherCollider.tag = "Ground";

      Vector2 hitNormal = Vector2.up;
      Vector2 checkNormal = Vector2.up;
      Assert.True(Collisions.IsHit(otherCollider, hitNormal, checkNormal));
    }

    [Test]
    public void Collisions_Hits_Direction_NonNormal() {
      SetupTest();

      otherCollider.isTrigger = false;
      otherCollider.tag = "Ground";

      Vector2 hitNormal = Vector2.up*1.5f;
      Vector2 checkNormal = Vector2.up*2;
      Assert.True(Collisions.IsHit(otherCollider, hitNormal, checkNormal));
    }

    [Test]
    public void Collisions_WrongDirection_NoHit() {
      SetupTest();

      otherCollider.isTrigger = false;
      otherCollider.tag = "Ground";

      Vector2 hitNormal = new Vector2(0, 1);
      Vector2 checkNormal = new Vector2(0.00001f, 1);
      Assert.False(Collisions.IsHit(otherCollider, hitNormal, checkNormal));
    }

  }
}