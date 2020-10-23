namespace Storm.Characters.Bosses {

  /// <summary>
  /// The interface for a boss battle.
  /// </summary>
  public interface IBoss {
    /// <summary>
    /// The total health that the boss has.
    /// </summary>
    /// <seealso cref="Boss.TotalHealth" />
    float TotalHealth { get; }

    /// <summary>
    /// The total health that the boss has.
    /// </summary>    
    /// <seealso cref="Boss.RemainingHealth" />
    float RemainingHealth { get; }

    /// <summary>
    /// Take a certain amount of damage.
    /// </summary>
    /// <param name="amount">The amount of damage to take.</param>
    /// <seealso cref="Boss.TakeDamage" />
    void TakeDamage(float amount);
  }
}