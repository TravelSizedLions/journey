using NUnit.Framework;
using UnityEngine;
using Storm.Subsystems.Save;
using System.IO;
using System.Collections.Generic;

namespace Tests.Subsystems.Save {
  public class VSaveTests {

    private void SetupTest() {
      VSave.Reset();
      VSave.FolderName = "tests";
    }

    [Test]
    public void Creates_Slots() {
      SetupTest();

      VSave.CreateSlot("test 1");
      VSave.CreateSlot("test 2");
      VSave.CreateSlot("test 3");

      Assert.AreEqual(3, VSave.SlotCount);
    }

    [Test]
    public void Creates_Slots_Folders_Exist() {
      SetupTest();

      VSave.CreateSlot("test 1");
      VSave.CreateSlot("test 2");
      VSave.CreateSlot("test 3");

      Assert.AreEqual(3, VSave.PhysicalSlotCount);
    }

    [Test]
    public void Resets_Delete_Folders_No_Slots_In_Memory() {
      SetupTest();

      VSave.CreateSlot("test 1");
      VSave.CreateSlot("test 2");
      VSave.CreateSlot("test 3");

      VSave.Reset();

      Assert.AreEqual(0, VSave.SlotCount);
    }

    [Test]
    public void Resets_Delete_Folders_No_Physical_Folders() {
      SetupTest();

      VSave.CreateSlot("test 1");
      VSave.CreateSlot("test 2");
      VSave.CreateSlot("test 3");

      VSave.Reset();

      Assert.AreEqual(0, VSave.PhysicalSlotCount);
    }

    [Test]
    public void Resets_Ignore_Folders_No_Slots_In_Memory() {
      SetupTest();

      VSave.CreateSlot("test 1");
      VSave.CreateSlot("test 2");
      VSave.CreateSlot("test 3");

      VSave.Reset(true);

      Assert.AreEqual(0, VSave.SlotCount);
    }

    [Test]
    public void Deletes_Folder() {
      SetupTest();

      VSave.CreateSlot("test 1");
      VSave.CreateSlot("test 2");
      VSave.CreateSlot("test 3");

      VSave.Delete("test 1");

      Assert.AreEqual(2, VSave.SlotCount);
    }

    [Test]
    public void Loads_Slots() {
      SetupTest();

      VSave.CreateSlot("test 1");
      VSave.CreateSlot("test 2");
      VSave.CreateSlot("test 3");

      VSave.Reset(true);

      VSave.LoadSlots();

      Assert.AreEqual(3, VSave.SlotCount);
    }

    [Test]
    public void Loads_Saved_File() {
      SetupTest();

      VSave.CreateSlot("test 1");
      VSave.CreateSlot("test 2");
      VSave.CreateSlot("test 3");

      VSave.ChooseSlot("test 1");

      VSave.Set("test 1", "test 1", "test 1");

      VSave.Save();

      VSave.Reset(true);

      VSave.LoadSlots();

      VSave.ChooseSlot("test 1");
      VSave.Get<string>("test 1", "test 1");

      Assert.AreEqual(1, VSave.ActiveDataCount);
    }

    [Test]
    public void SetsTryGets_Data_Lists() {
      SetupTest();
      VSave.CreateSlot("test 1");
      VSave.ChooseSlot("test 1");

      List<string> keys = new List<string>() { "A", "B", "C", "D" };
      List<bool> inputs = new List<bool>() { false, true, false, true };

      VSave.Set("folder", keys, inputs);

      if (VSave.Get("folder", keys, out List<bool> outputs)) {
        Assert.AreEqual(inputs, outputs);
      } else {
        Assert.Fail("Failed to get info.");
      }
    }

    [Test]
    public void SetsGets_Data_Lists() {
      SetupTest();
      VSave.CreateSlot("test 1");
      VSave.ChooseSlot("test 1");

      List<string> keys = new List<string>() { "A", "B", "C", "D" };
      List<bool> inputs = new List<bool>() { false, true, false, true };

      VSave.Set("folder", keys, inputs);

      List<bool> outputs = VSave.Get<bool>("folder", keys);

      if (outputs != null) {
        Assert.AreEqual(inputs, outputs);
      } else {
        Assert.Fail("Failed to get info.");
      }
    }

    [Test]
    public void SavesLoads_Data_Lists() {
      SetupTest();
      VSave.CreateSlot("test 1");
      VSave.ChooseSlot("test 1");

      List<string> keys = new List<string>() { "A", "B", "C", "D" };
      List<bool> inputs = new List<bool>() { false, true, false, true };

      VSave.Set("folder", keys, inputs);

      VSave.Save();
      VSave.Reset(true);

      VSave.LoadSlots();
      VSave.ChooseSlot("test 1");

      if (VSave.Get("folder", keys, out List<bool> outputs)) {
        Assert.AreEqual(inputs, outputs);
      } else {
        Assert.Fail("Failed to get info.");
      }
    }
  }
}