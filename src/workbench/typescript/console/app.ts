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
    }
}

$ts.mode = Modes.debug;
$ts(RWeb.run_app);
