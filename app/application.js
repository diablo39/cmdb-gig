// initialize the application
var app = Sammy('#main', function() {
    // include a plugin
    this.use('Mustache');
  
    // define a 'route'
    this.get('#/', function() {
      this.render('./app/views/main.html', {}).swap();


      // load some data
     
    //   this.load('posts.json')
    //       // render a template
    //       .renderEach('post.mustache')
    //       // swap the DOM with the new content
    //       .swap();
    });

    this.get("#/vlans", function() {
        alert("vlans");
    });
  });
  
  // start the application
  app.run('#/');