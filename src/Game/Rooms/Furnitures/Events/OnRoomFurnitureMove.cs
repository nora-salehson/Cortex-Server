using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Users.Furnitures;

using Cortex.Server.Game.Rooms;
using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Actions;
using Cortex.Server.Game.Rooms.Furnitures.Actions;

using Cortex.Server.Game.Furnitures;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Rooms.Furnitures.Events {
    class OnRoomFurnitureMove : ISocketEvent {
        public string Event => "OnRoomFurnitureMove";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;

            GameRoomUser roomUser = client.User.Room.GetUser(client.User.Id);

            if(!roomUser.HasRights())
                return 0;
            
            int id = data["id"].ToObject<int>();
            
            GameRoomFurniture roomFurniture = client.User.Room.Furnitures.Find(x => x.Id == id);

            if(roomFurniture == null)
                return 0;

            int row = data["position"]["row"].ToObject<int>();
            int column = data["position"]["column"].ToObject<int>();
            
            int direction = data["position"]["direction"].ToObject<int>();

            if(row == roomFurniture.Position.Row && column == roomFurniture.Position.Column && direction == roomFurniture.Position.Direction)
                return 0;

            if(!client.User.Room.Map.IsValidFloor(row, column))
                return 0;

            GameRoomFurniture stackedFurniture = client.User.Room.Map.GetFloorFurniture(row, column);

            double depth = client.User.Room.Map.GetFloorDepth(row, column);

            if(stackedFurniture != null) {
                if(!stackedFurniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Stackable))
                    return 0;

                depth = stackedFurniture.Position.Depth + stackedFurniture.GetDimension().Depth;
            }

            client.Send(new SocketMessage("OnRoomFurnitureMove", roomFurniture.Id).Compose());

            client.User.Room.Actions.AddEntity(roomFurniture.Id, new GameRoomFurniturePosition(roomFurniture, new GameRoomPoint(row, column, depth, direction)));

            return 1;
        }
    }
}
