window.PlayByPlay = (function ($) {
    var play = {
        addPlayerCard: function (color, team, name, attr1, attr2, pos, formation) {
            var data = { color: color, team: team, name: name, attr1: attr1, attr2: attr2, pos: pos };
            $('#playerCardTemplate').tmpl(data).appendTo('#' + formation);
        },
        addDetroitPlayers: function () {
            play.addPlayerCard("c00", "DET", "Zetterberg", 5, 3, "LW", "line1");
            play.addPlayerCard("c00", "DET", "Datsyuk", 4, 4, "C", "line1");
            play.addPlayerCard("c00", "DET", "Holmstrom", 3, 3, "RW", "line1");
            play.addPlayerCard("c00", "DET", "Lidstrom", 3, 4, "LD", "line1");
            play.addPlayerCard("c00", "DET", "Rafalski", 2, 4, "RD", "line1");
            play.addPlayerCard("c00", "DET", "Howard", 3, 5, "G", "goalies");

            play.addPlayerCard("c00", "DET", "Cleary", 4, 2, "LW", "line2");
            play.addPlayerCard("c00", "DET", "Filppula", 4, 2, "C", "line2");
            play.addPlayerCard("c00", "DET", "Bertuzzi", 4, 3, "RW", "line2");
            play.addPlayerCard("c00", "DET", "Kronwall", 3, 3, "LD", "line2");
            play.addPlayerCard("c00", "DET", "Stuart", 2, 4, "RD", "line2");
            play.addPlayerCard("c00", "DET", "Osgood", 3, 4, "G", "goalies");
        },
        addRangersPlayers: function () {
            play.addPlayerCard("00c", "NYR", "Dubinsky", 5, 2, "LW", "oppLine1");
            play.addPlayerCard("00c", "NYR", "Drury", 5, 3, "C", "oppLine1");
            play.addPlayerCard("00c", "NYR", "Gaborik", 6, 1, "RW", "oppLine1");
            play.addPlayerCard("00c", "NYR", "Girardi", 1, 4, "LD", "oppLine1");
            play.addPlayerCard("00c", "NYR", "Staal", 3, 4, "RD", "oppLine1");
            play.addPlayerCard("00c", "NYR", "Lundqvist", 4, 4, "G", "oppGoalies");

            play.addPlayerCard("00c", "NYR", "Zuccarello", 4, 2, "LW", "oppLine2");
            play.addPlayerCard("00c", "NYR", "Anisimov", 4, 2, "C", "oppLine2");
            play.addPlayerCard("00c", "NYR", "Callahan", 4, 3, "RW", "oppLine2");
            play.addPlayerCard("00c", "NYR", "McCabe", 2, 4, "LD", "oppLine2");
            play.addPlayerCard("00c", "NYR", "Del Zotto", 2, 3, "RD", "oppLine2");
            play.addPlayerCard("00c", "NYR", "Biron", 2, 3, "G", "oppGoalies");
        }
    };

   

    var chat = {
        init: function () {
            connection = $.connection.chat;

            connection.addMessage = function (name, message) {
                var data = { name: name, message: message };
                $('#chatMessageTemplate').tmpl(data).appendTo('#chatMessages');
            }

            $('#chatSubmit').live('click', function () {
                connection.send($('#chatInput').val())
            .fail(function (e) {
                alert(e);
            });
            }).keypress;

            $.delegate()

            $.connection.hub.start();
        }
    };

    $(function () {
        $('#console').tabs();
        $('#opponent').tabs();
        $('#player').tabs();

        play.addDetroitPlayers();
        play.addRangersPlayers();
        chat.init();
    });

    return play;
})(jQuery);
