var wifi = require("node-wifi");

module.exports = function (callback, dotnet_c) {

    wifi.init({
        iface: null // network interface, choose a random wifi interface if set to null
    });
    // Scan networks
    wifi.scan(
    ).then(function (networks) { callback(null, networks) });
    //dotnet_c.id = "3";
    //dotnet_c.WriteLine("Test");
    //callback(null,dotnet_c.id);
};