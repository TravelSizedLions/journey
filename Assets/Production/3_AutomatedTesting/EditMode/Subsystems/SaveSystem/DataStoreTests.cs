using NUnit.Framework;
using NSubstitute;

using UnityEngine;

using Storm.Subsystems.Dialog;
using XNode;
using Storm.Characters.Player;
using Storm.Subsystems.Saving;
using System.IO;
using System;

namespace Testing.Subsystems.Saving {
  public class DataStoreTests {

    private const string GAME_NAME="journey_data";

    private const string SLOT_NAME="tests";

    private const string LEVEL_NAME="data_store_testing";

    private DataStore<string> stringStore;

    private DataStore<int> intStore;

    private DataStore<Vector2> vecStore;


    private void SetupTest() {
      stringStore = new DataStore<string>(GAME_NAME, SLOT_NAME, LEVEL_NAME);
      intStore = new DataStore<int>(GAME_NAME, SLOT_NAME, LEVEL_NAME);
      vecStore = new DataStore<Vector2>(GAME_NAME, SLOT_NAME, LEVEL_NAME);

      stringStore.DeleteFile();
      intStore.DeleteFile();
      vecStore.DeleteFile();
    }

    [Test]
    public void Sets_Data() {
      SetupTest();
      string value = "test!";

      stringStore.Set("test", value);

      Assert.AreEqual(value, stringStore.Get("test"));
    }


    [Test]
    public void TryGet_Succeed() {
      SetupTest();
      string value = "test!";

      stringStore.Set("test", value);

      dynamic result;

      stringStore.Get("test", out result);

      Assert.True(stringStore.Get("test", out result));
    }

    [Test]
    public void TryGet_Fail() {
      SetupTest();

      Assert.False(stringStore.Get("test", out dynamic result));
    }


    [Test]
    public void Path_Is_Correct() {
      SetupTest();

      string expected = Path.Combine(new string[] {
        Application.persistentDataPath,
        GAME_NAME,
        SLOT_NAME,
        LEVEL_NAME,
        typeof(string).ToString()
      });

      expected += ".xml";
      expected = new Uri(expected).LocalPath;

      Assert.AreEqual(expected, stringStore.FilePath);
    }


    [Test]
    public void Saves_Data_Returns_True() {
      SetupTest();

      stringStore.Set("test 1", "test 1");
      stringStore.Set("test 2", "test 2");

      bool result = stringStore.Save();

      Assert.AreEqual(true, result);
    }

    [Test]
    public void Saves_Data_File_Exists() {
      SetupTest();

      stringStore.Set("test 1", "test 1");
      stringStore.Set("test 2", "test 2");

      stringStore.Save();

      Assert.True(File.Exists(stringStore.FilePath));
    }


    [Test]
    public void Clears_Data() {
      SetupTest();
      stringStore.Set("test 1", "test 1");
      stringStore.Set("test 2", "test 2");

      stringStore.Clear();

      Assert.AreEqual(0, stringStore.Count);
    }


    [Test]
    public void Deletes_File() {
      SetupTest();

      stringStore.Set("test 1", "test 1");
      stringStore.Set("test 2", "test 2");

      stringStore.Save();

      Assert.True(File.Exists(stringStore.FilePath));

      stringStore.DeleteFile();

      Assert.False(File.Exists(stringStore.FilePath));
    }

    [Test]
    public void Loads_Data_Fails() {
      SetupTest();

      bool result = stringStore.Load();

      Assert.False(result);
    }


    [Test]
    public void Loads_Data() {
      SetupTest();

      stringStore.Set("test 1", "test 1");
      stringStore.Set("test 2", "test 2");

      stringStore.Save();
      stringStore.Clear();
      stringStore.Load();

      Assert.AreEqual(2, stringStore.Count);
    }

    [Test]
    public void Load_Marks_Loaded() {
      SetupTest();

      stringStore.Set("test 1", "test 1");
      stringStore.Set("test 2", "test 2");

      stringStore.Save();
      stringStore.Clear();
      stringStore.Load();

      Assert.True(stringStore.Loaded);
    }


    [Test]
    public void Load_Marks_Synchronized() {
      SetupTest();

      stringStore.Set("test 1", "test 1");
      stringStore.Set("test 2", "test 2");

      stringStore.Save();
      stringStore.Clear();
      stringStore.Load();

      Assert.True(stringStore.Synchronized);
    }

    [Test]
    public void Set_Marks_Not_Synchronized() {
      SetupTest();

      stringStore.Set("test 1", "test 1");
      stringStore.Set("test 2", "test 2");

      stringStore.Save();
      stringStore.Clear();
      stringStore.Load();

      stringStore.Set("test 3", "test 3");

      Assert.False(stringStore.Synchronized);
    }

    [Test]
    public void Set_Marks_Loaded() {
      SetupTest();

      stringStore.Set("test 1", "test 1");
      stringStore.Set("test 2", "test 2");

      stringStore.Save();
      stringStore.Clear();

      stringStore.Set("test 3", "test 3");

      Assert.True(stringStore.Loaded);
    }

    [Test]
    public void Set_Loads_Data() {
      SetupTest();

      stringStore.Set("test 1", "test 1");
      stringStore.Set("test 2", "test 2");

      stringStore.Save();
      stringStore.Clear();

      stringStore.Set("test 3", "test 3");

      Assert.AreEqual(3, stringStore.Count); 
    }
  }
}