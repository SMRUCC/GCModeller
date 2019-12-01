/// <reference path="./Abstract/Enumerator.ts" />

/**
 * 按照某一个键值进行分组的集合对象
*/
class Group<TKey, T> extends IEnumerator<T> {

    /**
     * 当前的分组之中的值所都共有的键值对象
    */
    public Key: TKey;

    /**
     * Group members, readonly property.
    */
    public get Group(): T[] {
        return this.sequence;
    }

    constructor(key: TKey, group: T[]) {
        super(group);
        this.Key = key;
    }

    /**
     * 创建一个键值对映射序列，这些映射都具有相同的键名
    */
    public ToMaps(): MapTuple<TKey, T>[] {
        return $from(this.sequence)
            .Select(x => new MapTuple<TKey, T>(this.Key, x))
            .ToArray();
    }
}