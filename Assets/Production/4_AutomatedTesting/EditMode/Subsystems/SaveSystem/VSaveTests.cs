using NUnit.Framework;
using UnityEngine;
using HumanBuilders;

using System.IO;
using System.Collections.Generic;

namespace HumanBuilders.Tests {
  public class VSaveTests {

    private void SetupTest() {
      VSave.Reset();
      VSave.SavesFolderName = "tests";
      VSave.GameDataFolderName = "tests_game_data";
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

      VSave.DeleteSlot("test 1");

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

      VSave.SaveSlot();

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

      VSave.SaveSlot();
      VSave.Reset(true);

      VSave.LoadSlots();
      VSave.ChooseSlot("test 1");

      if (VSave.Get("folder", keys, out List<bool> outputs)) {
        Assert.AreEqual(inputs, outputs);
      } else {
        Assert.Fail("Failed to get info.");
      }
    }

    /*
      - Behaves just like normal save data
        - can set, save, clear, etc, just like save slots
      - Does not contaminate save slots
        - General data is in its own folder.
        - General data not seen as a save slot.
        - General data can be set with one save slot and accessed with another
        - General data can be set and accessed without save slots
    */

    public void Creates_General_Folder_On_First_Save() {
      SetupTest();
      VSave.SetGeneral("test", "test", "test");
      Assert.True(VSave.SaveGeneral());
    }

    public void Reset_Ignore_Folders_General_Data_Still_Present() {
      SetupTest();
      VSave.SetGeneral("test", "test", "test");
      VSave.SaveGeneral();

      VSave.Reset(true);

      Assert.AreEqual("test", VSave.GetGeneral<string>("test", "test"));
    }

    [Test]
    public void General_SetsTryGets_Data_Lists() {
      SetupTest();

      List<string> keys = new List<string>() { "A", "B", "C", "D" };
      List<bool> inputs = new List<bool>() { false, true, false, true };

      VSave.SetGeneral("folder", keys, inputs);

      if (VSave.GetGeneral("folder", keys, out List<bool> outputs)) {
        Assert.AreEqual(inputs, outputs);
      } else {
        Assert.Fail("Failed to get info.");
      }
    }

    [Test]
    public void General_SetsGets_Data_Lists() {
      SetupTest();

      List<string> keys = new List<string>() { "A", "B", "C", "D" };
      List<bool> inputs = new List<bool>() { false, true, false, true };

      VSave.SetGeneral("folder", keys, inputs);

      List<bool> outputs = VSave.GetGeneral<bool>("folder", keys);

      if (outputs != null) {
        Assert.AreEqual(inputs, outputs);
      } else {
        Assert.Fail("Failed to get info.");
      }
    }

    [Test]
    public void General_Sets_Data_Separate_From_Slot() {
      SetupTest();

      VSave.CreateSlot("slot 1");
      VSave.ChooseSlot("slot 1");

      VSave.Set<bool>("subfolder", "test", true);
      VSave.SetGeneral<bool>("subfolder", "test", false);

      VSave.SaveSlot();
      VSave.SaveGeneral();
      VSave.Reset(true);

      VSave.LoadSlots();
      VSave.ChooseSlot("slot 1");

      Assert.True(VSave.Get<bool>("subfolder", "test"));
      Assert.False(VSave.GetGeneral<bool>("subfolder", "test"));
    }

  }
}