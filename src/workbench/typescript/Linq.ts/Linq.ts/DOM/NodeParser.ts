namespace DOM {

    /**
     * 用于解析XML节点之中的属性值的正则表达式
    */
    export const attrs: RegExp = /\S+\s*[=]\s*((["].*["])|(['].*[']))/g;

    /**
     * 将表达式之中的节点名称，以及该节点上面的属性值都解析出来
    */
    export function ParseNodeDeclare(expr: string) {
        // <a href="..." onclick="...">
        var declare: string = expr
            .substr(1, expr.length - 2)
            .trim();
        var tagValue = Strings.GetTagValue(declare, " ");
        var tag: string = tagValue.name;
        var attrs: NamedValue<string>[] = [];

        if (tagValue.value.length > 0) {
            // 使用正则表达式进行解析
            attrs = $from(tagValue.value.match(DOM.attrs))
                .Where(s => s.length > 0)
                .Select(s => {
                    var attr = Strings.GetTagValue(s, "=");
                    var val: string = attr.value.trim();
                    val = val.substr(1, val.length - 2);
                    return new NamedValue(attr.name, val);
                }).ToArray();
        }

        return {
            tag: tag, attrs: attrs
        };
    }
}