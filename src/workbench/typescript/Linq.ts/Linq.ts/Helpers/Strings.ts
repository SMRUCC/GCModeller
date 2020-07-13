/// <reference path="../Collections/Map.ts" />

/**
 * TypeScript string helpers.
 * (这个模块之中的大部分的字符串处理函数的行为是和VisualBasic之中的字符串函数的行为是相似的)
*/
module Strings {

    export const x0: number = "0".charCodeAt(0);
    export const x9: number = "9".charCodeAt(0);
    export const asterisk: number = "*".charCodeAt(0);
    export const cr: number = "\c".charCodeAt(0);
    export const lf: number = "\r".charCodeAt(0);
    export const a: number = "a".charCodeAt(0);
    export const z: number = "z".charCodeAt(0);
    export const A: number = "A".charCodeAt(0);
    export const Z: number = "Z".charCodeAt(0);

    export const numericPattern: RegExp = /[-]?\d+(\.\d+)?/g;
    export const integerPattern: RegExp = /[-]?[0-9]+/g;

    export function PadLeft(text: string, padLen: number, c: string = " ") {
        if (Strings.Empty(text)) {
            return DataExtensions.Dim(padLen, c).join("");
        }

        padLen = padLen - text.length;

        if (padLen < 0) {
            return text;
        } else {
            return DataExtensions.Dim(padLen, c).join("") + text;
        }
    }

    /**
     * 判断所给定的字符串文本是否是任意实数的正则表达式模式
    */
    export function isNumericPattern(text: string): boolean {
        return IsPattern(text, Strings.numericPattern);
    }

    export function isIntegerPattern(text: string): boolean {
        return IsPattern(text, Strings.integerPattern);
    }

    /**
     * 尝试将任意类型的目标对象转换为数值类型
     * 
     * @returns 一个数值
    */
    export function as_numeric(obj: any): number {
        return AsNumeric(obj)(obj);
    }

    /**
     * 因为在js之中没有类型信息，所以如果要取得类型信息必须要有一个目标对象实例
     * 所以在这里，函数会需要一个实例对象来取得类型值
    */
    export function AsNumeric<T>(obj: T): (x: T) => number {
        if (obj == null || obj == undefined) {
            return null;
        }

        if (typeof obj === 'number') {
            return x => <number><any>x;
        } else if (typeof obj === 'boolean') {
            return x => {
                if (<boolean><any>x == true) {
                    return 1;
                } else {
                    return -1;
                }
            }
        } else if (typeof obj == 'undefined') {
            return x => 0;
        } else if (typeof obj == 'string') {
            return x => {
                return Strings.Val(<string><any>x);
            }
        } else {
            // 其他的所有情况都转换为零
            return x => 0;
        }
    }

    /**
     * 对bytes数值进行格式自动优化显示
     * 
     * @param bytes 
     * 
     * @return 经过自动格式优化过后的大小显示字符串
    */
    export function Lanudry(bytes: number): string {
        var symbols = ["B", "KB", "MB", "GB", "TB"];
        var exp = Math.floor(Math.log(bytes) / Math.log(1000));
        var symbol: string = symbols[exp];
        var val = (bytes / Math.pow(1000, Math.floor(exp)));

        return sprintf(`%.2f ${symbol}`, val);
    }

    /**
     * how to escape xml entities in javascript?
     * 
     * > https://stackoverflow.com/questions/7918868/how-to-escape-xml-entities-in-javascript
    */
    export function escapeXml(unsafe: string): string {
        return unsafe.replace(/[<>&'"]/g, function (c) {
            switch (c) {
                case '<': return '&lt;';
                case '>': return '&gt;';
                case '&': return '&amp;';
                case '\'': return '&apos;';
                case '"': return '&quot;';
            }
        });
    }

