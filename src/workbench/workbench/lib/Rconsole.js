var shell;
(function (shell) {
    var url = "http://127.0.0.1:7452/exec";
    function handle_command(command) {
        $ts.post(url, { script: command }, function (result) {
            if (result.code == 0) {
                con.log(result.info).classList.add("result");
            }
            else {
                con.error(result.info);
            }
        });
    }
    shell.handle_command = handle_command;
    ;
})(shell || (shell = {}));
/// <reference path="shell.ts" />
/// <reference path="../build/linq.d.ts" />
var con = new SimpleConsole({
    handleCommand: shell.handle_command,
    placeholder: "Enter JavaScript, or ASCII emoticons :)",
    storageID: "simple-console demo"
});
document.getElementById("Rconsole").append(con.element);
con.logHTML("<h1>Welcome to <a href='https://github.com/SMRUCC/GCModeller-workbench'>R# Workbench!</a></h1>" +
    "<p>" +
    "<span style='color: red;'><code><pre>" + "   , __           |</pre><code></span>".replace(/\s/g, "&nbsp;") + "<br />" +
    "<span style='color: red;'><code><pre>" + "  /|/  \  |  |    |</pre><code></span>".replace(/\s/g, "&nbsp;") + " Documentation: <a href='https://r_lang.dev.SMRUCC.org/'>https://r_lang.dev.SMRUCC.org/</a><br />" +
    "<span style='color: red;'><code><pre>" + "   |___/--+--+--  |</pre><code></span>".replace(/\s/g, "&nbsp;") + "<br />" +
    "<span style='color: red;'><code><pre>" + "   | \  --+--+--  |</pre><code></span>".replace(/\s/g, "&nbsp;") + " Version 2.333.7428.30319 (5/3/2020 4:50:38 PM)<br />" +
    "<span style='color: red;'><code><pre>" + "   |  \_/ |  |    |</pre><code></span>".replace(/\s/g, "&nbsp;") + " sciBASIC.NET Runtime: 4.7.7428.29489<br />" +
    "</p>\n<p>\nWelcome to the R# language\n\nType 'demo()' for some demos, 'help()' for on-line help, or\n'help.start()' for an HTML browser interface to help.\nType 'q()' to quit R.</p>");
//# sourceMappingURL=Rconsole.js.map