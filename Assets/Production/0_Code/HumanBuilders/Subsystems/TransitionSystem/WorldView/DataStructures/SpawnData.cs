using System;
using HumanBuilders;

namespace TSL.Subsystems.WorldView {
  [Serializable]
  public class SpawnData {
    public Guid ID => id;
    private Guid id;

    public string Name => name;
    private string name;

    public SpawnData(Guid id, string name) {
      this.id = id;
      this.name = name;
    }

    public SpawnData(SpawnPoint spawn) {
      id = spawn.ID;
      name = spawn.name;
    }

    public override bool Equals(object obj) {
      if (obj.GetType() != this.GetType()) {
        return false;
      }

      SpawnData other = (SpawnData) obj;

      return (
        id == other.id &&
        name == other.name
      );
    }

    public override int GetHashCode() {
      return base.GetHashCode();
    }
  }
}