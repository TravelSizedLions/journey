using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace HumanBuilders {
  public class FlickeringLight : MonoBehaviour {

    [FoldoutGroup("Intensity")]
    [LabelText("Min")]
    [LabelWidth(60)]
    public float MinIntensityAmount = 0.25f;

    [FoldoutGroup("Intensity")]
    [LabelText("Max")]
    [LabelWidth(60)]
    public float MaxIntensityAmount = 0.5f;

    [FoldoutGroup("Radius")]
    [LabelText("Min")]
    [LabelWidth(60)]
    public float MinRadiusAmount = .25f;

    [FoldoutGroup("Radius")]
    [LabelText("Max")]
    [LabelWidth(60)]
    public float MaxRadiusAmount = .5f;

    [FoldoutGroup("Duration")]
    [LabelText("Min")]
    [LabelWidth(60)]
    public float MinDuration = 0.1f;

    [FoldoutGroup("Duration")]
    [LabelText("Max")]
    [LabelWidth(60)]
    public float MaxDuration = 0.25f;

    [FoldoutGroup("Frequency")]
    [LabelText("Min")]
    [LabelWidth(60)]
    public float MinFrequency = 1f;

    [FoldoutGroup("Frequency")]
    [LabelText("Max")]
    [LabelWidth(60)]
    public float MaxFrequency = 4f;

    [SerializeField]
    [ReadOnly]
    private float timer;

    [SerializeField]
    [ReadOnly]
    private bool flickering = false;



    private Light2D lightSettings;

    private float baseIntensity;

    private float baseRadius;

    private void Awake() {
      timer = Random.Range(MinFrequency, MaxFrequency);
      lightSettings = GetComponent<Light2D>();
      baseIntensity = lightSettings.intensity;
      baseRadius = lightSettings.pointLightOuterRadius;
    }

    private void Update() {
      if (flickering) {
        timer -= Time.deltaTime;
        if (timer < 0) {
          flickering = false;
          timer = Random.Range(MinFrequency, MaxFrequency);
          lightSettings.intensity = baseIntensity;
          lightSettings.pointLightOuterRadius = baseRadius;
        }
      } else {
        timer -= Time.deltaTime;
        if (timer < 0) {
          flickering = true;
          timer = Random.Range(MinDuration, MaxDuration);
          lightSettings.intensity = baseIntensity - Random.Range(MinIntensityAmount, MaxIntensityAmount);
          lightSettings.pointLightOuterRadius = baseRadius - Random.Range(MinRadiusAmount, MaxRadiusAmount);
        }
      }
    }
  }

}