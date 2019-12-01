namespace TypeScript.ColorManager {

    export function toHex(n) {
        var hex = n.toString(16);
        while (hex.length < 2) { hex = "0" + hex; }
        return hex;
    }
    export function cl(x: any) {
        TypeScript.logging.log(x, TypeScript.ConsoleColors.DarkYellow);
    }
    export function w3trim(x) {
        return x.replace(/^\s+|\s+$/g, '');
    }
    export function isHex(x) {
        return ('0123456789ABCDEFabcdef'.indexOf(x) > -1);
    }
}