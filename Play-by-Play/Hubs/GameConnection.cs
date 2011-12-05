using Play_by_Play.Hubs.Models;
using SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

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
			if (!users.ContainsKey(Caller.Name)) return;
			if (games.Values.Count(x => x.HomeUser.Name == Caller.Name || (x.AwayUser != null && x.AwayUser.Name == Caller.Name)) > 0) return;
			var user = users[Caller.Name];
			Game game = new Game {
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
			game.Start();

			Caller.gameId = game.Id;
			Caller.startGame(game);
			Caller.addActionMessage("Game against " + game.HomeUser.Name + " has started");

			Clients[game.HomeUser.ClientId].startGame(game);
			Clients[game.HomeUser.ClientId].addActionMessage("Game against " + game.AwayUser.Name + " has started");
			Clients.removeGame(game.Id);
		}

		public void GetTacticCards(int amount) {
			var game = games.Values.First(x => x.AwayUser.ClientId.Equals(Context.ClientId) || x.HomeUser.ClientId.Equals(Context.ClientId));
			var tactics = game.GenerateTactics(amount);

			var writer = new JavaScriptSerializer();
			var result = writer.Serialize(tactics);
			Caller.createTacticCards(result);
		}

		public void GetPlayers() {
			var game = games.Values.First(x => x.AwayUser.ClientId.Equals(Context.ClientId) || x.HomeUser.ClientId.Equals(Context.ClientId));
			var isHome = game.HomeUser.ClientId == Context.ClientId;
			if (isHome)
				Caller.addPlayers(game.HomeUser.Team, game.AwayUser.Team);
			else
				Caller.addPlayers(game.AwayUser.Team, game.HomeUser.Team);
		}

		public void PlacePlayer(int playerId, string areaName) {
			var username = Caller.Name;
			var user = users[Caller.Name] as GameUser;
			if (user == null)
				throw new Exception("User does not exist");
			var game = games.Values.First(z => z.AwayUser == user || z.HomeUser == user);
			var coords = GameArea.GetCoords(areaName);
			var oppositeX = 1 - coords[0];
			var oppositeY = 3 - coords[1];
			var isHome = user == game.HomeUser;
			var opponentId = isHome
												? game.AwayUser.ClientId
												: game.HomeUser.ClientId;

			var player = user.Team.Players.FirstOrDefault(p => p.Id == playerId);

			var debugFlip = false;
			if (player == null && Caller.DebugMode) {
				player = isHome
				         	? game.AwayUser.Team.Players.First(p => p.Id == playerId)
				         	: game.HomeUser.Team.Players.First(p => p.Id == playerId);

				debugFlip = true;
			}
			game.Board.PlacePlayer(player, coords[0], coords[1], isHome);
			string name = GameArea.GetAreaName(oppositeX, oppositeY);
			if (Caller.DebugMode == true && !debugFlip) 
				return;
			Clients[opponentId].placeOpponentPlayer(playerId, name);
		}

		public void PlaceGoalkeeper(int playerId) {
			var user = users[Caller.Name] as GameUser;
			if (user == null)
				throw new Exception("User does not exist");
			var game = games.Values.First(z => z.AwayUser == user || z.HomeUser == user);
			var isHome = user == game.HomeUser;
			
			var goalie = user.Team.Goalies.SingleOrDefault(g => g.Id == playerId);
			var debugFlip = false;
			if (goalie == null && Caller.DebugMode) {
				goalie = isHome
									? game.AwayUser.Team.Goalies.FirstOrDefault(p => p.Id == playerId)
									: game.HomeUser.Team.Goalies.FirstOrDefault(p => p.Id == playerId);

				debugFlip = true;
			} else if (goalie == null) {
				throw new Exception("No goalkeeper with that ID in the team");
			}

			if (isHome)
				game.Board.HomeGoalie = goalie;
			else
				game.Board.AwayGoalie = goalie;

			var opponentId = isHome
												? game.AwayUser.ClientId
												: game.HomeUser.ClientId;
			if (Caller.DebugMode != true && !debugFlip)
				Clients[opponentId].placeOpponentPlayer(playerId, "gameBoardGoalkeeperOpponent");

			ExecuteFaceOff(game);
		}

		public void PlaceFaceOffPlayer(int playerId) {
			var user = users[Caller.Name] as GameUser;
			if (user == null)
				throw new Exception("User does not exist");
			var game = games.Values.First(z => z.AwayUser == user || z.HomeUser == user);
			var isHome = user == game.HomeUser;

			var player = user.Team.Players.SingleOrDefault(g => g.Id == playerId);
			var debugFlip = false;
			if (player == null && Caller.DebugMode) {
				player = isHome
									? game.AwayUser.Team.Goalies.FirstOrDefault(p => p.Id == playerId)
									: game.HomeUser.Team.Goalies.FirstOrDefault(p => p.Id == playerId);

				debugFlip = true;
			} else if (player == null) {
				throw new Exception("No player with that ID in the team");
			}

			if (isHome)
				game.Board.HomeFaceoff = player;
			else
				game.Board.AwayFaceoff = player;

			var opponentId = isHome
												? game.AwayUser.ClientId
												: game.HomeUser.ClientId;

			if (Caller.DebugMode != true && !debugFlip)
				Clients[opponentId].placeOpponentPlayer(playerId, "gameBoardFaceOffOpponent");

			ExecuteFaceOff(game);
		}

		private void ExecuteFaceOff(Game game) {
			if (!game.Board.IsReadyForFaceoff())
				return;

			var faceoffResult = game.ExecuteFaceOff();

			Clients[game.HomeUser.ClientId].faceOffResult(faceoffResult);
			Clients[game.AwayUser.ClientId].faceOffResult(faceoffResult);
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

		public void Disconnect() {
			var user = users.Values.FirstOrDefault(x => x.ClientId == Context.ClientId);

			if (user == null) return;

			users.Remove(user.Name);

			var gamesToRemove = games.Values.Where(x => x.HomeUser == user || x.AwayUser == user).ToArray();

			for (int i = 0; i < gamesToRemove.Count(); i++) {
				AbortGame(gamesToRemove[i]);
			}
		}

		# region Debugging

		public void FakeIt() {
			var homeuser = new GameUser("HomeUser", GetMD5Hash("HomeUser")) {
				ClientId = Context.ClientId
			};
			var awayuser = new GameUser("AwayUser", GetMD5Hash("AwayUser")) {
				ClientId = Context.ClientId
			};

			Caller.Name = homeuser.Name;
			Caller.Id = homeuser.Id;
			Caller.Hash = homeuser.Hash;

			users["Homeuser"] = homeuser;
			users["AwayUser"] = awayuser;

			Game game = new Game {
				HomeUser = homeuser,
				AwayUser = awayuser
			};
			games[game.Id] = game;

			game.Start();

			Caller.startGame(game);
			Caller.addActionMessage("Fake match started");
			Caller.DebugMode = true;
		}

		# endregion
	}
}