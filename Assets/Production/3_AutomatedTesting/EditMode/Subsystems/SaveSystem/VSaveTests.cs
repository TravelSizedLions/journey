using NUnit.Framework;
using UnityEngine;
using Storm.Subsystems.VSave;
using System.IO;

namespace Tests.Subsystems.VSave {
  public class VSaveTests {

    private void SetupTest() {
      Storm.Subsystems.VSave.VSave.Reset();
      Storm.Subsystems.VSave.VSave.FolderName = "tests";
    }

    [Test]
    public void Creates_Slots() {
      SetupTest();

      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 1");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 2");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 3");

      Assert.AreEqual(3, Storm.Subsystems.VSave.VSave.SlotCount);
    }

    [Test]
    public void Creates_Slots_Folders_Exist() {
      SetupTest();

      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 1");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 2");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 3");

      Assert.AreEqual(3, Storm.Subsystems.VSave.VSave.PhysicalSlotCount);
    }

    [Test]
    public void Resets_Delete_Folders_No_Slots_In_Memory() {
      SetupTest();

      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 1");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 2");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 3");

      Storm.Subsystems.VSave.VSave.Reset();

      Assert.AreEqual(0, Storm.Subsystems.VSave.VSave.SlotCount);
    }

    [Test]
    public void Resets_Delete_Folders_No_Physical_Folders() {
      SetupTest();

      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 1");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 2");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 3");

      Storm.Subsystems.VSave.VSave.Reset();

      Assert.AreEqual(0, Storm.Subsystems.VSave.VSave.PhysicalSlotCount);
    }

    [Test]
    public void Resets_Ignore_Folders_No_Slots_In_Memory() {
      SetupTest();

      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 1");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 2");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 3");

      Storm.Subsystems.VSave.VSave.Reset(true);

      Assert.AreEqual(0, Storm.Subsystems.VSave.VSave.SlotCount);
    }

    [Test]
    public void Deletes_Folder() {
      SetupTest();

      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 1");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 2");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 3");

      Storm.Subsystems.VSave.VSave.Delete("test 1");

      Assert.AreEqual(2, Storm.Subsystems.VSave.VSave.SlotCount);
    }

    [Test]
    public void Preloads_Data() {
      SetupTest();

      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 1");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 2");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 3");

      Storm.Subsystems.VSave.VSave.Reset(true);

      Storm.Subsystems.VSave.VSave.LoadSlots();

      Assert.AreEqual(3, Storm.Subsystems.VSave.VSave.SlotCount);
    }

    [Test]
    public void Loads_Saved_File() {
      SetupTest();

      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 1");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 2");
      Storm.Subsystems.VSave.VSave.CreateSaveSlot("test 3");

      Storm.Subsystems.VSave.VSave.ChooseSaveSlot("test 1");

      Storm.Subsystems.VSave.VSave.Set("test 1", "test 1", "test 1");

      Storm.Subsystems.VSave.VSave.Save();

      Storm.Subsystems.VSave.VSave.Reset(true);

      Storm.Subsystems.VSave.VSave.LoadSlots();

      Storm.Subsystems.VSave.VSave.Load("test 1");

      Assert.AreEqual(1, Storm.Subsystems.VSave.VSave.ActiveDataCount);
    }
  }
}