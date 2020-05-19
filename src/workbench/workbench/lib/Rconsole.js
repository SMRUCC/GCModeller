/// <reference path="../../../R-sharp/studio/RMessage.ts" />
var RWeb;
(function (RWeb) {
    var shell;
    (function (shell) {
        var url = "http://127.0.0.1:7452/exec";
        function handle_command(command) {
            $ts.post(url, { script: command }, function (data) {
                var result = data;
                if (result.code == 0) {
                    if (result.content_type.startsWith("text/html")) {
                        RWeb.console.log($ts("<pre>").display(base64_decode(result.info))).classList.add("result");
                    }
                    else {
                        RWeb.console.log($ts("<img>", { src: result.info })).classList.add("result");
                    }
                }
                else {
                    RWeb.console.error(result.info);
                }
                if (!isNullOrEmpty(result.warnings)) {
                    RWeb.console.warn($ts("<h5>").display("run with additional warning message:"));
                    for (var _i = 0, _a = result.warnings; _i < _a.length; _i++) {
                        var warn = _a[_i];
                        RWeb.console.warn($ts("<pre>").display($from(warn.environmentStack).Select(function (a) { return a.Method.Method; }).JoinBy(" -> ")));
                        for (var i = 0; i < warn.message.length; i++) {
                            RWeb.console.warn($ts("<pre>").display(i + ". " + warn.message[i]));
                        }
                        RWeb.console.warn("");
                    }
                }
            });
        }
        shell.handle_command = handle_command;
        ;
    })(shell = RWeb.shell || (RWeb.shell = {}));
})(RWeb || (RWeb = {}));
/// <reference path="shell.ts" />
/// <reference path="../build/linq.d.ts" />
/// <reference path="../../workbench/vendor/console/simple-console.d.ts" />
var RWeb;
(function (RWeb) {
    RWeb.console = new System.Console({
        handleCommand: RWeb.shell.handle_command,
        placeholder: "#",
        storageID: "simple-console"
    });
    function run_app() {
        $ts("#Rconsole").appendElement(RWeb.console.element);
        // show welcome message
        RWeb.console.logHTML("<h1><a href='https://github.com/SMRUCC/GCModeller-workbench' target=\"__blank\">GCModeller Workbench</a></h1>" +
            "<div>\n\t\t\t\n\t\t\t<div style=\"max-width: 200px; padding-right: 10px;\">\n<img src=\"../assets/images/R-sharp.png\" style=\"width: 100%;\"/>\n</div>\n\t\t\t\n\t\t\t\n\t\t\t<div style=\"float: left; width: 65%;\">\n\t\t\t<br />\n<p>\n<strong style=\"font-size: 2em;\">Welcome to the <code>R#</code> language</strong><br />\n<br />\nType '<code>demo()</code>' for some demos, '<code>help()</code>' for on-line help, or<br />\n'<code>help.start()</code>' for an HTML browser interface to help.<br />\nType '<code>q()</code>' to quit R.</p>\n</div>\n\n</div>\n");
        RWeb.console.logHTML("<br /><br />");
    }
    RWeb.run_app = run_app;
})(RWeb || (RWeb = {}));
$ts.mode = Modes.debug;
$ts(RWeb.run_app);
//# sourceMappingURL=Rconsole.js.map