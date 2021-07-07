using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {

  [CreateAssetMenu(fileName="New Character Profile", menuName="Dialog/Character Profile", order=1)]
  public class CharacterProfile : ScriptableObject {
    public string CharacterName;
    public CharacterCategory Category;

    [Space(8)]
    public bool UseDefaultColors = true;

    [HideIf("UseDefaultColors")]
    public Color PrimaryColor = new Color(0.345098f, 0.3607843f, 0.3843137f, 1f);
    
    [HideIf("UseDefaultColors")]
    public Color SecondaryColor = new Color(0.2078431f, 0.2117647f, 0.2352941f, 1f);
    
    
    [HideIf("UseDefaultColors")]
    public Color TextColor = Color.white;
  }
}