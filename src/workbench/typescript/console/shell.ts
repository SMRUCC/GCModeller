/// <reference path="../../../R-sharp/studio/RMessage.ts" />

namespace RWeb.shell {

    const url: string = "http://127.0.0.1:7452/exec";

    export function handle_command(command: string) {
        $ts.post(url, { script: command }, function (data: any) {
            let result: RInvoke = data;

            if (result.code == 0) {
                if (result.content_type.startsWith("text/html")) {
                    console.log($ts("<pre>").display(base64_decode(result.info))).classList.add("result");
                } else {
                    console.log($ts("<img>", { src: result.info })).classList.add("result");
                }
            } else {
                console.error(result.info);
            }

            if (!isNullOrEmpty(result.warnings)) {
                console.warn($ts("<h5>").display("run with additional warning message:"));

                for (let warn of result.warnings) {
                    let str: string = $from(warn.environmentStack)
                        .Select(a => a.Method.Method)
                        .JoinBy(" -> ") + "~:br/>";

                    for (let i = 0; i < warn.message.length; i++) {
                        str += `${i}. ${warn.message[i]}` + "~:br/>";
                    }

                    str = str
                        .replace(/\s/g, "&nbsp;")
                        .replace(/[<]/g, "&lt;")
                        .replace(/[~:]{2}/g, "<");
                    console.warn($ts("<span>").display(str));
                }
            }
        });
    };
}