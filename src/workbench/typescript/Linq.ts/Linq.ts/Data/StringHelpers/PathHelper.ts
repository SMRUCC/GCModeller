namespace TypeScript {

    /**
     * String helpers for the file path string.
    */
    export module PathHelper {

        /**
         * 只保留文件名（已经去除了文件夹路径以及文件名最后的拓展名部分）
        */
        export function basename(fileName: string): string {
            var nameTokens: string[] = $from(Strings.RTrim(fileName, "/").split("/")).Last.split(".");

            if (nameTokens.length == 1) {
                return nameTokens[0];
            }

            var name: string = new IEnumerator<string>(nameTokens)
                .Take(nameTokens.length - 1)
                .JoinBy(".");

            return name;
        }

        export function extensionName(fileName: string): string {
            var nameTokens: string[] = $from(Strings.RTrim(fileName, "/").split("/")).Last.split(".");

            if (nameTokens.length == 1) {
                // 没有拓展名
                return "";
            } else {
                return nameTokens[nameTokens.length - 1];
            }
        }

        /**
         * 函数返回文件名或者文件夹的名称
        */
        export function fileName(path: string): string {
            return $from(Strings.RTrim(path, "/").split("/")).Last;
        }
    }
}