window.PlayByPlay = (function ($) {
    // jQuery extensions
    $.scrollbarWidth = function () {
        var inner = document.createElement('p');
        inner.style.width = "100%";
        inner.style.height = "200px";

        var outer = document.createElement('div');
        outer.style.position = "absolute";
        outer.style.top = "0px";
        outer.style.left = "0px";
        outer.style.visibility = "hidden";
        outer.style.width = "200px";
        outer.style.height = "150px";
        outer.style.overflow = "hidden";
        outer.appendChild(inner);

        document.body.appendChild(outer);
        var w1 = inner.offsetWidth;
        outer.style.overflow = 'scroll';
        var w2 = inner.offsetWidth;
        if (w1 == w2) w2 = outer.clientWidth;

        document.body.removeChild(outer);

        return (w1 - w2);
    };
    $.fn.setFullWidth = function () {
        return this.each(function () {
            var $this = $(this);

            var margin = $this.marginLeft + $this.marginRight,
					border = $this.borderLeftWidth + $this.borderRightWidth,
					padding = $this.paddingLeft + $this.paddingRight;

            var parentWidth = $this.parent().width(),
					decoration = $this.outerWidth(true) - $this.width();

            $this.width(parentWidth - decoration);
        });
    };

    // Game
    var play = {
        addPlayerCard: function (color, team, name, attr1, attr2, pos, formation, draggable) {
            var data = { color: color, team: team, name: name, attr1: attr1, attr2: attr2, pos: pos, draggable: draggable };
            var elem = $('#playerCardTemplate').tmpl(data).css('background-color', '#' + data.color).appendTo('#' + formation);
        },
        addTacticCard: function (name, diff) {
            var data = { name: name, diff: diff };
            $('#tacticCardTemplate').tmpl(data).css('background-color', '#' + data.color).appendTo('#tacticCards');
        },
        addDetroitPlayers: function () {
            play.addPlayerCard("c00", "DET", "Zetterberg", 5, 3, "LW", "line1", " draggable");
            play.addPlayerCard("c00", "DET", "Datsyuk", 4, 4, "C", "line1", " draggable");
            play.addPlayerCard("c00", "DET", "Holmstrom", 3, 3, "RW", "line1", " draggable");
            play.addPlayerCard("c00", "DET", "Lidstrom", 3, 4, "LD", "line1", " draggable");
            play.addPlayerCard("c00", "DET", "Rafalski", 2, 4, "RD", "line1", " draggable");
            play.addPlayerCard("c00", "DET", "Howard", 3, 5, "G", "goalies", " draggable");

            play.addPlayerCard("c00", "DET", "Cleary", 4, 2, "LW", "line2", " draggable");
            play.addPlayerCard("c00", "DET", "Filppula", 4, 2, "C", "line2", " draggable");
            play.addPlayerCard("c00", "DET", "Bertuzzi", 4, 3, "RW", "line2", " draggable");
            play.addPlayerCard("c00", "DET", "Kronwall", 3, 3, "LD", "line2", " draggable");
            play.addPlayerCard("c00", "DET", "Stuart", 2, 4, "RD", "line2", " draggable");
            play.addPlayerCard("c00", "DET", "Osgood", 3, 4, "G", "goalies", " draggable");
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
        },
        addTacticCards: function () {
            play.addTacticCard("Give 'n Take", 4);
            play.addTacticCard("Left On", 4);
            play.addTacticCard("Longshot", 4);
            play.addTacticCard("Straight", 4);
            play.addTacticCard("Nailed", 3);
        }
    };

    // Server connectivity
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
                $('#chatInput').val('');
            });
            $('#chatInput').live('keypress', function (e) {
                var key = (e.keyCode || e.which);
                if (key == 13) {
                    $('#chatSubmit').click();
                }
            })

            $.connection.hub.start();
        }
    };

    // Layout
    var layout = {
        init: function () {
            $('#right').width(innerWidth - ($('#left').width() + $('#center').width()) - $.scrollbarWidth());
            $('.panel').setFullWidth();
        },

        drawGameboard: function () {
            // seting up canvas
            var canvas = document.getElementById("gameBoardCanvas");
            var context = canvas.getContext("2d");

            // editable values
            var iceColor = "#FFF";
            var borderColor = "#000";
            var redLineColor = "#F00";
            var blueLineColor = "#00F";

            // don't edit below
            var boardHeight = document.getElementById('center').offsetHeight;
            var boardWidth = document.getElementById('center').offsetWidth;
            var containerWidth = boardWidth;

            // correct proportions if neccessary
            var heightProportions = boardHeight / 8;
            var widthProportions = boardWidth / 5;

            if (heightProportions > widthProportions) {
                // set height depending on width
                boardHeight = boardHeight * (widthProportions / heightProportions);
            }
            else if (heightProportions < widthProportions) {
                // set width depending on height
                boardWidth = boardWidth * (heightProportions / widthProportions);
            }

            // set static size (used for testing)
            //var boardHeight = 80;
            //var boardWidth = 50;

            document.getElementById('gameBoardBackgroundLayer').style.height = boardHeight + 'px';
            document.getElementById('gameBoardBackgroundLayer').style.width = boardWidth + 'px';

            canvas.height = boardHeight;
            canvas.width = boardWidth;

            var margin = boardHeight / 80;
            var borderWidth = boardHeight / 80;
            var left = margin + borderWidth;
            var top = margin + borderWidth;
            var height = boardHeight - top * 2;
            var width = boardWidth - left * 2;
            var lineWidth = height / 160;


            // draw ice rink
            context.beginPath();
            context.arc(left + width / 5, top + height / 8, width / 5, -Math.PI / 2, Math.PI, true);
            context.arc(left + width / 5, top + height / 8 * 7, width / 5, Math.PI, Math.PI / 2, true);
            context.arc(left + width / 5 * 4, top + height / 8 * 7, width / 5, Math.PI / 2, 0, true);
            context.arc(left + width / 5 * 4, top + height / 8, width / 5, 0, -Math.PI / 2, true);
            context.closePath();
            context.fillStyle = iceColor;
            context.fill();
            context.lineWidth = borderWidth;
            context.strokeStyle = borderColor;
            context.stroke();


            // draw lines
            var lineLeft = left + borderWidth / 2;
            var lineRight = lineLeft + width - borderWidth;
            context.beginPath();
            context.strokeStyle = redLineColor;

            // red main middle line
            context.moveTo(lineLeft, top + height / 2);
            context.lineTo(lineRight, top + height / 2);
            context.lineWidth = lineWidth;
            context.stroke();

            // red goal lines
            context.moveTo(lineLeft, top + height / 8);
            context.lineTo(lineRight, top + height / 8);
            context.moveTo(lineLeft, top + height / 8 * 7 - lineWidth / 4);
            context.lineTo(lineRight, top + height / 8 * 7 - lineWidth / 4);
            context.lineWidth = lineWidth / 2;
            context.stroke();

            // red line between the goals
            context.moveTo(left + width / 2, top + height / 8);
            context.lineTo(left + width / 2, top + height / 8 * 7);
            context.lineWidth = lineWidth / 4;
            context.stroke();

            // blue lines
            context.beginPath();
            context.strokeStyle = blueLineColor;
            context.moveTo(lineLeft, top + height * 0.3125);
            context.lineTo(lineRight, top + height * 0.3125);
            context.moveTo(lineLeft, top + height * 0.6875);
            context.lineTo(lineRight, top + height * 0.6875);
            context.lineWidth = lineWidth;
            context.stroke();


            // set up boxes
            var boxLeft = lineLeft + 'px';
            var boxRight = lineLeft + 'px';
            var boxHeight = height * 0.1875 + 'px';
            var boxWidth = (lineRight - lineLeft) / 2 + 'px';
            document.getElementById('gameBoardGoalkeeperOpponent').style.marginTop = top + borderWidth + 'px';

            document.getElementById('gameBoardLW').style.left = boxLeft;
            document.getElementById('gameBoardLW').style.width = boxWidth;
            document.getElementById('gameBoardLW').style.height = boxHeight;

            document.getElementById('gameBoardRW').style.right = boxRight;
            document.getElementById('gameBoardRW').style.width = boxWidth;
            document.getElementById('gameBoardRW').style.height = boxHeight;

            document.getElementById('gameBoardLCW').style.left = boxLeft;
            document.getElementById('gameBoardLCW').style.width = boxWidth;
            document.getElementById('gameBoardLCW').style.height = boxHeight;

            document.getElementById('gameBoardRCW').style.right = boxRight;
            document.getElementById('gameBoardRCW').style.width = boxWidth;
            document.getElementById('gameBoardRCW').style.height = boxHeight;

            document.getElementById('gameBoardLCD').style.left = boxLeft;
            document.getElementById('gameBoardLCD').style.width = boxWidth;
            document.getElementById('gameBoardLCD').style.height = boxHeight;

            document.getElementById('gameBoardRCD').style.right = boxRight;
            document.getElementById('gameBoardRCD').style.width = boxWidth;
            document.getElementById('gameBoardRCD').style.height = boxHeight;

            document.getElementById('gameBoardLD').style.left = boxLeft;
            document.getElementById('gameBoardLD').style.width = boxWidth;
            document.getElementById('gameBoardLD').style.height = boxHeight;

            document.getElementById('gameBoardRD').style.right = boxRight;
            document.getElementById('gameBoardRD').style.width = boxWidth;
            document.getElementById('gameBoardRD').style.height = boxHeight;

            $('#gameboard').css('margin', '0 ' + (containerWidth - boardWidth) / 2 + 'px');

            layout.drawTactic();
        },

        drawTactic: function () {
            // seting up canvas
            var canvas = document.getElementById("gameBoardTacticalCanvas");
            var context = canvas.getContext("2d");

            // editable values
            var pointSize = 0.15; // radius of gameSquare height
            var relationWidth = 0.3;

            // don't edit below

            var gameSquareHeight = $("#gameBoardLD").height();
            var gameSquareWidth = $("#gameBoardLD").width();

            // draw start point

            // draw pass point
            //            context.beginPath();
            //            context.arc(10, 10, gameSquareHeight * pointSize, 0, Math.PI * 2, true);
            //            context.closePath();
            //            context.fill();

            // draw movment point

            // draw shot line

            // draw pass line

            // draw movment line

        }
    };

    $(window).bind('resize', function () {
        layout.init();
        layout.drawGameboard();
    });


    // On ready
    $(function () {
        layout.init();
        layout.drawGameboard();
        $('#console').tabs();
        $('#opponent').tabs();
        $('#player').tabs();

        play.addDetroitPlayers();
        play.addRangersPlayers();
        play.addTacticCards();
        $(".draggable").draggable({
            revert: "invalid",
            stack: ".draggable",
            start: function (event, ui) {
                if ($(this).hasClass("positionLW")) {
                    $(".gameSquareLW").each(function () {
                        $(this).addClass("gameSquareActiveStrong");
                    });
                }
                else if ($(this).hasClass("positionRW")) {
                    $(".gameSquareRW").each(function () {
                        $(this).addClass("gameSquareActiveStrong");
                    });
                }
                else if ($(this).hasClass("positionLD")) {
                    $(".gameSquareLD").each(function () {
                        $(this).addClass("gameSquareActiveStrong");
                    });
                }
                else if ($(this).hasClass("positionRD")) {
                    $(".gameSquareRD").each(function () {
                        $(this).addClass("gameSquareActiveStrong");
                    });
                }
            },
            stop: function (event, ui) {
                //
                if ($(this).hasClass("positionLW")) {
                    $(".gameSquareLW").each(function () {
                        $(this).removeClass("gameSquareActiveStrong");
                    });
                }
                else if ($(this).hasClass("positionRW")) {
                    $(".gameSquareRW").each(function () {
                        $(this).removeClass("gameSquareActiveStrong");
                    });
                }
                else if ($(this).hasClass("positionLD")) {
                    $(".gameSquareLD").each(function () {
                        $(this).removeClass("gameSquareActiveStrong");
                    });
                }
                else if ($(this).hasClass("positionRD")) {
                    $(".gameSquareRD").each(function () {
                        $(this).removeClass("gameSquareActiveStrong");
                    });
                }
            }
        });
        $(".gameSquare").droppable({
            accept: ".skater",
            activeClass: "gameSquareActive",
            hoverClass: "gameSquareHover",
            drop: function (event, ui) {
                // Find out offset depending on cards already put in the square
                var i = 2 + 30 * $(this).children().length;
                // Resize and move card
                ui.draggable.css('width', '50%');
                ui.draggable.appendTo($(this));
                // Place the card correctly
                ui.draggable.position({
                    of: $(this),
                    my: 'right bottom',
                    at: 'right bottom',
                    offset: '-' + i + 'px -2px'
                });
                // Remove strong hover if in place
                $(this).removeClass("gameSquareHoverStrong");
            },
            over: function (event, ui) {
                // add strong hovering if hovering over special square
                if ($(ui.draggable).hasClass("positionLW") && $(this).hasClass("gameSquareLW")) {
                    $(this).addClass("gameSquareHoverStrong");
                }
                else if ($(ui.draggable).hasClass("positionRW") && $(this).hasClass("gameSquareRW")) {
                    $(this).addClass("gameSquareHoverStrong");
                }
                else if ($(ui.draggable).hasClass("positionLD") && $(this).hasClass("gameSquareLD")) {
                    $(this).addClass("gameSquareHoverStrong");
                }
                else if ($(ui.draggable).hasClass("positionRD") && $(this).hasClass("gameSquareRD")) {
                    $(this).addClass("gameSquareHoverStrong");
                }
            },
            out: function (event, ui) {
                // remove strong hovering if leving a special square
                if ($(ui.draggable).hasClass("positionLW") && $(this).hasClass("gameSquareLW")) {
                    $(this).removeClass("gameSquareHoverStrong");
                }
                else if ($(ui.draggable).hasClass("positionRW") && $(this).hasClass("gameSquareRW")) {
                    $(this).removeClass("gameSquareHoverStrong");
                }
                else if ($(ui.draggable).hasClass("positionLD") && $(this).hasClass("gameSquareLD")) {
                    $(this).removeClass("gameSquareHoverStrong");
                }
                else if ($(ui.draggable).hasClass("positionRD") && $(this).hasClass("gameSquareRD")) {
                    $(this).removeClass("gameSquareHoverStrong");
                }
            }
        });
        $("#gameBoardGoalkeeper").droppable({
            accept: ".goalie",
            activeClass: "gameSquareActive",
            hoverClass: "gameSquareHover",
            drop: function (event, ui) {
                $(this).droppable("destroy");
                // Resize and move card
                ui.draggable.css('width', '100%');
                ui.draggable.appendTo($(this));
                // Place the card correctly
                ui.draggable.position({
                    of: $(this),
                    my: 'center center',
                    at: 'center center'
                });
            }
        });

        chat.init();

    });

    return play;
})(jQuery);
