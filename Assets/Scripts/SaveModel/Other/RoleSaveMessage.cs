using System;
using System.Collections.Generic;
using UnityEngine;
using GameFunctions;

namespace SaveLoadTutorial
{
    [GFBufferEncoderMessage]
    public struct RoleSaveMessage : IGFBufferEncoderMessage<RoleSaveMessage>
    {
        public int id;
        public Vector2 pos;
        public void WriteTo(byte[] dst, ref int offset)
        {
            GFBufferEncoderWriter.WriteInt32(dst, id, ref offset);
            GFBufferEncoderWriter.WriteVector2(dst, pos, ref offset);
        }

        public void FromBytes(byte[] src, ref int offset)
        {
            id = GFBufferEncoderReader.ReadInt32(src, ref offset);
            pos = GFBufferEncoderReader.ReadVector2(src, ref offset);
        }
    }
}