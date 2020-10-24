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

    /// <summary>
    /// Start the next phase of the battle!
    /// </summary>
    /// <param name="bossPhase">The phase to start.</param>
    /// <seealso cref="Boss.StartPhase" />
    void StartPhase(BossPhaseNode bossPhase);

    /// <summary>
    /// Start attacking.
    /// </summary>
    /// <seealso cref="Boss.StartAttacking" />
    void StartAttacking();

    /// <summary>
    /// Stop attacking. Also interrupts current attack.
    /// </summary>
    /// <seealso cref="Boss.StopAttacking" />
    void StopAttacking();
  }
}