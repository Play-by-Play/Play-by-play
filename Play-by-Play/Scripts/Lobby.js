var Lobby = function (username) {
	this.username = username;
};
Lobby.prototype = {

	initialize: function () {
		this.width = $('body').width() * 0.9,
		this.height = $('body').height() * 0.9;
		this.userDialog = $('#userDialogTmpl').tmpl();
		this.lobbyDialog = $('#lobbyTmpl').tmpl();

		this.userDialog.appendTo('body');

		this.userDialog.dialog({
			title: 'Lobby',
			height: this.height,
			width: this.width,
			modal: true,
			resizable: false,
			initialize: 'slide',
			autoOpen: false,
			draggable: false//,
			//open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); }
		});
		this.userDialog.dialog('open');
		$('#lobby-playerName').submit(function (e) {
			e.preventDefault();
			var username = $('#playerNameInput').val();
			if (username) {
				connection.createUser(username);
			}
		});



	},
	selectUsername: function (username) {
		this.username = username;
	},

	newGame: function (context) {
		connection.createGame();
		context.lobbyDialog.dialog('destroy');
		context.lobbyDialog.remove();
	},

	closeLobby: function () {
		this.lobbyDialog.dialog('close');
		this.lobbyDialog.remove();
	},

	openLobby: function (user) {
		this.userDialog.dialog('destroy');
		this.userDialog.remove();
		this.lobbyDialog.appendTo('body');

		connection.getGames();

		this.lobbyDialog.dialog({
			title: 'Lobby',
			height: this.height,
			width: this.width,
			modal: true,
			resizable: false,
			initialize: 'slide',
			draggable: false,
			autoOpen: false//,
			//open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); }
		});

		// $('#lobby-playerName').height(el.height() * 0.1);

		$('#lobby-games').height(this.lobbyDialog.height() - $('#lobby-playerName').height() - 2);
		$('#lobby-games').width(this.lobbyDialog.width() / 2 - 2);

		this.lobbyDialog.dialog('open');
		this.lobbyDialog.dialog('option', 'title', 'Lobby - ' + (user.Name || ""));

		var that = this;
		$('#new-game').click(function (e) {
			e.preventDefault();
			that.newGame(that);
		});

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
	}


};

