namespace TypeScript.Data {

    /**
     * 这个对象可以自动的将调用者的函数名称作为键名进行对应的键值的读取操作
    */
    export class MetaReader {

        /**
         * 字典对象
         * 
         * > 在这里不使用Dictionary对象是因为该对象为一个强类型约束对象
        */
        private readonly meta: object;

        public constructor(meta: object) {
            this.meta = meta;
        }

        /**
         * Read meta object value by call name
         * 
         * > https://stackoverflow.com/questions/280389/how-do-you-find-out-the-caller-function-in-javascript
        */
        public GetValue(key: string = null): any {
            if (!key) {
                key = Internal.StackTrace.GetCallerMember().memberName;
            }

            if (key in this.meta) {
                return this.meta[key];
            } else {
                return null;
            }
        }
    }
}