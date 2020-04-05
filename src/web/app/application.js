// This is what our customer data looks like.
const customerData = [
    { ssn: "444-44-4444", name: "Bill", age: 35, email: "bill@company.com" },
    { ssn: "555-55-5555", name: "Donna", age: 32, email: "donna@home.org" }
  ];
  
const dbName = "the_name";

var request = indexedDB.open(dbName, 2);

request.onerror = function(event) {
  // Handle errors.
};
request.onupgradeneeded = function(event) {
  var db = event.target.result;

  // Create an objectStore to hold information about our customers. We're
  // going to use "ssn" as our key path because it's guaranteed to be
  // unique - or at least that's what I was told during the kickoff meeting.
  var objectStore = db.createObjectStore("customers", { keyPath: "ssn" });

  // Create an index to search customers by name. We may have duplicates
  // so we can't use a unique index.
  objectStore.createIndex("name", "name", { unique: false });

  // Create an index to search customers by email. We want to ensure that
  // no two customers have the same email, so use a unique index.
  objectStore.createIndex("email", "email", { unique: true });

  // Use transaction oncomplete to make sure the objectStore creation is 
  // finished before adding data into it.
  objectStore.transaction.oncomplete = function(event) {
    // Store values in the newly created objectStore.
    var customerObjectStore = db.transaction("customers", "readwrite").objectStore("customers");
    customerData.forEach(function(customer) {
      customerObjectStore.add(customer);
    });
  };
};


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

    this.get("#/firewall-rules", function (context) {
        this.render('./app/views/firewall-rules/list.html', { "items": app.data['firewall-rules'] }).swap();
        activateMenu(context.path);
    });


    this.get("#/machine-templates", function () {
        this.render('./app/views/machine-templates/list.html', {items: app.data["machine-templates"]}).swap();
        activateMenu('#/machine-groups');
    });

    this.get("#/machine-templates/:name", function () {
        var name = this.params['name'];
        var item = getItem(app.data["machine-templates"], 'name', name);
        this.render('./app/views/machine-templates/details.html', item).swap();
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

    this.get("#/redis", function () {
        this.render('./app/views/redis/list.html', { "items": app.data.redis }).swap();
        activateMenu('#/redis');
    });

    this.get("#/redis/:name", function (context) {
        var name = this.params['name'];
        var item = getItem(app.data.redis, 'name', name);
        this.render('./app/views/redis/details.html', item).swap();
        activateMenu('#/redis');
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

        // TODO: work with vlans
    }

    return data;
}