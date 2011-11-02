namespace Play_by_Play.Models.StateMachine {
	public class GameStateMachine {
		public IGameState CurrentState { get; private set; }

		public GameStateMachine() {
			CurrentState = new StartGameState();
		}

		public GameStateMachine Execute() {
			CurrentState = CurrentState.Execute(null);
			return this;
		} 
	}
}