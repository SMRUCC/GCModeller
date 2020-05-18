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
    "<h1><a href='https://github.com/SMRUCC/GCModeller-workbench' target=\"__blank\">SMRUCC\\GCModeller Workbench</a></h1>" +
`
<p>
Welcome to the R# language<br />
<br />
Type '<code>demo()</code>' for some demos, '<code>help()</code>' for on-line help, or<br />
'<code>help.start()</code>' for an HTML browser interface to help.<br />
Type '<code>q()</code>' to quit R.</p>`
);