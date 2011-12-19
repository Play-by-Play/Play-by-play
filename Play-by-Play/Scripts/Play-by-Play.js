/// <reference path="underscore.js" />
/// <reference path="jquery-1.7-vsdoc.js" />
/// <reference path="jquery-ui-1.8.16.js" />
/// <reference path="~/Scripts/GameConnection.js" />


window.PlayByPlay = window.PlayByPlay || (function ($, _) {
	// Enable debug mode
	var debug = false;

	// Delay times
	var delay = 3000;

	//#region PlayersAndBonus
	var iceColor = "#FFF";
	var borderColor = "#000";
	var redLineColor = "#F00";
	var blueLineColor = "#00F";

	var players = {
		user: {
			line1: {},
			line2: {},
			goalies: []
		},
		opponent: {
			oppLine1: {},
			oppLine2: {},
			oppGoalies: []
		},

		add: function (card, team) {
			if (card.pos !== "G")
				team[card.getLine()][card.getPos()] = card;
			else
				team[card.getLine()].push(card);
		},

		find: function (id) {
			var player;
			$.each(players.opponent, function () {
				$.each(this, function () {
					if (this.id === +id) {
						player = this;
					}
				});
			});
			$.each(players.user, function () {
				$.each(this, function () {
					if (this.id === +id) {
						player = this;
					}
				});
			});
			return player;
		}
	};

	var Bonus = {
		NONE: 0,
		OFF: 1,
		DEF: 2
	};

	//#endregion

	//#region PlayerCard

	function PlayerCard(info, color, formation, userControlled, id) {
		this.color = color;
		this.team = info.team;
		this.name = info.Name;
		this.attr1 = info.Offense;
		this.attr2 = info.Defense;
		this.pos = info.Position;
		this.formation = formation;
		this.userControlled = userControlled;
		this.id = id;
		// assemble data
		var data = { color: this.color, team: this.team, name: this.name, attr1: this.attr1, attr2: this.attr2, pos: this.pos, draggable: (this.userControlled ? " draggable" : ""), id: "card" + this.id };
		// construct card
		$('#playerCardTemplate').tmpl(data).appendTo('#' + this.formation);

		this.bonus = Bonus.NONE;
		this.bonusAmount = 0;
		this.location = $("#" + this.formation);
	}

	PlayerCard.prototype = {
		getAttr1: function () {
			return this.attr1;
		},
		getAttr2: function () {
			return this.attr2;
		},
		getBonus: function () {
			return this.bonus;
		},
		getPos: function () {
			return this.pos;
		},
		getLine: function () {
			return this.formation;
		},
		getId: function () {
			return this.id;
		},
		isUserControlled: function () {
			return this.userControlled;
		},
		setBonus: function (bonus) {
			var off, def;
			// Find card div
			var cardDiv = $("#card" + this.id);
			switch (bonus) {
				case Bonus.OFF:
					this.bonusAmount++;
					// Get offense value
					off = cardDiv.find(".attr1");
					// Add bonus point
					off.text(this.attr1 + 1);
					off.css("color", "#0c0");
					break;
				case Bonus.DEF:
					this.bonusAmount++;
					// Get defense value
					def = cardDiv.find(".attr2");
					// Add bonus point
					def.text(this.attr2 + 1);
					def.css("color", "#0c0");
					break;
				default:
					this.bonusAmount--;
					if (this.bonusAmount == 0) {
						// Get offense value
						off = cardDiv.find(".attr1");
						// Remove bonus point
						off.text(this.attr1);
						off.css("color", "#fff");
						// Get defense value
						def = cardDiv.find(".attr2");
						// Remove bonus point
						def.text(this.attr2);
						def.css("color", "#fff");
					}
			}
			this.bonus = bonus;
		},
		setLocation: function (newLocation) {
			var align,
			    offset;

			var removePlayerFromBoard = function (card) {
				cardDiv.removeClass("onBoard");
				cardDiv.addClass("benched");
				layout.setCardSizes();
				cardDiv.appendTo(newLocation);
			};
			// Find card div
			var cardDiv = $("#card" + this.id);
			// Resize and move card
			var parent = newLocation.parent().first();
			var attr = parent.attr('id');
			if (attr == "gameBoardBackgroundLayer") { // to gameboard
				if (newLocation.hasClass(".gameSquare").length === 0) {
					//Goalie
					var otherCard = newLocation.find('.card');
					if (otherCard.length > 0)
						replaceGoalie(cardDiv, otherCard, "#oppGoalies");
				}
				cardDiv.removeClass("benched");
				cardDiv.addClass("onBoard");
				layout.setCardSizes();
				cardDiv.appendTo(newLocation);
				// Place the card correctly
				if (this.getPos() == "G" || newLocation.attr('id') == "gameBoardFaceOffOpponent" || newLocation.attr('id') == "gameBoardFaceOff") {
					align = "center center";
					offset = 0;
				} else if (!this.isUserControlled()) {
					// Opponent cards
					align = "left top";
					// Move each card currently in the square
					var cards = newLocation.children().length - newLocation.find(".draggable").length - 1;
					newLocation.children().each(function () {
						if (!$(this).hasClass("draggable")) {
							var i = 2 + cardDiv.width() * 0.2 * cards;
							offset = i + "px 2px";
							$(this).position({
								of: newLocation,
								my: align,
								at: align,
								offset: offset
							});
							cards--;
						}
					});
					offset = '2px 2px';
				} else {
					// User cards
					align = "right bottom";
					// Find out offset depending on cards on the same team already put in the square
					var i = -1 * (2 + cardDiv.width() * 0.2 * (newLocation.find(".draggable").length - 1));
					offset = i + 'px -2px';
				}
				cardDiv.position({
					of: newLocation,
					my: align,
					at: align,
					offset: offset
				});
			} else {
				removePlayerFromBoard(cardDiv);
			}
			// Potentially add bonus
			if (!this.userControlled) {
				// Bonus areas are on opposite sides of the board
				if (this.pos == "LW" && newLocation.hasClass("gameSquareRD") || this.pos == "RW" && newLocation.hasClass("gameSquareLD"))
					this.setBonus(Bonus.OFF);
				else if (this.pos == "LD" && newLocation.hasClass("gameSquareRW") || this.pos == "RD" && newLocation.hasClass("gameSquareLW"))
					this.setBonus(Bonus.DEF);
				else
					this.setBonus(Bonus.NONE);
			} else {
				if ((this.pos == "LW" || this.pos == "RW") && newLocation.hasClass("gameSquare" + this.pos))
					this.setBonus(Bonus.OFF);
				else if ((this.pos == "LD" || this.pos == "RD") && newLocation.hasClass("gameSquare" + this.pos))
					this.setBonus(Bonus.DEF);
				else
					this.setBonus(Bonus.NONE);
			}
			this.location = newLocation;
		}
	};

	//#endregion

	//#region Puck
	var puck = (function () {
		var getPixelPosition = function (x, y) {
			var puckRadius = $('#gameBoardPuck').height() / 2;

			var canvas = document.getElementById("gameBoardCanvas");
			var width = canvas.width;
			var height = canvas.height;

			var left = width * 0.26 - puckRadius;
			var top = height * 0.2209 - puckRadius;
			var gameSquareWidth = width * 0.48;
			var gameSquareHeight = height * 0.186;

			//			var gameSquareWidth = $('#gameBoardLW').width();
			//			var gameSquareHeight = $('#gameBoardLW').height();
			//			var left = $('#gameBoardLW').position().left + gameSquareWidth / 2 - puckRadius;
			//			var top = $('#gameBoardLW').position().top + gameSquareHeight / 2 - puckRadius;


			if (y < 0)
			// shot at opponent goal
				return {
					top: top - gameSquareHeight / 2,
					left: left + gameSquareWidth / 2
				};
			else if (y > 3)
			// shot at players goal
				return {
					top: top + (gameSquareHeight * 4) - gameSquareHeight / 2,
					left: left + (gameSquareWidth) - gameSquareWidth / 2
				};
			return {
				// pass of puck
				top: top + gameSquareHeight * y,
				left: left + gameSquareWidth * x
			};
		};

		//		var getSquare = function (x, y) {
		//			var square = "#gameBoard";
		//			if (x == 0) {
		//				square += "L";
		//			}
		//			else {
		//				square += "R";
		//			}
		//			switch (y) {
		//				case 0:
		//					square += "W";
		//					break;
		//				case 1:
		//					square += "CW";
		//					break;
		//				case 2:
		//					square += "CD";
		//					break;
		//				case 3:
		//					square += "D";
		//					break;
		//			}

		//			console.log($(square));

		//			return $(square);
		//		};

		return {
			placeAt: function (x, y) {
				var position = getPixelPosition(x, y);
				$('#gameBoardPuck').css({
					'visibility': 'visible',
					'top': position.top,
					'left': position.left
				});

				//				$('#gameBoardPuck').animate({}, {
				//					step: function () {
				//						$(this).position({
				//							of: square,
				//							my: 'center center',
				//							at: 'center center'
				//						});
				//					}
				//				});
			},
			moveTo: function (x, y) {
				if ($('#gameBoardPuck').css('visibility') == 'visible') {
					var position = getPixelPosition(x, y);
					$('#gameBoardPuck').animate({
						top: position.top,
						left: position.left
					}, delay);
				}
				else
					setPosition(x, y);
			},
			shoot: function (direction) {
				// direction = opponent/player
				var position;
				if (direction == "opponent") {
					position = getPixelPosition(0, -1);
				}
				else if (direction == "player") {
					position = getPixelPosition(0, 4);
				}
				$('#gameBoardPuck').animate({
					top: position.top,
					left: position.left
				}, delay);
			}
		};
	})();
	//#endregion

	//#region Game
	var replaceGoalie = function (cardDiv, replacedGoalie, bench) {
		// Check which placeholder to return to
		var curPlaceHolder = cardDiv.parent()[0];
		var placeHolder = $(bench).find(".placeholder")[0];
		if (curPlaceHolder === placeHolder)
			placeHolder = $(bench).find(".placeholder")[1];
		// Move it back to bench
		replacedGoalie.appendTo(placeHolder);
		// Remove styling
		replacedGoalie.removeClass("onBoard");
		replacedGoalie.addClass("benched");
		layout.setCardSizes();
		// Reconstruct draggable
		replacedGoalie.draggable("enable");
	};
	var convertTacticCard = function (card) {
		var start = [card.StartNode.X, card.StartNode.Y];
		var nodes = _.map(card.Nodes, function (node) {
			return [node.X, node.Y];
		});
		var movementNodes = _.map(card.MovementNodes, function (node) {
			return [node.X, node.Y];
		});
		var passes = _.map(card.Passes, function (pass) {
			return [[pass.Start.X, pass.Start.Y], [pass.End.X, pass.End.Y]];
		});
		var movingPass = _.map(card.Movements, function (movement) {
			return [[movement.Start.X, movement.Start.Y], [movement.End.X, movement.End.Y]];
		});
		var shot = [card.Shot.X, card.Shot.Y];

		return {
			Id: card.Id,
			Name: card.Name,
			Difficulty: card.Difficulty,
			startNode: start,
			nodes: nodes,
			movementNode: movementNodes,
			passes: passes,
			movingPass: movingPass,
			shot: shot
		};
	};
	var play = {
		addTacticCards: function (cards) {
			_.each(cards, function (card) {
				card = convertTacticCard(card);
				play.addTacticCard(card.Id, card.Name, card.Difficulty, { startNode: card.startNode, nodes: card.nodes, movementNode: card.movementNodes, passes: card.passes, movingPass: card.movingPass, shot: card.shot });
			});

			$("#tacticCards").hover(
				function () {
					// mouse over
					if (!layout.tacticCardsEnabled) {
						return;
					}

					$('.tacticCard').each(function () {
						$(this).css({ opacity: 0.5 });
					});
					// reset opacity to the player cards
					$("#gameBoardBackgroundLayer").css({ opacity: 0.3 });
				},
				function () {
					// mouse out
					if (!layout.tacticCardsEnabled) {
						return;
					}

					$('.tacticCard').each(function () {
						$(this).css({ opacity: 1.0 });
					});
					// reset opacity to the player cards
					$("#gameBoardBackgroundLayer").css({ opacity: 1.0 });
				}
			);
		},
		addTacticCard: function (id, name, diff, tactic) {
			var data = { name: name, diff: diff };
			var template = $('#tacticCardTemplate').tmpl(data).css('background-color', '#' + data.color).appendTo('#tacticCards');
			template.data('cardId', id);
			// set templates height depending on templates width, which is set in card.css
			template.height(template.width() / 5 * 8);

			jQuery.data(template, "tactic", tactic);

			// draw card and tactic
			var canvas = template.find('canvas')[0];
			canvas.height = template.height();
			canvas.width = template.width();
			layout.drawGameboard(canvas);
			layout.drawTactic(canvas, tactic);

			// save tactic in relation with canva			

			template.hover(
			// mouse over
				function () {
					// clear the canvas before proceding
					if (!layout.tacticCardsEnabled) {
						return;
					}

					layout.clearGameboardTactic();
					layout.drawTactic(document.getElementById("gameBoardTacticalCanvas"), tactic);
					$('.tacticCard').each(function () {
						$(this).css({ opacity: 0.5 });
					});
					$(this).css({ opacity: 1.0 });
				},
			// mouse out
				function () {
					if (!layout.tacticCardsEnabled) {
						return;
					}

					layout.clearGameboardTactic();
					$(this).css({ opacity: 0.5 });
				}
			);

			template.click(function () {
				if (!layout.tacticCardsEnabled) {
					return;
				}

				play.disableTacticCards();
				$(this).remove();

				layout.drawPlayerPlacedTactic(tactic, template);
				window.connection.playTactic(id);
			});

			// set card to disable as default
			//template.disable(true);
		},
		placePlayerCard: function (id, square) {
			// Get the card object
			var playerCard = players.find(id);
			// Set the new card location
			playerCard.setLocation($('#' + square));
		},
		showBattleView: function (result) {
			if (debug) {
				title = "Debug-battle";
				result = {
					IsHomePlayer: true,
					HomePlayers: [
						{ Name: "Datsyuk", Position: "C", Offense: 4, Defense: 4, Bonus: Bonus.NONE }
					],
					AwayPlayers: [
						{ Name: "Drury", Position: "C", Offense: 5, Defense: 3, Bonus: Bonus.NONE }
					],
					HomeTotal: 8,
					AwayTotal: 0
				};
			}
			// Function for adding a row with player values
			var addTableRow = function (table, player, isOffense) {
				var tr = $("<tr>");
				table.append(tr);

				var td = $("<td>");
				td.text(player.Name + " (" + player.Position + ")");
				td.addClass("player");
				tr.append(td);

				td = $("<td>");
				var attr = 0;
				if (isOffense) {
					attr = player.Offense;
					if (player.Bonus == Bonus.OFF) {
						attr++;
						td.css({ 'color': '#0c0' });
					}
				} else {
					attr = player.Defense;
					if (player.Bonus == Bonus.DEF) {
						attr++;
						td.css({ 'color': '#0c0' });
					}
				}
				total += attr;
				td.text(attr);
				td.addClass("attr");
				tr.append(td);
			};
			// Function for adding a row with total values
			var addTotal = function (table, amount, total) {
				var tr = $("<tr>");
				tr.css({ "border-top": "2px solid #fff" });
				table.append(tr);

				var td = $("<td>");
				td.text(amount + " player" + (amount > 1 ? "s" : ""));
				td.addClass("player");
				tr.append(td);

				td = $("<td>");
				td.text(total);
				td.addClass("bigAttr");
				tr.append(td);
			};

			// Get battle view divs
			var viewDiv = $("#battle-view");
			var userDiv = $("#userBattle");
			var oppDiv = $("#oppBattle");
			var resultDiv = $("#battleResult");
			var animDiv = $("#battleAnim");
			var puck = $("#battlePuck");

			userDiv.empty();
			userDiv.append($("<h1>").text("You"));
			oppDiv.empty();
			oppDiv.append($("<h1>").text("Opponent"));

			// Determine if current user is home or away team
			if (result.IsHomePlayer) {
				var homeDiv = userDiv;
				var awayDiv = oppDiv;
			} else {
				var homeDiv = oppDiv;
				var awayDiv = userDiv;
			}
			// Construct home team table
			var table = $("<table>");
			var total = 0;
			$.each(result.HomePlayers, function () {
				addTableRow(table, this, result.IsHomeAttacking);
			});
			for (var i = result.HomePlayers.length; i < 5; i++) {
				tr = $("<tr>");
				table.append(tr);
			}
			addTotal(table, result.HomePlayers.length, total);
			homeDiv.append(table);
			// Construct away team table
			table = $("<table>");
			total = 0;
			$.each(result.AwayPlayers, function () {
				addTableRow(table, this, !result.IsHomeAttacking);
			});
			for (var i = result.AwayPlayers.length; i < 5; i++) {
				tr = $("<tr>");
				table.append(tr);
			}
			addTotal(table, result.AwayPlayers.length, total);
			awayDiv.append(table);

			// Add result text
			var span = $("<span>");
			// Check if current user won the battle
			if (result.IsHomePlayer && result.HomeTotal > result.AwayTotal || !result.IsHomePlayer && result.HomeTotal < result.AwayTotal) {
				span.text("You won!");
			} else {
				span.text("Your opponent won...");
			}
			span.css({ visibility: "hidden" });
			resultDiv.find("span").remove();
			span.prependTo(resultDiv);

			// Set size on dialog
			var baseWidth = 836;
			var baseHeight = 530;

			var totalWidth = $(document).width();

			var width = totalWidth * baseWidth / 1280;
			var height = baseHeight * width / baseWidth;

			// Set position on puck
			puck.position({
				of: $("#battleAnim"),
				my: 'center center',
				at: 'center center',
				offset: '0'
			});

			// Open battle view
			viewDiv.dialog({
				title: result.Title,
				modal: true,
				draggable: false,
				resizable: false,
				width: width,
				height: height,
				open: function () {
					$(this).parent().find(".ui-dialog-titlebar-close").hide();
				}
			});

			// Animate battle
			if (result.IsHomePlayer && result.HomeTotal > result.AwayTotal || !result.IsHomePlayer && result.HomeTotal < result.AwayTotal) {
				var anim = '-';
			} else {
				var anim = '+';
			}
			$("#battlePuck").animate({
				left: anim + (width / 2 - 0.1 * width)
			}, delay);
			// Show results
			setTimeout(function () {
				span.css({ visibility: "visible" });
			}, delay);
			// Close battle view
			setTimeout(function () {
				viewDiv.dialog('close');
			}, delay * 2);
		},
		showFaceoff: function () {
			// Show faceoff squares
			$(".gameSquareFaceOff").show();
			// Disable other squares
			$(".gameSquare").droppable({ disabled: true });
		},
		hideFaceoff: function () {
			// Restore centers
			play.restorePlayers();
			// Hide faceoff squares
			$(".gameSquareFaceOff").hide();
			// Enable other squares
			$(".gameSquare").droppable({ disabled: false });
		},
		enablePlayers: function (tab) {
			$('#' + tab).find(".card").draggable("enable");
		},
		enableAllPlyers: function () {
			$('#playerBench').find('.card').draggable("enable");
		},
		disablePlayers: function (tab) {
			$('#playerBench').find('#' + tab)
											 .find(".card")
											 .draggable("disable")/*.css({ opacity: 1 })*/;
		},
		disablePlayersExceptOn: function (tab) {
			$('#playerBench').find('.tab')
											 .not('#' + tab)
											 .find(".card")
											 .draggable("disable")/*.css({ opacity: 1 })*/;
		},
		restorePlayers: function () {
			$(".onBoard").each(function () {
				var cardDiv = $(this);
				var pos = cardDiv.find(".playerPos").text();
				if (pos != "G") {
					// Find out which line player belongs to
					var id = cardDiv.attr('id').substring(4);
					var playerCard = players.find(id);

					// Check which team player belongs to
					var element = $("#" + playerCard.getLine());
					var order = _.indexOf(["LW", "C", "RW", "LD", "RD"], pos) + 1;
					var newLocation = element.children(':nth-child(' + order + ')');
					playerCard.setLocation(newLocation);
					playerCard.setBonus(Bonus.NONE);
					cardDiv.draggable("enable");
				}
			});
		},
		opponentPlaceTacticCard: function (tactic) {
			layout.drawOpponentPlacedTactic(tactic);
		},
		playTactic: function (result) {
			var cont = true;
			var userScore = false;
			var oppScore = false;
			$.each(result.Battles, function (index, battle) {
				var attackerWon = (result.IsHomeAttacking && (battle.HomeTotal > battle.AwayTotal) || !result.IsHomeAttacking && (battle.HomeTotal < battle.AwayTotal));
				setTimeout(function () {
					// Get puck to the battle
					if (battle.Type == "Shot") {
						if (result.IsHomeAttacking && battle.IsHomePlayer || !result.IsHomeAttacking && !battle.IsHomePlayer) {
							puck.shoot("opponent");
							if (attackerWon)
								userScore = true;
						} else {
							puck.shoot("player");
							if (attackerWon)
								oppScore = true;
						}
					} else {
						if (index == 0)
							puck.placeAt(battle.Area.X, battle.Area.Y);
						else
							puck.moveTo(battle.Area.X, battle.Area.Y);
					}
				}, (index * 3) * delay);
				setTimeout(function () {
					// Show battle view
					if (!(result.IsHomeAttacking && battle.AwayPlayers.length == 0 || !result.IsHomeAttacking && battle.HomePlayers.length == 0)) {
						play.showBattleView(battle);
					}
				}, (index * 3 + (index == 0 ? 0 : 1)) * delay);
				// Check if attack continues
				if (result.IsHomeAttacking && (battle.HomeTotal < battle.AwayTotal) || !result.IsHomeAttacking && (battle.HomeTotal > battle.AwayTotal)) {// attacker lost
					setTimeout(function () {
						layout.clearGameboardTactic();
					}, ((index + 1) * 3) * delay);
					cont = false;
					return cont;
				}
				// Player movement... TODO: Fix rotated view!
				setTimeout(function () {
					$.each(result.Card.Movements, function (index, movement) {
						if (index != (result.Battles.length - 1) && battle.Area.X == movement.Start.X && battle.Area.Y == movement.Start.Y) {
							var x = movement.End.X;
							var y = movement.End.Y;
							// Assemble game square
							var square = "#gameBoard";
							if (x == 0) {
								square += "L";
							}
							else {
								square += "R";
							}
							switch (y) {
								case 0:
									square += "W";
									break;
								case 1:
									square += "CW";
									break;
								case 2:
									square += "CD";
									break;
								case 3:
									square += "D";
									break;
							}
							if (result.isHomeAttacking) {
								var playerCard = players.find(battle.HomePlayers[0].Id);
								playerCard.setLocation($(square));
							} else {
								var playerCard = players.find(battle.AwayPlayers[0].Id);
								playerCard.setLocation($(square));
							}
							return false;
						}
					});
				}, ((index + 1) * 3) * delay);
			});
//			if (!cont)
//				return false;
			setTimeout(function () {
				layout.clearGameboardTactic();
				window.connection.nextTurn();
				if (oppScore)
					play.addGoal("opponent");
				else if (userScore)
					play.addGoal("player");
			}, result.Battles.length * 3 * delay);
		},
		addGoal: function (user) {
			var scoreDiv = $("#" + user + "Goals");
			scoreDiv.text(parseInt(scoreDiv.text()) + 1);
		},
		disableTacticCards: function () {
			layout.tacticCardsEnabled = false;
			$('.tacticCard').each(function () {
				$(this).css({ opacity: 0.3 });
			});
			// reset opacity to the player cards
			$("#gameBoardBackgroundLayer").css({ opacity: 1.0 });
		},
		enableTacticCards: function () {
			layout.clearGameboardTactic();
			$('.tacticCard').each(function () {
				$(this).css({ opacity: 1 });
			});
			layout.tacticCardsEnabled = true;
		},
		addUserPlayers: function (team) {
			var color = team.Color;
			var userControlled = true;

			_.each({ "line1": "Line1", "line2": "Line2", "goalies": "Goalies" }, function (serverLine, formation) {
				_.each(team[serverLine], function (player) {
					player.team = team.Name;
					players.add(new PlayerCard(player, color, formation, userControlled, player.Id), players.user);
				});
			});
		},
		addOpponentPlayers: function (team) {
			var color = team.Color;
			var userControlled = false;

			_.each({ "oppLine1": "Line1", "oppLine2": "Line2", "oppGoalies": "Goalies" }, function (serverLine, formation) {
				_.each(team[serverLine], function (player) {
					player.team = team.Name;
					players.add(new PlayerCard(player, color, formation, userControlled, player.Id), players.opponent);
				});
			});
		},
		addPlayers: function (userteam, opponentteam) {

			play.addUserPlayers(userteam);
			play.addOpponentPlayers(opponentteam);

			function draggableStartStop() {
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
				scroll: "false",
				containment: "document",
				start: draggableStartStop,
				stop: draggableStartStop
			});
			$(".gameSquare").droppable({
				accept: ".skater",
				activeClass: "gameSquareActive",
				hoverClass: "gameSquareHover",
				drop: function (event, ui) {
					// Get hold of the card div
					var cardDiv = ui.draggable;
					// Get the card object
					var id = cardDiv.attr('id').substring(4);
					var playerCard = players.find(id);
					playerCard.setLocation($($(this)));
					window.connection.placePlayer(playerCard.id, $(this).attr('id'));
					// Disable draggability
					cardDiv.draggable({ disable: true }).css({ opacity: 1 });
					// Remove strong hover and active if in place
					$(this).removeClass("gameSquareHoverStrong");
				},
				over: function (event, ui) {
					// Get hold of the card div
					var cardDiv = ui.draggable;
					// Check player position
					var pos = cardDiv.find(".playerPos").text();
					// Get the card object
					var id = cardDiv[0].id.substring(4);
					var playerCard = players.find(id);
					// Add strong hovering if hovering over special square
					// and potentially add bonus point
					if ((pos == "LW" || pos == "RW") && $(this).hasClass("gameSquare" + pos)) {
						playerCard.setBonus(Bonus.OFF);
						$(this).addClass("gameSquareHoverStrong");
					} else if ((pos == "LD" || pos == "RD") && $(this).hasClass("gameSquare" + pos)) {
						playerCard.setBonus(Bonus.DEF);
						$(this).addClass("gameSquareHoverStrong");
					}
				},
				out: function (event, ui) {
					// Get hold of the card div
					var cardDiv = ui.draggable;
					var id = cardDiv[0].id.substring(4);
					// Get the card object
					var playerCard = players.find(id);
					var pos = playerCard.getPos();
					// Remove strong hovering and bonus point
					if ((pos == "LW" || pos == "RW") && $(this).hasClass("gameSquare" + pos) || (pos == "LD" || pos == "RD") && $(this).hasClass("gameSquare" + pos)) {
						playerCard.setBonus(Bonus.NONE);
						$(this).removeClass("gameSquareHoverStrong");
					}
				}
			});
			$("#gameBoardGoalkeeper").droppable({
				revert: 'invalid',
				accept: ".goalie",
				activeClass: "gameSquareActive",
				hoverClass: "gameSquareHover",
				drop: function (event, ui) {
					// Get hold of the card div
					var cardDiv = ui.draggable;
					// Get the card object
					var id = cardDiv[0].id.substring(4);
					var playerCard = players.find(id);
					var $this = $(this);
					window.connection.placeGoalkeeper(id).done(function (result) {
						// Remove existing card
						if ($this.has(".card")) {
							// Get replaced goalie card
							var replacedGoalie = $this.find(".card");
							replaceGoalie(cardDiv, replacedGoalie, "#goalies");
						}
						playerCard.setLocation($($this));
						cardDiv.draggable("disable");
						cardDiv.css({ opacity: 1 });
						// Remove strong hover
						$this.removeClass("gameSquareHoverStrong");
					}).fail(function (error) {
						console.warn(error);
						layout.setCardSizes();
					});
				},
				over: function () {
					$(this).addClass("gameSquareHoverStrong");
				},
				out: function () {
					$(this).removeClass("gameSquareHoverStrong");
				}
			});
			layout.setCardSizes();
			$("#gameBoardFaceOff").droppable({
				accept: function (draggable) {
					return draggable.find(".playerPos").text() == "C";
				},
				drop: function (event, ui) {
					// Get hold of the card div
					var cardDiv = ui.draggable;
					// Get the card object
					var id = cardDiv[0].id.substring(4);
					var $this = $(this);
					window.connection.placeFaceOffPlayer(id).done(function (result) {

					}).fail(function (error) {
						console.warn(error);
					});
					// Resize and move card
					cardDiv.removeClass("benched");
					cardDiv.addClass("onBoard");
					layout.setCardSizes();
					cardDiv.appendTo($this);
					// Place the card correctly
					cardDiv.position({
						of: $this,
						my: 'center center',
						at: 'center center'
					});
				},
				stop: function (event, ui) {
					ui.draggable.draggable("destroy");
				}
			});
		},
		players: players,
		Bonus: Bonus,
		debug: debug,
		puck: puck,
		setTacticsEnabled: function (isTurn) {
			if (isTurn === true) {
				play.enableTacticCards();
			} else {
				play.disableTacticCards();
			}
		}
	};
	//#endregion

	//#region Layout
	var layout = {
		init: function () {
			play.disableTacticCards();
			$('#right').width(innerWidth - ($('#left').width() + $('#center').width()) - $.scrollbarWidth());
			$('.panel').setFullWidth();
		},

		drawMainGameboard: function () {
			// draw main gameboard

			var margin = $(window).height() / 70;
			$('#gameboard').css({ 'margin': margin + 'px' });

			height = $(window).height() - margin * 2;
			width = $(window).width() * 0.4 - margin * 2;
			$('#center').width(width);

			// correct proportions if neccessary
			var heightProportions = height / 8;
			var widthProportions = width / 5;

			if (heightProportions > widthProportions) {
				// set height depending on width
				height = height * (widthProportions / heightProportions);
			}
			else if (heightProportions < widthProportions) {
				// set width depending on height
				width = width * (heightProportions / widthProportions);
			}

			$('#gameBoardBackgroundLayer').height(height * 0.9739);
			$('#gameBoardBackgroundLayer').width(width * 0.962);

			// seting up canvas
			var canvas = document.getElementById("gameBoardCanvas");
			canvas.height = height;
			canvas.width = width;

			var tacticCanvas = document.getElementById("gameBoardTacticalCanvas");
			tacticCanvas.height = height;
			tacticCanvas.width = width;

			// draw game board on canvas
			layout.drawGameboard(canvas);
		},

		drawGameboard: function (canvas) {
			// canvas to draw game board in
			var context = canvas.getContext("2d");

			// get dimension of canvas
			var height = canvas.height;
			var width = canvas.width;

			// calculate line width
			var outerLineWidth = height / 80;
			var lineWidthBold = height / 120;
			var lineWidthNormal = height / 320;
			var lineWidthThin = height / 640;

			var lineLeft = outerLineWidth;
			var lineRight = width - outerLineWidth;

			// add margin to include complete outer border inside the canvas
			var left = outerLineWidth / 2;
			var top = outerLineWidth / 2;
			height -= outerLineWidth;
			width -= outerLineWidth;

			context.clearRect(0, 0, width, height);

			// Set margin for the container of game squares
			//			var squares = $('#gameBoardBackgroundLayer');
			//			squares.css('margin', Math.ceil(outerLineWidth) + 'px');

			// draw ice rink
			context.beginPath();
			context.arc(left + width / 5, top + height / 8, width / 5, -Math.PI / 2, Math.PI, true);
			context.arc(left + width / 5, top + height / 8 * 7, width / 5, Math.PI, Math.PI / 2, true);
			context.arc(left + width / 5 * 4, top + height / 8 * 7, width / 5, Math.PI / 2, 0, true);
			context.arc(left + width / 5 * 4, top + height / 8, width / 5, 0, -Math.PI / 2, true);
			context.closePath();
			context.fillStyle = iceColor;
			context.fill();
			context.lineWidth = outerLineWidth;
			context.strokeStyle = borderColor;
			context.stroke();

			// draw lines

			// red main middle line
			context.beginPath();
			context.strokeStyle = redLineColor;
			context.moveTo(lineLeft, top + height / 2);
			context.lineTo(lineRight, top + height / 2);
			context.lineWidth = lineWidthBold;
			context.stroke();

			// red line between the goals
			context.beginPath();
			context.moveTo(left + width / 2, top + height / 8);
			context.lineTo(left + width / 2, top + height / 8 * 7);
			context.lineWidth = lineWidthThin;
			context.stroke();

			// draw goal areas
			context.beginPath();
			context.moveTo(left + width / 2 + Math.cos(-Math.PI / 4) * height / 15, top + height / 8 * 7 - lineWidthThin);
			context.arc(left + width / 2, top + height / 8 * 7 - lineWidthThin, height / 15, -Math.PI / 4, Math.PI + Math.PI / 4, true);
			context.lineTo(left + width / 2 + Math.cos(Math.PI + Math.PI / 4) * height / 15, top + height / 8 * 7 - lineWidthThin);
			context.closePath();
			context.fillStyle = "#6cf";
			context.fill();
			context.stroke();

			context.beginPath();
			context.moveTo(left + width / 2 + Math.cos(Math.PI + Math.PI / 4) * height / 15, top + height / 8);
			context.arc(left + width / 2, top + height / 8 - lineWidthThin, height / 15, Math.PI - Math.PI / 4, Math.PI / 4, true);
			context.lineTo(left + width / 2 + Math.cos(-Math.PI / 4) * height / 15, top + height / 8);
			context.closePath();
			context.fillStyle = "#6cf";
			context.fill();
			context.stroke();

			// red goal lines
			context.beginPath();
			context.moveTo(lineLeft, top + height / 8);
			context.lineTo(lineRight, top + height / 8);
			context.moveTo(lineLeft, top + height / 8 * 7 - lineWidthThin);
			context.lineTo(lineRight, top + height / 8 * 7 - lineWidthThin);
			context.lineWidth = lineWidthNormal;
			context.stroke();

			// blue lines
			context.beginPath();
			context.strokeStyle = blueLineColor;
			context.moveTo(lineLeft, top + height * 0.3125);
			context.lineTo(lineRight, top + height * 0.3125);
			context.moveTo(lineLeft, top + height * 0.6875);
			context.lineTo(lineRight, top + height * 0.6875);
			context.lineWidth = lineWidthBold;
			context.stroke();

			// draw middle circle
			context.beginPath();
			context.arc(left + width / 2, top + height / 2, width / 10, 0, Math.PI * 2, true);
			context.lineWidth = lineWidthNormal;
			context.strokeStyle = "#33f";
			context.fillStyle = "#fff";
			context.fill();
			context.stroke();

			// draw inner arcs
			// blue arc
			context.beginPath();
			context.arc(left + width / 2, top + height / 2, width / 12, 0, -Math.PI / 2, true);
			context.lineWidth = lineWidthBold;
			context.strokeStyle = "#00f";
			context.stroke();

			// red arc
			context.beginPath();
			context.arc(left + width / 2, top + height / 2, width / 12, Math.PI, Math.PI / 2, true);
			context.lineWidth = lineWidthBold;
			context.strokeStyle = "#f00";
			context.stroke();
			context.beginPath();

			// draw Play-by-Play text
			var a = (left + width / 2);
			var b = (top + height / 2);
			context.fillStyle = "#000";
			context.rotate(-30 * Math.PI / 180);
			context.textAlign = "center";
			context.font = height / 45 + "pt Calibri bold";
			context.fillText("Play",
				(a - height / 128) * Math.cos(Math.PI / 6) - (b - height / 70) * Math.sin(Math.PI / 6),
				(a - height / 128) * Math.sin(Math.PI / 6) + (b - height / 70) * Math.cos(Math.PI / 6));
			context.font = height / 55 + "pt Calibri bold";
			context.fillText("-by-",
				(a + height / 256) * Math.cos(Math.PI / 6) - (b + height / 256) * Math.sin(Math.PI / 6),
				(a + height / 256) * Math.sin(Math.PI / 6) + (b + height / 256) * Math.cos(Math.PI / 6));
			context.font = height / 45 + "pt Calibri bold";
			context.fillText("Play",
				(a + height / 64) * Math.cos(Math.PI / 6) - (b + height / 40) * Math.sin(Math.PI / 6),
				(a + height / 64) * Math.sin(Math.PI / 6) + (b + height / 40) * Math.cos(Math.PI / 6));

			// restore context
			context.rotate(30 * Math.PI / 180);
		},

		drawPlayerPlacedTactic: function (tactic, card) {
			// lock the other cards
			$('.tacticCard').each(function () {
				$(this).attr('disabled', true);
			});

			layout.clearGameboardTactic();


			//layout.disableTacticCards();
			$('.tacticCard').css({ opacity: 0.1 });


			// inform server of selected tactical card
			window.connection.playTactic(card.data('cardId'));

			// lock tactic cards
			//			$('.tacticCard').each(function () {
			//				//$(this).disable(true);
			//			});

			var canvas = document.getElementById("gameBoardTacticalCanvas");
			layout.placedTactic = tactic;

			layout.drawTactic(canvas, tactic);
		},

		drawOpponentPlacedTactic: function (tactic) {
			var canvas = document.getElementById("gameBoardCanvas");
			var context = canvas.getContext("2d");

			context.rotate(-Math.PI);
			layout.drawTactic(canvas, tactic);

			// restore context rotation
			context.rotate(Math.PI);
		},

		drawTactic: function (canvas, tactic) {
			// seting up canvas
			var context = canvas.getContext("2d");
			var width = canvas.width;
			var height = canvas.height;

			// editable values
			var pointSize = 0.12; // radius of gameSquare height
			var relationWidth = 0.025;
			var outerCircle = 1.4;

			// don't edit below

			// remove any pass node which also is a movment node
			//			for (index in tactic.nodes) {
			//				console.log(passNode);
			//				console.log(jQuery.inArray(index, tactic.movementNode));
			//				if (jQuery.inArray(index, tactic.movementNode))
			//					tactic.nodes.remove(index);
			//			};

			// calculate position
			var left = width * 0.26;
			var top = height * 0.2209;
			var gameSquareWidth = width * 0.48;
			var gameSquareHeight = height * 0.186;

			var lineWidthNormal = gameSquareWidth * relationWidth;
			var lineWidthThin = lineWidthNormal / 2;

			// draw pass lines
			context.strokeStyle = "#000";
			context.lineWidth = lineWidthNormal;
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
			context.lineWidth = lineWidthThin;
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
			context.lineWidth = lineWidthNormal;
			context.fillStyle = "#fff";
			for (index in tactic.movementNode) {
				context.beginPath();
				context.arc(left + gameSquareWidth * tactic.movementNode[index][0], top + gameSquareHeight * tactic.movementNode[index][1], gameSquareHeight * pointSize, 0, Math.PI * 2, true);
				context.closePath();
				context.fill();
				context.stroke();
			}

		},

		clearGameboardTactic: function () {
			// remove drawn tactic on gameboard
			var canvas = $("#gameBoardTacticalCanvas").get(0);
			layout.clearTactic(canvas);
		},

		clearTactic: function (canvas) {
			// remove drawn tactic on element
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
			$(".benched").css({
				'width': (width + 2 * border) + 'px',
				'height': (height + 2 * border) + 'px',
				'top': -border + 'px',
				'left': -border + 'px'
			});
			$(".benched .teamLogo").css({
				'width': (width + 2 * border - 2 * cardPadding) + 'px',
				'height': (height + 2 * border - 2 * cardPadding) + 'px',
				'border': border + 'px solid #fff',
				'border-radius': (6 * width / baseWidth) + 'px',
				'margin': cardPadding + 'px auto'
			});

			baseFont = 14;

			$(".benched .playerName").css({
				'left': (border) + 'px',
				'top': (border) + 'px',
				'font-size': (baseFont * width / baseWidth) + 'px',
				'line-height': (baseFont * width / baseWidth) + 'px'
			});

			baseFont = 18;

			$(".benched .attr1").css({
				'right': (cardPadding + border) + 'px',
				'bottom': (cardPadding + border + 2 * ((baseFont + 2) * width / baseWidth)) + 'px',
				'font-size': (baseFont * width / baseWidth) + 'px',
				'line-height': (baseFont * width / baseWidth) + 'px'
			});
			$(".benched .attr2").css({
				'right': (cardPadding + border) + 'px',
				'bottom': (cardPadding + border + ((baseFont + 2) * width / baseWidth)) + 'px',
				'font-size': (baseFont * width / baseWidth) + 'px',
				'line-height': (baseFont * width / baseWidth) + 'px'
			});

			baseFont = 16;

			$(".benched .playerPos").css({
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

			baseFont = 14;

			$(".onBoard .playerName").css({
				'left': (border) + 'px',
				'top': (border) + 'px',
				'font-size': (baseFont * width / baseWidth) + 'px',
				'line-height': (baseFont * width / baseWidth) + 'px'
			});

			baseFont = 18;

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

			baseFont = 16;

			$(".onBoard .playerPos").css({
				'right': (cardPadding + border) + 'px',
				'bottom': (cardPadding + border) + 'px',
				'font-size': (baseFont * width / baseWidth) + 'px',
				'line-height': (baseFont * width / baseWidth) + 'px'
			});

			// Black squares --------------------------------------------------------
			baseWidth = 100;
			baseHeight = 70;
			baseFont = 36;
			baseBorder = 2;
			basePadding = 6;

			totalWidth = $("#gameBoardLW").width();

			width = totalWidth * baseWidth / 222;
			height = baseHeight * width / baseWidth;
			margin = (totalWidth - 3 * width) / 4;

			font = baseFont * width / baseWidth;
			border = baseBorder * width / baseWidth;
			cardPadding = basePadding * width / baseWidth;

			$(".gameBoardPlaceholder").css({
				'width': width + 'px',
				'height': height + 'px',
				'font-size': font + 'px',
				'line-height': height + 'px',
				'margin': (margin / 2) + 'px ' + (margin / 2) + 'px ' + (margin / 2) + 'px ' + (margin / 2) + 'px',
				'border': border + 'px solid #666',
				'border-radius': (10 * width / baseWidth) + 'px'
			});

			// update tactic cards height
			var tacticWidth = $(".tacticCard").width();
			var tacticHeight = (tacticWidth / 5) * 8;
			$(".tacticCard").each(function () {
				$(this).height(tacticHeight);
				//				var elem = $(this).find("canvas");
				//				layout.clearTactic(elem[0]);
				//				layout.drawTactic(elem[0], jQuery.data($(this), "tactic"));
			});
		},
		setBattleViewSize: function () {
			var baseWidth = 836;
			var baseHeight = 530;

			var totalWidth = document.width;

			var width = totalWidth * baseWidth / 1280;
			var height = baseHeight * width / baseWidth;

			$("#battle-view").dialog("option", {
				'width': width,
				'height': height,
				'position': center
			});
		}
	};

	$(window).bind('resize', function () {
		layout.init();
		layout.drawMainGameboard();
		if (layout.placedTactic != null) {
			var canvas = document.getElementById("gameBoardTacticalCanvas");
			layout.drawTactic(canvas, layout.placedTactic);
		}
		layout.setCardSizes();
		layout.setBattleViewSize();
	});



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

	//#endregion

	//#region Init

	// On ready
	$(function () {
		layout.init();
		layout.drawMainGameboard();
		layout.setCardSizes();
		$('#console').tabs();
		$('#oppBench').tabs();
		$('#playerBench').tabs();
		$('#chatMessages').nanoScroller();

		PlayByPlay.lobby = new Lobby();
		if (!debug) {
			PlayByPlay.lobby.initialize();
		} else {
			//play.showBattleView("", "");
		}
	});

	$('#chatMessage').submit(function (evt) {
		evt.preventDefault();
		window.connection.send($('#chatInput').val())
						.fail(function (e) {
							alert(e);
						});
		$('#chatInput').val('');
	});

	//#endregion

	return play;
})(jQuery, _);
