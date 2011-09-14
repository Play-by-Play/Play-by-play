window.PlayByPlay = (function($) {
  var play = {};

  var chat = {
    init: function() {
      connection = $.connection.chat;

      connection.addMessage = function(message) {
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

  $(function() {
    $('#console').tabs();
    chat.init();
  });

  return play;
})(jQuery);
