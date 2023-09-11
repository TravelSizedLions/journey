using System;
using HumanBuilders;

namespace TSL.Subsystems.WorldView {
  [Serializable]
  public class SpawnData {
    public Guid ID;
    public string Name;

    public SpawnData(Guid id, string name) {
      this.ID = id;
      this.Name = name;
    }

    public SpawnData(SpawnPoint spawn) {
      ID = spawn.ID;
      Name = spawn.name;
    }

    public override bool Equals(object obj) {
      if (obj.GetType() != this.GetType()) {
        return false;
      }

      SpawnData other = (SpawnData) obj;

      return (
        ID == other.ID &&
        Name == other.Name
      );
    }

    public override int GetHashCode() {
      return base.GetHashCode();
    }
  }
}