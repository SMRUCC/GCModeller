namespace shell {

    const url: string = "http://127.0.0.1:7452/exec";

    export function handle_command(command: string) {
        $ts.post(url, { script: command }, function (result: IMsg<string>) {
            if (result.code == 0) {
                con.log(result.info).classList.add("result");
            } else {
                con.error(result.info);
            }
        });
    };
}
