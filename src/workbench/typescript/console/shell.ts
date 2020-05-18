namespace shell {

    const url: string = "http://127.0.0.1:7452/exec";

    export function handle_command(command: string) {
        $ts.post(url, { script: command }, function (result: Message) {
            if (result.code == 0) {
                let message = base64_decode(result.info);

                con.log(message).classList.add("result");
            } else {
                con.error(result.info);
            }
        });
    };
}

interface Message extends IMsg<string> {
    content_type: string;
    err: any;
    server_time: number;
    warnings: any[];
}