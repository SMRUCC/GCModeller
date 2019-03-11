/// <reference path="./Abstract/Enumerator.ts" />

/**
 * 序列之中的对某一个区域的滑窗操作结果对象
*/
class SlideWindow<T> extends IEnumerator<T> {

    /**
     * 这个滑窗对象在原始的数据序列之中的最左端的位置
    */
    public index: number;

    public constructor(index: number, src: T[] | IEnumerator<T>) {
        super(src);
        this.index = index;
    }

    /**
     * 创建指定片段长度的滑窗对象
     * 
     * @param winSize 滑窗片段的长度
     * @param step 滑窗的步进长度，默认是一个步进
    */
    public static Split<T>(src: T[] | IEnumerator<T>, winSize: number, step: number = 1): IEnumerator<SlideWindow<T>> {
        if (!Array.isArray(src)) {
            src = src.ToArray();
        }

        var len: number = src.length - winSize;
        var windows: SlideWindow<T>[] = [];

        for (var i: number = 0; i < len; i += step) {
            var chunk: T[] = new Array(winSize);

            for (var j: number = 0; j < winSize; j++) {
                chunk[j] = src[i + j];
            }

            windows.push(new SlideWindow<T>(i, chunk));
        }

        return new IEnumerator<SlideWindow<T>>(windows);
    }
}