var Lobby = function (username) {
	this.username = username;
};
Lobby.prototype = {
	initialize: function () {
		var width = $('body').width() * 0.9,
			height = $('body').height() * 0.9,
		    userDialog = $('#new-user'),
			el = $('#lobby');

		el.dialog({
			title: 'Lobby',
			height: height,
			width: width,
			modal: true,
			resizable: false,
			initialize: 'slide',
			draggable: false,
			autoOpen: false
		});

		if (!window.user) {
			userDialog.dialog({
				title: 'Lobby',
				height: height,
				width: width,
				modal: true,
				resizable: false,
				initialize: 'slide',
				draggable: false
			});
			$('#add-user').click(function () {
				var username = $('#playerNameInput').val();
				if (username) {
					connection.createUser(username);
				}
				userDialog.dialog('close');
				el.dialog('open');
				el.dialog('option', 'title', 'Lobby - ' + username);
			});
		} else {
			el.dialog('open');
			el.dialog('option', 'title', 'Lobby - ' + user.Name);
		}

		$('#lobby-playerName').height(el.height() * 0.1);

		$('#lobby-games').height(el.height() - $('#lobby-playerName').height() - 2);
		$('#lobby-games').width(el.width() / 2 - 2);

		$('#new-game').click(this.newGame);
	},
	selectUsername: function (username) {
		this.username = username;
	},

	newGame: function () {
		connection.createGame();
	}


};

