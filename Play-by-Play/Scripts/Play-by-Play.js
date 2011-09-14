window.PlayByPlay = (function($) {
  var play = {};

  var chat = {
    init: function() {
      connection = $.connection.chat;

      connection.addMessage = function(name, message) {
        var data = {name: name, message: message};
        $('#chatMessageTemplate').tmpl(data).appendTo('#chatMessages');
      }

      $('#chatSubmit').live('click', function() {
        connection.send($('#chatInput').val())
            .fail(function(e) {
              alert(e);
            });
      }).keypress;

      $.delegate()

      $.connection.hub.start();
    }
  };

  $(function() {
    $('#console').tabs();
    chat.init();
  });

  return play;
})(jQuery);
