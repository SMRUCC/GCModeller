module workbench.Shell {

    export function Rweb() {
        let spawn = require("child_process").spawn;
        let bat = spawn("Rweb", [
            "--session",
            "--port", "7452",
            "--workspace", "./Rweb",
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

    export function initialize() {
        // 导入Rstudio环境诊断组件
        let request = require('request');
        let script: string = "imports 'diagnostics' from 'Rstudio';";

        request(`http://127.0.0.1:7452/exec?script=${encodeURIComponent(script)}`, function (error, response, body) {
            if (!error && response.statusCode == 200) {
                console.log(body)
            }
        });
    }
}
