using System.Collections.Generic;
using System.Linq;
using Play_by_Play.Models;

namespace Play_by_Play.Hubs.Models {
	public class TacticResult {
		public TacticCard Card { get; set; }
		public bool IsHomeAttacking { get; set; }
		public List<BattleResult> Battles { get; set; }

		public TacticResult GetHomeResult() {
			var tacticResult = new TacticResult {
				Card = IsHomeAttacking ? Card : Card.Reverse(),
				IsHomeAttacking = IsHomeAttacking,
				Battles = Battles.Select(x => x.GetHomeResult()).ToList()
			};
			return tacticResult;
		}

		public TacticResult GetAwayResult() {
			var tacticResult = new TacticResult {
				Card = IsHomeAttacking ? Card.Reverse() : Card,
				IsHomeAttacking = IsHomeAttacking,
				Battles = Battles.Select(x => x.GetAwayResult()).ToList()
			};
			return tacticResult;
		}
	}
}