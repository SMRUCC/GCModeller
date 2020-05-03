var shell;
(function (shell) {
    function handle_command(command) {
        // Conversational trivialities
        var log_emoticon = function (face, rotate_direction) {
            // top notch emotional mirroring (*basically* artificial general intelligence :P)
            var span = document.createElement("span");
            span.style.display = "inline-block";
            span.style.transform = "rotate(" + (rotate_direction / 4) + "turn)";
            span.style.cursor = "vertical-text";
            span.style.fontSize = "1.3em";
            span.innerText = face.replace(">", "?").replace("<", "?");
            con.log(span);
        };
        if (command.match(/^((Well|So|Um|Uh),? )?(Hi|Hello|Hey|Greetings|Hola)/i)) {
            con.log((command.match(/^[A-Z]/) ? "Hello" : "hello") + (command.match(/\.|!/) ? "." : ""));
        }
        else if (command.match(/^((Well|So|Um|Uh),? )?(What'?s up|Sup)/i)) {
            con.log((command.match(/^[A-Z]/) ? "Not much" : "not much") + (command.match(/\?|!/) ? "." : ""));
        }
        else if (command.match(/^(>?[:;8X][-o ]?[O03PCDS\\/|()[\]{}])$/i)) {
            log_emoticon(command, +1);
        }
        else if (command.match(/^([O03PCDS\\/|()[\]{}][-o ]?[:;8X]<?)$/i)) {
            log_emoticon(command, -1);
        }
        else if (command.match(/^<3$/i)) {
            con.log("?");
            // Unhelp
        }
        else if (command.match(/^(!*\?+!*|(please |plz )?(((I )?(want|need)[sz]?|display|show( me)?|view) )?(the |some )?help|^(gimme|give me|lend me) ((the |some )?)help| a hand( here)?)/i)) { // overly comprehensive, much?
            con.log("I could definitely help you if I wanted to.");
        }
        else {
            var err;
            try {
                var result = eval(command);
            }
            catch (error) {
                err = error;
            }
            if (err) {
                con.error(err);
            }
            else {
                con.log(result).classList.add("result");
            }
        }
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