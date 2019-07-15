namespace data.sprintf {

    /**
     * 对占位符的匹配结果
    */
    export class match {

        public match: string;
        public left: boolean;
        public sign: string;
        public pad: string;
        public min: string;
        public precision: string;
        public code: string;
        public negative: boolean;
        public argument: string;

        public toString(): string {
            return JSON.stringify(this);
        }
    }

    /**
     * 格式化占位符
     * 
     * Possible format values:
     *
     * + ``%%`` – Returns a percent sign
     * + ``%b`` – Binary number
     * + ``%c`` – The character according to the ASCII value
     * + ``%d`` – Signed decimal number
     * + ``%f`` – Floating-point number
     * + ``%o`` – Octal number
     * + ``%s`` – String
     * + ``%x`` – Hexadecimal number (lowercase letters)
     * + ``%X`` – Hexadecimal number (uppercase letters)
     * 
     * Additional format values. These are placed between the % and the letter (example %.2f):
     *
     * + ``+``      (Forces both + and – in front of numbers. By default, only negative numbers are marked)
     * + ``–``      (Left-justifies the variable value)
     * + ``0``      zero will be used for padding the results to the right string size
     * + ``[0-9]``  (Specifies the minimum width held of to the variable value)
     * + ``.[0-9]`` (Specifies the number of decimal digits or maximum string length)
     *
    */
    export const placeholder: RegExp = new RegExp(/(%([%]|(\-)?(\+|\x20)?(0)?(\d+)?(\.(\d)?)?([bcdfosxX])))/g);

    /**
     * @param argumentList ERROR - "arguments" cannot be redeclared in strict mode
    */
    export function parseFormat(string: string, argumentList: any[]) {
        var stringPosStart = 0;
        var stringPosEnd = 0;
        var matchPosEnd = 0;
        var convCount: number = 0;
        var match: RegExpExecArray = null;
        var matches: sprintf.match[] = [];
        var strings: string[] = [];

        while (match = placeholder.exec(string)) {
            stringPosStart = matchPosEnd;
            stringPosEnd = placeholder.lastIndex - match[0].length;
            strings[strings.length] = string.substring(stringPosStart, stringPosEnd);

            matchPosEnd = placeholder.lastIndex;
            matches[matches.length] = <sprintf.match>{
                match: match[0],
                left: match[3] ? true : false,
                sign: match[4] || '',
                pad: match[5] || ' ',
                min: match[6] || 0,
                precision: match[8],
                code: match[9] || '%',
                negative: parseInt(argumentList[convCount]) < 0 ? true : false,
                argument: String(argumentList[convCount])
            };

            if (match[9]) {
                convCount += 1;
            }
        }

        strings[strings.length] = string.substring(matchPosEnd);

        return {
            matches: matches,
            convCount: convCount,
            strings: strings
        }
    }

    /**
     * ### Javascript sprintf
     * 
     * > http://www.webtoolkit.info/javascript-sprintf.html#.W5sf9FozaM8
     *  
     * Several programming languages implement a sprintf function, to output a 
     * formatted string. It originated from the C programming language, printf 
     * function. Its a string manipulation function.
     *
     * This is limited sprintf Javascript implementation. Function returns a 
     * string formatted by the usual printf conventions. See below for more details. 
     * You must specify the string and how to format the variables in it.
    */
    export function doFormat(format: string, ...argv: any[]): string {

        if (typeof arguments == "undefined") { return null; }
        if (arguments.length < 1) { return null; }
        if (typeof arguments[0] != "string") { return null; }
        if (typeof RegExp == "undefined") { return null; }

        var parsed = sprintf.parseFormat(format, argv);
        var convCount: number = parsed.convCount;

        if (parsed.matches.length == 0) {
            // 没有格式化参数的占位符，则直接输出原本的字符串
            return format;
        } else {
            // console.log(parsed);
        }

        if (argv.length < convCount) {
            // 格式化参数的数量少于占位符的数量，则抛出错误
            throw `Mismatch format argument numbers (${argv.length} !== ${convCount})!`;
        } else {
            return sprintf.doSubstitute(
                parsed.matches,
                parsed.strings
            );
        }
    }

    /**
     * 进行格式化占位符对格式化参数的字符串替换操作
    */
    export function doSubstitute(matches: sprintf.match[], strings: string[]): string {
        var i: number = null;
        var substitution: string = null;
        var numVal: number = 0;
        var newString = '';

        for (i = 0; i < matches.length; i++) {

            if (matches[i].code == '%') {
                substitution = '%'
            } else if (matches[i].code == 'b') {
                matches[i].argument = String(Math.abs(parseInt(matches[i].argument)).toString(2));
                substitution = sprintf.convert(matches[i], true);
            } else if (matches[i].code == 'c') {
                numVal = Math.abs(parseInt(matches[i].argument));
                matches[i].argument = String(String.fromCharCode(parseInt(String(numVal))));
                substitution = sprintf.convert(matches[i], true);
            } else if (matches[i].code == 'd') {
                matches[i].argument = String(Math.abs(parseInt(matches[i].argument)));
                substitution = sprintf.convert(matches[i]);
            } else if (matches[i].code == 'f') {
                numVal = matches[i].precision ? parseFloat(matches[i].precision) : 6;
                matches[i].argument = String(Math.abs(parseFloat(matches[i].argument)).toFixed(numVal));
                substitution = sprintf.convert(matches[i]);
            } else if (matches[i].code == 'o') {
                matches[i].argument = String(Math.abs(parseInt(matches[i].argument)).toString(8));
                substitution = sprintf.convert(matches[i]);
            } else if (matches[i].code == 's') {
                numVal = matches[i].precision ?
                    parseFloat(matches[i].precision) :
                    matches[i].argument.length;
                matches[i].argument = matches[i].argument.substring(0, numVal);
                substitution = sprintf.convert(matches[i], true);
            } else if (matches[i].code == 'x') {
                matches[i].argument = String(Math.abs(parseInt(matches[i].argument)).toString(16));
                substitution = sprintf.convert(matches[i]);
            } else if (matches[i].code == 'X') {
                matches[i].argument = String(Math.abs(parseInt(matches[i].argument)).toString(16));
                substitution = sprintf.convert(matches[i]).toUpperCase();
            } else {
                substitution = matches[i].match;
            }

            newString += strings[i];
            newString += substitution;
        }

        return newString + strings[i];
    }

    export function convert(match: sprintf.match, nosign: boolean = false): string {
        if (nosign) {
            match.sign = '';
        } else {
            match.sign = match.negative ? '-' : match.sign;
        }

        var l: number = parseFloat(match.min) -
            match.argument.length + 1 -
            match.sign.length;
        var pad = new Array(l < 0 ? 0 : l).join(match.pad);

        if (!match.left) {
            if (match.pad == "0" || nosign) {
                return match.sign + pad + match.argument;
            } else {
                return pad + match.sign + match.argument;
            }
        } else {
            if (match.pad == "0" || nosign) {
                return match.sign + match.argument + pad.replace(/0/g, ' ');
            } else {
                return match.sign + match.argument + pad;
            }
        }
    }
}