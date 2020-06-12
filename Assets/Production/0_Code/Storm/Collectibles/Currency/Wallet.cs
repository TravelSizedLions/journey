using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Storm.Collectibles.Currency {


  /// <summary>
  /// A UI component that tracks how much of a certain currency the player has collected. 
  /// Gravitating currency will move towards wherever you place the wallet onscreen.
  /// </summary>
  public class Wallet : MonoBehaviour {

    #region Variables

    #region UI
    [Header("UI Components", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// A reference to the UI component that shows how much currency the player has collected.
    /// </summary>
    [Tooltip("A reference to the UI component that shows how much currency the player has collected.")]
    [SerializeField]
    public TextMeshProUGUI DisplayText;

    [Space(10, order=2)]
    #endregion

    #region Currency Settings
    [Header("Currency Settings", order=3)]
    [Space(5, order=4)]

    /// <summary>
    /// The name of the currency (i.e. Money, Scrap, Credits).
    /// </summary>
    [Tooltip("The name of the currency (i.e. Money, Scrap, Credits).")]
    [SerializeField]
    private string currencyName = "Toilet Paper";

    /// <summary>
    /// How much of a certain type of currency the player has collected.
    /// </summary>
    [Tooltip("How much of a certain type of currency the player has collected.")]
    [SerializeField]
    private float total;
    #endregion
    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    // Update is called once per frame
    private void Update() {
      DisplayText.text = currencyName + ": " + total;
    }

    private void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Currency")) {
        Currency currency = col.gameObject.GetComponent<Currency>();
        if (currency.IsCollected() && (currency.GetName() == currencyName)) {
          total += currency.GetValue();

          Destroy(col.gameObject);
        }
      }
    }
    #endregion


    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// The name of the currency this wallet tracks.
    /// </summary>
    public string GetCurrencyName() {
      return currencyName;
    }

    /// <summary>
    /// How much the player has collected for the currency this wallet tracks.
    /// </summary>
    public float GetTotal() {
      return total;
    }
    #endregion
  }
}