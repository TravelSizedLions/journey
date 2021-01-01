using System.Collections.Generic;
using NUnit.Framework;
using Storm.Characters.Bosses;
using UnityEngine;

namespace Tests.Characters.Bosses {
  public class AttackEngineTests {

    private GameObject gameObject;
    private AttackEngine engine;

    public void SetupTest() {
      gameObject = new GameObject();
      engine = gameObject.AddComponent<AttackEngine>();
    }

    public BossPhase CreatePhase(string name, float interval, float variance, int numAttacks) {
      return new BossPhase(name, interval, variance, CreateAttacks(numAttacks));
    }

    public List<BossAttack> CreateAttacks(int number) {
      float freq = 1f/((float)number);
      List<BossAttack> attacks = new List<BossAttack>();
      for (int i = 0; i < number; i++) {
        attacks.Add(new BossAttack("attack_"+i, freq, "attack_"+i));
      }
      return attacks;
    }


    [Test]
    public void Chooses_Attacks_Single() {
      SetupTest();
      BossPhase phase = CreatePhase("test", 0, 0, 1);
      BossAttack attack = engine.GetNextAttack(phase.Attacks);
      Assert.AreEqual(attack, phase.Attacks[0]);
    }

    [Test]
    public void Chooses_Attacks_Randomly() {
      SetupTest();
      BossPhase phase = CreatePhase("test", 0, 0, 10);
      BossAttack attack = engine.GetNextAttack(phase.Attacks);
      Assert.True(phase.Attacks.Contains(attack));
    }

    [Test]
    public void Chooses_Attacks_No_Duplicates() {
      SetupTest();
      BossPhase phase = CreatePhase("test", 0, 0, 2);
      BossAttack attack1 = engine.GetNextAttack(phase.Attacks);
      BossAttack attack2 = engine.GetNextAttack(phase.Attacks, attack1);
      Assert.AreNotEqual(attack1.Name, attack2.Name);
    }

    [Test]
    public void Chooses_Attack_Timing() {
      SetupTest();
      float timing = engine.PickTiming(1, 1);
      Assert.True((timing >= 0) && (timing <= 2));
    }
  }
}