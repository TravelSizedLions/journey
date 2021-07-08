using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using HumanBuilders;

using UnityEngine;

namespace HumanBuilders.Tests {
  public class HealthBelowConditionTests {
    private GameObject gameObject;

    private Boss boss;

    private HealthBelowCondition condition;


    public void SetupTest() {
      gameObject = new GameObject();
      boss = gameObject.AddComponent<Boss>();
      boss.TotalHealth = 3;
      boss.ResetValues();
      condition = gameObject.AddComponent<HealthBelowCondition>();
      condition.boss = boss;
    }

    [Test]
    public void Passes_Condition() {
      SetupTest();

      condition.RemainingHealth = 3;
      Assert.False(condition.IsMet());
    }

    [Test]
    public void Fails_Condition() {
      SetupTest();

      condition.RemainingHealth = 3;
      boss.TakeDamage(1);
      Assert.True(condition.IsMet());
    }

  }
}