namespace algorithm.BTree {

    /**
     * data extension module for binary tree nodes data sequence
    */
    export module binaryTreeExtensions {

        /**
         * Convert a binary tree object as a node array.
        */
        export function populateNodes<T, V>(tree: node<T, V>): node<T, V>[] {
            var out: node<T, V>[] = [];
            visitInternal(tree, out);
            return out;
        }

        function visitInternal<T, V>(tree: node<T, V>, out: node<T, V>[]): void {
            // 20180929 为什么会存在undefined的节点呢？
            if (isNullOrUndefined(tree)) {
                if (TypeScript.logging.outputWarning) {
                    console.warn(tree);
                }
                
                return;
            } else {
                out.push(tree);
            }            

            if (tree.left) {
                visitInternal(tree.left, out);
            }
            if (tree.right) {
                visitInternal(tree.right, out);
            }
        }
    }
}

