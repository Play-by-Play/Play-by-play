window.PlayByPlay = (function($) {
  var play = {};

  var chat = {
    init: function() {
      var connection = $.connection.chat;

      chat.addMessage = function(message) {
        $('<li>' + message + '</li>').appendTo('#chatMessages');
      }

      $('#chatSubmit').live('click', function() {
        connection.send($('#chatInput').val())
            .fail(function(e) {
              alert(e);
            });
      });

      $.connection.hub.start();
    }
  };

  $('#chatSubmit').live('click', function() {
    $('<li>' + $('#chatInput').val() + '</li>').appendTo('#chatMessages');
  });

  $(function() {
    $('#console').tabs();
    chat.init();
  });

  return play;
})
    (jQuery);