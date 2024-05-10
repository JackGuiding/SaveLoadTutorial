using System;
using System.Collections.Generic;
using GameFunctions;

namespace SaveLoadTutorial
{
    [GFBufferEncoderMessage]
    public struct RoomSaveMessage : IGFBufferEncoderMessage<RoomSaveMessage>
    {
        public int id;
        public List<RoleSaveMessage> roles;
        public void WriteTo(byte[] dst, ref int offset)
        {
            GFBufferEncoderWriter.WriteInt32(dst, id, ref offset);
            GFBufferEncoderWriter.WriteMessageList(dst, roles, ref offset);
        }

        public void FromBytes(byte[] src, ref int offset)
        {
            id = GFBufferEncoderReader.ReadInt32(src, ref offset);
            roles = GFBufferEncoderReader.ReadMessageList(src, () => new RoleSaveMessage(), ref offset);
        }
    }
}