    /**
     * 这个函数会将字符串起始的数字给匹配出来
     * 如果匹配失败会返回零
     * 
     * 与VB之中的val函数的行为相似，但是这个函数返回整形数
     * 
     * @param text 这个函数并没有执行trim操作，所以如果字符串的起始为空白符的话
     *     会导致解析结果为零
    */
    export function parseInt(text: string): number {
        var number: string[];
        var c: string;
        var ascii: number;

        if (Strings.Empty(text, true)) {
            return 0;
        } else {
            number = [];
        }

        for (var i: number = 0; i < text.length; i++) {
            c = text.charAt(i);
            ascii = c.charCodeAt(0);

            if (ascii >= x0 && ascii <= x9) {
                number.push(c);
            } else {
                break;
            }
        }

        if (number.length == 0) {
            return 0;
        } else {
            return Number(number.join(""));
        }
    }

    /**
     * Create new string value by repeats a given char n times.
     * 
     * @param c A single char
     * @param n n chars
    */
    export function New(c: string, n: number): string {
        if (n == 0) {
            return "";
        } else if (n == 1) {
            return c;
        } else {
            var s: string = "";

            for (var i: number = 1; i < n; ++i) {
                s = s + c;
            }

            return s;
        }
    }

    /**
     * Round the number value or number text in given decimals.
     * 
     * @param decimals 默认是保留3位有效数字的
    */
    export function round(x: number | string, decimals: number = 3) {
        var floatX = typeof x == "number" ? x : parseFloat(x);
        var n: number = Math.pow(10, decimals);

        if (isNaN(floatX)) {
            if (TypeScript.logging.outputWarning) {
                console.warn(`Invalid number value: '${x}'`);
            }
            return false;
        } else {
            return Math.round(floatX * n) / n;
        }
    }

    /**
     * 判断当前的这个字符是否是一个数字？
     * 
     * @param c A single character, length = 1
    */
    export function isNumber(c: string): boolean {
        var code = c.charCodeAt(0);
        return code >= x0 && code <= x9;
    }

    /**
     * 判断当前的这个字符是否是一个字母？
     * 
     * @param c A single character, length = 1
    */
    export function isAlphabet(c: string): boolean {
        var code = c.charCodeAt(0);
        return (code >= a && code <= z) || (code >= A && code <= Z);
    }

    /**
     * 将字符串转换为一个实数
     * 这个函数是直接使用parseFloat函数来工作的，如果不是符合格式的字符串，则可能会返回NaN
    */
    export function Val(str: string): number {
        if (str == null || str == '' || str == undefined || str == "undefined") {
            // 将空字符串转换为零
            return 0;
        } else if (str == "NA" || str == "NaN") {
            return Number.NaN;
        } else if (str == "Inf") {
            return Number.POSITIVE_INFINITY;
        } else if (str == "-Inf") {
            return Number.NEGATIVE_INFINITY;
        } else {
            return parseFloat(str);
        }
    }

    /**
     * 将文本字符串按照newline进行分割
    */
    export function lineTokens(text: string): string[] {
        return (!text) ? <string[]>[] : text.trim().split("\n");
    }

    /**
     * 如果不存在``tag``分隔符，则返回来的``tuple``里面，``name``是输入的字符串，``value``则是空字符串
     * 
     * @param tag 分割name和value的分隔符，默认是一个空白符号
    */
    export function GetTagValue(str: string, tag: string = " "): NamedValue<string> {
        if (!str) {
            return new NamedValue<string>();
        } else {
            return tagValueImpl(str, tag);
        }
    }

    function tagValueImpl(str: string, tag: string): NamedValue<string> {
        var i: number = str.indexOf(tag);
        var tagLen: number = Len(tag);

        if (i > -1) {
            var name: string = str.substr(0, i);
            var value: string = str.substr(i + tagLen);

            return new NamedValue<string>(name, value);
        } else {
            return new NamedValue<string>(str, "");
        }
    }

    /**
     * 取出大文本之中指定的前n行文本
    */
    export function PeekLines(text: string, n: number): string[] {
        let p: number = 0;
        let out: string[] = [];

        for (var i: number = 0; i < n; i++) {
            let pn = text.indexOf("\n", p);

            if (pn > -1) {
                out.push(text.substr(p, pn - p));
                p = pn;
            } else {
                // 已经到头了
                break;
            }
        }

        return out;
    }

