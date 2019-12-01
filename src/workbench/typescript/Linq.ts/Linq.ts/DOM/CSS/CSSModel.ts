namespace DOM.CSS {

    export interface ICSS {
        selector: string;
        styles: NamedValue<string>[];
    }

    /**
     * 解析选择器所对应的样式配置信息
    */
    export function parseStylesheet(content: string): NamedValue<string>[] {
        let i = new Pointer<string>(CharEnumerator(content));
        let stylesheet: NamedValue<string>[] = [];
        let buf: string[] = [];
        let name: string;
        let value: string;
        let c: string;

        while (!i.EndRead) {
            c = i.Current;

            i.MoveNext();

            if (c == ":") {
                name = buf.join("").trim();
                buf = [];
            } else if (c == ";") {
                value = buf.join("").trim();
                buf = [];

                stylesheet.push(new NamedValue(name, value));
            } else {
                buf.push(c);
            }
        }

        if (buf.length > 0) {
            stylesheet.push(new NamedValue(name, buf.join("")));
        }

        return stylesheet;
    }
}