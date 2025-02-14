using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Users.Furnitures;
using Cortex.Server.Game.Rooms;
using Cortex.Server.Game.Shop;
using Cortex.Server.Game.Furnitures;
using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Users.Actions;
using Cortex.Server.Game.Rooms.Furnitures;
using Cortex.Server.Game.Rooms.Furnitures.Logics;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Users.Events {
    class Temp_DevFurniUpdate : ISocketEvent {
        public string Event => "Temp_DevFurniUpdate";

        public int Execute(SocketClient client, JToken data) {
            string id = data["id"].ToString();

            GameFurniture furniture = GameFurnitureManager.GetGameFurniture(id);

            if(furniture == null)
                return 0;

            if(data.SelectToken("depth") != null) {
                double depth = data["depth"].ToObject<double>();

                if(Program.Discord != null)
                    Program.Discord.Furniture(client.User, furniture, depth);

                furniture.Dimension.Depth = depth;

                using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                    connection.Open();

                    using(MySqlCommand command = new MySqlCommand("UPDATE furnitures SET depth = @depth WHERE id = @id", connection)) {
                        command.Parameters.AddWithValue("@id", furniture.Id);
                        command.Parameters.AddWithValue("@depth", furniture.Dimension.Depth);

                        command.ExecuteNonQuery();
                    }
                }
            }

            if(data.SelectToken("stackable") != null) {
                furniture.Flags ^= GameFurnitureFlags.Stackable;

                using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                    connection.Open();

                    using(MySqlCommand command = new MySqlCommand("UPDATE furnitures SET flags = @flags WHERE id = @id", connection)) {
                        command.Parameters.AddWithValue("@id", furniture.Id);
                        command.Parameters.AddWithValue("@flags", furniture.Flags);

                        command.ExecuteNonQuery();
                    }
                }
            }

            if(data.SelectToken("sitable") != null) {
                furniture.Flags ^= GameFurnitureFlags.Sitable;

                using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                    connection.Open();

                    using(MySqlCommand command = new MySqlCommand("UPDATE furnitures SET flags = @flags WHERE id = @id", connection)) {
                        command.Parameters.AddWithValue("@id", furniture.Id);
                        command.Parameters.AddWithValue("@flags", furniture.Flags);

                        command.ExecuteNonQuery();
                    }
                }
            }

            if(data.SelectToken("standable") != null) {
                furniture.Flags ^= GameFurnitureFlags.Standable;

                using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                    connection.Open();

                    using(MySqlCommand command = new MySqlCommand("UPDATE furnitures SET flags = @flags WHERE id = @id", connection)) {
                        command.Parameters.AddWithValue("@id", furniture.Id);
                        command.Parameters.AddWithValue("@flags", furniture.Flags);

                        command.ExecuteNonQuery();
                    }
                }
            }

            if(data.SelectToken("walkable") != null) {
                furniture.Flags ^= GameFurnitureFlags.Walkable;

                using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                    connection.Open();

                    using(MySqlCommand command = new MySqlCommand("UPDATE furnitures SET flags = @flags WHERE id = @id", connection)) {
                        command.Parameters.AddWithValue("@id", furniture.Id);
                        command.Parameters.AddWithValue("@flags", furniture.Flags);

                        command.ExecuteNonQuery();
                    }
                }
            }

            if(data.SelectToken("sleepable") != null) {
                furniture.Flags ^= GameFurnitureFlags.Sleepable;

                using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                    connection.Open();

                    using(MySqlCommand command = new MySqlCommand("UPDATE furnitures SET flags = @flags WHERE id = @id", connection)) {
                        command.Parameters.AddWithValue("@id", furniture.Id);
                        command.Parameters.AddWithValue("@flags", furniture.Flags);

                        command.ExecuteNonQuery();
                    }
                }
            }

            if(data.SelectToken("logic") != null) {
                string logic = data["logic"].ToString();

                furniture.Logic = logic;

                using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                    connection.Open();

                    using(MySqlCommand command = new MySqlCommand("UPDATE furnitures SET logic = @logic WHERE id = @id", connection)) {
                        command.Parameters.AddWithValue("@id", furniture.Id);
                        command.Parameters.AddWithValue("@logic", furniture.Logic);

                        command.ExecuteNonQuery();
                    }
                }

                foreach(GameRoom room in GameRoomManager.Rooms) {
                    foreach(GameRoomFurniture roomFurniture in room.Furnitures) {
                        if(roomFurniture.UserFurniture.Furniture.Id != furniture.Id)
                            continue;

                        roomFurniture.Logic = GameRoomFurnitureLogics.CreateLogic(roomFurniture);
                    }
                }
            }

            client.Send(new SocketMessage("Temp_DevFurniUpdate", true).Compose());

            return 1;
        }
    }

    class Temp_DevShopUpdate : ISocketEvent {
        public string Event => "Temp_DevShopUpdate";

        public int Execute(SocketClient client, JToken data) {
            int id = data["id"].ToObject<int>();
            int icon = data["icon"].ToObject<int>();

            GameShopPage page = GameShop.Pages.Find(x => x.Id == id);

            if(page == null)
                return 0;

            if(Program.Discord != null)
                Program.Discord.Shop(client.User, page, icon);

            page.Icon = icon;

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("UPDATE shop SET icon = @icon WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@id", page.Id);
                    command.Parameters.AddWithValue("@icon", page.Icon);

                    command.ExecuteNonQuery();
                }
            }

            client.Send(new SocketMessage("Temp_DevShopUpdate", true).Compose());

            return 1;
        }
    }
}