    export function LCase(str: string): string {
        if (isNullOrUndefined(str)) {
            return "";
        } else {
            return str.toLowerCase();
        }
    }

    export function UCase(str: string): string {
        if (isNullOrUndefined(str)) {
            return "";
        } else {
            return str.toUpperCase();
        }
    }

    /**
     * Get all regex pattern matches in target text value.
    */
    export function getAllMatches(text: string, pattern: string | RegExp) {
        let match: RegExpExecArray = null;
        let out: RegExpExecArray[] = [];

        if (typeof pattern == "string") {
            pattern = new RegExp(pattern);
        }

        if (pattern.global) {
            while (match = pattern.exec(text)) {
                out.push(match);
            }
        } else {
            if (match = pattern.exec(text)) {
                out.push(match);
            }
        }

        return out;
    }

    /**
     * Removes the given chars from the begining of the given 
     * string and the end of the given string.
     * 
     * @param chars A collection of characters that will be trimmed.
     *    (如果这个参数为空值，则会直接使用字符串对象自带的trim函数来完成工作)
     *    
     * @returns 这个函数总是会确保返回来的值不是空值，如果输入的字符串参数为空值，则会直接返回零长度的空字符串
    */
    export function Trim(str: string, chars: string | number[] = null): string {
        if (Strings.Empty(str, false)) {
            return "";
        } else if (isNullOrUndefined(chars)) {
            return str.trim();
        }

        if (typeof chars == "string") {
            chars = $from(<string[]>Strings.ToCharArray(chars))
                .Select(c => c.charCodeAt(0))
                .ToArray(false);
        }

        return function (chars: number[]) {
            return $from(<string[]>Strings.ToCharArray(str))
                .SkipWhile(c => chars.indexOf(c.charCodeAt(0)) > -1)
                .Reverse()
                .SkipWhile(c => chars.indexOf(c.charCodeAt(0)) > -1)
                .Reverse()
                .JoinBy("");
        }(<number[]>chars);
    }

    export function LTrim(str: string, chars: string | number[] = " "): string {
        if (Strings.Empty(str, false)) {
            return "";
        }

        if (typeof chars == "string") {
            chars = $from(<string[]>Strings.ToCharArray(chars))
                .Select(c => c.charCodeAt(0))
                .ToArray(false);
        }

        return function (chars: number[]) {
            return $from(<string[]>Strings.ToCharArray(str))
                .SkipWhile(c => chars.indexOf(c.charCodeAt(0)) > -1)
                .JoinBy("");
        }(<number[]>chars);
    }

    export function RTrim(str: string, chars: string | number[] = " "): string {
        if (Strings.Empty(str, false)) {
            return "";
        }

        if (typeof chars == "string") {
            chars = $from(<string[]>Strings.ToCharArray(chars))
                .Select(c => c.charCodeAt(0))
                .ToArray(false);
        }

        var strChars: string[] = <string[]>Strings.ToCharArray(str);
        var lefts: number = 0;

        for (var i: number = strChars.length - 1; i > 0; i--) {
            if (chars.indexOf(strChars[i].charCodeAt(0)) == -1) {
                lefts = i;
                break;
            }
        }

        if (lefts == 0) {
            return "";
        } else {
            return str.substr(0, lefts + 1);
        }
    }

