class StringBuilder {

    private buffer: string;
    private newLine: string;

    /**
     * 返回得到当前的缓冲区的字符串数据长度大小
    */
    public get Length(): number {
        return this.buffer.length;
    }

    /**
     * @param newLine 换行符的文本，默认为纯文本格式，也可以指定为html格式的换行符``<br />``
    */
    public constructor(str: string | StringBuilder = null, newLine: string = "\n") {
        if (!str) {
            this.buffer = "";
        } else if (typeof str == "string") {
            this.buffer = "" + str;
        } else {
            this.buffer = "" + str.buffer;
        }

        this.newLine = newLine;
    }

    /**
     * 向当前的缓冲之中添加目标文本
    */
    public Append(text: string): StringBuilder {
        this.buffer = this.buffer + text;
        return this;
    }

    /**
     * 向当前的缓冲之中添加目标文本病在最末尾添加一个指定的换行符
    */
    public AppendLine(text: string = ""): StringBuilder {
        return this.Append(text + this.newLine);
    }

    public toString(): string {
        return this.buffer + "";
    }
}