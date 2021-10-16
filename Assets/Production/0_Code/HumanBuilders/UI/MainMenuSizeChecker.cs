using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace HumanBuilders {
  public class MainMenuSizeChecker : MonoBehaviour {
    public Canvas MenuCanvas;
    public Canvas BackgroundCanvas;
    public Image BackgroundImage;

    
    public TextMeshProUGUI Text;

    public void Update() {
      if (Text != null) {
        string message = "";
        if (MenuCanvas != null) {
          RectTransform tform = MenuCanvas.GetComponent<RectTransform>();
          message += "menu canv:\n";
          message += string.Format("  pixelRect: {0}, {1}\n", MenuCanvas.pixelRect.x, MenuCanvas.pixelRect.y);
          message += string.Format("  rect transform (scale): {0}, {1}\n", tform.sizeDelta.x, tform.sizeDelta.y);
          message += string.Format("  rect transform (pos): {0}, {1}\n", tform.position.x, tform.position.y);
          message += string.Format("  rect transform (rect): {0}, {1}\n", tform.rect.x, tform.rect.y);
        }

        if (BackgroundCanvas != null) {
          RectTransform tform = BackgroundCanvas.GetComponent<RectTransform>();
          message += "bg canv:\n";
          message += string.Format("  pixelRect: {0}, {1}\n", BackgroundCanvas.pixelRect.x, BackgroundCanvas.pixelRect.y);
          message += string.Format("  rect transform (scale): {0}, {1}\n", tform.sizeDelta.x, tform.sizeDelta.y);
          message += string.Format("  rect transform (pos): {0}, {1}\n", tform.position.x, tform.position.y);
          message += string.Format("  rect transform (rect): {0}, {1}\n", tform.rect.x, tform.rect.y);
        }

        if (BackgroundImage != null) {
          RectTransform tform = BackgroundImage.GetComponent<RectTransform>();
          message += "bg canv:\n";
          message += string.Format("  rect transform (scale): {0}, {1}\n", tform.sizeDelta.x, tform.sizeDelta.y);
          message += string.Format("  rect transform (pos): {0}, {1}\n", tform.position.x, tform.position.y);
          message += string.Format("  rect transform (rect): {0}, {1}\n", tform.rect.x, tform.rect.y);
        }

        Text.text = message;
      }
    }
  }
}