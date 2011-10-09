var Lobby = function (username) {
	this.username = username;
};
Lobby.prototype = {
	initialize: function () {
		var width = $('body').width() * 0.9,
			height = $('body').height() * 0.9,
		    userDialog = $('#new-user'),
			el = $('#lobby');

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
			el.dialog({
				title: 'Lobby',
				height: height,
				width: width,
				modal: true,
				resizable: false,
				initialize: 'slide',
				draggable: false
			});

		});



		$('#lobby-playerName').height(el.height() * 0.1);

		$('#lobby-games').height(el.height() - $('#lobby-playerName').height() - 2);
		$('#lobby-games').width(el.width() / 2 - 2);

		$('#new-game').click(this.newGame);
	},
	selectUsername: function (username) {
		this.username = username;
	},

	newGame: function () {

	}
};

