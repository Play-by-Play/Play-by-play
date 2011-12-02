var Lobby = function (username) {
	this.username = username;
};
Lobby.prototype = {
	initialize: function () {
		var width = $('body').width() * 0.9,
		    height = $('body').height() * 0.9;
		var userDialog = this.userDialog = $('#new-user');
		var el = this.el = $('#lobby');

		this.el.dialog({
			title: 'Lobby',
			height: height,
			width: width,
			modal: true,
			resizable: false,
			initialize: 'slide',
			draggable: false,
			autoOpen: false//,
			//open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); }
		});

		this.userDialog.dialog({
			title: 'Lobby',
			height: height,
			width: width,
			modal: true,
			resizable: false,
			initialize: 'slide',
			draggable: false//,
			//open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); }
		});
		$('#lobby-playerName').submit(function (e) {
			e.preventDefault();
			var username = $('#playerNameInput').val();
			if (username) {
				connection.createUser(username);
			}
		});


		// $('#lobby-playerName').height(el.height() * 0.1);

		$('#lobby-games').height(el.height() - $('#lobby-playerName').height() - 2);
		$('#lobby-games').width(el.width() / 2 - 2);

		$('#new-game').click(this.newGame);

		$('.lobby-game').live('click', function (evt) {
			evt.preventDefault();
			var that = $(this);

			if (!that.hasClass('selected')) {
				// Not yet selected
				$('.selected').removeClass('selected');
				that.addClass('selected');
			} else {
				// Selected
			}
		});

		$('#join-game').click(function (evt) {
			evt.preventDefault();
			var game = $('.selected');
			if (game.length == 1) {
				var gameId = game.data('game-id');
				connection.joinGame(gameId);
			} else {
				alert("Not a valid game");
			}
		});
	},
	selectUsername: function (username) {
		this.username = username;
	},

	newGame: function () {
		connection.createGame();
		$('#lobby').dialog('close');
	},

	openLobby: function (message) {
		this.userDialog.dialog('close');
		this.el.dialog('open');
		this.el.dialog('option', 'title', 'Lobby - ' + username);
	}


};

