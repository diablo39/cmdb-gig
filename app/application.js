// initialize the application
var app = Sammy('#main', function () {
    // include a plugin
    this.use('Mustache', 'html');

    this.get('#/', function () {
        this.render('./app/views/home.html', {}).swap();
        activateMenu('#/');

        // load some data

        //   this.load('posts.json')
        //       // render a template
        //       .renderEach('post.mustache')
        //       // swap the DOM with the new content
        //       .swap();
    });

    this.get("#/env", function () {
        this.render('./app/views/env/list.html', { "items": app.data.env }).swap();
        activateMenu('#/env');
    });

    this.get("#/env/:name", function (context) {
        var name = this.params['name'];
        var item = getItem(app.data.env, 'name', name);
        this.render('./app/views/env/details.html', item).swap();
        activateMenu('#/env');
    });

    this.get("#/vlans", function () {
        this.render('./app/views/vlans/list.html', { "items": app.data.vlans }).swap();
        activateMenu('#/vlans');
    });

    this.get("#/vlans/:name", function (context) {
        var name = this.params['name'];
        var item = getItem(app.data.vlans, 'name', name);
        this.render('./app/views/vlans/details.html', item).swap();
        activateMenu('#/vlans');
    });

    this.get("#/f5-vs", function () {
        this.render('./app/views/common/soon.html', {}).swap();
        activateMenu('#/f5-vs');
    });

    this.get("#/firewall-rules", function () {
        this.render('./app/views/common/soon.html', {}).swap();
        activateMenu('#/firewall-rules');
    });


    this.get("#/machine-groups", function () {
        this.render('./app/views/common/soon.html', {}).swap();
        activateMenu('#/machine-groups');
    });

    this.get("#/machines", function () {
        this.render('./app/views/machines/list.html', { "items": app.data.machines }).swap();
        activateMenu('#/machines');
    });

    this.get("#/machines/:name", function (context) {
        var name = this.params['name'];
        var item = getItem(app.data.machines, 'name', name);
        this.render('./app/views/machines/details.html', item).swap();
        activateMenu('#/machines');
    });

    //this.swap = function(content, callback) {	    	    var context = this;	    context.$element().fadeOut('slow', function() {	        context.$element().html(content);	        context.$element().fadeIn('slow', function() { 	             if (callback) {	                callback.apply();	             }	         }); 	    });	 };

    this.get("#/applications", function () {
        this.render('./app/views/common/soon.html', {}).swap();
        activateMenu('#/applications');
    });


    this.get("#/soon", function () {
        this.render('./app/views/common/soon.html', {}).swap();
        activateMenu('#/soon');
    });
});


// start the application

$.get("./app/data/cmdb.json", function (data) {
    app.data = postProcessData(data);
    app.run('#/');
}).fail(function () {
    alert("fetching data failed");
});


function getItem(array, keyName, value) {
    for (i = 0; i < array.length; i++) {
        var item = array[i];
        if (item[keyName] === value) {
            return item;
        }
    }
    return null;
}

function activateMenu(url) {
    $('#mainNavigation').find('a').removeClass('mm-active');
    $('#mainNavigation').find('a[href="' + url + '"]').addClass('mm-active');
}

function postProcessData(data) {
    data = processMachines(data);

    return data;
}

function processMachines(data) {
    
    var machines = data['machines'];
    var templates = data['machine-templates'];

    for (var i = 0; i < machines.length; i++) {
        var machine = machines[i];
        var template = getItem(templates, 'name', machine.template);

        for (key in template) {
            if (key === "name") continue;
            
            var value = template[key];

            if (machine[key] === undefined || machine[key] === null) {
                machine[key] = value;
            }
        }
    }

    return data;
}