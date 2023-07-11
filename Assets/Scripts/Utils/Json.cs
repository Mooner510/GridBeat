using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Utils {
    public static class Json {
        public static void CreateJsonFile(string fileName, object obj) {
            var fileStream = new FileStream(fileName, FileMode.Create);
            var json = JsonUtility.ToJson(obj);
            var data = Encoding.UTF8.GetBytes(json);
            fileStream.Write(data, 0, data.Length);
            fileStream.Close();
        }

        public static T LoadJsonFile<T>(string fileName) {
            var fileStream = new FileStream(fileName, FileMode.Open);
            var reader = new StreamReader(fileStream);
            var jsonData = reader.ReadToEnd();
            reader.Close();
            fileStream.Close();
            return JsonUtility.FromJson<T>(jsonData);
        }
    }
}