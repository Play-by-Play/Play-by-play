using System;
using System.Collections.Generic;
using System.Linq;
using Play_by_Play.Models;

namespace Play_by_Play.Hubs.Models {
	[Serializable]
	public class GameUser {
		public string ClientId { get; set; }
		public string Id { get; set; }
		public string Name { get; set; }
		public string Hash { get; set; }
		public List<TacticCard> CurrentCards { get; private set; }
		public List<TacticCard> UsedCards { get; private set; }
		public Team Team { get; set; }

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

		public void AddTactics(List<TacticCard> tacticCards) {
			CurrentCards.AddRange(tacticCards);
		}

		public bool UseTactic(int id) {
			var tactic = CurrentCards.SingleOrDefault(x => x.Id == id);
			if (tactic == null)
				return false;

			UsedCards.Add(tactic);
			return true;
		}
	}
}