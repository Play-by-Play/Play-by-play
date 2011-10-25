using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SignalR.Hubs;

namespace Play_by_Play.Hubs {
	[HubName("game")]
	public class GameConnection : Hub, IDisconnect {
		private static Dictionary<string, GameUser> users = new Dictionary<string, GameUser>(StringComparer.OrdinalIgnoreCase);
		private static Dictionary<string, Game> games = new Dictionary<string, Game>(StringComparer.OrdinalIgnoreCase);

		public void Send(string message) {
			var user = users.Values.FirstOrDefault(x => x.ClientId == Context.ClientId);
			if (user == null) return;
			Clients.addChatMessage(user.Name, message);
		}

		public void GetUser() {

			Caller.getUser(users.FirstOrDefault(x => x.Value.ClientId.Equals(Context.ClientId)).Value);
		}

		public void CreateUser(string username) {
			var userExists = users.FirstOrDefault(x => x.Key.Equals(username)).Value != null;
			if (!userExists) {
				var user = new GameUser(username, GetMD5Hash(username)) {
					ClientId = Context.ClientId
				};

				users[username] = user;

				Caller.Name = user.Name;
				Caller.Id = user.Id;
				Caller.Hash = user.Hash;

				Caller.addUser(user);

				return;
			}
			Caller.usernameExists();
		}

		public void CreateGame() {
			string name = Caller.Name;
			if (!users.ContainsKey(Caller.Name)) return;
			if (games.Values.Where(x => x.HomeUser.Name == Caller.Name || (x.AwayUser != null && x.AwayUser.Name == Caller.Name)).Count() > 0) return;
			var user = users[Caller.Name];
			var game = new Game {
				HomeUser = user
			};
			games[game.Id] = game;

			Caller.gameId = game.Id;

			Clients.addGame(game);
		}

		public void GetGames() {
			var activeGames = games.Values.Where(x => x.AwayUser == null).ToList();
			Caller.gameList(activeGames);
		}

		public void JoinGame(string gameId) {
			var game = games.FirstOrDefault(x => x.Key.Equals(gameId)).Value;

			if (game == null) return;

			var user = users.FirstOrDefault(x => x.Key.Equals(Caller.Name)).Value;

			game.AwayUser = user;

			Caller.gameId = game.Id;
			Caller.startGame(game);
			Caller.addActionMessage("Game against " + game.HomeUser.Name + " has started");

			Clients[game.HomeUser.ClientId].startGame(game);
			Clients[game.HomeUser.ClientId].addActionMessage("Game against " + game.AwayUser.Name + " has started");
			Clients.removeGame(game.Id);
		}

		public void AbortGame(Game game) {
			if (game.HomeUser != null) {
				Clients[game.HomeUser.ClientId].abortGame();
				game.HomeUser = null;
			}

			if (game.AwayUser != null) {
				Clients[game.AwayUser.ClientId].abortGame();
				game.AwayUser = null;
			}

			Clients.removeGame(game.Id);
			games.Remove(game.Id);
		}

		private string GetMD5Hash(string username) {
			return String.Join("", MD5.Create()
				.ComputeHash(Encoding.Default.GetBytes(username))
				.Select(b => b.ToString("x2")));
		}

		[Serializable]
		public class GameUser {
			public string ClientId { get; set; }
			public string Id { get; set; }
			public string Name { get; set; }
			public string Hash { get; set; }

			public GameUser() {

			}

			public GameUser(string name, string hash) {
				Name = name;
				Hash = hash;
				Id = Guid.NewGuid().ToString("d");
			}
		}

		public class Game {
			public string Id { get; set; }
			public List<GameEvent> Events { get; set; }
			public GameUser HomeUser { get; set; }
			public GameUser AwayUser { get; set; }

			public Game() {
				Id = Guid.NewGuid().ToString("d");
				Events = new List<GameEvent>();
			}
		}
		public class GameEvent {

		}

		public void Disconnect() {
			var user = users.Values.FirstOrDefault(x => x.ClientId == Context.ClientId);

			if (user == null) return;
			
			users.Remove(user.Name);

			var gamesToRemove = games.Values.Where(x => x.HomeUser == user || x.AwayUser == user).ToArray();
			
			for (int i = 0; i < gamesToRemove.Count(); i++) {
				AbortGame(gamesToRemove[i]);
			}
		}
	}
}