/**
 * How to Encode and Decode Strings with Base64 in JavaScript
 * 
 * https://gist.github.com/ncerminara/11257943
*/
class Base64 {

    private static readonly keyStr: string = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

    /**
     * 将任意文本编码为base64字符串
    */
    public static encode(text: string): string {
        var base64: string[] = [];
        var n, r, i, s, o, u, a;
        var f = 0;

        text = Base64.utf8_encode(text);

        while (f < text.length) {
            n = text.charCodeAt(f++);
            r = text.charCodeAt(f++);
            i = text.charCodeAt(f++);
            s = n >> 2;
            o = (n & 3) << 4 | r >> 4;
            u = (r & 15) << 2 | i >> 6;
            a = i & 63;

            if (isNaN(r)) {
                u = a = 64;
            } else if (isNaN(i)) {
                a = 64;
            }

            base64.push(this.keyStr.charAt(s));
            base64.push(this.keyStr.charAt(o));
            base64.push(this.keyStr.charAt(u));
            base64.push(this.keyStr.charAt(a));
        }

        return base64.join("");
    }

    /**
     * 将base64字符串解码为普通的文本字符串
    */
    public static decode(base64: string): string {
        var text = "";
        var n, r, i;
        var s, o, u, a;
        var f = 0;

        base64 = base64.replace(/[^A-Za-z0-9+/=]/g, "");

        while (f < base64.length) {
            s = this.keyStr.indexOf(base64.charAt(f++));
            o = this.keyStr.indexOf(base64.charAt(f++));
            u = this.keyStr.indexOf(base64.charAt(f++));
            a = this.keyStr.indexOf(base64.charAt(f++));
            n = s << 2 | o >> 4; r = (o & 15) << 4 | u >> 2;
            i = (u & 3) << 6 | a;
            text = text + String.fromCharCode(n);

            if (u != 64) {
                text = text + String.fromCharCode(r);
            }
            if (a != 64) {
                text = text + String.fromCharCode(i);
            }
        }

        text = Base64.utf8_decode(text);

        return text
    }

    /**
     * 将文本转换为utf8编码的文本字符串
    */
    public static utf8_encode(text: string): string {
        var chars: string[] = [];

        text = text.replace(/rn/g, "n");

        for (var n = 0; n < text.length; n++) {
            var r = text.charCodeAt(n);

            if (r < 128) {
                chars.push(String.fromCharCode(r));
            } else if (r > 127 && r < 2048) {
                chars.push(String.fromCharCode(r >> 6 | 192));
                chars.push(String.fromCharCode(r & 63 | 128));
            } else {
                chars.push(String.fromCharCode(r >> 12 | 224));
                chars.push(String.fromCharCode(r >> 6 & 63 | 128));
                chars.push(String.fromCharCode(r & 63 | 128));
            }
        }

        return chars.join("");
    }

    /**
     * 将utf8编码的文本转换为原来的文本
    */
    public static utf8_decode(text: string): string {
        var t: string[] = [];
        var n = 0;
        var r = 0;
        var c2 = 0;
        var c3 = 0;

        while (n < text.length) {
            r = text.charCodeAt(n);

            if (r < 128) {
                t.push(String.fromCharCode(r));
                n++;
            } else if (r > 191 && r < 224) {
                c2 = text.charCodeAt(n + 1);
                t.push(String.fromCharCode((r & 31) << 6 | c2 & 63));
                n += 2;
            } else {
                c2 = text.charCodeAt(n + 1);
                c3 = text.charCodeAt(n + 2);
                t.push(String.fromCharCode((r & 15) << 12 | (c2 & 63) << 6 | c3 & 63));
                n += 3;
            }
        }

        return t.join("");
    }
}