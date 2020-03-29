// initialize the application
var app = Sammy('#main', function () {
  // include a plugin
  this.use('Mustache', 'html');

  this.get('#/', function () {
    this.render('./app/views/home.html', {}).swap();


    // load some data

    //   this.load('posts.json')
    //       // render a template
    //       .renderEach('post.mustache')
    //       // swap the DOM with the new content
    //       .swap();
  });

  this.get("#/vlans", function () {
    debugger;
    this.render('./app/views/vlans/list.html', { "items": app.data.vlans }).swap();
  });

  this.get("#/vlans/:name", function (name) {
    var name = this.params['name'];
    debugger;
    this.render('./app/views/vlans/details.html', { "name": name }).swap();
  });
  //this.swap = function(content, callback) {	    	    var context = this;	    context.$element().fadeOut('slow', function() {	        context.$element().html(content);	        context.$element().fadeIn('slow', function() { 	             if (callback) {	                callback.apply();	             }	         }); 	    });	 };

});

$.get("./app/data/cmdb.json", function (data) {
  debugger;
  app.data = data;
  app.run('#/');
});

  // start the application


  //$('a[src="#/vlans]"')