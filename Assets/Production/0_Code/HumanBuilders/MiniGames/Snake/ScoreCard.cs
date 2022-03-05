using TMPro;

namespace HumanBuilders {
  public class ScoreCard : Singleton<ScoreCard> {
    public TextMeshProUGUI DisplayText;

    private float score = 0;

    public float Score {get { return score;}}

    public static void Reset() { 
      Instance.score = 0;
      UpdateScore();  
    }

    public static void ScorePoints(float points) {
      Instance.score += points;
      UpdateScore();
    }

    public static void UpdateScore() {
      if (Instance.DisplayText != null) {
        Instance.DisplayText.text = string.Format("Score: {0}", Instance.score);
      }
    }
  }
}