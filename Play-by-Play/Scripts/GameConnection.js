﻿window.connection = (function (parameters) {
	var connection = $.connection.game;
	// Server connectivity

	$.connection.hub.start(function () {
		window.user = connection.getUser();
		connection.getGames();
	});

	connection.addChatMessage = function (name, message) {
		var data = { name: name, message: message };
		$('#chatMessageTemplate').tmpl(data).appendTo('#chatMessages');
	};

	connection.addActionMessage = function (message, type) {
		alert(true);
		$('#actionTemplate').tmpl({ type: type, message: message }).appendTo('#actions');
	};

	connection.getUser = function (user) {
		window.user = user;
	};

	connection.addUser = function (user) {
		PlayByPlay.lobby.openLobby();
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
		$('#lobby').dialog('close');
	};

	connection.usernameExists = function () {
		$('#username-exist').text('Another user is using that username.').slideDown(200);
	};

	connection.removeGame = function (gameId) {
		$('.lobby-game[data-game-id=' + gameId + ']').remove();
	};

	return connection;
})();