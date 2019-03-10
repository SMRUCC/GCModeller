class StringBuilder {

    private buffer: string;
    private newLine: string;

    public get Length(): number {
        return this.buffer.length;
    }

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

    public Append(text: string): StringBuilder {
        this.buffer = this.buffer + text;
        return this;
    }

    public AppendLine(text: string = ""): StringBuilder {
        return this.Append(text + this.newLine);
    }

    public toString(): string {
        return this.buffer + "";
    }
}