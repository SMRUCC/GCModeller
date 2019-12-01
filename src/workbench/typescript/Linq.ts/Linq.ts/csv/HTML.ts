namespace csv.HTML {

    const bootstrap: string[] = ["table", "table-hover"];

    /**
     * 将数据框对象转换为HTMl格式的表格对象的html代码
     * 
     * @param tblClass 所返回来的html表格代码之中的table对象的类型默认是bootstrap类型的，
     * 所以默认可以直接应用bootstrap的样式在这个表格之上
     * 
     * @returns 表格的HTML代码
    */
    export function toHTMLTable(data: dataframe, tblClass: string[] = bootstrap): string {
        var th: string = data.headers
            .Select(h => `<th>${h}</th>`)
            .JoinBy("\n");
        var tr: string = data.contents
            .Select(r => r.Select(c => `<td>${c}</td>`).JoinBy(""))
            .Select(r => `<tr>${r}</tr>`)
            .JoinBy("\n");

        return `
            <table class="${tblClass}">
                <thead>
                    <tr>${th}</tr>
                </thead>
                <tbody>
                    ${tr}
                </tbody>
            </table>`;
    }

    export function createHTMLTable<T extends object>(data: IEnumerator<T>, tblClass: string[] = bootstrap): string {
        return toHTMLTable(csv.toDataFrame(data), tblClass);
    }
}