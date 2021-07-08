using NUnit.Framework;
using UnityEngine;

using HumanBuilders;

namespace HumanBuilders.Tests {
    public class CoyoteTimerTests {
      
      private GameObject go;

      private CoyoteTimer timer;

      private MovementSettings settings;

      private void SetupTest(float coyoteTime) {
        go = new GameObject();
        timer = go.AddComponent<CoyoteTimer>();
        settings = go.AddComponent<MovementSettings>();
        timer.CoyoteTime += () => settings.CoyoteTime;
      }

      [Test]
      public void CoyoteTimer_Can_End() {
        SetupTest(0.5f);

        for (int i=0; i < 100; i++) {
          timer.Tick();
        }
        
        Assert.False(timer.InCoyoteTime());
      }

      [Test]
      public void CoyoteTimer_Can_Reset() {
        SetupTest(0.5f);

        for (int i=0; i < 100; i++) {
          timer.Tick();
        }

        timer.StartCoyoteTime();

        Assert.True(timer.InCoyoteTime());
      }
      

      [Test]
      public void CoyoteTimer_UseCoyoteTime() {
        SetupTest(0.5f);

        timer.UseCoyoteTime();

        Assert.False(timer.InCoyoteTime());
      }

    }
}
