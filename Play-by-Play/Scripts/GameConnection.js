window.connection = window.connection || (function () {
	var userInfo = {};
	var nextTurnActions = [];

	var connection = $.connection.game;
	// Server connectivity    
	$.connection.hub.start(function () {
		if (PlayByPlay.debug) {
			connection.fakeIt();
		} else {
			window.user = connection.getUser(userInfo.Name || "");
			connection.getGames();
		}
	});

	connection.addChatMessage = function (name, message) {
		var data = { name: name, message: message };
		$('#chatMessageTemplate').tmpl(data).appendTo('#chatMessages > .content');
		$('#chatMessages').nanoScroller({ scroll: 'bottom' });
	};

	connection.addActionMessage = function (message, type) {
		$('#actionTemplate').tmpl({ type: type, message: message }).appendTo('#actions');
	};

	connection.setUser = function (user) {
		userInfo = user;
	};

	connection.addUser = function (user) {
		connection.setUser(user);
		PlayByPlay.lobby.openLobby(user);
	};

	connection.addGame = function (game) {
		$('#lobby-game-tmpl').tmpl(game).appendTo('#active-games');
	};

	connection.gameList = function (games) {
		$('#active-games').empty();
		for (var game in games) {
			$('#lobby-game-tmpl').tmpl(games[game]).appendTo('#active-games');
		}
	};

	connection.startGame = function (game) {
		window.Game = game;
		PlayByPlay.lobby.closeLobby();
		connection.getPlayers();
	};

	connection.newPeriod = function () {
		nextTurnActions.push(function () {
			PlayByPlay.restorePlayers();
			PlayByPlay.enableAllPlayers();
			connection.getTacticCards();
			PlayByPlay.showFaceoff();
		});
	};

	connection.substitution = function () {
		var activeLine = $('#playerBench').data('active-line');
		var nextLine = 'line' + (1 + (+activeLine.substr(4, 1) % 2));
		var tabIds = [];
		$('#playerBench').children('div').each(function () { tabIds.push(this.id); });

		var lineIndex = $.inArray(nextLine, tabIds);

		nextTurnActions.push(function () {
			PlayByPlay.restorePlayers();

			$('#playerBench').tabs('select', lineIndex);

			window.PlayByPlay.disablePlayers(activeLine);
			window.PlayByPlay.enablePlayers(nextLine);
		});
	};

	connection.createTacticCards = function (cards) {
		PlayByPlay.addTacticCards(cards);
	};

	connection.opponentSelectTacticCard = function (tactic) {

	};

	connection.addPlayers = function (userteam, opponentteam) {
		PlayByPlay.addPlayers(userteam, opponentteam);
	};

	connection.placeOpponentPlayer = function (playerId, square) {
		PlayByPlay.placePlayerCard(playerId, square);
	};

	connection.faceOffResult = function (result) {
		var activeLine;

		if (result.IsHomePlayer === true) {
			activeLine = window.PlayByPlay.players.find(result.HomePlayers[0].Id).getLine();
		} else {
			activeLine = window.PlayByPlay.players.find(result.AwayPlayers[0].Id).getLine();
		}

		var tabIds = [];
		$('#playerBench').children('div').each(function () { tabIds.push(this.id); });

		var lineIndex = $.inArray(activeLine, tabIds);
		$('#playerBench').tabs('select', lineIndex);

		$('#playerBench').data('active-line', activeLine);

		window.PlayByPlay.disablePlayersExceptOn(activeLine);

		PlayByPlay.hideFaceoff();
		PlayByPlay.showBattleView(result);
		connection.nextTurn();
	};

	connection.tacticResult = function (result) {
		console.log(result);

		window.PlayByPlay.playTactic(result);
	};

	connection.usernameExists = function () {
		$('#username-exist').text('Another user is using that username.').slideDown(200);
	};

	connection.removeGame = function (gameId) {
		$('.lobby-game[data-game-id=' + gameId + ']').remove();
	};

	connection.setTurn = function (isTurn) {
		nextTurnActions.push(function () {
			window.PlayByPlay.setTacticsEnabled(isTurn);
		});
	};

	connection.nextTurn = function () {
		var actions = nextTurnActions;
		_.each(actions, function (action) {
			if (typeof action === 'function')
				action();
			else {
				var func = action[0];
				var parameters = _.without(action, func);
				func.apply(this, parameters);
			}
			nextTurnActions = _.without(nextTurnActions, action);
		});
	};

	connection.endGame = function () {
		PlayByPlay.endGame();
	};

	return connection;
})();
