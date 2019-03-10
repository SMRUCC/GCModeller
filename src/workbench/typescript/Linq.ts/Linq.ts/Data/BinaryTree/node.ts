namespace algorithm.BTree {

    /**
     * A binary tree node.
    */
    export class node<T, V> {

        public key: T;
        public value: V;
        public left: node<T, V>;
        public right: node<T, V>;

        constructor(key: T, value: V = null, left: node<T, V> = null, right: node<T, V> = null) {
            this.key = key;
            this.left = left;
            this.right = right;
            this.value = value;
        }

        public toString(): string {
            return <string>(<any>this.key).toString();
        }
    }
}