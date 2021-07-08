using NUnit.Framework;
using UnityEngine.Playables;

namespace HumanBuilders.Tests {
  public class TimelineToolsTests {
    private PlayableGraph graph;

    private Playable playable;

    private void SetupTest(float[] inputWeights) {
      graph = PlayableGraph.Create();

      playable = Playable.Create(graph, inputWeights.Length);
      for (int i = 0; i < inputWeights.Length; i++) {
        playable.SetInputWeight(i, inputWeights[i]);
      }
    }

    [Test]
    public void IsSingleClipPlaying_ReturnsTrue() {
      SetupTest(new float[] {0, 0, 1});
      Assert.True(TimelineTools.IsSingleClipPlaying(playable, out int _));
    }

    [Test]
    public void IsSingleClipPlaying_ReturnsFalse() {
      SetupTest(new float[] {0, 0.5f, 0.5f});
      Assert.False(TimelineTools.IsSingleClipPlaying(playable, out int _));
    }

    [Test]
    public void IsSingleClipPlaying_ReturnsCorrectIndex() {
      SetupTest(new float[] {0, 0, 1});
      TimelineTools.IsSingleClipPlaying(playable, out int num);
      Assert.AreEqual(2, num);
    }

    [Test]
    public void FindClipsToMix_NonePlaying_ReturnsNegatives() {
      SetupTest(new float[] {0, 0, 0});
      TimelineTools.FindClipsToMix(playable, out int a, out int b);
      Assert.True(a == -1 && b == -1);
    }

    [Test]
    public void FindClipsToMix_OnePlaying_ReturnsNegatives() {
      SetupTest(new float[] {0, 1, 0});
      TimelineTools.FindClipsToMix(playable, out int a, out int b);
      Assert.True(a == -1 && b == -1);
    }

    [Test]
    public void FindClipsToMix_ReturnsRightIndices() {
      SetupTest(new float[] {0, 0, 0.66f, 0.33f});
      TimelineTools.FindClipsToMix(playable, out int a, out int b);
      Assert.True(a == 2 && b == 3);
    }

    [Test]
    public void ClipsAreValid_ReturnsTrue() {
      Assert.True(TimelineTools.ClipsAreValid(1, 2));
    }

    [Test]
    public void ClipsAreValid_SameIndex_ReturnsFalse() {
      Assert.False(TimelineTools.ClipsAreValid(1, 1));
    }

    [Test]
    public void ClipsAreValid_BothInvalid_ReturnsFalse() {
      Assert.False(TimelineTools.ClipsAreValid(-1, -1));
    }

    [Test]
    public void ClipsAreValid_LeftInvalid_ReturnsFalse() {
      Assert.False(TimelineTools.ClipsAreValid(-1, 1));
    }

    [Test]
    public void ClipsAreValid_RightInvalid_ReturnsFalse() {
      Assert.False(TimelineTools.ClipsAreValid(1, -1));
    }
  }
}