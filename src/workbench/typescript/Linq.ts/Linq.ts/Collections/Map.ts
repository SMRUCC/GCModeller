
/**
 * 描述了一个键值对集合
*/
class MapTuple<K, V> {

    /**
     * 创建一个新的键值对集合
     * 
     * @param key 键名称，一般是字符串
     * @param value 目标键名所映射的值
    */
    public constructor(public key: K = null, public value: V = null) {
        this.key = key;
        this.value = value;
    }

    public valueOf(): V {
        return this.value;
    }

    public ToArray(): any[] {
        return [this.key, this.value];
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
     * 获取得到变量值的类型定义信息
    */
    public get TypeOfValue(): TypeScript.Reflection.TypeInfo {
        return $ts.typeof(<any>this.value);
    }

    /**
     * 这个之对象是否是空的？
    */
    public get IsEmpty(): boolean {
        return Strings.Empty(this.name) && (!this.value || this.value == undefined);
    }

    /**
     * @param name 变量值的名字属性
     * @param value 这个变量值
    */
    public constructor(public name: string = null, public value: T = null) {}

    public valueOf(): T {
        return this.value;
    }

    public ToArray(): any[] {
        return [this.name, this.value];
    }

    public toString(): string {
        return this.name;
    }
}