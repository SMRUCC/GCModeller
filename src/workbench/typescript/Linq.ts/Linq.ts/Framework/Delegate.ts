/**
 * 通用的JavaScript闭包函数指针抽象接口集合
 * 这些接口之间可以相互重载
*/
namespace Delegate {

    /**
     * 带有一个参数的子程序
    */
    export interface Sub {
        (arg: any): void;
    }

    /**
     * 带有两个参数的子程序
    */
    export interface Sub {
        (arg1: any, arg2: any): void;
    }

    export interface Sub {
        (arg1: any, arg2: any, arg3: any): void;
    }

    export interface Sub {
        (arg1: any, arg2: any, arg3: any, arg4: any): void;
    }

    export interface Sub {
        (arg1: any, arg2: any, arg3: any, arg4: any, arg5: any): void;
    }

    /**
     * 不带任何参数的子程序
    */
    export interface Action {
        (): void;
    }

    /**
     * 不带参数的函数指针
    */
    export interface Func<V> {
        <V>(): V;
    }

    /**
     * 带有一个函数参数的函数指针
    */
    export interface Func<V> {
        <T, V>(arg: T): V;
    }

    export interface Func<V> {
        <T1, T2, V>(arg1: T1, arg2: T2): V;
    }

    export interface Func<V> {
        <T1, T2, T3, V>(arg1: T1, arg2: T2, arg3: T3): V;
    }

    export interface Func<V> {
        <T1, T2, T3, T4, V>(arg1: T1, arg2: T2, arg3: T3, arg4: T4): V;
    }

    export interface Func<V> {
        <T1, T2, T3, T4, T5, V>(arg1: T1, arg2: T2, arg3: T3, arg4: T4, arg5: T5): V;
    }
}