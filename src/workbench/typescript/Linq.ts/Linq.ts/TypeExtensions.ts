/// <reference path="./Collections/Map.ts" />

namespace TypeExtensions {

    export const objectIsNothing: string = "Object is nothing! [https://docs.microsoft.com/en-us/dotnet/visual-basic/language-reference/nothing]";

    /**
     * 字典类型的元素类型名称字符串
    */
    export const DictionaryMap: string = TypeInfo.getClass(new MapTuple("", ""));

    export function ensureNumeric(x: number | string): number {
        if (typeof x == "number") {
            return x;
        } else {
            return parseFloat(x);
        }
    }
}