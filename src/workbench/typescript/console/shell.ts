namespace shell {

    export function handle_command(command: string) {
        // Conversational trivialities
        var log_emoticon = function (face, rotate_direction) {
            // top notch emotional mirroring (*basically* artificial general intelligence :P)
            var span = document.createElement("span");
            span.style.display = "inline-block";
            span.style.transform = "rotate(" + (rotate_direction / 4) + "turn)";
            span.style.cursor = "vertical-text";
            span.style.fontSize = "1.3em";
            span.innerText = face.replace(">", "?").replace("<", "?");
            con.log(span);
        };
        if (command.match(/^((Well|So|Um|Uh),? )?(Hi|Hello|Hey|Greetings|Hola)/i)) {
            con.log((command.match(/^[A-Z]/) ? "Hello" : "hello") + (command.match(/\.|!/) ? "." : ""));
        } else if (command.match(/^((Well|So|Um|Uh),? )?(What'?s up|Sup)/i)) {
            con.log((command.match(/^[A-Z]/) ? "Not much" : "not much") + (command.match(/\?|!/) ? "." : ""));
        } else if (command.match(/^(>?[:;8X][-o ]?[O03PCDS\\/|()[\]{}])$/i)) {
            log_emoticon(command, +1);
        } else if (command.match(/^([O03PCDS\\/|()[\]{}][-o ]?[:;8X]<?)$/i)) {
            log_emoticon(command, -1);
        } else if (command.match(/^<3$/i)) {
            con.log("?");
            // Unhelp
        } else if (command.match(/^(!*\?+!*|(please |plz )?(((I )?(want|need)[sz]?|display|show( me)?|view) )?(the |some )?help|^(gimme|give me|lend me) ((the |some )?)help| a hand( here)?)/i)) { // overly comprehensive, much?
            con.log("I could definitely help you if I wanted to.");
        } else {
            var err;
            try {
                var result = eval(command);
            } catch (error) {
                err = error;
            }
            if (err) {
                con.error(err);
            } else {
                con.log(result).classList.add("result");
            }
        }
    };
}
