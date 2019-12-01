/// <reference path="./Collections/Map.ts" />

namespace TypeExtensions {

    /**
     * Warning message of Nothing
    */
    export const objectIsNothing: string = "Object is nothing! [https://docs.microsoft.com/en-us/dotnet/visual-basic/language-reference/nothing]";

    /**
     * 字典类型的元素类型名称字符串
    */
    export const DictionaryMap: string = TypeScript.Reflection.getClass(new MapTuple("", ""));

    /**
     * Make sure target input is a number
    */
    export function ensureNumeric(x: number | string): number {
        if (typeof x == "number") {
            return x;
        } else {
            return parseFloat(x);
        }
    }

    /**
     * 判断目标是否为可以直接转换为字符串的数据类型
    */
    export function isPrimitive(any: any): boolean {
        let type = typeof any;

        return type == "string" ||
            type == "number" ||
            type == "boolean";
    }

    export function isElement(obj: any): boolean {
        try {
            //Using W3 DOM2 (works for FF, Opera and Chrome)
            return obj instanceof HTMLElement;
        }
        catch (e) {
            //Browsers not supporting W3 DOM2 don't have HTMLElement and
            //an exception is thrown and we end up here. Testing some
            //properties that all elements have (works on IE7)
            return (typeof obj === "object") &&
                (obj.nodeType === 1) && (typeof obj.style === "object") &&
                (typeof obj.ownerDocument === "object");
        }
    }

    export function isMessageObject(obj: any): boolean {
        let type = $ts.typeof(obj);
        let members = Activator.EmptyObject(type.property, true);

        if ("code" in members && "info" in members && Strings.IsPattern(obj["code"].toString(), /\d+/g)) {
            return true;
        } else {
            return false;
        }
    }
}