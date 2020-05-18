/// <reference path="shell.ts" />
/// <reference path="../build/linq.d.ts" />
/// <reference path="../../workbench/vendor/console/simple-console.d.ts" />

let con = new System.Console({
    handleCommand: shell.handle_command,
    placeholder: "#",
    storageID: "simple-console"
});

document.getElementById("Rconsole").append(con.element);

con.logHTML(
    "<h1>Welcome to <a href='https://github.com/SMRUCC/GCModeller-workbench'>R# Workbench!</a></h1>" +
    "<p>" +
    "<span style='color: red;'><code><pre>" + "   , __           |</pre><code></span>".replace(/\s/g, "&nbsp;") + "<br />" +
    "<span style='color: red;'><code><pre>" + "  /|/  \  |  |    |</pre><code></span>".replace(/\s/g, "&nbsp;") + " Documentation: <a href='https://r_lang.dev.SMRUCC.org/'>https://r_lang.dev.SMRUCC.org/</a><br />" +
    "<span style='color: red;'><code><pre>" + "   |___/--+--+--  |</pre><code></span>".replace(/\s/g, "&nbsp;") + "<br />" +
    "<span style='color: red;'><code><pre>" + "   | \  --+--+--  |</pre><code></span>".replace(/\s/g, "&nbsp;") + " Version 2.333.7428.30319 (5/3/2020 4:50:38 PM)<br />" +
    "<span style='color: red;'><code><pre>" + "   |  \_/ |  |    |</pre><code></span>".replace(/\s/g, "&nbsp;") + " sciBASIC.NET Runtime: 4.7.7428.29489<br />" +
    `</p>
<p>
Welcome to the R# language

Type 'demo()' for some demos, 'help()' for on-line help, or
'help.start()' for an HTML browser interface to help.
Type 'q()' to quit R.</p>`
);