var env = {
    isInContainer: false,
    authority: "https://localhost:44311",
    snrepo: "https://localhost:10501",
    api: "https://localhost:10501",
    thisUrl: "https://localhost:5003"
};

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

document.getElementById("environment").innerText = "Environment:\n" + JSON.stringify(env, null, 2);

document.getElementById("login").addEventListener("click", login, false);
document.getElementById("api").addEventListener("click", api, false);
document.getElementById("logout").addEventListener("click", logout, false);


var config = {
    authority: env.authority,
    client_id: "spa",
    redirect_uri: env.thisUrl + "/callback.html",
    response_type: "code",
    scope: "openid profile sensenet",
    post_logout_redirect_uri: env.thisUrl + "/index.html",
    extraQueryParams: { snrepo: env.snrepo }
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
        var url = env.api + "/odata.svc/Root?metadata=no&$select=Id,Name,Path&enableautofilters=false";

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