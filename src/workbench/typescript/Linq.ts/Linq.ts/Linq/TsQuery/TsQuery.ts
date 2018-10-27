
namespace Linq.TsQuery {

    export const handler = {
        /**
         * HTML document query handler
        */
        string: () => new stringEval(),
        /**
         * Create a linq object
        */
        array: () => new arrayEval()
    };

    export interface IEval<T> {
        doEval(expr: T, type: TypeInfo, args: object): any;
    }

    /**
     * Create a Linq Enumerator
    */
    export class arrayEval<V> implements IEval<V[]> {

        doEval(expr: V[], type: TypeInfo, args: object): any {
            return From(expr);
        }
    }
}