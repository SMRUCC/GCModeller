/**
 * Binary tree implements
*/
namespace algorithm.BTree {

    /**
     * 用于进行数据分组所需要的最基础的二叉树数据结构
     * 
     * ``{key => value}``
    */
    export class binaryTree<T, V> {

        /**
         * 根节点，根节点的key值可能会对二叉树的构建造成很大的影响
        */
        public root: node<T, V>;
        /**
         * 这个函数指针描述了如何对两个``key``之间进行比较
         * 
         * 返回结果值：
         * 
         * + ``等于0`` 表示二者相等
         * + ``大于0`` 表示a大于b
         * + ``小于0`` 表示a小于b
        */
        public compares: (a: T, b: T) => number;

        /**
         * 构建一个二叉树对象
         * 
         * @param comparer 这个函数指针描述了如何进行两个对象之间的比较操作，如果这个函数参数使用默认值的话
         *                 则只能够针对最基本的数值，逻辑变量进行操作
        */
        constructor(comparer: (a: T, b: T) => number = (a: T, b: T) => {
            var x: number = Strings.as_numeric(a);
            var y: number = Strings.as_numeric(b);

            return x - y;
        }) {
            this.compares = comparer;
        }

        /**
         * 向这个二叉树对象之中添加一个子节点
        */
        public add(term: T, value: V = null): void {
            var np: node<T, V> = this.root;
            var cmp: number = 0;

            if (!np) {
                // 根节点是空的，则将当前的term作为根节点
                this.root = new node<T, V>(term, value);
                return;
            }

            while (np) {
                cmp = this.compares(term, np.key);

                if (cmp == 0) {
                    // this node is existed
                    // value replace??
                    np.value = value;
                    break;
                } else if (cmp < 0) {
                    if (np.left) {
                        np = np.left;
                    } else {
                        // np is a leaf node?
                        // add at here
                        np.left = new node<T, V>(term, value);
                        break;
                    }
                } else {
                    if (np.right) {
                        np = np.right;
                    } else {
                        np.right = new node<T, V>(term, value);
                        break;
                    }
                }
            }
        }

        /**
         * 根据key值查找一个节点，然后获取该节点之中与key所对应的值
         * 
         * @returns 如果这个函数返回空值，则表示可能未找到目标子节点
        */
        public find(term: T): V {
            var np = this.root;
            var cmp = 0;

            while (np) {
                cmp = this.compares(term, np.key);

                if (cmp == 0) {
                    return np.value;
                } else if (cmp < 0) {
                    np = np.left;
                } else {
                    np = np.right;
                }
            }

            // not exists
            return null;
        }

        /**
         * 将这个二叉树对象转换为一个节点的数组
        */
        public ToArray(): node<T, V>[] {
            return binaryTreeExtensions.populateNodes(this.root);
        }

        /**
         * 将这个二叉树对象转换为一个Linq查询表达式所需要的枚举器类型
        */
        public AsEnumerable(): IEnumerator<node<T, V>> {
            return new IEnumerator<node<T, V>>(this.ToArray());
        }
    }
}