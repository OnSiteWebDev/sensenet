using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace SenseNet.Web.Client.Javascript.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppJsController : ControllerBase
    {
        private IConfiguration _configuration;
        private SenseNetEnvironment _snEnvironment;

        public AppJsController(IConfiguration configuration, IOptions<SenseNetEnvironment> snEnvironment)
        {
            _configuration = configuration;
            _snEnvironment = snEnvironment.Value;
        }

        // GET: api/AppJs
        [HttpGet]
        public string Get()
        {
            var isInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
            var authority = _snEnvironment.Authentication.Authority;
            var snrepo = _configuration["SnRepo"];
            var api = _configuration["Api"];
            var thisUrl = _configuration["ThisUrl"];

            return string.Format(_template, isInContainer.ToString().ToLower(), authority, snrepo, api, thisUrl);
        }

        private static readonly string _template = @"
var env = {{
    isInContainer: {0},
    authority: ""{1}"",
    snrepo: ""{2}"",
    api: ""{3}"",
    thisUrl: ""{4}""
}};

function log() {{
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {{
        if (msg instanceof Error) {{
            msg = ""Error: "" + msg.message;
        }}
        else if (typeof msg !== 'string') {{
            msg = JSON.stringify(msg, null, 2);
        }}
        document.getElementById('results').innerHTML += msg + '\r\n';
    }});
}}

document.getElementById(""environment"").innerText = ""Environment:\n"" +  JSON.stringify(env, null, 2);

document.getElementById(""login"").addEventListener(""click"", login, false);
document.getElementById(""api"").addEventListener(""click"", api, false);
document.getElementById(""logout"").addEventListener(""click"", logout, false);


var config = {{
    authority: env.authority,
    client_id: ""spa"",
    redirect_uri: env.thisUrl + ""/callback.html"",
    response_type: ""code"",
    scope: ""openid profile sensenet"",
    post_logout_redirect_uri: env.thisUrl + ""/index.html"",
    extraQueryParams: {{ snrepo: env.snrepo }}
}};

var mgr = new Oidc.UserManager(config);

mgr.getUser().then(function (user) {{
    if (user) {{
        log(""User logged in"", user.profile);
    }}
    else {{
        log(""User not logged in"");
    }}
}});

function login() {{
    mgr.signinRedirect();
}}

function api() {{
    mgr.getUser().then(function (user) {{
        var url = env.api + ""/odata.svc/Root?metadata=no&$select=Id,Name,Path&enableautofilters=false"";

        var xhr = new XMLHttpRequest();
        xhr.open(""GET"", url);
        xhr.onload = function () {{
            log(xhr.status, JSON.parse(xhr.responseText));
        }}
        xhr.setRequestHeader(""Authorization"", ""Bearer "" + user.access_token);
        xhr.send();
    }});
}}

function logout() {{
    mgr.signoutRedirect();
}}";

    }
}
