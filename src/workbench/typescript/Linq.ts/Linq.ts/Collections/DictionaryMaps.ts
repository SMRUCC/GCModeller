/// <reference path="./Abstract/Enumerator.ts" />
/// <reference path="../Framework/StackTrace/StackTrace.ts" />
/// <reference path="../Framework/Reflection/Reflector.ts" />

/**
 * 键值对映射哈希表
 * 
 * ```
 * IEnumerator<MapTuple<string, V>>
 * ```
*/
class Dictionary<V> extends IEnumerator<MapTuple<string, V>>  {

    private maps: object;

    /**
     * 返回一个被复制的当前的map对象
    */
    public get Object(): object {
        return Framework.Extensions.extend(this.maps);
    }

    ///**
    // * 可以使用``for (var [key, value] of Maps) {}``的语法来进行迭代
    //*/
    //public get Maps(): Map {        
    //    var maps: Map = new Map();

    //    // 将内部的object转换为可以被迭代的ES6的Map对象
    //    Object.keys(this.maps)
    //        .forEach(key => maps.set(key, this.maps[key]));

    //    return maps;
    //}

    /**
     * 如果键名称是空值的话，那么这个函数会自动使用caller的函数名称作为键名进行值的获取
     * 
     * https://stackoverflow.com/questions/280389/how-do-you-find-out-the-caller-function-in-javascript
     * 
     * @param key 键名或者序列的索引号
    */
    public Item(key: string | number = null): V {
        if (!key) {
            key = Internal.StackTrace.GetCallerMember().memberName;
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
        return $from(Object.keys(this.maps));
    }

    /**
     * 获取这个字典对象之中的所有的键值
    */
    public get Values(): IEnumerator<V> {
        return this.Select(m => m.value);
    }

    /**
     * 将目标对象转换为一个类型约束的映射序列集合
    */
    public constructor(maps: object | MapTuple<string, V>[] | IEnumerator<MapTuple<string, V>> = null) {
        super(Dictionary.ObjectMaps<V>(maps));

        if (isNullOrUndefined(maps)) {
            this.maps = {};
        } else if (Array.isArray(maps)) {
            this.maps = Activator.CreateObject(maps);
        } else if ($ts.typeof(maps).class == "IEnumerator") {
            this.maps = Activator.CreateObject(<IEnumerator<MapTuple<string, V>>>maps);
        } else {
            this.maps = maps;
        }
    }

    public static FromMaps<V>(maps: MapTuple<string, V>[] | IEnumerator<MapTuple<string, V>>): Dictionary<V> {
        return new Dictionary<V>(maps);
    }

    public static FromNamedValues<V>(values: NamedValue<V>[] | IEnumerator<NamedValue<V>>): Dictionary<V> {
        return new Dictionary<V>(Activator.CreateObject(values));
    }

    public static MapSequence<V>(maps: object | MapTuple<string, V>[] | IEnumerator<MapTuple<string, V>>): IEnumerator<MapTuple<string, V>> {
        return new IEnumerator(this.ObjectMaps(maps));
    }

    /**
     * 将目标对象转换为一个类型约束的映射序列集合
    */
    public static ObjectMaps<V>(maps: object | MapTuple<string, V>[] | IEnumerator<MapTuple<string, V>>): MapTuple<string, V>[] {
        var type = TypeScript.Reflection.$typeof(maps);

        if (isNullOrUndefined(maps)) {
            return [];
        }

        if (Array.isArray(maps)) {
            return maps;
        } else if (type.class == "IEnumerator") {
            return (<IEnumerator<MapTuple<string, V>>>maps).ToArray();
        } else {
            return $from(Object.keys(maps))
                .Select(key => new MapTuple<string, V>(key, maps[key]))
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