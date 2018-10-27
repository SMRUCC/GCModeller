module TypeExtensions {

    export function ensureNumeric(x: number | string): number {
        if (typeof x == "number") {
            return x;
        } else {
            return parseFloat(x);
        }
    }
}