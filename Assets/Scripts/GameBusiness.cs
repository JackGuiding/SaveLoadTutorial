using System;
using System.Collections.Generic;
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

            ctx.rooms.Add(1, CreatRoom());
            ctx.rooms.Add(2, CreatRoom());

        }

        static int roomIDRecord = 0;
        static RoomEntity CreatRoom() {
            RoomEntity room = new RoomEntity();
            room.id = roomIDRecord++;
            room.roles = new RoleEntity[UnityEngine.Random.Range(1, 5)];
            for (int i = 0; i < room.roles.Length; i++) {
                var role = new GameObject("ROLE").AddComponent<RoleEntity>();
                role.id = i;
                role.SetPos(UnityEngine.Random.insideUnitCircle * 5);
                room.roles[i] = role;
            }
            return room;
        }

        // ==== 原理 ====
        // 怎么存, 就用相同方式读
        public static void LoadGame(GameContext ctx) {
            Vector2 pos = new Vector2();

            // Type1: 字符串
            // pos = LoadType1();
            // pos = LoadType2();
            // pos = LoadType3();
            LoadType4(ctx);

            ctx.role.SetPos(pos);

        }

        public static void SaveGame(GameContext ctx) {
            // 存角色坐标
            Vector2 pos = ctx.role.transform.position;

            // Type1: 字符串
            // SaveType1(pos);
            // SaveType2(pos);
            // SaveType3(pos);
            SaveType4(ctx);

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

        #region Type4: BufferEncoder, Room
        static void SaveType4(GameContext ctx) {
            byte[] data = new byte[200 * 1024 * 1024];
            int offset = 4;
            foreach (var room in ctx.rooms.Values) {
                RoomSaveMessage message = new RoomSaveMessage();
                message.id = room.id;
                message.roles = new List<RoleSaveMessage>(room.roles.Length);
                for (int i = 0; i < room.roles.Length; i++) {
                    RoleEntity role = room.roles[i];
                    RoleSaveMessage roleMessage = new RoleSaveMessage();
                    roleMessage.id = role.id;
                    roleMessage.pos = role.transform.position;
                    message.roles.Add(roleMessage);
                }

                message.WriteTo(data, ref offset);
            }

            int length = offset;
            offset = 0;
            GFBufferEncoderWriter.WriteUInt32(data, (uint)length, ref offset);

            // 语法糖
            // using (FileStream fs = new FileStream("slot1.save", FileMode.Create)) {
            //     fs.Write(data, 0, offset);
            // }

            // 本质
            // GC 不托管 Stream
            FileStream fs = new FileStream("slot1.save", FileMode.Create);
            try {
                fs.Write(data, 0, length);
            } finally {
                fs.Dispose();
            }

            // 打印房间信息
            foreach (var room in ctx.rooms.Values) {
                Debug.Log("Room ID: " + room.id);
                foreach (var role in room.roles) {
                    Debug.Log("\tRole ID: " + role.id + " Pos: " + role.transform.position);
                }
            }

        }

        static void LoadType4(GameContext ctx) {
            byte[] data = File.ReadAllBytes("slot1.save");
            int offset = 0;
            int length = (int)GFBufferEncoderReader.ReadUInt32(data, ref offset);
            // Load Game
            while (offset < length) {

                RoomSaveMessage roomMsg = new RoomSaveMessage();
                roomMsg.FromBytes(data, ref offset);

                RoomEntity room = new RoomEntity();
                room.id = roomMsg.id;
                room.roles = new RoleEntity[roomMsg.roles.Count];
                for (int i = 0; i < roomMsg.roles.Count; i++) {
                    RoleSaveMessage roleMessage = roomMsg.roles[i];
                    RoleEntity role = new GameObject("ROLE").AddComponent<RoleEntity>();
                    role.id = roleMessage.id;
                    role.SetPos(roleMessage.pos);
                    room.roles[i] = role;
                }
                ctx.rooms.Add(room.id, room);

            }

            // 打印房间信息
            foreach (var room in ctx.rooms.Values) {
                Debug.Log("Room ID: " + room.id);
                foreach (var role in room.roles) {
                    Debug.Log("\tRole ID: " + role.id + " Pos: " + role.transform.position);
                }
            }

        }
        #endregion

    }
}