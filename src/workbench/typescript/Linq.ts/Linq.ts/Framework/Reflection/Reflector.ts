/// <reference path="Type.ts" />

namespace TypeScript.Reflection {

    /**
     * 获取某一个对象的类型信息
    */
    export function $typeof<T>(obj: T): TypeInfo {
        var type = typeof obj;
        var isObject: boolean = type == "object";
        var isArray: boolean = Array.isArray(obj);
        var isNull: boolean = isNullOrUndefined(obj);
        var typeInfo: TypeInfo = new TypeInfo;
        var className: string = getClassInternal(obj, isArray, isObject, isNull);

        typeInfo.typeOf = isArray ? "array" : type;
        typeInfo.class = className;

        if (isNull) {
            typeInfo.property = [];
            typeInfo.methods = [];
        } else {
            typeInfo.property = isObject ? Object.keys(<any>obj) : [];
            typeInfo.methods = GetObjectMethods(obj);
        }

        return typeInfo;
    }

    /**
     * 获取object对象上所定义的所有的函数
    */
    export function GetObjectMethods<T>(obj: T): string[] {
        var res: string[] = [];

        for (var m in obj) {
            if (typeof obj[m] == "function") {
                res.push(m)
            }
        }

        return res;
    }

    /**
     * 获取得到类型名称
    */
    export function getClass(obj: any): string {
        var type = typeof obj;
        var isObject: boolean = type == "object";
        var isArray: boolean = Array.isArray(obj);
        var isNull: boolean = isNullOrUndefined(obj);

        return getClassInternal(obj, isArray, isObject, isNull);
    }

    export function getClassInternal(obj: any, isArray: boolean, isObject: boolean, isNull: boolean): string {
        if (isArray) {
            return getElementType(obj);
        } else if (isObject) {
            return getObjectClassName(obj, isNull);
        } else {
            return "";
        }
    }

    export function getObjectClassName(obj: object, isnull: boolean): string {
        if (isnull) {
            TypeScript.logging.log(TypeExtensions.objectIsNothing);

            return "null";
        } else {
            return (<any>obj.constructor).name;
        }
    }

    export function getElementType(array: Array<any>): string {
        var x: any = array[0];
        var className: string;

        if ((className = typeof x) == "object") {
            className = x.constructor.name;
        } else {
            // do nothing
        }

        return className;
    }
}