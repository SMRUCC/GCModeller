/// <reference path="../../../R-sharp/studio/RMessage.ts" />
var shell;
(function (shell) {
    var url = "http://127.0.0.1:7452/exec";
    function handle_command(command) {
        $ts.post(url, { script: command }, function (data) {
            var result = data;
            if (result.code == 0) {
                var message = base64_decode(result.info);
                con.log(message).classList.add("result");
            }
            else {
                con.error(result.info);
            }
            if (!isNullOrEmpty(result.warnings)) {
                con.warn("with additional warning message:");
                for (var _i = 0, _a = result.warnings; _i < _a.length; _i++) {
                    var warn = _a[_i];
                    con.warn($from(warn.environmentStack).Select(function (a) { return a.Method.Method; }).JoinBy(" -> "));
                    for (var i = 0; i < warn.message.length; i++) {
                        con.warn(i + ". " + warn.message[i]);
                    }
                    con.warn("");
                }
            }
        });
    }
    shell.handle_command = handle_command;
    ;
})(shell || (shell = {}));
/// <reference path="shell.ts" />
/// <reference path="../build/linq.d.ts" />
/// <reference path="../../workbench/vendor/console/simple-console.d.ts" />
var con = new System.Console({
    handleCommand: shell.handle_command,
    placeholder: "#",
    storageID: "simple-console"
});
document.getElementById("Rconsole").append(con.element);
con.logHTML("<h1>Welcome to <a href='https://github.com/SMRUCC/GCModeller-workbench' target=\"__blank\">R# Workbench!</a></h1>" +
    "\n<p>\nWelcome to the R# language<br />\n<br />\nType 'demo()' for some demos, 'help()' for on-line help, or<br />\n'help.start()' for an HTML browser interface to help.<br />\nType 'q()' to quit R.</p>");
//# sourceMappingURL=Rconsole.js.map