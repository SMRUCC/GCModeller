class LINQIterator {
    constructor(array) {
        this.i = 0;
        this.sequence = array;
    }
    /**
     * 实现迭代器的关键元素之1
    */
    [Symbol.iterator]() { return this; }
    /**
     * The number of elements in the data sequence.
    */
    get Count() {
        return this.sequence.length;
    }
    reset() {
        this.i = 0;
        return this;
    }
    // next(value?: any): IteratorResult<T>;
    /**
     * 实现迭代器的关键元素之2
    */
    next(value) {
        return this.i < this.Count ?
            { value: this[this.i++], done: false } :
            { value: undefined, done: true };
    }
}
class enumerator extends Array {
    constructor(array) {
        super(0);
        array.forEach(x => this.push(x));
    }
}
var ddd = new enumerator(["sss", "tttt", "8888", "[[[[", "tete", "5555", "@@@@@", "985645", "//", "****", "+"]);
var eeee = new LINQIterator(ddd);
for (var jj of eeee) {
    console.error(jj);
}
for (var ddddddd of ddd) {
    console.log(ddddddd);
}
//# sourceMappingURL=app.js.map