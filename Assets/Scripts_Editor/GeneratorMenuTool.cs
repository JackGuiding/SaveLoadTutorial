#if UNITY_EDITOR
// 仅在开发时使用
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using GameFunctions;
using GameFunctions.Editors;

namespace SaveLoadTutorial.Editor {

    public static class GeneratorMenuTool {

        [MenuItem("Tools/Generate Save Model")]
        public static void Generate() {
            string dir = Path.Combine(Application.dataPath, "Scripts", "SaveModel");
            Debug.Log(Application.dataPath);
            GFEBufferEncoderGenerator.GenModel(dir);
        }

    }

}
#endif