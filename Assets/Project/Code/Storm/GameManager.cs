using System.Collections;
using System.Collections.Generic;
using Storm.AudioSystem;
using Storm.Cameras;
using Storm.Characters.Player;
using Storm.DialogSystem;
using Storm.Extensions;
using Storm.ResetSystem;
using Storm.TransitionSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storm {

  public class GameManager : Singleton<GameManager> {
    public TransitionManager transitions;
    public ResetManager resets;
    public AudioManager sounds;
    public InGameDialogManager inGameDialogs;

    public SpawnPoint initialSpawn;

    public float gravity;

    public PlayerCharacter player;

    private Animator UIAnimator;

    // Start is called before the first frame update
    protected override void Awake() {
      base.Awake();
      player = FindObjectOfType<PlayerCharacter>();
      transitions = TransitionManager.Instance;
      resets = ResetManager.Instance;
      sounds = AudioManager.Instance;
      inGameDialogs = InGameDialogManager.Instance;

      Physics2D.gravity = new Vector2(0, -gravity);

      string currentSpawn = transitions.GetCurrentSpawnName();
      if (currentSpawn == null || currentSpawn == "") {
        if (initialSpawn == null) {
          if (player != null) {
            transitions.RegisterSpawn("SCENE_START", GameObject.FindGameObjectWithTag("Player").transform.position, true);
            transitions.SetCurrentSpawn("SCENE_START");
          }
        } else {
          transitions.SetCurrentSpawn(initialSpawn.name);
        }
      }
      transitions.SetCurrentScene(SceneManager.GetActiveScene().name);

    }

    void Start() {
      var cam = FindObjectOfType<TargettingCamera>();

      if (player != null) {
        RespawnPlayer(player);
        cam.transform.position = player.transform.position;
      }

      UIAnimator = GetComponent<Animator>();
    }


    public void FixedUpdate() {
      if (player == null) {
        player = FindObjectOfType<PlayerCharacter>();
      }
    }

    public void KillPlayer(PlayerCharacter player) {
      resets.Reset();
      RespawnPlayer(player);
    }

    IEnumerator _RespawnPlayer(PlayerCharacter player) {
      yield return new WaitForSeconds(1.5f);
      player.transform.position = transitions.GetCurrentSpawnPosition();
      UIAnimator.SetBool("Faded", false);
    }

    public void RespawnPlayer(PlayerCharacter player) {
      //TODO: Add spawn particles
      if (player == null) {
        return;
      }
      player.transform.position = transitions.GetCurrentSpawnPosition();
      if (player.rb != null) {
        player.rb.velocity = Vector3.zero;
      }
    }

    public void ReturnToMainMenu() {
      TransitionManager.Instance.MakeTransition("MainMenu");
    }

    public void ExitApplication() {
      Application.Quit();
    }
  }
}