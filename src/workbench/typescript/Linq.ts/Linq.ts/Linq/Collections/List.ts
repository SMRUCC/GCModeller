/**
 * 表示一个动态列表对象
*/
class List<T> extends IEnumerator<T> {

    public constructor(src: T[] | IEnumerator<T> = null) {
        super(src || []);
    }

    /**
     * 可以使用这个方法进行静态代码的链式添加
    */
    public Add(x: T): List<T> {
        this.sequence.push(x);
        return this;
    }

    /**
     * 批量的添加
    */
    public AddRange(x: T[] | IEnumerator<T>): List<T> {
        if (Array.isArray(x)) {
            x.forEach(o => this.sequence.push(o));
        } else {
            x.ForEach(o => this.sequence.push(o));
        }

        return this;
    }

    /**
     * 查找给定的元素在当前的这个列表之中的位置，不存在则返回-1
    */
    public IndexOf(x: T): number {
        return this.sequence.indexOf(x);
    }

    /**
     * 返回列表之中的第一个元素，然后删除第一个元素，剩余元素整体向前平移一个单位
    */
    public Pop(): T {
        var x1 = this.First;
        this.sequence = this.sequence.slice(1);
        return x1;
    }
}