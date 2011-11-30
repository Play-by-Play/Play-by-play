using System.Collections.Generic;
using System.Linq;

namespace Play_by_Play.Hubs {
	public class Team {
		public string Name { get; set; }
		public string Color { get; set; }
		public List<Player> Players { get; set; } 
		public List<Player> Line1 { 
			get { return Players.Where(x => x.Formation == Formation.Line1).ToList(); }
		}
		public List<Player> Line2 {
			get { return Players.Where(x => x.Formation == Formation.Line2).ToList(); }
		}
		public List<Player> Goalies {
			get { return Players.Where(x => x.Formation == Formation.Goalies).ToList(); }
		}
	}
}