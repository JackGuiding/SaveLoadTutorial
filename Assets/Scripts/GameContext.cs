using System.Collections.Generic;

namespace SaveLoadTutorial {

    public class GameContext {

        public RoleEntity role;
        public Dictionary<int, RoomEntity> rooms = new Dictionary<int, RoomEntity>();

    }

}