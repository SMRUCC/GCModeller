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
                    if (!Strings.Empty(result.info)) {
                        if (result.content_type.startsWith("text/html")) {
                            RWeb.console.log($ts("<pre>").display(base64_decode(result.info))).classList.add("result");
                        }
                        else {
                            RWeb.console.log(image(result.info)).classList.add("result");
                        }
                    }
                }
                else {
                    RWeb.console.error($ts("<h5>").display(" Error in:"));
                    RWeb.console.error(messageText(result.err));
                }
                if (!isNullOrEmpty(result.warnings)) {
                    RWeb.console.warn($ts("<h5>").display("with additional warning message:"));
                    for (var _i = 0, _a = result.warnings; _i < _a.length; _i++) {
                        var warn = _a[_i];
                        RWeb.console.warn(messageText(warn));
                    }
                }
            });
        }
        shell.handle_command = handle_command;
        ;
        function image(base64) {
            var link = $ts("<a>", {
                id: "image_fancybox",
                class: "fancybox",
                "data-rel": "fancybox",
                "data-fancybox": "",
                "data-caption": "",
                href: base64
            }).display($ts("<img>", {
                class: "img-responsive",
                src: base64,
                style: "width: 600px;"
            }));
            return link;
        }
        function messageText(msg) {
            var str = $from(msg.environmentStack)
                .Select(function (a) { return a.Method.Method; })
                .Reverse()
                .JoinBy(" -> ")
                .replace(/[<]/g, "&lt;") + "<br/>";
            for (var i = 0; i < msg.message.length; i++) {
                str += (i + ". " + msg.message[i]).replace(/[<]/g, "&lt;") + "<br/>";
            }
            str = str.replace(/\s/g, "&nbsp;");
            return $ts("<span>").display(str);
        }
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