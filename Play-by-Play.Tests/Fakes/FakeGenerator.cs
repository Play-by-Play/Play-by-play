using Play_by_Play.Hubs.Models;

namespace Play_by_Play.Tests.Fakes {
	public class FakeGenerator : RandomGenerator {

		public FakeGenerator() : this(3,6) {}

		public FakeGenerator(int homeMod, int awayMod) {
			numbers = new[]{homeMod, awayMod};
		}

		private static int[] numbers;
		private int num = 1;
		public override int Next(int min, int max) {
			return numbers[num++ % 2];
		}
	}
}