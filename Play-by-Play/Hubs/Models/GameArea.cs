using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Play_by_Play.Hubs.Models {
	public class GameArea {
		public int X { get; set; }
		public int Y { get; set; }
		[ScriptIgnore]
		public GameArea Opposite { get; set; }

		public List<Player> HomePlayers { get; set; }
		public List<Player> AwayPlayers { get; set; }

		public GameArea() {
			HomePlayers = new List<Player>();
			AwayPlayers = new List<Player>();
		}

		private static readonly string[][] areaNames = new[]{
			new[]{"gameBoardLW", "gameBoardRW"},
			new[]{"gameBoardLCW", "gameBoardRCW"}, 
			new[]{"gameBoardLCD", "gameBoardRCD"},
			new[]{"gameBoardLD", "gameBoardRD"} 
		};
		public static string GetAreaName(int x, int y) {
			return areaNames[y][x];
		}
		public static int[] GetCoords(string areaName) {
			for (int y = 0; y < areaNames.Count(); y++) {
				for (int x = 0; x < areaNames[y].Count(); x++) {
					if (areaNames[y][x].Equals(areaName))
						return new[] { x, y };
				}
			}
			throw new Exception("No area with the specified name");
		}

		public bool AddHomePlayer(Player player) {
			if (HomePlayers.Count >= 5)
				return false;
			if (HomePlayers.Contains(player))
				return false;
			string side = (X == 0
			            	? "L"
			            	: "R");
			string pos = (Y < 2
			            	? "W"
			            	: "D");
			string bonusPos = side +
												pos;
			if(player.Position == bonusPos)
				player.Bonus = pos.Equals("W") ? Bonus.Offense : Bonus.Defense;
			HomePlayers.Add(player);
			return true;
		}

		public bool AddAwayPlayer(Player player) {
			if (AwayPlayers.Count >= 5)
				return false;
			if (AwayPlayers.Contains(player))
				return false;
			string side = (X != 0
			            	? "L"
			            	: "R");
			string pos = (Y >= 2
			            	? "W"
			            	: "D");
			string bonusPos = side +
												pos;
			if (player.Position == bonusPos)
				player.Bonus = pos.Equals("W") ? Bonus.Offense : Bonus.Defense;
			AwayPlayers.Add(player);
			return true;
		}

		public int GetHomeTotal(bool attacking) {
			string bonusPos = (X == 0 ? "L" : "R") +
												(Y < 2 && attacking ? "W" : "") +
												(Y >= 2 && !attacking ? "D" : "");

			return HomePlayers.Sum(x => attacking ? x.Offense : x.Defense) + HomePlayers.Count(x => x.Position == bonusPos && x.Bonus != Bonus.None);
		}

		public int GetAwayTotal(bool attacking) {
			string bonusPos = (X != 0 ? "L" : "R") +
												(Y >= 2 && attacking ? "W" : "") +
												(Y < 2 && !attacking ? "D" : "");

			return AwayPlayers.Sum(x => attacking ? x.Offense : x.Defense) + AwayPlayers.Count(x => x.Position == bonusPos && x.Bonus != Bonus.None);
		}

		public void Clear() {
			HomePlayers.ForEach(player => player.Bonus = Bonus.None);
			HomePlayers = new List<Player>();
			AwayPlayers = new List<Player>();
		}
	}
}