using System.Collections.Generic;
using System.Linq;
using Play_by_Play.Models;

namespace Play_by_Play.Hubs.Models {
	public class TacticResult {
		public TacticCard Card { get; set; }
		public bool IsHomeAttacking { get; set; }
		public List<BattleResult> Battles { get; set; }

		public TacticResult GetHomeResult() {
			return new TacticResult {
				Card = Card,
				IsHomeAttacking = IsHomeAttacking,
				Battles = Battles.Select(x => x.GetHomeResult()).ToList()
			};
		}

		public TacticResult GetAwayResult() {
			return new TacticResult {
				Card = Card,
				IsHomeAttacking = IsHomeAttacking,
				Battles = Battles.Select(x => x.GetAwayResult()).ToList()
			};
		}
	}
}