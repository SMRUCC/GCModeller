class LINQIterator<T> implements Iterator<T>, Iterable<T> {

    private i: number = 0;
    protected sequence: T[];

    /**
     * 实现迭代器的关键元素之1
    */
    [Symbol.iterator]() { return this; }

    /**
     * The number of elements in the data sequence.
    */
    public get Count(): number {
        return this.sequence.length;
    }

    public constructor(array: T[]) {
        this.sequence = array;
    }

    public reset(): LINQIterator<T> {
        this.i = 0;
        return this;
    }

    // next(value?: any): IteratorResult<T>;

    /**
     * 实现迭代器的关键元素之2
    */
    public next(value?: any): IPopulated<T> {
        return this.i < this.Count ?
            <IPopulated<T>>{ value: this[this.i++], done: false } :
            <IPopulated<T>>{ value: undefined, done: true };
    }
}

/**
 * 迭代器对象所产生的一个当前的index状态值
*/
interface IPopulated<T> extends IteratorResult<T> {
    value: T;
    done: boolean;
}


class enumerator<T> extends Array<T> {

    public constructor(array: T[]) {
        super(0);

        array.forEach(x => this.push(x));
    }
}


var ddd = new enumerator(["sss", "tttt", "8888", "[[[[", "tete", "5555", "@@@@@", "985645", "//", "****", "+"]);

var eeee = new LINQIterator(ddd);


for (var jj of eeee) {
    console.error(jj)
}

for (var ddddddd of ddd) {
    console.log(ddddddd);
}

