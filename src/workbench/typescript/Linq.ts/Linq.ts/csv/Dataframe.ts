/// <reference path="../Collections/Abstract/Enumerator.ts" />

/**
 * http://www.rfc-editor.org/rfc/rfc4180.txt
*/
namespace csv {

    /**
     * Common Format and MIME Type for Comma-Separated Values (CSV) Files
    */
    const contentType: string = "text/csv";

    /**
     * ``csv``文件模型
    */
    export class dataframe extends IEnumerator<csv.row> {

        /**
         * Csv文件的第一行作为header
        */
        public get headers(): IEnumerator<string> {
            return new IEnumerator<string>(this.sequence[0]);
        }

        /**
         * 获取除了第一行作为``header``数据的剩余的所有的行数据
        */
        public get contents(): IEnumerator<row> {
            return this.Skip(1);
        }

        /**
         * 从行序列之中构建出一个csv对象模型
        */
        public constructor(rows: row[] | IEnumerator<row>) {
            super(rows);
        }

        /**
         * 获取指定列名称的所有的行的列数据
         * 
         * @param name csv文件的列名称，第一行之中的文本数据的内容
         * 
         * @returns 该使用名称所指定的列的所有的内容字符串的枚举序列对象
        */
        public Column(name: string): IEnumerator<string> {
            var index: number = this.sequence[0].indexOf(name);

            if (index == -1) {
                return new IEnumerator<string>([]);
            } else {
                return this.Select(r => r.ElementAt(index));
            }
        }

        /**
         * 向当前的数据框对象之中添加一行数据
        */
        public AppendLine(line: row): dataframe {
            this.sequence.push(line);
            return this;
        }

        /**
         * 向当前的数据框对象之中添加多行数据
        */
        public AppendRows(data: IEnumerator<row> | row[]): dataframe {
            if (Array.isArray(data)) {
                data.forEach(r => this.sequence.push(r));
            } else {
                data.ForEach(r => this.sequence.push(r));
            }

            return this;
        }

        /**
         * 将当前的这个数据框对象转换为csv文本内容
        */
        public buildDoc(): string {
            return this.Select(r => r.rowLine).JoinBy("\n");
        }

        /**
         * 使用反射操作将csv文档转换为特定类型的对象数据序列
         * 
         * @param fieldMaps 这个参数是一个对象，其描述了如何将csv文档之中在js之中
         *     的非法标识符转换为合法的标识符的映射
         * @param activator 这个函数指针描述了如何创建一个新的指定类型的对象的过程，
         *     这个函数指针不可以有参数的传递。
         *     
         * @returns 这个函数返回类型约束的对象Linq序列集合
        */
        public Objects<T>(
            fieldMaps: object = {},
            activator: () => T = () => {
                return <T>{};
            }): IEnumerator<T> {

            var header = dataframe.ensureMapsAll(fieldMaps, this.headers.ToArray());
            var objs: IEnumerator<T> = this
                .Skip(1)
                .Select<T>(r => {
                    var o: any = activator();

                    r.ForEach((c, i) => {
                        o[header(i)] = c;
                    });

                    return <T>o;
                });

            return objs;
        }

        private static ensureMapsAll(fieldMaps: object, headers: string[]): (i: number) => string {
            for (var i = 0; i < headers.length; i++) {
                var column: string = headers[i];

                if (column in fieldMaps) {
                    // do nothing
                } else {
                    // fill gaps
                    fieldMaps[column] = column;
                }
            }

            return function (i: number) {
                return <string>fieldMaps[headers[i]];
            }
        }

        /**
         * 使用ajax将csv文件保存到服务器
         * 
         * @param url csv文件数据将会被通过post方法保存到这个url所指定的网络资源上面
         * @param callback ajax异步回调，默认是打印返回结果到终端之上
         * 
        */
        public save(url: string, fileName: string = "upload.csv",
            callback: (response: string) => void =
                (response: string) => {
                    console.log(response);
                }): void {

            var file: string = this.buildDoc();

            HttpHelpers.UploadFile(url, file, fileName, callback);
        }

        /**
         * 使用ajax GET加载csv文件数据，不推荐使用这个方法处理大型的csv文件数据
         * 
         * @param callback 当这个异步回调为空值的时候，函数使用同步的方式工作，返回csv对象
         *                 如果这个参数不是空值，则以异步的方式工作，此时函数会返回空值
         * @param parseText 如果url返回来的数据之中还包含有其他的信息，则会需要这个参数来进行csv文本数据的解析
        */
        public static Load(url: string,
            callback: (csv: dataframe) => void = null,
            parseText: (response: string, contentType?: string) => content = this.defaultContent): dataframe {

            if (callback == null || callback == undefined) {
                // 同步
                var load: content = parseText(HttpHelpers.GET(url));
                var tsv: boolean = this.isTsv(load);

                return dataframe.Parse(load.content, tsv);
            } else {
                // 异步
                HttpHelpers.GetAsyn(url, (text, code, contentType) => {
                    if (code == 200) {
                        var load: content = parseText(text, contentType);
                        var tsv: boolean = this.isTsv(load);
                        var data: dataframe = dataframe.Parse(load.content, tsv);

                        console.log(data.headers);

                        callback(data);
                    } else {
                        throw `Error while load csv data source, http ${code}: ${text}`;
                    }
                });
            }

            return null;
        }

        private static isTsv(load: content): boolean {
            var type: string = load.type.trim();
            var tsv: boolean = (type == "tsv") || (type == "#tsv");

            return tsv;
        }

        /**
         * 默认是直接加个csv标签将格式设为默认的csv文件
        */
        private static defaultContent(content: string): content {
            return {
                type: "csv",
                content: content
            };
        }

        /**
         * 将所给定的文本文档内容解析为数据框对象
         * 
         * @param tsv 所需要进行解析的文本内容是否为使用``<TAB>``作为分割符的tsv文本文件？
         *   默认不是，即默认使用逗号``,``作为分隔符的csv文本文件。
        */
        public static Parse(text: string, tsv: boolean = false): dataframe {
            var parse: (line: string) => row = tsv ? row.ParseTsv : row.Parse;
            var allTextLines: IEnumerator<string> = $ts.from(text.split(/\n/));
            var rows: IEnumerator<row>;

            if (Strings.Empty(allTextLines.Last)) {
                // 2019-1-2 因为文本文件很有可能是以空行结尾的
                // 所以在这里需要做下额外的判断
                // 否则会在序列的最后面出现一行空数据
                // 这个空数据很有可能会对下游程序代码产生bug影响
                rows = allTextLines
                    .Take(allTextLines.Count - 1)
                    .Select(parse);

            } else {
                rows = allTextLines.Select(parse);
            }

            return new dataframe(rows);
        }
    }

    export interface content {
        /**
         * 文档的类型为``csv``还是``tsv``
        */
        type: string;
        content: string;
    }
}