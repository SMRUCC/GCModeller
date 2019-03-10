/**
 * 可能对unicode的支持不是很好，推荐只用来压缩ASCII字符串
*/
module LZW {

    /**
     * LZW-compress a string
    */
    export function encode(s: string): string {
        var dict = {};
        var data = (s + "").split("");
        var out = [];
        var currChar: string;
        var phrase = data[0];
        var code = 256;

        for (var i = 1; i < data.length; i++) {
            currChar = data[i];

            if (dict[phrase + currChar] != null) {
                phrase += currChar;
            } else {
                out.push(phrase.length > 1 ? dict[phrase] : phrase.charCodeAt(0));
                dict[phrase + currChar] = code;
                code++;
                phrase = currChar;
            }
        }

        out.push(phrase.length > 1 ? dict[phrase] : phrase.charCodeAt(0));

        for (var i = 0; i < out.length; i++) {
            out[i] = String.fromCharCode(out[i]);
        }

        return out.join("");
    }

    /**
     * Decompress an LZW-encoded string
    */
    export function decode(s: string): string {
        var dict = {};
        var data = (s + "").split("");
        var currChar = data[0];
        var oldPhrase = currChar;
        var out = [currChar];
        var code = 256;
        var phrase: string;

        for (var i = 1; i < data.length; i++) {
            var currCode = data[i].charCodeAt(0);

            if (currCode < 256) {
                phrase = data[i];
            } else {
                phrase = dict[currCode] ? dict[currCode] : (oldPhrase + currChar);
            }

            out.push(phrase);
            currChar = phrase.charAt(0);
            dict[code] = oldPhrase + currChar;
            code++;
            oldPhrase = phrase;
        }

        return out.join("");
    }
}

