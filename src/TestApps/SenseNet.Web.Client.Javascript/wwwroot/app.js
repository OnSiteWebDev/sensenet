var container = true;
var _authority = container ? "https://localhost:9998" : "https://localhost:44311";
var _snrepo = container ? "https://172.17.0.3" : "https://localhost:44362";
var _api = container ? "https://localhost:10001" : "https://localhost:44362";
var _thisUrl = "https://localhost:5001/"; // "https://localhost:44341";

function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('results').innerHTML += msg + '\r\n';
    });
}

document.getElementById("login").addEventListener("click", login, false);
document.getElementById("api").addEventListener("click", api, false);
document.getElementById("logout").addEventListener("click", logout, false);


var config = {
    authority: _authority,
    client_id: "spa",
    redirect_uri: _thisUrl + "/callback.html",
    response_type: "code",
    scope: "openid profile sensenet",
    post_logout_redirect_uri: _thisUrl + "/index.html",
    extraQueryParams: { snrepo: _snrepo }
};

var mgr = new Oidc.UserManager(config);

mgr.getUser().then(function (user) {
    if (user) {
        log("User logged in", user.profile);
    }
    else {
        log("User not logged in");
    }
});

function login() {
    mgr.signinRedirect();
}

function api() {
    mgr.getUser().then(function (user) {
        var url = _api + "/odata.svc/Root?metadata=no&$select=Id,Name,Path&enableautofilters=false";

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            log(xhr.status, JSON.parse(xhr.responseText));
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

function logout() {
    mgr.signoutRedirect();
}