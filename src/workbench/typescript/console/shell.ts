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
                    } else if (result.content_type == "inspector/json") {
                        openView("./inspector.html");
                    } else if (result.content_type == "text/csv") {
                        let csv = base64_decode(result.info);
                        openView("./inspector.table.html");
                        console.info(csv);
                    } else {
                        console.log(image(result.info)).classList.add("result");
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

    function image(base64: string): HTMLElement {
        let link = $ts("<a>", {
            id: "image_fancybox",
            class: "fancybox",
            "data-rel": "fancybox",
            "data-fancybox": "",
            "data-caption": "",
            href: base64
        }).display(
            $ts("<img>", {
                class: "img-responsive",
                src: base64,
                style: "width: 600px;"
            })
        );

        return link;
    }

    function messageText(msg: RMessage): HTMLElement {
        let str: string = $from(msg.environmentStack)
            .Select(a => a.Method.Method)
            .Reverse()
            .JoinBy(" -> ")
            .replace(/[<]/g, "&lt;") + "<br/>";

        for (let i = 0; i < msg.message.length; i++) {
            str += `${i + 1}. ${msg.message[i]}`.replace(/[<]/g, "&lt;") + "<br/>";
        }

        str = str.replace(/\s/g, "&nbsp;");

        return $ts("<span>").display(str);
    }
}