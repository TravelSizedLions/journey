using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Storm.Characters.Bosses;
using UnityEngine;


namespace Tests.Characters.Bosses {

  // A test weakspot.
  public class ConcreteWeakSpot : BossWeakSpot {

  }

  public class BossWeakSpotTests {

    private GameObject go;

    private BossWeakSpot bossWeakSpot;

    public void SetupTest() {
      go = new GameObject();
      bossWeakSpot = go.AddComponent<ConcreteWeakSpot>();
    }

    [Test]
    public void Exposes() {
      SetupTest();
      bossWeakSpot.Expose();
      Assert.True(bossWeakSpot.Exposed);
    } 

    [Test]
    public void Hides() {
      SetupTest();
      bossWeakSpot.Expose();
      bossWeakSpot.Hide();
      Assert.False(bossWeakSpot.Exposed);
    }

    [Test]
    public void Hits() {
      SetupTest();
      bossWeakSpot.Expose();
      bossWeakSpot.Hit(null);
      Assert.False(bossWeakSpot.Exposed);
    }
  }
}