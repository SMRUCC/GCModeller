/// <reference path="./stringEval.ts" />

namespace Internal.Handlers {

    /**
     * 在这个字典之中的键名称主要有两大类型:
     * 
     * + typeof 类型判断结果
     * + TypeInfo.class 类型名称
    */
    export const Shared = {
        /**
         * HTML document query handler
        */
        string: () => new stringEval(),
        /**
         * Create a linq object
        */
        array: () => new arrayEval(),
        NodeListOf: () => new DOMCollection(),
        HTMLCollection: () => new DOMCollection()
    };

    export interface IEval<T> {
        doEval(expr: T, type: TypeScript.Reflection.TypeInfo, args: object): any;
    }

    /**
     * Create a Linq Enumerator
    */
    export class arrayEval<V> implements IEval<V[]> {

        doEval(expr: V[], type: TypeScript.Reflection.TypeInfo, args: object): any {
            return $from(expr);
        }
    }

    export class DOMCollection<V extends HTMLElement> implements IEval<V[]> {

        doEval(expr: V[], type: TypeScript.Reflection.TypeInfo, args: object) {
            return new DOMEnumerator(expr);
        }
    }
}