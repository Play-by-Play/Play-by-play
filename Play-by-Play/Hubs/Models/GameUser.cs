using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using SignalR.Hubs;

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
		[ScriptIgnore]
		public GameUser Oppenent { get; set; }

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
			return UseTactic(tactic);
		}

		public bool UseTactic(TacticCard tactic) {
			if (tactic == null)
				return false;
			string message = string.Format("{0} played \"{1}\"", this.Name, tactic.Name);
			SendActionMessage(message, "info");
			CurrentCards.Remove(tactic);
			UsedCards.Add(tactic);
			return true;
		}

		public void SetTurn(bool isPlayersTurn) {
			string message;
			if (isPlayersTurn) {
				message = "Now it's your turn";
			} else {
				var oppenentName = Oppenent.Name;
				message = string.Format("It is {0}'s turn now", oppenentName);
			}
			SendActionMessage(message, "info");
			Hub.GetClients<GameConnection>()[ClientId].setTurn(isPlayersTurn);
		}

		public void SendActionMessage(string message, string type) {
			Hub.GetClients<GameConnection>()[ClientId].addActionMessage(message, type);
		}
	}
}