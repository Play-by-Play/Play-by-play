using System.Collections.Generic;
using System.Linq;

namespace Play_by_Play.Hubs.Models {
	public class BattleResult {
		public List<Player> HomePlayers { get; set; }
		public List<Player> AwayPlayers { get; set; }
		public int HomeModifier { get; set; }
		public int AwayModifier { get; set; }

		public int HomeTotal { 
			get {
				if (HomeModifier == 1)
					return 0;

				var playerSum = HomePlayers.Sum(x => x.Offence);
				return playerSum + HomeModifier;
			}
		}
		public int AwayTotal {
			get {
				if (AwayModifier == 1)
					return 0;

				var playerSum = AwayPlayers.Sum(x => x.Offence);
				return playerSum + AwayModifier;
			}
		}
	}
}