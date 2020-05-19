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
                        var str = $from(warn.environmentStack)
                            .Select(function (a) { return a.Method.Method; })
                            .JoinBy(" -> ") + "~:br/>";
                        for (var i = 0; i < warn.message.length; i++) {
                            str += i + ". " + warn.message[i] + "~:br/>";
                        }
                        str = str
                            .replace(/\s/g, "&nbsp;")
                            .replace(/[<]/g, "&lt;")
                            .replace(/[~:]{2}/g, "<");
                        RWeb.console.warn($ts("<span>").display(str));
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
    }
    RWeb.run_app = run_app;
})(RWeb || (RWeb = {}));
$ts.mode = Modes.debug;
$ts(RWeb.run_app);
//# sourceMappingURL=Rconsole.js.map