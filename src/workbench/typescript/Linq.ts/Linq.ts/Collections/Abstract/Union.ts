namespace Enumerable {

    export class JoinHelper<T, U> {

        private xset: T[];
        private yset: U[];
        private keysT: string[];
        private keysU: string[];

        public constructor(x: T[], y: U[]) {
            this.xset = x;
            this.yset = y;
            this.keysT = AllKeys(x);
            this.keysU = AllKeys(y);
        }

        public JoinProject<V>(x: T, y: U): V {
            var out: object = {};

            this.keysT.forEach(k => out[k] = x[k]);
            this.keysU.forEach(k => out[k] = y[k]);

            return <V><any>out;
        }

        public Union<K, V>(
            tKey: (x: T) => K,
            uKey: (x: U) => K,
            compare: (a: K, b: K) => number,
            project: (x: T, y: U) => V = this.JoinProject): IEnumerator<V> {

            var tree = this.buildUtree(uKey, compare);
            var output: V[] = [];
            var keyX = new algorithm.BTree.binaryTree<K, K>(compare);

            this.xset.forEach(x => {
                var key: K = tKey(x);
                var list: U[] = tree.find(key);

                if (list) {
                    // 有交集，则进行叠加投影
                    list.forEach(y => output.push(project(x, y)));

                    if (!keyX.find(key)) {
                        keyX.add(key);
                    }
                } else {
                    // 没有交集，则投影空对象
                    output.push(project(x, <U>{}));
                }
            });
            this.yset.forEach(y => {
                var key: K = uKey(y);

                if (!keyX.find(key)) {
                    // 没有和X进行join，则需要union到最终的结果之中
                    // 这个y是找不到对应的x元素的
                    output.push(project(<T>{}, y));
                }
            });

            return new IEnumerator<V>(output);
        }

        private buildUtree<K>(uKey: (x: U) => K, compare: (a: K, b: K) => number): algorithm.BTree.binaryTree<K, U[]> {
            var tree = new algorithm.BTree.binaryTree<K, U[]>(compare);

            this.yset.forEach(obj => {
                var key: K = uKey(obj);
                var list: U[] = tree.find(key);

                if (list) {
                    list.push(obj);
                } else {
                    tree.add(key, [obj]);
                }
            });

            return tree;
        }

        public LeftJoin<K, V>(
            tKey: (x: T) => K,
            uKey: (x: U) => K,
            compare: (a: K, b: K) => number,
            project: (x: T, y: U) => V = this.JoinProject): IEnumerator<V> {

            var tree = this.buildUtree(uKey, compare);
            var output: V[] = [];

            this.xset.forEach(x => {
                var key: K = tKey(x);
                var list: U[] = tree.find(key);

                if (list) {
                    // 有交集，则进行叠加投影
                    list.forEach(y => output.push(project(x, y)));
                } else {
                    // 没有交集，则投影空对象
                    output.push(project(x, <U>{}));
                }
            });

            return new IEnumerator<V>(output);
        }
    }
}