using NUnit.Framework;
using NSubstitute;

using HumanBuilders;

namespace HumanBuilders.Tests {
  public class CarryCrouchEndTests : PlayerStateTest<CarryCrouchEnd> {

    [Test]
    public void Can_Start_Crouch_While_Carrying() {
      SetupTest();
            
      player.CarriedItem = BuildCarriable();
      player.HoldingDown().Returns(true);
      
      state.OnUpdate();

      AssertStateChange<CarryCrouchStart>();
    }

    [Test]
    public void Can_Run_While_Carrying() {
      SetupTest();
            
      player.CarriedItem = BuildCarriable();
      player.HoldingDown().Returns(false);
      player.TryingToMove().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<CarryRun>();
    }

    [Test]
    public void Can_Idle_While_Carrying() {
      SetupTest();
            
      player.CarriedItem = BuildCarriable();
      state.OnCarryCrouchEndFinished();
      
      AssertStateChange<CarryIdle>();
    }
  }
}