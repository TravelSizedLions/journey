using NUnit.Framework;
using UnityEngine;
using Storm.Subsystems.Save;
using System.IO;

namespace Tests.Subsystems.Save {
  public class SaveSlotTests {

    private const string GAME_NAME="journey_data";

    private const string SLOT_NAME="tests";

    private const string L1="data_store_testing_1";
    private const string L2="data_store_testing_2";

    private SaveSlot file;


    private void SetupTest() {
      file = new SaveSlot(GAME_NAME, SLOT_NAME);
      file.DeleteFolder();
      file.RegisterLevel(L1);
      file.RegisterLevel(L2);
    }


    private void SetData(bool multi) {
      if (multi) {
        string[] strs = new string[] { "test 1", "test 2", "test 3" };

        file.Set(L1, strs, strs);
        file.Set(L2, strs, strs);
      } else {
        file.Set(L1, "test 1", "test 1");
        file.Set(L1, "test 2", "test 2");
        file.Set(L1, "test 3", "test 3");
        file.Set(L2, "test 1", "test 1");
        file.Set(L2, "test 2", "test 2");
        file.Set(L2, "test 3", "test 3");
      }
    }

    [Test]
    public void Creates_Levels() {
      file = new SaveSlot(GAME_NAME, SLOT_NAME);
      file.RegisterLevel("1");
      file.RegisterLevel("2");
      file.RegisterLevel("3");

      Assert.AreEqual(3, file.FolderCount);
    }

    [Test]
    public void Sets_Data_Single() {
      SetupTest();

      SetData(false);

      Assert.AreEqual(6, file.DataCount);
    }

    [Test]
    public void Sets_Data_Multi() {
      SetupTest();

      SetData(true);

      Assert.AreEqual(6, file.DataCount);
    }

    [Test]
    public void Gets_Data_Single() {
      SetupTest();
      SetData(true);

      Assert.AreEqual("test 1", file.Get<string>(L1, "test 1"));
    }

    [Test]
    public void TryGets_Data_Single_Succeed() {
      SetupTest();
      SetData(true);

      Assert.True(file.Get(L1, "test 1", out string value));
    }

    [Test]
    public void TryGets_Data_Single_Fail() {
      SetupTest();
      SetData(true);

      Assert.False(file.Get(L1, "test", out string value));
    }

    [Test]
    public void TryGets_Data_Single_Correct_Value() {
      SetupTest();
      SetData(true);

      string value;

      file.Get(L1, "test 1", out value);
      Assert.AreEqual("test 1", value);
    }

    [Test]
    public void TryGets_Data_Multi_Succeed() {
      SetupTest();
      SetData(true);

      string[] keys = new string[] { "test 1", "test 2", "test 3" };
      
      Assert.True(file.Get<string>(L1, keys, out string[] values));
    }

    [Test]
    public void TryGets_Data_Multi_Fail() {
      SetupTest();
      SetData(true);

      string[] keys = new string[] { "test 1", "test", "test 3" };

      Assert.False(file.Get<string>(L1, keys, out string[] values));
    }

    [Test]
    public void TryGets_Data_Multi_Correct_Value() {
      SetupTest();
      SetData(true);

      string[] keys = new string[] { "test 1", "test 2", "test 3" };
      string[] values;

      file.Get<string>(L1, keys, out values);
      Assert.AreEqual(keys, values);
    }

    [Test]
    public void Clears_Data() {
      SetupTest();
      SetData(true);

      file.Clear();

      Assert.AreEqual(0, file.DataCount);
    }


    [Test]
    public void Deletes_Data() {
      SetupTest();
      SetData(true);

      file.DeleteFolder();

      string path = Path.Combine(new string[] {
        Application.persistentDataPath,
        GAME_NAME,
        SLOT_NAME
      });

      Assert.False(Directory.Exists(path));
    }


    [Test]
    public void Lazy_Loads_Data() {
      SetupTest();
      SetData(true);

      file.Save();

      file.Clear();

      file.Get(L1, "test 1", out string value1);
      file.Get(L2, "test 1", out string value2);

      Assert.AreEqual(6, file.DataCount);
    }
  }
}