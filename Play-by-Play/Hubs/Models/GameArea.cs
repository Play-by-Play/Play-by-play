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

		private static string[][] areaNames = new []{
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
						return new[] {x, y};
				}
			}
			throw new Exception("No area with the specified name");
		}

		public bool AddHomePlayer(Player player) {
			if (HomePlayers.Count >= 5)
				return false;
			HomePlayers.Add(player);
			return true;
		}

		public bool AddAwayPlayer(Player player) {
			if (AwayPlayers.Count >= 5)
				return false;
			AwayPlayers.Add(player);
			return true;
		}

		public void Clear() {
			HomePlayers = new List<Player>();
			AwayPlayers = new List<Player>();
		}
	}
}