/// <reference path="../../Data/StackTrace/StackTrace.ts" />

/**
 * 键值对映射哈希表
*/
class Dictionary<V> extends IEnumerator<Map<string, V>>  {

    private maps: object;

    public get Object(): object {
        return Linq.extend(this.maps);
    }

    /**
     * 如果键名称是空值的话，那么这个函数会自动使用caller的函数名称作为键名进行值的获取
     * 
     * https://stackoverflow.com/questions/280389/how-do-you-find-out-the-caller-function-in-javascript
     * 
     * @param key 键名或者序列的索引号
    */
    public Item(key: string | number = null): V {
        if (!key) {
            key = TsLinq.StackTrace.GetCallerMember().memberName;
        }

        if (typeof key == "string") {
            return <V>(this.maps[key]);
        } else {
            return this.sequence[key].value;
        }
    }

    /**
     * 获取这个字典对象之中的所有的键名
    */
    public get Keys(): IEnumerator<string> {
        return From(Object.keys(this.maps));
    }

    /**
     * 获取这个字典对象之中的所有的键值
    */
    public get Values(): IEnumerator<V> {
        return this.Keys.Select<V>(key => this.Item(key));
    }

    /**
     * 将目标对象转换为一个类型约束的映射序列集合
    */
    public constructor(maps: object | Map<string, V>[] | IEnumerator<Map<string, V>> = null) {
        super(Dictionary.ObjectMaps<V>(maps));

        if (isNullOrUndefined(maps)) {
            this.maps = {};
        } else if (Array.isArray(maps)) {
            this.maps = TypeInfo.CreateObject(maps);
        } else if (TypeInfo.typeof(maps).class == "IEnumerator") {
            this.maps = TypeInfo.CreateObject(<IEnumerator<Map<string, V>>>maps);
        } else {
            this.maps = maps;
        }
    }

    public static FromMaps<V>(maps: Map<string, V>[] | IEnumerator<Map<string, V>>): Dictionary<V> {
        return new Dictionary<V>(maps);
    }

    public static FromNamedValues<V>(values: NamedValue<V>[] | IEnumerator<NamedValue<V>>): Dictionary<V> {
        return new Dictionary<V>(TypeInfo.CreateObject(values));
    }

    /**
     * 将目标对象转换为一个类型约束的映射序列集合
    */
    public static ObjectMaps<V>(maps: object | Map<string, V>[] | IEnumerator<Map<string, V>>): Map<string, V>[] {
        var type = TypeInfo.typeof(maps);

        if (isNullOrUndefined(maps)) {
            return [];
        }

        if (Array.isArray(maps)) {
            return maps;
        } else if (type.class == "IEnumerator") {
            return (<IEnumerator<Map<string, V>>>maps).ToArray();
        } else {
            return From(Object.keys(maps))
                .Select(key => new Map<string, V>(key, maps[key]))
                .ToArray();
        }
    }

    /**
     * 查看这个字典集合之中是否存在所给定的键名
    */
    public ContainsKey(key: string): boolean {
        return key in this.maps;
    }

    /**
     * 向这个字典对象之中添加一个键值对，请注意，如果key已经存在这个字典对象中了，这个函数会自动覆盖掉key所对应的原来的值
    */
    public Add(key: string, value: V): Dictionary<V> {
        this.maps[key] = value;
        this.sequence = Dictionary.ObjectMaps<V>(this.maps);
        return this;
    }

    /**
     * 删除一个给定键名所指定的键值对
    */
    public Delete(key: string): Dictionary<V> {
        if (key in this.maps) {
            delete this.maps[key];
            this.sequence = Dictionary.ObjectMaps<V>(this.maps);
        }

        return this;
    }
}