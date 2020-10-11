using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using Storm.Flexible;
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
    #endregion

    #region Other Variables
    /// <summary>
    /// A reference to the player character.
    /// </summary>
    private PlayerCharacter player;
    #endregion
    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      player = GameManager.Player;
    }

    private void Update() {
      if (player == null) {
        player = GameManager.Player;
      }
      
      DisplayText.text = string.Format("{0}", player.GetCurrencyTotal(currencyName));
    }

    private void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Currency")) {
        Currency currency = col.gameObject.GetComponent<Currency>();

        if (currency.IsCollected() && (currency.GetName() == currencyName)) { 
          player.AddCurrency(currency.GetName(), currency.GetValue());
          SelfDestructing destructing = currency.GetComponent<SelfDestructing>();
          if (destructing != null) {
            destructing.KeepDestroyed();
            destructing.SelfDestruct();
          }
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
    #endregion
  }
}