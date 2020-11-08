
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.LevelMechanics {
  public abstract class FlingFlower : MonoBehaviour {
    public virtual void Fling(IPlayer player) {}

    public virtual void PickDirection(IPlayer player) {}
  }
}