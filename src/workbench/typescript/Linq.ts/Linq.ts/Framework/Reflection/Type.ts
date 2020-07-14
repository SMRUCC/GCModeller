namespace TypeScript.Reflection {

    /**
     * 类似于反射类型
    */
    export class TypeInfo {

        /**
         * 直接使用系统内置的``typeof``运算符得到的结果
         * 
         * This property have one of the values in these strings: 
         * ``string object|string|number|boolean|symbol|undefined|function|array``
        */
        public typeOf: string;

        /**
         * 类型class的名称，例如TypeInfo, IEnumerator等。
         * 如果这个属性是空的，则说明是js之中的基础类型
        */
        public class: string;
        public namespace: string;

        /**
         * class之中的字段域列表
        */
        public property: string[];
        /**
         * 函数方法名称列表
        */
        public methods: string[];

        /**
         * 是否是js之中的基础类型？
        */
        public get isPrimitive(): boolean {
            return !this.class;
        }

        /**
         * 是否是一个数组集合对象？
        */
        public get isArray(): boolean {
            return this.typeOf == "array";
        }

        /**
         * 是否是一个枚举器集合对象？
        */
        public get isEnumerator(): boolean {
            return this.typeOf == "object" && ((this.class == "IEnumerator" || this.class == "DOMEnumerator") || Internal.isEnumeratorSignature(this));
        }

        /**
         * 当前的对象是某种类型的数组集合对象
        */
        public isArrayOf(genericType: string): boolean {
            return this.isArray && this.class == genericType;
        }

        public toString() {
            if (this.typeOf == "object") {
                return `<${this.typeOf}> ${this.class}`;
            } else {
                return this.typeOf;
            }
        }
    }
}