    /**
     * Determine that the given string is empty string or not?
     * (判断给定的字符串是否是空值？)
     * 
     * @param stringAsFactor 假若这个参数为真的话，那么字符串``undefined``或者``NULL``以及``null``也将会被当作为空值处理
    */
    export function Empty(str: string, stringAsFactor = false): boolean {
        if (!str) {
            return true;
        } else if (str == undefined || typeof str == "undefined") {
            return true;
        } else if (str.length == 0) {
            return true;
        } else if (stringAsFactor && (str == "undefined" || str == "null" || str == "NULL")) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * 测试字符串是否是空白集合
     * 
     * @param stringAsFactor 如果这个参数为真，则``\t``和``\s``等也会被当作为空白
    */
    export function Blank(str: string, stringAsFactor = false): boolean {
        if (!str || IsPattern(str, /\s+/g)) {
            return true;
        } else if (str == undefined || typeof str == "undefined") {
            return true;
        } else if (str.length == 0) {
            return true;
        } else if (stringAsFactor && (str == "\\s" || str == "\\t")) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * Determine that the whole given string is match a given regex pattern. 
    */
    export function IsPattern(str: string, pattern: RegExp | string): boolean {
        if (!str) {
            // 字符串是空的，则肯定不满足
            return false;
        }

        var matches = str.match(ensureRegexp(pattern));

        if (isNullOrUndefined(matches)) {
            return false;
        } else {
            var match: string = matches[0];
            var test: boolean = match == str;

            return test;
        }
    }

    function ensureRegexp(pattern: RegExp | string): RegExp {
        if (typeof pattern == "string") {
            return new RegExp(pattern);
        } else {
            return pattern;
        }
    }

    /**
     * Remove duplicate string values from JS array
     * 
     * https://stackoverflow.com/questions/9229645/remove-duplicate-values-from-js-array
    */
    export function Unique(a: string[]): string[] {
        var seen = {};

        return a.filter(function (item) {
            return seen.hasOwnProperty(item) ? false : (seen[item] = true);
        });
    }

    /**
     * Count char numbers appears in the given string value
    */
    export function Count(str: string, c: string): number {
        let counts = 0;

        for (let chr of str) {
            if (chr == c) {
                counts = counts + 1;
            }
        }

        return counts;
    }

    /**
     * 将字符串转换为字符数组
     * 
     * @description > https://jsperf.com/convert-string-to-char-code-array/9
     *    经过测试，使用数组push的效率最高
     *    
     * @param charCode 返回来的数组是否应该是一组字符的ASCII值而非字符本身？默认是返回字符数组的。 
     *    
     * @returns A character array, all of the string element in the array 
     *      is one character length.
    */
    export function ToCharArray(str: string, charCode: boolean = false): string[] | number[] {
        var cc: string[] | number[] = [];
        var strLen: number = str.length;

        if (charCode) {
            for (var i = 0; i < strLen; ++i) {
                (<number[]>cc).push(str.charCodeAt(i));
            }
        } else {
            for (var i = 0; i < strLen; ++i) {
                (<string[]>cc).push(str.charAt(i));
            }
        }

        return cc;
    }

    /**
     * Measure the string length, a null string value or ``undefined`` 
     * variable will be measured as ZERO length.
    */
    export function Len(s: string): number {
        if (!s || s == undefined) {
            return 0;
        } else {
            return s.length;
        }
    }

    /**
     * 比较两个字符串的大小，可以同于字符串的分组操作
    */
    export function CompareTo(s1: string, s2: string): number {
        var l1 = Strings.Len(s1);
        var l2 = Strings.Len(s2);
        var minl = Math.min(l1, l2);

        for (var i: number = 0; i < minl; i++) {
            var x = s1.charCodeAt(i);
            var y = s2.charCodeAt(i);

            if (x > y) {
                return 1;
            } else if (x < y) {
                return -1;
            }
        }

        if (l1 > l2) {
            return 1;
        } else if (l1 < l2) {
            return -1;
        } else {
            return 0;
        }
    }

    export const sprintf = data.sprintf.doFormat;

    /**
     * @param charsPerLine 每一行文本之中的字符数量的最大值
    */
    export function WrappingLines(text: string, charsPerLine: number = 200, lineTrim: boolean = false): string {
        var sb: string = "";
        var lines: string[] = Strings.lineTokens(text);
        var p: number;

        for (var i: number = 0; i < lines.length; i++) {
            var line: string = lineTrim ? Strings.Trim(lines[i]) : lines[i];

            if (line.length < charsPerLine) {
                sb = sb + line + "\n";
            } else {
                p = 0;

                while (true) {
                    sb = sb + line.substr(p, charsPerLine) + "\n";
                    p += charsPerLine;

                    if ((p + charsPerLine) > line.length) {
                        // 下一个起始的位置已经超过文本行的长度了
                        // 则是终止的时候了
                        sb = sb + line.substr(p) + "\n";
                        break;
                    }
                }
            }
        }

        return sb;
    }
}