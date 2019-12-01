namespace csv {

    /**
     * csv文件之中的一行数据，相当于当前行的列数据的集合
    */
    export class row extends IEnumerator<string> {

        /**
         * 当前的这一个行对象的列数据集合
         * 
         * 注意，你无法通过直接修改这个数组之中的元素来达到修改这个行之中的值的目的
         * 因为这个属性会返回这个行的数组值的复制对象
        */
        public get columns(): string[] {
            return [...this.sequence];
        }

        /**
         * 这个只读属性仅用于生成csv文件
        */
        public get rowLine(): string {
            return $from(this.columns)
                .Select(row.autoEscape)
                .JoinBy(",");
        }

        public constructor(cells: string[] | IEnumerator<string>) {
            super(cells);
        }

        /**
         * Returns the index of the first occurrence of a value in an array.
         * 
         * 函数得到指定的值在本行对象之中的列的编号
         * 
         * @param value The value to locate in the array.
         * @param fromIndex The array index at which to begin the search. If ``fromIndex`` is omitted, 
         *      the search starts at index 0.
         * 
         * @returns 如果这个函数返回-1则表示找不到
        */
        public indexOf(value: string, fromIndex: number = null): number {
            if (isNullOrUndefined(fromIndex)) {
                return this.sequence.indexOf(value);
            } else {
                return this.sequence.indexOf(value, fromIndex);
            }
        }

        public ProjectObject(headers: string[] | IEnumerator<string>): object {
            var obj: object = {};
            var data: string[] = this.columns;

            if (Array.isArray(headers)) {
                headers.forEach((h, i) => {
                    obj[h] = data[i];
                });
            } else {
                headers.ForEach((h, i) => {
                    obj[h] = data[i];
                });
            }

            return obj;
        }

        private static autoEscape(c: string): string {
            if (c.indexOf(",") > -1) {
                return `"${c}"`;
            } else {
                return c;
            }
        }

        public static Parse(line: string): row {
            return new row(csv.CharsParser(line));
        }

        public static ParseTsv(line: string): row {
            return new row(csv.CharsParser(line, "\t"));
        }
    }
}