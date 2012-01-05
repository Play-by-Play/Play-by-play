namespace Play_by_Play.Hubs.Models {
	public class Player {
		public int Id { get; set; }
		public string Name { get; set; }
		public string Position { get; set; }
		public string Formation { get; set; }
		public int Offense { get; set; }
		public int Defense { get; set; }
		public Bonus Bonus { get; set; }

		public Player Clone() {
			return new Player {
				Id = Id,
				Name = Name,
				Position = Position,
				Formation = Formation,
				Offense = Offense,
				Defense = Defense,
				Bonus = Bonus
			};
		}
	}
}