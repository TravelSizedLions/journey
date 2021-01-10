using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using UnityEngine;
using HumanBuilders;


namespace HumanBuilders.Tests {

  public class PlayerInventoryTests {

    private GameObject gameObject;
    private PlayerInventory inventory;

    private void SetupTest() {
      gameObject = new GameObject();
      inventory = gameObject.AddComponent<PlayerInventory>();
    }

    [Test]
    public void AddCurrency_CreatesCurrency() {
      SetupTest();

      inventory.AddCurrency("test", 1);

      Assert.True(inventory.ContainsCurrency("test"));
    }

    [Test]
    public void AddCurrency_CorrectBalance() {
      SetupTest();

      inventory.AddCurrency("test", 25);
      inventory.AddCurrency("test", 15);

      Assert.AreEqual(40, inventory.GetCurrencyTotal("test"));
    }


    [Test]
    public void SpendCurrency_ReturnsTrue() {
      SetupTest();

      inventory.AddCurrency("test", 50);
      bool result = inventory.SpendCurrency("test", 10);

      Assert.True(result);
    }

    [Test]
    public void SpendCurrency_CorrectBalance() {
      SetupTest();

      inventory.AddCurrency("test", 50);
      inventory.SpendCurrency("test", 10);

      Assert.AreEqual(inventory.GetCurrencyTotal("test"), 40);
    }

    [Test]
    public void SpendCurrency_NotEnough_ReturnsFalse() {
      SetupTest();

      inventory.AddCurrency("test", 10);
      bool result = inventory.SpendCurrency("test", 50);

      Assert.False(result);
    }


    [Test]
    public void SpendCurrency_NoCurrency_ReturnsFalse() {
      SetupTest();

      bool result = inventory.SpendCurrency("test", 50);

      Assert.False(result);
    }

    [Test]
    public void GetCurrencyTotal_NoCurrency() {
      SetupTest();

      float total = inventory.GetCurrencyTotal("test");

      Assert.AreEqual(total, 0);
    }

  }
}
