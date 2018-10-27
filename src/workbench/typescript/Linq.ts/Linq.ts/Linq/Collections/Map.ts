
/**
 * 描述了一个键值对集合
*/
class Map<K, V> {

    /**
     * 键名称，一般是字符串
    */
    public key: K;
    /**
     * 目标键名所映射的值
    */
    public value: V;

    /**
     * 创建一个新的键值对集合
     * 
    */
    public constructor(key: K = null, value: V = null) {
        this.key = key;
        this.value = value;
    }

    public toString(): string {
        return `[${this.key.toString()}, ${this.value.toString()}]`;
    }
}

/**
 * 描述了一个带有名字属性的变量值
*/
class NamedValue<T> {

    /**
     * 变量值的名字属性
    */
    public name: string;
    /**
     * 这个变量值
    */
    public value: T;

    public constructor(name: string = null, val: T = null) {
        this.name = name;
        this.value = val;
    }

    /**
     * 获取得到变量值的类型定义信息
    */
    public get TypeOfValue(): TypeInfo {
        return TypeInfo.typeof(this.value);
    }

    /**
     * 这个之对象是否是空的？
    */
    public get IsEmpty(): boolean {
        return Strings.Empty(this.name) && (!this.value || this.value == undefined);
    }

    public toString(): string {
        return this.name;
    }
}