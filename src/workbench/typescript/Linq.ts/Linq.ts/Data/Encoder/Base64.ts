/**
 * How to Encode and Decode Strings with Base64 in JavaScript
 * 
 * https://gist.github.com/ncerminara/11257943
 * 
 * In base64 encoding, the character set is ``[A-Z, a-z, 0-9, and + /]``. 
 * If the rest length is less than 4, the string is padded with ``=`` 
 * characters.
 * 
 * (符号``=``只是用来进行字符串的长度填充使用的，因为base64字符串的长度应该总是4的倍数)
*/
module Base64 {

    const base64Pattern: RegExp = /^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)?$/g;
    const keyStr: string = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

    /**
     * 简单的检测一下所给定的字符串是否是有效的base64字符串
    */
    export function isValidBase64String(text: string): boolean {
        if (text && (text.length % 4 == 0)) {
            return base64Pattern.test(text);
        } else {
            return false;
        }
    }

    /**
     * 将任意文本编码为base64字符串
    */
    export function encode(text: string): string {
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

            base64.push(keyStr.charAt(s));
            base64.push(keyStr.charAt(o));
            base64.push(keyStr.charAt(u));
            base64.push(keyStr.charAt(a));
        }

        return base64.join("");
    }

    /**
     * 将base64字符串解码为普通的文本字符串
    */
    export function decode(base64: string): string {
        let raw: number[] = decode_rawBuffer(base64);
        let text: string = "";

        for (let code of raw) {
            text = text + String.fromCharCode(code);
        }

        text = Base64.utf8_decode(text);

        return text
    }

    export function decode_rawBuffer(base64: string): number[] {
        var buffer: number[] = [];
        var n, r, i;
        var s, o, u, a;
        var f = 0;

        base64 = base64.replace(/[^A-Za-z0-9+/=]/g, "");

        while (f < base64.length) {
            s = keyStr.indexOf(base64.charAt(f++));
            o = keyStr.indexOf(base64.charAt(f++));
            u = keyStr.indexOf(base64.charAt(f++));
            a = keyStr.indexOf(base64.charAt(f++));
            n = s << 2 | o >> 4; r = (o & 15) << 4 | u >> 2;
            i = (u & 3) << 6 | a;

            buffer.push(n);

            if (u != 64) {
                buffer.push(r);
            }
            if (a != 64) {
                buffer.push(i);
            }
        }

        return buffer;
    }

    /**
     * 将base64字符串解码为字节数组->普通数组
     */
    export function bytes_decode(str: string, num: number): number[] {
        let arr:number[] = [];
        let base64 = new Uint8Array(decode_rawBuffer(str));
        let view = new DataView(base64.buffer);
        for (var i = 0; i < num; i++) {
            arr.push(view.getFloat64(i * 8));
        }
        return arr;
    }

    /**
     * 将文本转换为utf8编码的文本字符串
    */
    export function utf8_encode(text: string): string {
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
    export function utf8_decode(text: string): string {
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