/// <reference path="shell.ts" />
/// <reference path="../build/linq.d.ts" />
/// <reference path="../../workbench/vendor/console/simple-console.d.ts" />

namespace RWeb {

    export const console: System.Console = new System.Console({
        handleCommand: shell.handle_command,
        placeholder: "#",
        storageID: "simple-console"
    });

    export function run_app() {
        $ts("#Rconsole").appendElement(console.element);

        // show welcome message
        console.logHTML(
            "<h1><a href='https://github.com/SMRUCC/GCModeller-workbench' target=\"__blank\">GCModeller Workbench</a></h1>" +
            `<div>
			
			<div style="max-width: 200px; padding-right: 10px;">
<img src="../assets/images/R-sharp.png" style="width: 100%;"/>
</div>
			
			
			<div style="float: left; width: 65%;">
			<br />
<p>
<strong style="font-size: 2em;">Welcome to the <code>R#</code> language</strong><br />
<br />
Type '<code>demo()</code>' for some demos, '<code>help()</code>' for on-line help, or<br />
'<code>help.start()</code>' for an HTML browser interface to help.<br />
Type '<code>q()</code>' to quit R.</p>
</div>

</div>
`
        );
		console.logHTML("<br /><br />");
    }
}

$ts.mode = Modes.debug;
$ts(RWeb.run_app);
