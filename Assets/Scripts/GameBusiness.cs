using System;
using System.IO;
using UnityEngine;
using GameFunctions;

namespace SaveLoadTutorial {

    public static class GameBusiness {

        public static void NewGame(GameContext ctx) {
            // 角色在初始位置
            float posX = 5;
            float posY = 5;
            Vector2 pos = new Vector2(posX, posY);
            ctx.role.SetPos(pos);
        }

        // ==== 原理 ====
        // 怎么存, 就用相同方式读
        public static void LoadGame(GameContext ctx) {
            Vector2 pos = new Vector2();

            // Type1: 字符串
            // pos = LoadType1();
            pos = LoadType2();
            // pos = LoadType3();

            ctx.role.SetPos(pos);

        }

        public static void SaveGame(GameContext ctx) {
            // 存角色坐标
            Vector2 pos = ctx.role.transform.position;

            // Type1: 字符串
            // SaveType1(pos);
            SaveType2(pos);
            // SaveType3(pos);

        }

        #region Type1
        static void SaveType1(Vector2 pos) {
            // 一种存档格式
            string x = pos.x.ToString();
            string y = pos.y.ToString();
            string result = x + "," + y;
            File.WriteAllText("slot1.save", result, System.Text.Encoding.UTF8);
        }

        static Vector2 LoadType1() {
            string lines = File.ReadAllText("slot1.save", System.Text.Encoding.UTF8);
            string[] parts = lines.Split(',');
            // 角色读档位置
            float posX = float.Parse(parts[0]); // tryparse
            float posY = float.Parse(parts[1]);
            return new Vector2(posX, posY);
        }
        #endregion

        // 字符串
        // JSON

        // 二进制
        // Google: Protobuf
        #region Type2
        static void SaveType2(Vector2 pos) {

            byte[] data = new byte[200 * 1024 * 1024]; // length 0 x 0 0 0 y 0 0 0
            int offset = 2;
            GFBufferEncoderWriter.WriteSingle(data, pos.x, ref offset);
            GFBufferEncoderWriter.WriteSingle(data, pos.y, ref offset);
            int length = offset;
            offset = 0;
            GFBufferEncoderWriter.WriteUInt16(data, (ushort)length, ref offset);

            Debug.Log("save length: " + length);

            using (FileStream fs = new FileStream("slot1.save", FileMode.Create)) {
                fs.Write(data, 0, length);
            }

        }

        static Vector2 LoadType2() {
            byte[] data = File.ReadAllBytes("slot1.save");
            int offset = 0;
            ushort length = GFBufferEncoderReader.ReadUInt16(data, ref offset);
            float posX = GFBufferEncoderReader.ReadSingle(data, ref offset);
            float posY = GFBufferEncoderReader.ReadSingle(data, ref offset);
            Debug.Log("load length: " + length);
            return new Vector2(posX, posY);
        }
        #endregion

        #region Type3: Json
        static void SaveType3(Vector2 pos) {
            string str = JsonUtility.ToJson(pos);
            File.WriteAllText("slot1.save", str, System.Text.Encoding.UTF8);
        }

        static Vector2 LoadType3() {
            string str = File.ReadAllText("slot1.save", System.Text.Encoding.UTF8);
            return JsonUtility.FromJson<Vector2>(str);
        }
        #endregion

    }
}