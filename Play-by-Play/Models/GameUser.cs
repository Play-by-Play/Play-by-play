using System;
using System.Collections.Generic;

namespace Play_by_Play.Models {
	[Serializable]
	public class GameUser {
		public string ClientId { get; set; }
		public string Id { get; set; }
		public string Name { get; set; }
		public string Hash { get; set; }
		public Team Team { get; set; }

		public GameUser() {

		}

		public GameUser(string name, string hash) {
			Name = name;
			Hash = hash;
			Id = Guid.NewGuid().ToString("d");
		}
	}

	public class Team {
		public List<Player> Players { get; set; }
	}

	public class Player {
		public string Name { get; private set; }
		public Position Position { get; set; }

		public Player(string name) {
			Name = name;
		}
	}

	public enum Position {
		C,
		LW,
		RW,
		LD,
		RD,
		G
	}
}