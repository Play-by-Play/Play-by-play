using System;
using System.Collections.Generic;
using Play_by_Play.Models;

namespace Play_by_Play.Hubs.Models {
	[Serializable]
	public class GameUser {
		public string ClientId { get; set; }
		public string Id { get; set; }
		public string Name { get; set; }
		public string Hash { get; set; }
		private List<TacticCard> CurrentCards { get; set; }
		private List<TacticCard> UsedCards { get; set; }
		public Team Team { get; set; }

		public GameUser() {

		}

		public GameUser(string name, string hash) {
			Name = name;
			Hash = hash;
			Id = Guid.NewGuid().ToString("d");
			CurrentCards = new List<TacticCard>();
			UsedCards = new List<TacticCard>();
		}

		public void AddTactic(TacticCard card) {
			CurrentCards.Add(card);
		}
	}
}