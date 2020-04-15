using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Storm.Collectibles.Currency {


  /// <summary>
  /// Transform coordinates of the wallet are determined by casting the DisplayText to world space.
  /// </summary>
  public class Wallet : MonoBehaviour {

    public TextMeshProUGUI DisplayText;

    [SerializeField]
    private string currencyName = "Toilet Paper";

    [SerializeField]
    private int total;

    // Update is called once per frame
    void Update() {
      DisplayText.text = currencyName + ": " + total;
    }

    public string GetCurrencyName() {
      return currencyName;
    }

    public int GetTotal() {
      return total;
    }


    public void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Currency")) {
        Currency currency = col.gameObject.GetComponent<Currency>();
        if (currency.IsCollected() && (currency.GetName() == currencyName)) {
          total += currency.GetValue();

          Destroy(col.gameObject);
        }
      }
    }
  }
}