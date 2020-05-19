/// <reference path="../../../R-sharp/studio/RMessage.ts" />

namespace RWeb.shell {

    const url: string = "http://127.0.0.1:7452/exec";

    export function handle_command(command: string) {
        $ts.post(url, { script: command }, function (data: any) {
            let result: RInvoke = data;

            if (result.code == 0) {
                if (!Strings.Empty(result.info)) {
                    if (result.content_type.startsWith("text/html")) {
                        console.log($ts("<pre>").display(base64_decode(result.info))).classList.add("result");
                    } else {
                        console.log($ts("<img>", { src: result.info })).classList.add("result");
                    }
                }
            } else {
                console.error($ts("<h5>").display(" Error in:"));
                console.error(messageText(result.err));
            }

            if (!isNullOrEmpty(result.warnings)) {
                console.warn($ts("<h5>").display("with additional warning message:"));

                for (let warn of result.warnings) {
                    console.warn(messageText(warn));
                }
            }
        });
    };

    function messageText(msg: RMessage): HTMLElement {
        let str: string = $from(msg.environmentStack)
            .Select(a => a.Method.Method)
            .Reverse()
            .JoinBy(" -> ")
            .replace(/[<]/g, "&lt;") + "<br/>";

        for (let i = 0; i < msg.message.length; i++) {
            str += `${i}. ${msg.message[i]}`.replace(/[<]/g, "&lt;") + "<br/>";
        }

        str = str.replace(/\s/g, "&nbsp;");

        return $ts("<span>").display(str);
    }
}