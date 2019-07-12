/**
 * 类似于反射类型
*/
class TypeInfo {

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
    public get IsPrimitive(): boolean {
        return !this.class;
    }

    /**
     * 是否是一个数组集合对象？
    */
    public get IsArray(): boolean {
        return this.typeOf == "array";
    }

    /**
     * 是否是一个枚举器集合对象？
    */
    public get IsEnumerator(): boolean {
        return this.typeOf == "object" && this.class == "IEnumerator";
    }

    /**
     * 当前的对象是某种类型的数组集合对象
    */
    public IsArrayOf(genericType: string): boolean {
        return this.IsArray && this.class == genericType;
    }

    /**
     * 获取得到类型名称
    */
    public static getClass(obj: any): string {
        var type = typeof obj;
        var isObject: boolean = type == "object";
        var isArray: boolean = Array.isArray(obj);
        var isNull: boolean = isNullOrUndefined(obj);

        return TypeInfo.getClassInternal(obj, isArray, isObject, isNull);
    }

    private static getClassInternal(obj: any, isArray: boolean, isObject: boolean, isNull: boolean): string {
        if (isArray) {
            var x = (<any>obj)[0];
            var className: string;

            if ((className = typeof x) == "object") {
                className = x.constructor.name;
            } else {
                // do nothing
            }

            return className;
        } else if (isObject) {
            if (isNull) {
                if (TypeScript.logging.outputWarning) {
                    console.warn(TypeExtensions.objectIsNothing);
                }

                return "null";
            } else {
                return (<any>obj.constructor).name;
            }
        } else {
            return "";
        }
    }

    /**
     * 获取某一个对象的类型信息
    */
    public static typeof<T>(obj: T): TypeInfo {
        var type = typeof obj;
        var isObject: boolean = type == "object";
        var isArray: boolean = Array.isArray(obj);
        var isNull: boolean = isNullOrUndefined(obj);
        var typeInfo: TypeInfo = new TypeInfo;
        var className: string = TypeInfo.getClassInternal(obj, isArray, isObject, isNull);

        typeInfo.typeOf = isArray ? "array" : type;
        typeInfo.class = className;

        if (isNull) {
            typeInfo.property = [];
            typeInfo.methods = [];
        } else {
            typeInfo.property = isObject ? Object.keys(obj) : [];
            typeInfo.methods = TypeInfo.GetObjectMethods(obj);
        }

        return typeInfo;
    }

    /**
     * 获取object对象上所定义的所有的函数
    */
    public static GetObjectMethods<T>(obj: T): string[] {
        var res: string[] = [];

        for (var m in obj) {
            if (typeof obj[m] == "function") {
                res.push(m)
            }
        }

        return res;
    }

    public toString() {
        if (this.typeOf == "object") {
            return `<${this.typeOf}> ${this.class}`;
        } else {
            return this.typeOf;
        }
    }   

    /**
     * MetaReader对象和字典相似，只不过是没有类型约束，并且为只读集合
    */
    public static CreateMetaReader<V>(nameValues: NamedValue<V>[] | IEnumerator<NamedValue<V>>): TypeScript.Data.MetaReader {
        return new TypeScript.Data.MetaReader(Activator.CreateObject(nameValues));        
    }
}