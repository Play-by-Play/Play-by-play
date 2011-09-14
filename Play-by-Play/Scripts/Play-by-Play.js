window.PlayByPlay = (function($) {
  var play = {};

  var chat = {
    init: function() {
      connection = $.connection.chat;

      connection.addMessage = function(name, message) {
        var data = {name: name, message: message};
        $('#chatMessageTemplate').tmpl(data).appendTo('#chatMessages');
      }

      $('#chatSubmit').live({
        click: function() {
          connection.send($('#chatInput').val())
              .fail(function(e) {
                alert(e);
              });
          $('#chatInput').val("");
        }
      });
      $('#chatMessage').live({
        keypress: function(evt) {
          var key = (evt.keyCode || evt.which);
          if (key == 13) {
            $('#chatSubmit').click();
          }
        }
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
