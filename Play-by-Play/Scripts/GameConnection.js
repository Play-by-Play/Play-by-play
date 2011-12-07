window.connection = (function () {
	var connection = $.connection.game;
	// Server connectivity

	$.connection.hub.start(function () {
		if (PlayByPlay.debug) {
			connection.fakeIt();
		} else {
			window.user = connection.getUser();
			connection.getGames();
		}
	});

	connection.addChatMessage = function (name, message) {
		var data = { name: name, message: message };
		$('#chatMessageTemplate').tmpl(data).appendTo('#chatMessages');
	};

	connection.addActionMessage = function (message, type) {
		$('#actionTemplate').tmpl({ type: type, message: message }).appendTo('#actions');
	};

	connection.getUser = function (user) {
		window.user = user;
	};

	connection.addUser = function (user) {
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
		PlayByPlay.addTacticCards(eval(cards));
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
		PlayByPlay.hideFaceoff();
		PlayByPlay.showBattleView("Face Off", result);
	};

	connection.usernameExists = function () {
		$('#username-exist').text('Another user is using that username.').slideDown(200);
	};

	connection.removeGame = function (gameId) {
		$('.lobby-game[data-game-id=' + gameId + ']').remove();
	};

	return connection;
})();
