namespace shell {

    export function handle_command(command: string) {
        $ts.post("http://127.0.0.1:4752/exec", function (result: IMsg<string>) {
            if (result.code == 0) {
                con.log(result.info).classList.add("result");
            } else {
                con.error(result.info);
            }
        });
    };
}
