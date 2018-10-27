/**
 * TypeScript string helpers
*/
module Strings {

    export const x0: number = "0".charCodeAt(0);
    export const x9: number = "9".charCodeAt(0);
    export const numericPattern: RegExp = /[-]?\d+(\.\d+)?/g;

    /**
     * 判断所给定的字符串文本是否是任意实数的正则表达式模式
    */
    export function isNumericPattern(text: string): boolean {
        return IsPattern(text, Strings.numericPattern);
    }

    /**
     * @param text A single character
    */
    export function isNumber(text: string): boolean {
        var code = text.charCodeAt(0);
        return code >= x0 && code <= x9;
    }

    /**
     * 将字符串转换为一个实数
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
     * Removes the given chars from the begining of the given 
     * string and the end of the given string.
     * 
     * @param chars A collection of characters that will be trimmed.
    */
    export function Trim(str: string, chars: string | number[]): string {
        if (typeof chars == "string") {
            chars = From(Strings.ToCharArray(chars))
                .Select(c => c.charCodeAt(0))
                .ToArray();
        }

        return function (chars: number[]) {
            return From(Strings.ToCharArray(str))
                .SkipWhile(c => chars.indexOf(c.charCodeAt(0)) > -1)
                .Reverse()
                .SkipWhile(c => chars.indexOf(c.charCodeAt(0)) > -1)
                .Reverse()
                .JoinBy("");
        }(<number[]>chars);
    }

    /**
     * Determine that the given string is empty string or not?
     * (判断给定的字符串是否是空值？)
     * 
     * @param stringAsFactor 假若这个参数为真的话，那么字符串``undefined``也将会被当作为空值处理
    */
    export function Empty(str: string, stringAsFactor = false): boolean {
        if (!str) {
            return true;
        } else if (str == undefined) {
            return true;
        } else if (str.length == 0) {
            return true;
        } else if (stringAsFactor && str.toString() == "undefined") {
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
    export function uniq(a: string[]): string[] {
        var seen = {};

        return a.filter(function (item) {
            return seen.hasOwnProperty(item) ? false : (seen[item] = true);
        });
    }

    /**
     * 将字符串转换为字符数组
     * 
     * @description > https://jsperf.com/convert-string-to-char-code-array/9
     *    经过测试，使用数组push的效率最高
     *    
     * @returns A character array, all of the string element in the array 
     *      is one character length.
    */
    export function ToCharArray(str: string): string[] {
        var cc: string[] = [];
        var strLen: number = str.length;

        for (var i = 0; i < strLen; ++i) {
            cc.push(str.charAt(i));
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
}