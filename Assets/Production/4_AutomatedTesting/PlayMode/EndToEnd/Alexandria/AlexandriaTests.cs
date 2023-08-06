using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace HumanBuilders.Tests {
  public class AlexandriaTests {
    public const string SCENE_NAME = "test-scene-alexandria-music";

    /// <summary>
    /// Before each test.
    /// </summary>
    [UnitySetUp]
    public IEnumerator SetupTest() {
      AsyncOperation op = SceneManager.LoadSceneAsync(SCENE_NAME);

      while (!op.isDone) {
        yield return null;
      }
    }

    //-------------------------------------------------------------------------
    // Tests
    //-------------------------------------------------------------------------
    [UnityTest]
    public IEnumerator Alexandria_Plays_Sounds() {
      yield return null;
      Assert.False(AlexandriaAudioManager.EffectsSource.isPlaying);
      var trigger = TestUtils.Find<SoundTrigger>("sound-trigger");
      trigger.Pull();
      Assert.True(AlexandriaAudioManager.EffectsSource.isPlaying);
    }

    [UnityTest]
    public IEnumerator Alexandria_Plays_Music() {
      yield return null;
      Assert.False(AlexandriaAudioManager.MusicSource.isPlaying);
      var trigger = TestUtils.Find<MusicTrigger>("music-trigger");
      trigger.Pull();
      Assert.True(AlexandriaAudioManager.MusicSource.isPlaying);
      var stopTrigger = TestUtils.Find<StopMusicTrigger>("stop-music-trigger");
      stopTrigger.Pull();
      Assert.False(AlexandriaAudioManager.MusicSource.isPlaying);
    }

    [UnityTest]
    public IEnumerator Alexandria_Adds_Removes_Background_Effects() {
      yield return null;
      Assert.True(AlexandriaAudioManager.BackgroundSources.Count == 0);
      var toggle = TestUtils.Find<BackgroundEffectToggle>("background-sound-effect-toggle");
      toggle.TurnOn();
      Assert.True(AlexandriaAudioManager.BackgroundSources.Count == 1);
      Assert.True(AlexandriaAudioManager.BackgroundSources["test-bg-effect"].isPlaying);

      toggle.TurnOff();
      Assert.True(AlexandriaAudioManager.BackgroundSources.Count == 0);
    }
  }
}