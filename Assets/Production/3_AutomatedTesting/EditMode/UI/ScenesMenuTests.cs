using NUnit.Framework;
using UnityEngine;
using HumanBuilders;
using System.IO;

namespace HumanBuilders.Tests {
  public class ScenesMenuTests {

    [OneTimeTearDown]
    public void TearDown() {
      string path = Path.Combine(Application.persistentDataPath, ScenesMenu.MAP_PATH);
      
      if (File.Exists(path)) {
        File.Delete(path);
      }
    }

    [Test]
    public void ScenesMenu_Generates_Map_Data() {
      ScenesMenu.GenerateMapData();
      string path = Path.Combine(Application.persistentDataPath, ScenesMenu.MAP_PATH);
      bool exists = File.Exists(path);
      
      Assert.True(exists);

      StreamReader reader = new StreamReader(path);
      // File should contain more than empty object "{ }". 
      Assert.True(reader.ReadToEnd().Length > 8);

      reader.Close();
    }
  }
}