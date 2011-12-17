window.connection = window.connection || (function () {
	var userInfo = {};

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
		connection.getTacticCards(5);
		PlayByPlay.showFaceoff();
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

		window.PlayByPlay.disablePlayersExceptOn(activeLine);

		PlayByPlay.hideFaceoff();
		PlayByPlay.showBattleView(result);
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
		window.PlayByPlay.setTacticsEnabled(isTurn);
	};

	return connection;
})();
