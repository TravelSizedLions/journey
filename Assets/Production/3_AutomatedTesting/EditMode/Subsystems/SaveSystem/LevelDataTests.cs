using NUnit.Framework;
using UnityEngine;
using Storm.Subsystems.Saving;
using System.IO;

namespace Testing.Subsystems.Saving {
  public class LevelDataTests {
    private const string GAME_NAME="journey_data";

    private const string SLOT_NAME="tests";

    private const string LEVEL_NAME="data_store_testing";

    private LevelData data;

    private void SetupTest() {
      data = new LevelData(GAME_NAME, SLOT_NAME, LEVEL_NAME);
      data.DeleteFiles();
    }


    private void SetTestData(bool sameKey) {
      if (sameKey) {
        data.Set("test 1", "test 1");
        data.Set("test 1", 1);
        data.Set("test 1", new Vector2(1, 1));
      } else {
        data.Set("string", "string");
        data.Set("int", 1);
        data.Set("vector2", new Vector2(1, 1));
      }
    }

    [Test]
    public void Sets_Data_Same_Key() {
      SetupTest();

      SetTestData(true);


      Assert.AreEqual(3, data.Count);
    }

    [Test]
    public void Sets_Data_Diff_Key() {
      SetupTest();

      SetTestData(false);

      Assert.AreEqual(3, data.Count);
    }

    [Test]
    public void Sets_Data_Single_Type() {
      SetupTest();

      data.Set<string>("test 1", "test 1");
      data.Set<string>("test 2", "test 2");
      data.Set<string>("test 3", "test 3");

      Assert.AreEqual(3, data.Count);
    }

    [Test]
    public void Gets_Data() {
      SetupTest();

      SetTestData(true);

      string expected = "test 1";
      string result = data.Get<string>("test 1");

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void TryGet_Data_Success() {
      SetupTest();

      SetTestData(true);

      Assert.True(data.Get("test 1", out string value));
    }

    [Test]
    public void TryGet_Data_Failure() {
      SetupTest();

      SetTestData(true);

      Assert.False(data.Get("garbagio!", out string result));
    }

    [Test]
    public void TryGet_Data_Correct_Output() {
      SetupTest();
      SetTestData(true);
      string expected = "test 1";
      string result;
      data.Get("test 1", out result);

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void Saves_Data() {
      SetupTest();
      SetTestData(true);

      data.Save();

      foreach (string path in data.GetPaths()) {
        Assert.True(File.Exists(path));
      }
    }

    [Test]
    public void Loads_Data_No_Paths() {
      SetupTest();

      Assert.True(data.Load());
    }

    [Test]
    public void Loads_Data_Paths_Exist() {
      SetupTest();

      SetTestData(true);

      data.Save();

      data.Clear();

      Assert.True(data.Load());
    }

    [Test]
    public void DeleteFiles_Deletes_When_No_Data_In_Memory() {
      SetupTest();

      SetTestData(true);

      data.Save();

      data = new LevelData(GAME_NAME, SLOT_NAME, LEVEL_NAME);

      data.DeleteFiles();

      Assert.True(data.GetPaths().Count == 0);
    }

    [Test]
    public void DeleteFiles_Deletes_When_Data_In_Memory() {
      SetupTest();

      SetTestData(true);

      data.Save();
      
      Debug.Log(data.GetPaths().Count);

      data.DeleteFiles();

      Debug.Log(data.GetPaths().Count);

      Assert.True(data.GetPaths().Count == 0);
    }

  }
}