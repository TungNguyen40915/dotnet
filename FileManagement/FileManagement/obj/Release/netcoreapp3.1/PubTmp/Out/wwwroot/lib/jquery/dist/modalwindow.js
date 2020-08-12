$(function() {
  $('#open01').on('click', function() {
    $('#overlay, #modalWindow01').fadeIn();
  });
  $('#open010').on('click', function() {
    $('#overlay, #modalWindow10').fadeIn();
  });
  $('#open02').on('click', function() {
    $('#overlay, #modalWindow02').fadeIn();
  });
  $('#open03').on('click', function() {
    $('#overlay, #modalWindow03').fadeIn();
  });  
  $('#close01').on('click', function() {
    $('#overlay, #modalWindow01').fadeOut();
  });
  $('#close02').on('click', function() {
    $('#overlay, #modalWindow02').fadeOut();
  });
  $('#close03').on('click', function() {
    $('#overlay, #modalWindow03').fadeOut();
  });    
  locateCenter();
  $(window).resize(locateCenter);

  function locateCenter() {
    let w = $(window).width();
    let h = $(window).height();
    
    let cw = $('#modalWindow01, #modalWindow02, #modalWindow03, #modalWindow10').outerWidth();
    let ch = $('#modalWindow01, #modalWindow02, #modalWindow03, #modalWindow10').outerHeight();
   
    $('#modalWindow01, #modalWindow02, #modalWindow03, #modalWindow10').css({
      'left': ((w - cw) / 2) + 'px',
      'top': ((h - ch) / 2) + 'px'
    });
  }
});
