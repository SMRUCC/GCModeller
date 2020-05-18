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
    "<h1>Welcome to <a href='https://github.com/SMRUCC/GCModeller-workbench' target=\"__blank\">R# Workbench!</a></h1>" +
`
<p>
Welcome to the R# language<br />
<br />
Type 'demo()' for some demos, 'help()' for on-line help, or<br />
'help.start()' for an HTML browser interface to help.<br />
Type 'q()' to quit R.</p>`
);