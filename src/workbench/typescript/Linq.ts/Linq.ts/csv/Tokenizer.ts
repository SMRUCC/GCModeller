/// <reference path="../Collections/Pointer.ts" />

namespace csv {

    /**
     * 通过Chars枚举来解析域，分隔符默认为逗号
     * > https://github.com/xieguigang/sciBASIC/blame/701f9d0e6307a779bb4149c57a22a71572f1e40b/Data/DataFrame/IO/csv/Tokenizer.vb#L97
     * 
    */
    export function CharsParser(s: string, delimiter: string = ",", quot: string = '"'): string[] {
        var tokens: string[] = [];
        var temp: string[] = [];
        var openStack: boolean = false;
        var buffer: Pointer<string> = From(<string[]>Strings.ToCharArray(s)).ToPointer();
        var dblQuot: RegExp = new RegExp(`[${quot}]{2}`, 'g');
        var cellStr = function () {
            // https://stackoverflow.com/questions/1144783/how-to-replace-all-occurrences-of-a-string-in-javascript
            // 2018-09-02
            // 如果join函数的参数是空的话，则js之中默认是使用逗号作为连接符的 
            return temp.join("").replace(dblQuot, quot);
        }
        var procEscape = function (c: string) {
            if (!StartEscaping(temp)) {

                // 查看下一个字符是否为分隔符
                // 因为前面的 Dim c As Char = +buffer 已经位移了，所以在这里直接取当前的字符
                var peek = buffer.Current;
                // 也有可能是 "" 转义 为单个 "
                var lastQuot = (temp.length > 0 && temp[temp.length - 1] != quot);

                if (temp.length == 0 && peek == delimiter) {
                    // openStack意味着前面已经出现一个 " 了
                    // 这里又出现了一个 " 并且下一个字符为分隔符
                    // 则说明是 "", 当前的cell内容是一个空字符串
                    tokens.push("");
                    temp = [];
                    buffer.MoveNext();
                    openStack = false;
                } else if ((peek == delimiter || buffer.EndRead) && lastQuot) {
                    // 下一个字符为分隔符，则结束这个token
                    tokens.push(cellStr());
                    temp = [];
                    // 跳过下一个分隔符，因为已经在这里判断过了
                    buffer.MoveNext();
                    openStack = false;
                } else {
                    // 不是，则继续添加
                    temp.push(c);
                }

            } else {
                // \" 会被转义为单个字符 "
                temp[temp.length - 1] = c;
            }
        }

        while (!buffer.EndRead) {
            var c: string = buffer.Next;

            if (openStack) {
                if (c == quot) {
                    procEscape(c);
                } else {
                    // 由于双引号而产生的转义          
                    temp.push(c);
                }
            } else {
                if (temp.length == 0 && c == quot) {
                    // token的第一个字符串为双引号，则开始转义
                    openStack = true;
                } else {
                    if (c == delimiter) {
                        tokens.push(cellStr());
                        temp = [];
                    } else {
                        temp.push(c);
                    }
                }
            }
        }

        if (temp.length > 0) {
            tokens.push(cellStr());
        }

        return tokens;
    }

    /**
     * 当前的token对象之中是否是转义的起始，即当前的token之中的最后一个符号
     * 是否是转义符<paramref name="escape"/>?
    */
    function StartEscaping(buffer: string[], escape: string = "\\"): boolean {
        if (IsNullOrEmpty(buffer)) {
            return false;
        } else {
            return buffer[buffer.length - 1] == escape;
        }
    }
}