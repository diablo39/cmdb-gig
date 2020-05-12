// initialize the application
window.app = Sammy('#main', function () {
    // include a plugin
    this.use('Mustache', 'html');

    this.get('#/', function () {
        this.render('./app/views/home.html', {}).swap();
        activateMenu('#/');
    });

    this.get("#/env", function () {
        this.render('./app/views/env/list.html' + this.app.qs(true), { "items": app.data.env }).swap();
        activateMenu('#/env');
    });

    this.get("#/env/:name", function (context) {
        var name = this.params['name'];
        var item = getItem(app.data.env, 'name', name);
        this.render('./app/views/env/details.html' + this.app.qs(true), item).swap();
        activateMenu('#/env');
    });

    this.get("#/vlans", function () {
        this.render('./app/views/vlans/list.html' + this.app.qs(true), { "items": app.data.vlans }).swap();
        activateMenu('#/vlans');
    });

    this.get("#/vlans/:name", function (context) {
        var name = this.params['name'];
        this
            .load('./app/data/vlans/' + name + '.json' + this.app.qs())
            .then(function (data) {
                window.app.currentItem = data;
            })
            .render('./app/views/vlans/details.html' + this.app.qs(true))
            .swap();

        activateMenu('#/vlans');
    });

    this.get("#/f5-vs", function () {
        this.render('./app/views/common/soon.html' + this.app.qs(true), {}).swap();
        activateMenu('#/f5-vs');
    });

    this.get("#/firewall-rules", function (context) {
        this.render('./app/views/firewall-rules/list.html' + this.app.qs(true)).swap();
        activateMenu("#/firewall-rules");
    });

    this.get("#/firewall-rules/:name", function (context) {
        var name = this.params['name'];
        var item = getFirewallRule(app.data['firewall-rules'], 'id', name);
        this
            .render('./app/views/firewall-rules/details.html' + this.app.qs(true), item)
            .swap();
        activateMenu('#/firewall-rules');
    });


    this.get("#/machine-templates", function () {
        this.render('./app/views/machine-templates/list.html' + this.app.qs(true), { items: app.data["machine-templates"] }).swap();
        activateMenu('#/machine-groups');
    });

    this.get("#/machine-templates/:name", function () {
        var name = this.params['name'];
        var item = getItem(app.data["machine-templates"], 'name', name);
        this.render('./app/views/machine-templates/details.html' + this.app.qs(true), item).swap();
        activateMenu('#/machine-groups');
    });

    this.get("#/machines", function () {
        this.render('./app/views/machines/list.html' + this.app.qs(true), { "items": app.data.machines }).swap();
        activateMenu('#/machines');
    });

    this.get("#/machines/:name", function (context) {
        var name = this.params['name'];

        this
            .load('./app/data/machines/' + name + '.json' + this.app.qs())
            .then(function (data) {
                window.app.currentItem = data;
            })
            .render('./app/views/machines/details.html' + this.app.qs(true))
            .swap();
        activateMenu('#/machines');
    });

    // this.swap = function(content, callback) {	    	    var context = this;	    context.$element().fadeOut('fast', function() {	        context.$element().html(content);	        context.$element().fadeIn('slow', function() { 	             if (callback) {	                callback.apply();	             }	         }); 	    });	 };

    this.get("#/applications", function () {
        this.render('./app/views/common/soon.html' + this.app.qs(true), {}).swap();
        activateMenu('#/applications');
    });

    this.get("#/redis", function () {
        this.render('./app/views/redis/list.html' + this.app.qs(true), { "items": app.data.redis }).swap();
        activateMenu('#/redis');
    });

    this.get("#/redis/:name", function (context) {
        var name = this.params['name'];
        var item = getItem(app.data.redis, 'name', name);
        this.render('./app/views/redis/details.html' + this.app.qs(true), item).swap();
        activateMenu('#/redis');
    });


    this.get("#/soon", function () {
        this.render('./app/views/common/soon.html' + this.app.qs(true), {}).swap();
        activateMenu('#/soon');
    });

});

window.app.qs = function (template) {

    return "?hash=" + (template ? (window.cmdbVersion === 'devversion' ? new Date().getTime() : window.cmdbVersion) : this.hash);
}


// start the application

$.get("./app/data/manifest.json?c=" + new Date().getTime(), function (manifest) {

    app.hash = manifest.hash;

    $.get("./app/data/cmdb.json?c=" + manifest.hash, function (data) {
        app.data = postProcessData(data);

        $('#data-generated').text(new Date(app.data['generated']).toLocaleString());

        app.run('#/');
    }, 'json').fail(function () {
        alert("Fetching data failed");
    });

}, 'json').fail(function () {
    alert("Fetching data failed");
});




function getItem(array, keyName, value) {
    for (i = 0; i < array.length; i++) {
        var item = array[i];
        if (item[keyName] == value) {
            return item;
        }
    }
    return null;
}

function getFirewallRule(array,keyName, value) {
    for(var i=0; i< array.length; i++) {
        for(var j = 0; j< array[i]['rules'].length; j++) {
            var item = array[i]['rules'][j];
            if (item[keyName] == value) {
                return item;
            }
        }
    }

    return null;
}

function activateMenu(url) {
    $('#mainNavigation').find('a').removeClass('mm-active');
    $('#mainNavigation').find('a[href="' + url + '"]').addClass('mm-active');
}

function postProcessData(data) {
    //data = processMachines(data);

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