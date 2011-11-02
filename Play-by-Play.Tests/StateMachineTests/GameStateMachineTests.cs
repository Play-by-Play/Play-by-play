using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Should;
using Play_by_Play.Models.StateMachine;

namespace Play_by_Play.Tests.StateMachineTests {
	
	class GameStateMachineTests {
		[Fact]
		public void ItHasAnInitialState() {
			var machine = new GameStateMachine();
			var initialState = machine.CurrentState as StartGameState;
			initialState.ShouldNotBeNull();
		}

		[Fact]
		public void ItChangesStateOnExecute() {
			var machine = new GameStateMachine();
			var currentState = machine.CurrentState;
			var nextState = machine.Execute().CurrentState;

			currentState.ShouldNotEqual(nextState);
		}
	}
}
