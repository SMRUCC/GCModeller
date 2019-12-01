namespace csv {

    /**
     * 将对象序列转换为``dataframe``对象
     * 
     * 这个函数只能够转换object类型的数据，对于基础类型将不保证能够正常工作
     * 
     * @param data 因为这个对象序列对象是具有类型约束的，所以可以直接从第一个
     *    元素对象之中得到所有的属性名称作为csv文件头的数据
    */
    export function toDataFrame<T extends object>(data: IEnumerator<T> | T[]): dataframe {
        var seq: IEnumerator<T> = Array.isArray(data) ? new IEnumerator<T>(data) : data;
        var header: IEnumerator<string> = $ts(Object.keys(seq.First));
        var rows: IEnumerator<row> = seq
            .Select(obj => {
                var columns: IEnumerator<string> = header
                    .Select((ref, i) => {
                        return toString(obj[ref]);
                    });

                return new row(columns);
            });

        return new dataframe([new row(header)]).AppendRows(rows);
    }

    function toString(obj: any): string {
        if (isNullOrUndefined(obj)) {
            // 这个对象值是空的，所以在csv文件之中是空字符串
            return "";
        } else {
            return "" + obj;
        }
    }

    export function isTsvFile(content: string): boolean {
        let lines: string[] = Strings.lineTokens(content);
        let countTab = $from(lines).Select(l => Strings.Count(l, "\t")).Average();
        let countComma = $from(lines).Select(l => Strings.Count(l, ",")).Average();

        return countTab >= countComma;
    }
}