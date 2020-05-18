/// <reference path="../../../R-sharp/studio/RMessage.ts" />

namespace shell {

    const url: string = "http://127.0.0.1:7452/exec";

    export function handle_command(command: string) {
        $ts.post(url, { script: command }, function (data: any) {
            let result: RInvoke = data;

            if (result.code == 0) {
                let message = base64_decode(result.info);

                con.log(message).classList.add("result");
            } else {
                con.error(result.info);
            }

            if (!isNullOrEmpty(result.warnings)) {
                con.warn("with additional warning message:");

                for (let warn of result.warnings) {
                    con.warn($from(warn.environmentStack).Select(a => a.Method.Method).JoinBy(" -> "));

                    for (let i = 0; i < warn.message.length; i++) {
                        con.warn(`${i}. ${warn.message[i]}`);
                    }

                    con.warn("");
                }
            }
        });
    };
}