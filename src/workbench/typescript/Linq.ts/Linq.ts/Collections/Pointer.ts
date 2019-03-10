/// <reference path="./Abstract/Enumerator.ts" />

/**
 * A data sequence object with a internal index pointer.
*/
class Pointer<T> extends IEnumerator<T> {

    /**
     * The index pointer of the current data sequence.
    */
    public p: number;

    /**
     * The index pointer is at the end of the data sequence?
    */
    public get EndRead(): boolean {
        return this.p >= this.Count;
    }

    /**
     * Get the element value in current location i;
    */
    public get Current(): T {
        return this.sequence[this.p];
    }

    /**
     * Get current index element value and then move the pointer 
     * to next position.
    */
    public get Next(): T {
        var x = this.Current;
        this.p = this.p + 1;
        return x;
    }

    public constructor(src: T[] | IEnumerator<T>) {
        super(src);
        // 2018-09-02 在js里面，数值必须要进行初始化
        // 否则会出现NA初始值，导致使用EndRead属性判断失败
        // 可能会导致死循环的问题出现
        this.p = 0;
    }

    /**
     * Just move the pointer to the next position and then 
     * returns current pointer object.
    */
    public MoveNext(): Pointer<T> {
        this.p = this.p + 1;
        return this;
    }

    /**
     * 以当前的位置为基础，得到偏移后的位置的值，但不会改变现有的指针的位置值
    */
    public Peek(offset: number): T {
        return this.sequence[this.p + offset];
    }
}