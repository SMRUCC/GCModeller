namespace GCModeller.Workbench {

    export function supports_text(ctx): boolean {
        if (!ctx.fillText) {
            return false;
        }
        if (!ctx.measureText) {
            return false;
        }
        return true;
    }

    /*
 * Fast string trimming implementation found at
 * http://blog.stevenlevithan.com/archives/faster-trim-javascript
 *
 * Note that regex is good at removing leading space but
 * bad at removing trailing space as it has to first go through
 * the whole string.
 */
    export function trim(str: string): string {
        "use strict";

        var ws, i;
        str = str.replace(/^\s\s*/, '');
        ws = /\s/; i = str.length;
        while (ws.test(str.charAt(--i)));
        return str.slice(0, i + 1);
    }
}