window.connection = (function (parameters) {
	var connection = $.connection.game;

	$.connection.hub.start(function () {
		window.user = connection.getUser();
		connection.getGames();
	});

	connection.getUser = function (user) {
		window.user = user;
		console.log(user);
	};

	connection.addUser = function (user) {
		console.log(user);
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
		console.log('Starting game');
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