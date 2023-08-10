using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {

  public class FloatingObject : MonoBehaviour {
    [Tooltip("How much to oscillate up and down (in unity units)")]
    public float Magnitude = 1;

    [FoldoutGroup("Frequency")]
    [LabelText("Min")]
    public float MinFrequency = 1;

    [FoldoutGroup("Frequency")]
    [LabelText("Max")]
    public float MaxFrequency = 2f;

    [HorizontalGroup("Rotation Speed", Title = "Rotation Speed Variance")]
    [LabelText("Min")]
    [LabelWidth(60)]
    public float MinRotation = 0.001f;
    
    [HorizontalGroup("Rotation Speed", marginLeft: 10)]
    [LabelText("Max")]
    [LabelWidth(60)]
    public float MaxRotation = 0.1f;

    [SerializeField]
    [ReadOnly]
    private Vector3 StartingPosition;

    // scales the period of sine from 2pi to 1.
    private const float TIME_SCALE = 2*Mathf.PI;

    [SerializeField]
    [ReadOnly]
    private float timer = 0;

    [SerializeField]
    [ReadOnly]
    private float actualFrequency;

    [SerializeField]
    [ReadOnly]
    private float actualRotationSpeed;

    #if UNITY_EDITOR
    private void OnDrawGizmos() {
      Gizmos.color = Color.green;
      Gizmos.DrawLine(
        transform.position + Vector3.up*Magnitude + Vector3.left,
        transform.position + Vector3.up*Magnitude + Vector3.right
      );

      Gizmos.DrawLine(
        transform.position + Vector3.down*Magnitude + Vector3.left,
        transform.position + Vector3.down*Magnitude + Vector3.right
      );
    }
    #endif

    private void Awake() {
      StartingPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
      actualRotationSpeed = 360*Random.Range(MinRotation, MaxRotation);
      if (Random.Range(0f, 1f) < .5f) {
        actualRotationSpeed *= -1;
      }
      actualFrequency = Random.Range(MinFrequency, MaxFrequency);
      transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
    }

    private void Update() {
      transform.position = UpdatePosition(Time.deltaTime);
      transform.rotation = UpdateRotation(Time.deltaTime);
    }

    public Vector3 UpdatePosition(float deltaTime) {
      timer = (timer + deltaTime);
      float period = 1/actualFrequency;
      if (timer > period) {
        timer -= period;
      }

      float sin = Magnitude*Mathf.Sin(timer*actualFrequency*TIME_SCALE);
      return new Vector3(
        StartingPosition.x,
        StartingPosition.y+sin,
        StartingPosition.z
      );
    }

    public Quaternion UpdateRotation(float deltaTime) {
      float amount = deltaTime * actualRotationSpeed;
      return Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, amount));
    }
  }

}