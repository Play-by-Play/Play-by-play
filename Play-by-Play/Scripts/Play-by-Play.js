// variables

// drawing
//var layout.margin, layout.borderWidth, left, top, height, width, lineWidth;
var iceColor = "#FFF";
var borderColor = "#000";
var redLineColor = "#F00";
var blueLineColor = "#00F";


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
        addTacticCard: function (name, diff, tactic) {
            var data = { name: name, diff: diff };
            var template = $('#tacticCardTemplate').tmpl(data).css('background-color', '#' + data.color).appendTo('#tacticCards');
            template.hover(function () {
                layout.drawTactic(tactic);
                $('.tacticCard').each(function () {
                    $(this).css({ opacity: 0.5 });
                });
                $(this).css({ opacity: 1.0 });
            },
            function () {
                layout.clearTactic();
                $(this).css({ opacity: 0.5 });
            });
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
            play.addTacticCard("Give 'n Take", 4, { startNode: [0, 2], nodes: [[0, 2], [1, 1], [0, 0]], movementNode: [[1, 0]], passes: [[[0, 2], [1, 1]], [[1, 1], [0, 0]], [[0, 0], [1, 0]]], movingPass: [[[1, 1], [1, 0]]], shot: [1, 0] });
            play.addTacticCard("Left On", 4, { startNode: [0, 3], nodes: [[0, 3], [0, 0]], movementNode: [], passes: [[[0, 3], [0, 0]]], movingPass: [], shot: [0, 0] });
            play.addTacticCard("Longshot", 4, { startNode: [0, 3], nodes: [[0, 3]], movementNode: [], passes: [], movingPass: [], shot: [0, 3] });
            play.addTacticCard("Straight", 4, { startNode: [0, 3], nodes: [[0, 3], [0, 2], [0, 0]], movementNode: [], passes: [[[0, 3], [0, 2]], [[0, 2], [0, 0]]], movingPass: [], shot: [0, 0] });
            play.addTacticCard("Nailed", 3, { startNode: [0, 2], nodes: [[0, 2]], movementNode: [], passes: [], movingPass: [], shot: [0, 2] });
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

            layout.boardHeight = document.getElementById('center').offsetHeight;
            layout.boardWidth = document.getElementById('center').offsetWidth;
            var containerWidth = layout.boardWidth;

            // correct proportions if neccessary
            var heightProportions = layout.boardHeight / 8;
            var widthProportions = layout.boardWidth / 5;

            if (heightProportions > widthProportions) {
                // set height depending on width
                layout.boardHeight = layout.boardHeight * (widthProportions / heightProportions);
            }
            else if (heightProportions < widthProportions) {
                // set width depending on height
                layout.boardWidth = layout.boardWidth * (heightProportions / widthProportions);
            }

            document.getElementById('gameBoardBackgroundLayer').style.height = layout.boardHeight + 'px';
            document.getElementById('gameBoardBackgroundLayer').style.width = layout.boardWidth + 'px';

            canvas.height = layout.boardHeight;
            canvas.width = layout.boardWidth;

            layout.margin = layout.boardHeight / 80;
            layout.borderWidth = layout.boardHeight / 80;
            var left = layout.margin + layout.borderWidth;
            var top = layout.margin + layout.borderWidth;
            var height = layout.boardHeight - top * 2;
            var width = layout.boardWidth - left * 2;
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
            context.lineWidth = layout.borderWidth;
            context.strokeStyle = borderColor;
            context.stroke();


            // draw lines
            var lineLeft = left + layout.borderWidth / 2;
            var lineRight = lineLeft + width - layout.borderWidth;
            context.beginPath();
            context.strokeStyle = redLineColor;

            // red main middle line
            context.moveTo(lineLeft, top + height / 2);
            context.lineTo(lineRight, top + height / 2);
            context.lineWidth = lineWidth;
            context.stroke();

            // red line between the goals
            context.beginPath();
            context.moveTo(left + width / 2, top + height / 8);
            context.lineTo(left + width / 2, top + height / 8 * 7);
            context.lineWidth = lineWidth / 4;
            context.stroke();

            // draw goal areas
            context.beginPath();
            context.moveTo(left + width / 2 + Math.cos(-Math.PI / 4) * height / 15, top + height / 8 * 7 - lineWidth / 4);
            context.arc(left + width / 2, top + height / 8 * 7 - lineWidth / 4, height / 15, -Math.PI / 4, Math.PI + Math.PI / 4, true);
            context.lineTo(left + width / 2 + Math.cos(Math.PI + Math.PI / 4) * height / 15, top + height / 8 * 7 - lineWidth / 4);
            context.closePath();
            context.fillStyle = "#6cf";
            context.fill();
            context.stroke();

            context.beginPath();
            context.moveTo(left + width / 2 + Math.cos(Math.PI + Math.PI / 4) * height / 15, top + height / 8);
            context.arc(left + width / 2, top + height / 8 - lineWidth / 4, height / 15, Math.PI - Math.PI / 4, Math.PI / 4, true);
            context.lineTo(left + width / 2 + Math.cos(-Math.PI / 4) * height / 15, top + height / 8);
            context.closePath();
            context.fillStyle = "#6cf";
            context.fill();
            context.stroke();

            // red goal lines
            context.beginPath();
            context.moveTo(lineLeft, top + height / 8);
            context.lineTo(lineRight, top + height / 8);
            context.moveTo(lineLeft, top + height / 8 * 7 - lineWidth / 4);
            context.lineTo(lineRight, top + height / 8 * 7 - lineWidth / 4);
            context.lineWidth = lineWidth / 2;
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

            // draw middle circle
            context.beginPath();
            context.arc(left + width / 2, top + height / 2, width / 10, 0, Math.PI * 2, true);
            context.lineWidth = lineWidth / 2;
            context.strokeStyle = "#000";
            context.fillStyle = "#fff";
            context.fill();
            context.stroke();

            // draw inner arcs
            context.beginPath();
            context.arc(left + width / 2, top + height / 2, width / 12, 0, -Math.PI / 2, true);
            context.lineWidth = lineWidth;
            context.strokeStyle = "#f00";
            context.stroke();

            context.beginPath();
            context.arc(left + width / 2, top + height / 2, width / 12, Math.PI, Math.PI / 2, true);
            context.lineWidth = lineWidth;
            context.strokeStyle = "#00f";
            context.stroke();

            // draw Play-by-Play text
            context.fillStyle = "#000";
            context.rotate(-30 * Math.PI / 180);
            context.font = height / 45 + "pt Calibri bold";
            context.textAlign = "center";
            context.fillText("Play", left - left / 5, top * 4 + height / 2);
            context.fillText("-by-", left - left / 5, top * 4 + height / 2 + height / 45);
            context.fillText("Play", left - left / 5, top * 4 + height / 2 + height / 22.5);


            // set up boxes
            var boxLeft = lineLeft + 'px';
            var boxRight = lineLeft + 'px';
            var boxHeight = height * 0.1875 + 'px';
            var boxWidth = (lineRight - lineLeft) / 2 + 'px';
            document.getElementById('gameBoardGoalkeeperOpponent').style.marginTop = top + layout.borderWidth + 'px';

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

            $('#gameboard').css('layout.margin', '0 ' + (containerWidth - layout.boardWidth) / 2 + 'px');
            //layout.drawTactic({ startNode: [0, 2], nodes: [[0, 2], [1, 1], [0, 0]], movementNode: [[1, 0]], passes: [[[0, 2], [1, 1]], [[1, 1], [0, 0]], [[0, 0], [1, 0]]], movingPass: [[[1, 1], [1, 0]]], shot: [1, 0] });
        },

        drawTactic: function (tactic) {
            // seting up canvas
            var canvas = document.getElementById("gameBoardTacticalCanvas");
            var context = canvas.getContext("2d");

            // clear the canvas before proceding
            layout.clearTactic();

            // editable values
            var pointSize = 0.15; // radius of gameSquare height
            var relationWidth = 0.3;
            var outerCircle = 1.2;

            // don't edig below
            canvas.height = layout.boardHeight;
            canvas.width = layout.boardWidth;

            var gameSquareHeight = $("#gameBoardLD").height();
            var gameSquareWidth = $("#gameBoardLD").width();

            var left = layout.margin + layout.borderWidth + layout.borderWidth / 2 + gameSquareWidth / 2;
            var top = layout.margin + layout.borderWidth;
            var height = layout.boardHeight - top * 2;
            var width = layout.boardWidth - left * 2;
            top = top + height / 8 + gameSquareHeight / 2;
            var lineWidth = height / 160;

            // set opacity for player cards
            $("#gameBoardBackgroundLayer").css({ opacity: 0.3 });

            // draw pass lines
            context.lineWidth = layout.borderWidth / 2;
            for (index in tactic.passes) {
                context.moveTo(left + gameSquareWidth * tactic.passes[index][0][0], top + gameSquareHeight * tactic.passes[index][0][1]);
                context.lineTo(left + gameSquareWidth * tactic.passes[index][1][0], top + gameSquareHeight * tactic.passes[index][1][1]);
                context.stroke();
            }

            // draw movment lines
            context.beginPath();
            for (index in tactic.movingPass) {
                context.dashedLine(left + gameSquareWidth * tactic.movingPass[index][0][0], top + gameSquareHeight * tactic.movingPass[index][0][1], left + gameSquareWidth * tactic.movingPass[index][1][0], top + gameSquareHeight * tactic.movingPass[index][1][1], gameSquareHeight / 10);
                context.stroke();
            }

            // draw shot line
            context.beginPath();
            context.strokeStyle = "#000";
            context.fillStyle = "#000";
            context.moveTo(left + gameSquareWidth * tactic.shot[0], top + gameSquareHeight * tactic.shot[1]);
            context.lineTo(left + gameSquareWidth / 2, top - gameSquareHeight / 2);
            context.stroke();

            // draw arrow on shot line
            var fromX = left + gameSquareWidth * tactic.shot[0];
            var fromY = top + gameSquareHeight * tactic.shot[1];
            var toX = left + gameSquareWidth / 2;
            var toY = top - gameSquareHeight / 2;
            var headlenght = gameSquareHeight / 5;
            var angle = Math.atan2(toY - fromY, toX - fromX);
            context.beginPath();
            context.moveTo(toX - headlenght * Math.cos(angle - Math.PI / 6), toY - headlenght * Math.sin(angle - Math.PI / 6));
            context.lineTo(toX, toY);
            context.lineTo(toX - headlenght * Math.cos(angle + Math.PI / 6), toY - headlenght * Math.sin(angle + Math.PI / 6));
            context.closePath();
            context.stroke();
            context.fill();

            // draw startnode
            context.lineWidth = layout.borderWidth / 4;
            context.strokeStyle = "#000";
            context.fillStyle = "#fff";
            context.beginPath();
            context.arc(left + gameSquareWidth * tactic.startNode[0], top + gameSquareHeight * tactic.startNode[1], gameSquareHeight * pointSize * outerCircle, 0, Math.PI * 2, true);
            context.closePath();
            context.fill();
            context.stroke();

            // draw pass point
            context.fillStyle = "#000";
            for (index in tactic.nodes) {
                context.beginPath();
                context.arc(left + gameSquareWidth * tactic.nodes[index][0], top + gameSquareHeight * tactic.nodes[index][1], gameSquareHeight * pointSize, 0, Math.PI * 2, true);
                context.closePath();
                context.fill();
            }

            // draw movment point
            context.lineWidth = layout.borderWidth / 2;
            context.fillStyle = "#fff";
            for (index in tactic.movementNode) {
                context.beginPath();
                context.arc(left + gameSquareWidth * tactic.movementNode[index][0], top + gameSquareHeight * tactic.movementNode[index][1], gameSquareHeight * pointSize, 0, Math.PI * 2, true);
                context.closePath();
                context.fill();
                context.stroke();
            }

        },

        clearTactic: function () {
            var canvas = document.getElementById("gameBoardTacticalCanvas");
            // remove drawn tactic
            canvas.width = canvas.width;
            // reset opacity to the player cards
            $("#gameBoardBackgroundLayer").css({ opacity: 1.0 });
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
        $("#tacticCards").hover(function () {
            // mouse over
            $('.tacticCard').each(function () {
                $(this).css({ opacity: 0.5 });
            });
        },
        function () {
            // mouse out
            $('.tacticCard').each(function () {
                $(this).css({ opacity: 1.0 });
            });
        });
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
                else if ($(this).hasClass("goalie")) {
                    $("#gameBoardGoalkeeper").each(function () {
                        $(this).addClass("gameSquareActiveStrong");
                    });
                }
            },
            stop: function (event, ui) {
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
                else if ($(this).hasClass("goalie")) {
                    $("#gameBoardGoalkeeper").each(function () {
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
                // Potentially add bonus point
                // Get hold of the card div
                var card = $(this).children()[$(this).children().length - 1];
                // Check player position
                var pos = card.getElementsByClassName("playerPos")[0].firstChild.nodeValue;
                if (pos == "RW") {
                    // Check game square
                    if ($(this).attr("id") == "gameBoardRW" || $(this).attr("id") == "gameBoardRCW") {
                        // Get offense value
                        var off = card.getElementsByClassName("attr1")[0];
                        // Add bonus point
                        off.innerHTML = parseInt(off.firstChild.nodeValue) + 1;
                        off.setAttribute("style", "color: #0c0");
                    }
                }
                if (pos == "LW") {
                    // Check game square
                    if ($(this).attr("id") == "gameBoardLW" || $(this).attr("id") == "gameBoardLCW") {
                        // Get offense value
                        var off = card.getElementsByClassName("attr1")[0];
                        // Add bonus point
                        off.innerHTML = parseInt(off.firstChild.nodeValue) + 1;
                        off.setAttribute("style", "color: #0c0");
                    }
                }
                if (pos == "RD") {
                    // Check game square
                    if ($(this).attr("id") == "gameBoardRD" || $(this).attr("id") == "gameBoardRCD") {
                        // Get offense value
                        var off = card.getElementsByClassName("attr2")[0];
                        // Add bonus point
                        off.innerHTML = parseInt(off.firstChild.nodeValue) + 1;
                        off.setAttribute("style", "color: #0c0");
                    }
                }
                if (pos == "LD") {
                    // Check game square
                    if ($(this).attr("id") == "gameBoardLD" || $(this).attr("id") == "gameBoardLCD") {
                        // Get offense value
                        var off = card.getElementsByClassName("attr2")[0];
                        // Add bonus point
                        off.innerHTML = parseInt(off.firstChild.nodeValue) + 1;
                        off.setAttribute("style", "color: #0c0");
                    }
                }
                ui.draggable.draggable("destroy");
                // Remove strong hover and active if in place
                $(this).removeClass("gameSquareHoverStrong");
                $(".gameSquare").each(function () {
                    $(this).removeClass("gameSquareActiveStrong");
                });
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
                $(this).removeClass("gameSquareHoverStrong");
            }
        });
        $("#gameBoardGoalkeeper").droppable({
            accept: ".goalie",
            activeClass: "gameSquareActive",
            hoverClass: "gameSquareHover",
            drop: function (event, ui) {
                // Resize and move card
                ui.draggable.css('width', '100%');
                ui.draggable.appendTo($(this));
                // Place the card correctly
                ui.draggable.position({
                    of: $(this),
                    my: 'center center',
                    at: 'center center'
                });
                ui.draggable.draggable("destroy");
            },
            over: function (event, ui) {
                $(this).addClass("gameSquareHoverStrong");
            },
            out: function (event, ui) {
                $(this).removeClass("gameSquareHoverStrong");
            }
        });

        chat.init();
    });

    return play;
})(jQuery);

CanvasRenderingContext2D.prototype.dashedLine = function (x1, y1, x2, y2, dashLen) {
    if (dashLen == undefined) dashLen = 2;

    this.beginPath();
    this.moveTo(x1, y1);

    var dX = x2 - x1;
    var dY = y2 - y1;
    var dashes = Math.floor(Math.sqrt(dX * dX + dY * dY) / dashLen);
    var dashX = dX / dashes;
    var dashY = dY / dashes;

    var q = 0;
    while (q++ < dashes) {
        x1 += dashX;
        y1 += dashY;
        this[q % 2 == 0 ? 'moveTo' : 'lineTo'](x1, y1);
    }
    this[q % 2 == 0 ? 'moveTo' : 'lineTo'](x2, y2);

    this.stroke();
    this.closePath();
};