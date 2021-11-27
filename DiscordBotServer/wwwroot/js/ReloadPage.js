
let idletime = 10 * 60 * 1000;

$.ajax({
    url: "https://apollon-music-keycloak.herokuapp.com/auth"
}).done((msg) => {
    console.log("Keycloak is alive");
    console.log(msg);
})

$.ajax({
    url: "https://apollon-music-resource-server.herokuapp.com/ping"
}).done((msg) => {
    console.log("Resource server is alive");
    console.log(msg);
})


setTimeout(function () {
    window.location.reload();
}, idletime);