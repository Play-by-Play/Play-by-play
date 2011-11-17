// variables

// drawing
//var layout.margin, layout.borderWidth, left, top, height, width, lineWidth;


window.PlayByPlay = (function ($) {

    var iceColor = "#FFF";
    var borderColor = "#000";
    var redLineColor = "#F00";
    var blueLineColor = "#00F";

    var players = {
        home: {
            line1: {},
            line2: {},
            goalies: {}
        },
        away: {
            oppLine1: {},
            oppLine2: {},
            oppGoalies: {}
        },

        add: function (card, team) {
            team[card.getLine()][card.getPos()] = card;
        }
    };

    function PlayerCard(info, color, formation, draggable) {
        this.color = color;
        this.team = info.team;
        this.name = info.name;
        this.attr1 = info.attr1;
        this.attr2 = info.attr2;
        this.pos = info.pos;
        this.formation = formation;
        this.draggable = draggable;
        // assemble data
        var data = { color: this.color, team: this.team, name: this.name, attr1: this.attr1, attr2: this.attr2, pos: this.pos, draggable: (this.draggable ? " draggable" : "") };
        // construct card
        $('#playerCardTemplate').tmpl(data).appendTo('#' + this.formation);
    }
    PlayerCard.prototype = {
        getAttr1: function () {
            return this.attr1;
        },
        getAttr2: function () {
            return this.attr2;
        },
        getPos: function () {
            return this.pos;
        },
        getLine: function () {
            return this.formation;
        }
    }

    // Game
    var play = {
        addTacticCards: function () {
            play.addTacticCard("Give 'n Take", 4, { startNode: [0, 2], nodes: [[0, 2], [1, 1], [0, 0]], movementNode: [[1, 0]], passes: [[[0, 2], [1, 1]], [[1, 1], [0, 0]], [[0, 0], [1, 0]]], movingPass: [[[1, 1], [1, 0]]], shot: [1, 0] });
            play.addTacticCard("Left On", 4, { startNode: [0, 3], nodes: [[0, 3], [0, 0]], movementNode: [], passes: [[[0, 3], [0, 0]]], movingPass: [], shot: [0, 0] });
            play.addTacticCard("Longshot", 4, { startNode: [0, 3], nodes: [[0, 3]], movementNode: [], passes: [], movingPass: [], shot: [0, 3] });
            play.addTacticCard("Straight", 4, { startNode: [0, 3], nodes: [[0, 3], [0, 2], [0, 0]], movementNode: [], passes: [[[0, 3], [0, 2]], [[0, 2], [0, 0]]], movingPass: [], shot: [0, 0] });
            play.addTacticCard("Nailed", 3, { startNode: [0, 2], nodes: [[0, 2]], movementNode: [], passes: [], movingPass: [], shot: [0, 2] });
        },
        addTacticCard: function (name, diff, tactic) {
            var data = { name: name, diff: diff };
            var template = $('#tacticCardTemplate').tmpl(data).css('background-color', '#' + data.color).appendTo('#tacticCards');
            template.hover(function () {
                // mouse over
                layout.drawTactic(tactic);
                $('.tacticCard').each(function () {
                    $(this).css({ opacity: 0.5 });
                });
                $(this).css({ opacity: 1.0 });
            },
			function () {
			    // mouse out
			    layout.clearTactic();
			    $(this).css({ opacity: 0.5 });
			});
        },
        placeOpponentPlayerCard: function (card, square) {
            // Find card
            var cardDiv = $("#" + card.getLine()).find("." + card.getPos());
            // Find out offset depending on cards already put in the square
            var i = 2 + cardDiv.width() * 0.2 * $("#" + square).children().length;
            // Resize and move card
            cardDiv.addClass("onBoard");
            layout.setCardSizes();
            cardDiv.appendTo($("#" + square));
            // Place the card correctly
            cardDiv.position({
                of: $("#" + square),
                my: 'left top',
                at: 'left top',
                offset: i + 'px 2px'
            });
        },
        showBattleView: function (title, square) {
            $("#battle-view").dialog({
                title: 'Battle View',
                modal: true,
                draggable: false,
                resizable: false,
                open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); }
            });
        },
        restorePlayers: function () {
            var sum = 0;
            $(".card").each(function () {
                var pos = $(this).find(".playerPos").text();
                if ($(this).hasClass("onBoard") && pos != "G") {
                    var cardDiv = $(this);
                    // check which team player belongs to
                    if (cardDiv.hasClass("draggable")) { // player team
                        // get hold of the 1st line div according to players position
                        $("#line1").find(".placeholder").each(function () {
                            if ($(this).text().trim() == pos) {
                                // if already full, must be other line
                                if ($(this).length > 1) {
                                    // get hold of the 2nd line div according to players position
                                    $("#line2").find(".placeholder").each(function () {
                                        if ($(this).text().trim() == pos) {
                                            // place card in line 2
                                            cardDiv.appendTo($(this));
                                            cardDiv.removeClass("onBoard");
                                            layout.setCardSizes();
                                            // TODO: remove bonus points
                                        }
                                    });
                                } else {
                                    // place card in line 1
                                    cardDiv.appendTo($(this));
                                    cardDiv.removeClass("onBoard");
                                    layout.setCardSizes();
                                    // TODO: remove bonus points
                                }
                            }
                        });
                    } else { // opponent team
                        // get hold of the 1st line div according to players position
                        $("#oppLine1").find(".placeholder").each(function () {
                            if ($(this).text().trim() == pos) {
                                // if already full, must be other line
                                if ($(this).length > 1) {
                                    // get hold of the 2nd line div according to players position
                                    $("#oppLine2").find(".placeholder").each(function () {
                                        if ($(this).text().trim() == pos) {
                                            // place card in line 2
                                            cardDiv.appendTo($(this));
                                            cardDiv.removeClass("onBoard");
                                            layout.setCardSizes();
                                            // TODO: remove bonus points
                                        }
                                    });
                                } else {
                                    // place card in line 1
                                    cardDiv.appendTo($(this));
                                    cardDiv.removeClass("onBoard");
                                    layout.setCardSizes();
                                    // TODO: remove bonus points
                                }
                            }
                        });
                    }
                    sum++;
                }
            });
            //alert(sum + " cards are out");
        },
        addDetroitPlayers: function () {
            var color = "c00";
            var draggable = true;

            var formation = "line1";
            players.add(new PlayerCard({ team: "DET", name: "Zetterberg", attr1: 5, attr2: 3, pos: "LW" }, color, formation, draggable), players.home);
            players.add(new PlayerCard({ team: "DET", name: "Datsyuk", attr1: 4, attr2: 4, pos: "C" }, color, formation, draggable), players.home);
            players.add(new PlayerCard({ team: "DET", name: "Holmstrom", attr1: 3, attr2: 3, pos: "RW" }, color, formation, draggable), players.home);
            players.add(new PlayerCard({ team: "DET", name: "Lidstrom", attr1: 3, attr2: 4, pos: "LD" }, color, formation, draggable), players.home);
            players.add(new PlayerCard({ team: "DET", name: "Rafalski", attr1: 2, attr2: 4, pos: "RD" }, color, formation, draggable), players.home);

            var formation = "line2";
            players.add(new PlayerCard({ team: "DET", name: "Cleary", attr1: 4, attr2: 2, pos: "LW" }, color, formation, draggable), players.home);
            players.add(new PlayerCard({ team: "DET", name: "Filppula", attr1: 4, attr2: 2, pos: "C" }, color, formation, draggable), players.home);
            players.add(new PlayerCard({ team: "DET", name: "Bertuzzi", attr1: 4, attr2: 3, pos: "RW" }, color, formation, draggable), players.home);
            players.add(new PlayerCard({ team: "DET", name: "Kronwall", attr1: 3, attr2: 3, pos: "LD" }, color, formation, draggable), players.home);
            players.add(new PlayerCard({ team: "DET", name: "Stuart", attr1: 2, attr2: 4, pos: "RD" }, color, formation, draggable), players.home);

            var formation = "goalies";
            players.add(new PlayerCard({ team: "DET", name: "Howard", attr1: 3, attr2: 5, pos: "G" }, color, formation, draggable), players.home);
            players.add(new PlayerCard({ team: "DET", name: "Osgood", attr1: 3, attr2: 4, pos: "G" }, color, formation, draggable), players.home);
        },
        addRangersPlayers: function () {
            var color = "00c";
            var draggable = false;

            var formation = "oppLine1";
            players.add(new PlayerCard({ team: "NYR", name: "Dubinsky", attr1: 5, attr2: 2, pos: "LW" }, color, formation, draggable), players.away);
            players.add(new PlayerCard({ team: "NYR", name: "Drury", attr1: 5, attr2: 3, pos: "C" }, color, formation, draggable), players.away);
            players.add(new PlayerCard({ team: "NYR", name: "Gaborik", attr1: 6, attr2: 1, pos: "RW" }, color, formation, draggable), players.away);
            players.add(new PlayerCard({ team: "NYR", name: "Girardi", attr1: 1, attr2: 4, pos: "LD" }, color, formation, draggable), players.away);
            players.add(new PlayerCard({ team: "NYR", name: "Staal", attr1: 3, attr2: 4, pos: "RD" }, color, formation, draggable), players.away);

            var formation = "oppLine2";
            players.add(new PlayerCard({ team: "NYR", name: "Zuccarello", attr1: 4, attr2: 2, pos: "LW" }, color, formation, draggable), players.away);
            players.add(new PlayerCard({ team: "NYR", name: "Anisimov", attr1: 4, attr2: 2, pos: "C" }, color, formation, draggable), players.away);
            players.add(new PlayerCard({ team: "NYR", name: "Callahan", attr1: 4, attr2: 3, pos: "RW" }, color, formation, draggable), players.away);
            players.add(new PlayerCard({ team: "NYR", name: "McCabe", attr1: 2, attr2: 4, pos: "LD" }, color, formation, draggable), players.away);
            players.add(new PlayerCard({ team: "NYR", name: "Del Zotto", attr1: 2, attr2: 3, pos: "RD" }, color, formation, draggable), players.away);

            var formation = "oppGoalies";
            players.add(new PlayerCard({ team: "NYR", name: "Lundqvist", attr1: 4, attr2: 4, pos: "G" }, color, formation, draggable), players.away);
            players.add(new PlayerCard({ team: "NYR", name: "Biron", attr1: 2, attr2: 3, pos: "G" }, color, formation, draggable), players.away);
        },
        players: players
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

            layout.boardHeight = $(window).height();
            layout.boardWidth = $(window).width() * 0.4;
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
            document.getElementById('center').style.width = layout.boardWidth + 'px';

            canvas.height = layout.boardHeight;
            canvas.width = layout.boardWidth;

            layout.margin = layout.boardHeight / 80;
            layout.borderWidth = layout.boardHeight / 80;
            layout.left = layout.margin + layout.borderWidth + layout.borderWidth / 2;
            layout.top = layout.margin + layout.borderWidth;
            var left = layout.margin + layout.borderWidth;
            var top = layout.margin + layout.borderWidth;
            var height = layout.boardHeight - top * 2;
            var width = layout.boardWidth - left * 2;
            var lineWidth = height / 160;
            layout.innerTop = layout.top + height / 8;
            layout.innerHeight = layout.boardHeight - top * 2;
            layout.innerWidth = layout.boardWidth - left * 2;


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
            context.strokeStyle = "#33f";
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
            context.textAlign = "center";
            context.font = height / 45 + "pt Calibri bold";
            context.fillText("Play", left - left / 5, top * 4 + height / 2);
            context.font = height / 55 + "pt Calibri bold";
            context.fillText("-by-", left - left / 5, top * 4 + height / 2 + height / 45);
            context.font = height / 45 + "pt Calibri bold";
            context.fillText("Play", left - left / 5, top * 4 + height / 2 + height / 22.5);


            // set up boxes
            var boxLeft = lineLeft + 'px';
            var boxRight = lineLeft + 'px';
            var boxHeight = height * 0.1875 + 'px';
            var boxWidth = (lineRight - lineLeft) / 2 + layout.borderWidth / 10 + 'px';
            document.getElementById('gameBoardGoalkeeperOpponent').style.marginTop = top + layout.borderWidth + 'px';

            $('.gameSquareLeft').css('left', boxLeft)
				.width(boxWidth)
				.height(boxHeight);

            $('.gameSquareRight').css('right', boxRight)
				.width(boxWidth)
				.height(boxHeight);

            //          $("#gameBoardFaceOffOpponent").css("top", 40 + 'px');
            //          $("#gameBoardFaceOff").css("top", layout.innerTop + (height * 0.1875 - $("#gameBoardFaceOff").height()) / 2 + height * 0.1875 * 2));
            //			document.getElementById('gameBoardFaceOffOpponent').style.top = layout.innerTop + (height * 0.1875 - $("#gameBoardFaceOffOpponent").height()) / 2 + height * 0.1875 + 'px';
            //			document.getElementById('gameBoardFaceOff').style.top = layout.innerTop + (height * 0.1875 - $("#gameBoardFaceOff").height()) / 2 + height * 0.1875 * 2 + 'px';

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
            var pointSize = 0.14; // radius of gameSquare height
            var relationWidth = 0.3;
            var outerCircle = 1.4;

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
            var elem = $("#gameBoardTacticalCanvas");
            var canvas = elem.get(0);
            var context = canvas.getContext("2d");

            context.clearRect(0, 0, canvas.width, canvas.height);
            context.beginPath();
        },

        setCardSizes: function () {
            var baseWidth = 120;
            var baseHeight = 85;
            var baseFont = 36;
            var baseBorder = 2.5;
            var basePadding = 8;

            var totalWidth = $("#playerBench").width();

            var width = totalWidth * baseWidth / 410;
            var height = baseHeight * width / baseWidth;
            var margin = (totalWidth - 3 * width) / 4;

            var font = baseFont * width / baseWidth;
            var border = baseBorder * width / baseWidth;
            var cardPadding = basePadding * width / baseWidth;

            $(".placeholder").css({
                'width': width + 'px',
                'height': height + 'px',
                'font-size': font + 'px',
                'line-height': height + 'px',
                'margin': (margin / 2) + 'px ' + (margin / 2) + 'px ' + (margin / 2) + 'px ' + (margin / 2) + 'px',
                'border': border + 'px solid #666',
                'border-radius': (10 * width / baseWidth) + 'px'
            });
            $(".card").css({
                'width': (width + 2 * border) + 'px',
                'height': (height + 2 * border) + 'px',
                'top': -border + 'px',
                'left': -border + 'px'
            });
            $(".teamLogo").css({
                'width': (width + 2 * border - 2 * cardPadding) + 'px',
                'height': (height + 2 * border - 2 * cardPadding) + 'px',
                'border': border + 'px solid #fff',
                'border-radius': (6 * width / baseWidth) + 'px',
                'margin': cardPadding + 'px auto'
            });

            var baseFont = 14;

            $(".playerName").css({
                'left': (border) + 'px',
                'top': (border) + 'px',
                'font-size': (baseFont * width / baseWidth) + 'px',
                'line-height': (baseFont * width / baseWidth) + 'px'
            });

            var baseFont = 18;

            $(".attr1").css({
                'right': (cardPadding + border) + 'px',
                'bottom': (cardPadding + border + 2 * ((baseFont + 2) * width / baseWidth)) + 'px',
                'font-size': (baseFont * width / baseWidth) + 'px',
                'line-height': (baseFont * width / baseWidth) + 'px'
            });
            $(".attr2").css({
                'right': (cardPadding + border) + 'px',
                'bottom': (cardPadding + border + ((baseFont + 2) * width / baseWidth)) + 'px',
                'font-size': (baseFont * width / baseWidth) + 'px',
                'line-height': (baseFont * width / baseWidth) + 'px'
            });

            var baseFont = 16;

            $(".playerPos").css({
                'right': (cardPadding + border) + 'px',
                'bottom': (cardPadding + border) + 'px',
                'font-size': (baseFont * width / baseWidth) + 'px',
                'line-height': (baseFont * width / baseWidth) + 'px'
            });

            // For cards on the game board... ----------------------------------------------------
            baseWidth = 100;
            baseHeight = 70;
            baseFont = 15;
            baseBorder = 2;
            basePadding = 6;

            totalWidth = $("#gameBoardLW").width();

            width = totalWidth * baseWidth / 220;
            height = baseHeight * width / baseWidth;
            margin = (totalWidth - 3 * width) / 4;

            font = baseFont * width / baseWidth;
            border = baseBorder * width / baseWidth;
            cardPadding = basePadding * width / baseWidth;

            $(".onBoard").css({
                'width': (width + 2 * border) + 'px',
                'height': (height + 2 * border) + 'px'
            });
            $(".onBoard .teamLogo").css({
                'width': (width + 2 * border - 2 * cardPadding) + 'px',
                'height': (height + 2 * border - 2 * cardPadding) + 'px',
                'border': border + 'px solid #fff',
                'border-radius': (6 * width / baseWidth) + 'px',
                'margin': cardPadding + 'px auto'
            });

            var baseFont = 14;

            $(".onBoard .playerName").css({
                'left': (border) + 'px',
                'top': (border) + 'px',
                'font-size': (baseFont * width / baseWidth) + 'px',
                'line-height': (baseFont * width / baseWidth) + 'px'
            });

            var baseFont = 18;

            $(".onBoard .attr1").css({
                'right': (cardPadding + border) + 'px',
                'bottom': (cardPadding + border + 2 * ((baseFont + 2) * width / baseWidth)) + 'px',
                'font-size': (baseFont * width / baseWidth) + 'px',
                'line-height': (baseFont * width / baseWidth) + 'px'
            });
            $(".onBoard .attr2").css({
                'right': (cardPadding + border) + 'px',
                'bottom': (cardPadding + border + ((baseFont + 2) * width / baseWidth)) + 'px',
                'font-size': (baseFont * width / baseWidth) + 'px',
                'line-height': (baseFont * width / baseWidth) + 'px'
            });

            var baseFont = 16;

            $(".onBoard .playerPos").css({
                'right': (cardPadding + border) + 'px',
                'bottom': (cardPadding + border) + 'px',
                'font-size': (baseFont * width / baseWidth) + 'px',
                'line-height': (baseFont * width / baseWidth) + 'px'
            });
        }
    };

    $(window).bind('resize', function () {
        layout.init();
        layout.drawGameboard();
        layout.setCardSizes();
    });


    // On ready
    $(function () {
        layout.init();
        layout.drawGameboard();
        $('#console').tabs();
        $('#oppBench').tabs();
        $('#playerBench').tabs();

        play.addDetroitPlayers();
        play.addRangersPlayers();
        play.addTacticCards();
        $("#tacticCards").hover(function () {
            // mouse over
            $('.tacticCard').each(function () {
                $(this).css({ opacity: 0.5 });
                // reset opacity to the player cards
                $("#gameBoardBackgroundLayer").css({ opacity: 0.3 });
            });
        },
		function () {
		    // mouse out
		    $('.tacticCard').each(function () {
		        $(this).css({ opacity: 1.0 });
		        // reset opacity to the player cards
		        $("#gameBoardBackgroundLayer").css({ opacity: 1.0 });
		    });
		});

        function draggableStartStop(event, ui) {
            /*var card = ui.draggable.getPlayerCard();
            var pos = card.getPos();*/
            var pos = $(this).find(".playerPos").text();
            if (pos == "G") {
                $("#gameBoardGoalkeeper").each(function () {
                    $(this).toggleClass("gameSquareActiveStrong");
                });
            } else {
                $(".gameSquare" + pos).each(function () {
                    $(this).toggleClass("gameSquareActiveStrong");
                });
            }
        }

        $(".draggable").draggable({
            revert: "invalid",
            stack: ".draggable",
            start: draggableStartStop,
            stop: draggableStartStop
        });

        function droppableOverOut(change, color) {
            return function (event, ui) {
                // Get hold of the card div
                var card = ui.draggable;
                // Check player position
                var pos = card.find(".playerPos").text();
                // Add strong hovering if hovering over special square
                // and potentially add bonus point
                if ((pos == "LW" || pos == "RW") && $(this).hasClass("gameSquare" + pos)) {
                    // Get offense value
                    var off = card.find(".attr1");
                    // Add bonus point
                    off.text(parseInt(off.text()) + change);
                    off.css("color", color);
                    $(this).toggleClass("gameSquareHoverStrong");
                }
                else if ((pos == "LD" || pos == "RD") && $(this).hasClass("gameSquare" + pos)) {
                    // Get defense value
                    var def = card.find(".attr2");
                    // Add bonus point
                    def.text(parseInt(def.text()) + change);
                    def.css("color", color);
                    $(this).toggleClass("gameSquareHoverStrong");
                }
            };
        }

        $(".gameSquare").droppable({
            accept: ".skater",
            activeClass: "gameSquareActive",
            hoverClass: "gameSquareHover",
            drop: function (event, ui) {
                // Check how many cards that are already in the square
                var children = $(this).children().length;
                // Resize and move card
                //ui.draggable.effect("scale", { percent: 80, scale: 'content' }, 1000);
                ui.draggable.addClass("onBoard");
                layout.setCardSizes();
                ui.draggable.appendTo($(this));
                // Find out offset depending on cards already put in the square
                var offset = 2 + ui.draggable.width() * 0.2 * children;
                // Place the card correctly
                //ui.draggable.css('right', offset + 'px');
                //ui.draggable.css('bottom', '2px');
                /*ui.draggable.position({
                of: $(this),
                my: 'right bottom',
                at: 'right bottom',
                offset: '-' + offset + 'px -2px'
                });*/
                ui.draggable.draggable("disable");
                ui.draggable.css({ opacity: 1 });
                // Remove strong hover and active if in place
                $(this).removeClass("gameSquareHoverStrong");
            },
            over: droppableOverOut(1, '#0c0'),
            out: droppableOverOut(-1, '#fff')
        });
        $("#gameBoardGoalkeeper").droppable({
            accept: ".goalie",
            activeClass: "gameSquareActive",
            hoverClass: "gameSquareHover",
            drop: function (event, ui) {
                // Remove existing card
                if ($(this).has(".card")) {
                    // Get replaced goalie card
                    var replacedGoalie = $(this).find(".card");
                    // Move it back to bench
                    replacedGoalie.appendTo($("#goalies").find(".placeholder")[1]);
                    // Remove styling
                    replacedGoalie.removeClass("onBoard");
                    layout.setCardSizes();
                    // Reconstruct draggable
                    replacedGoalie.draggable("enable");
                }
                // Resize and move card
                ui.draggable.addClass("onBoard");
                layout.setCardSizes();
                ui.draggable.appendTo($(this));
                // Place the card correctly
                ui.draggable.position({
                    of: $(this),
                    my: 'center center',
                    at: 'center center'
                });
                ui.draggable.draggable("disable");
                ui.draggable.css({ opacity: 1 });
                // Remove strong hover
                $(this).removeClass("gameSquareHoverStrong");
            },
            over: function (event, ui) {
                $(this).addClass("gameSquareHoverStrong");
            },
            out: function (event, ui) {
                $(this).removeClass("gameSquareHoverStrong");
            }
        });
        layout.setCardSizes();
        $("#gameBoardFaceOff").droppable({
            accept: function (draggable) {
                return draggable.find(".playerPos").text() == "C";
            },
            drop: function (event, ui) {
                // Resize and move card
                ui.draggable.addClass("onBoard");
                layout.setCardSizes();
                ui.draggable.appendTo($(this));
                // Place the card correctly
                ui.draggable.position({
                    of: $(this),
                    my: 'center center',
                    at: 'center center'
                });
                ui.draggable.draggable("destroy");
            }
        });

        PlayByPlay.lobby = new Lobby();

        PlayByPlay.lobby.initialize();
    });


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
    });
    $('#playerNameInput').live('keypress', function (e) {
        var key = (e.keyCode || e.which);
        if (key == 13) {
            $('#add-user').click();
        }
    });

    return play;
})(jQuery);

CanvasRenderingContext2D.prototype.dashedLine = function (x1, y1, x2, y2, dashLen) {
    // code that extends the canvas drawing with a dashed line functionality
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