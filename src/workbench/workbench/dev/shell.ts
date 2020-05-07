module workbench.Shell {

    export function Rweb() {
        let spawn = require("child_process").spawn;
        let bat = spawn("Rweb.exe", [
            "--start",
            "--port", "7452",
            "--Rweb", "./Rweb",
            "--show_error",
            "--n_threads", "2"
        ]);

        bat.stdout.on("data", (data) => {
            // Handle data...
        });

        bat.stderr.on("data", (err) => {
            // Handle error...
        });

        bat.on("exit", (code) => {
            // Handle exit
        });

        return bat;
    }
}
