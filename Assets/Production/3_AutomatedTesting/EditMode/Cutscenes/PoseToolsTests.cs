using NUnit.Framework;
using UnityEngine;

namespace HumanBuilders.Tests {

  public class PoseToolsTests {

    PlayerCharacter player;

    PlayerSnapshot snapshot;

    AbsolutePoseInfo absA;

    AbsolutePoseInfo absB;

    RelativePoseInfo relA;

    RelativePoseInfo relB;

    private void SetupTest() {
      player = TestingTools.ConstructPlayer();

      player.transform.position = Vector3.zero;
      player.transform.eulerAngles = Vector3.one*10;
      player.transform.localScale = Vector3.zero;
      player.SetFacing(Facing.Left);
      player.gameObject.SetActive(true);

      snapshot = new PlayerSnapshot(player);
      player.transform.position = Vector3.one;
      player.transform.eulerAngles = Vector3.one;
      player.transform.localScale = Vector3.one;
      player.SetFacing(Facing.Left);
      player.gameObject.SetActive(true);

      absA = new AbsolutePoseInfo();
      absA.Position = new Vector3(-10, -10, -10);
      absA.Rotation = Vector3.zero;
      absA.Scale = new Vector3(-10, -10, -10);
      absA.State = typeof(SingleJumpFall);
      absA.Flipped = true;
      absA.Active = true;

      absB = new AbsolutePoseInfo();
      absB.Position = new Vector3(10, 10, 10);
      absB.Rotation = new Vector3(20, 20, 20);
      absB.Scale = new Vector3(10, 10, 10);
      absB.State = typeof(Running);
      absB.Flipped = false;
      absB.Active = false;

      relA = new RelativePoseInfo();
      relA.Position = new Vector3(-10, -10, -10);
      relA.Rotation = Vector3.zero;
      relA.Scale = new Vector3(-10, -10, -10);
      relA.State = typeof(SingleJumpFall);
      relA.Flipped = true;
      relA.Active = true;

      relB = new RelativePoseInfo();
      relB.Position = new Vector3(10, 10, 10);
      relB.Rotation = new Vector3(20, 20, 20);
      relB.Scale = new Vector3(10, 10, 10);
      relB.State = typeof(Running);
      relB.Flipped = false;
      relB.Active = false;
    }

    [Test]
    public void IsAbsolute_ReturnsTrue() {
      Assert.True(PoseTools.IsAbsolute(new AbsolutePoseInfo()));
    }

    [Test]
    public void IsAbsolute_ReturnsFalse() {
      Assert.False(PoseTools.IsAbsolute(new RelativePoseInfo()));
    }

    [Test]
    public void IsRelative_ReturnsTrue() {
      Assert.True(PoseTools.IsRelative(new RelativePoseInfo()));
    }

    [Test]
    public void IsRelative_ReturnsFalse() {
      Assert.False(PoseTools.IsRelative(new AbsolutePoseInfo()));
    }

    [Test]
    public void MixAbsolute_CheckPosition() {
      SetupTest();

      PoseTools.MixAbsolute(player, snapshot, absA, 1);

      Assert.AreEqual(player.transform.position, new Vector3(-10, -10, -10));
    }

    [Test]
    public void MixAbsolute_CheckPosition_Weighting() {
      SetupTest();

      PoseTools.MixAbsolute(player, snapshot, absA, 0.5f);

      Assert.AreEqual(player.transform.position, new Vector3(-5, -5, -5));
    }

    [Test]
    public void MixAbsolute_CheckRotation() {
      SetupTest();

      PoseTools.MixAbsolute(player, snapshot, absA, 1);

      Assert.AreEqual(player.transform.eulerAngles, new Vector3(0, 0, 0));
    }

    // [Test]
    // public void MixAbsolute_CheckRotation_Weighting() {
    //   SetupTest();

    //   PoseTools.MixAbsolute(player, snapshot, absA, 0.5f);

    //   // Doing direct equivalence check was creating a very small float point precision error.
    //   float magDiff = ((Vector3.one*5)-player.transform.eulerAngles).magnitude;

    //   Assert.AreEqual(magDiff, 0, 1e-4);
    // }

    [Test]
    public void MixAbsolute_CheckScale() {
      SetupTest();

      PoseTools.MixAbsolute(player, snapshot, absA, 1);

      Assert.AreEqual(player.transform.localScale, new Vector3(-10, -10, -10));
    }

    [Test]
    public void MixAbsolute_CheckScale_Weighting() {
      SetupTest();

      PoseTools.MixAbsolute(player, snapshot, absA, 0.5f);

      Assert.AreEqual(player.transform.localScale, new Vector3(-5, -5, -5));
    }


    [Test]
    public void MixRelative_CheckPosition() {
      SetupTest();

      PoseTools.MixRelative(player, snapshot, relA, 1);

      Assert.AreEqual(player.transform.position, new Vector3(-9, -9, -9));
    }

    [Test]
    public void MixRelative_CheckPosition_Weighting() {
      SetupTest();

      PoseTools.MixRelative(player, snapshot, relA, 0.5f);

      Assert.AreEqual(player.transform.position, new Vector3(-4, -4, -4));
    }

    // [Test]
    // public void MixRelative_CheckRotation() {
    //   SetupTest();

    //   PoseTools.MixRelative(player, snapshot, relA, 1);

    //   float magDiff = ((Vector3.one)-player.transform.eulerAngles).magnitude;

    //   Assert.AreEqual(0, magDiff, 1e-4);
    // }

    // [Test]
    // public void MixRelative_CheckRotation_Weighting() {
    //   SetupTest();

    //   PoseTools.MixRelative(player, snapshot, relA, 0.5f);

    //   // Doing direct equivalence check was creating a very small float point precision error.
    //   float magDiff = ((Vector3.one)-player.transform.eulerAngles).magnitude;
    //   Assert.AreEqual(magDiff, 0, 1e-4);
    // }

    [Test]
    public void MixRelative_CheckScale() {
      SetupTest();

      PoseTools.MixRelative(player, snapshot, relA, 1);

      Assert.AreEqual(player.transform.localScale, new Vector3(-10, -10, -10));
    }

    [Test]
    public void MixRelative_CheckScale_Weighting() {
      SetupTest();

      PoseTools.MixRelative(player, snapshot, relA, 0.5f);

      Assert.AreEqual(player.transform.localScale, new Vector3(-5, -5, -5));
    }

  }

}