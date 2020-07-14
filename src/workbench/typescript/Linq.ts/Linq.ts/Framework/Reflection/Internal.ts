/**
 * 这里的函数主要是用来解决经过gcc工具压缩之后，原来的名字被破坏掉了的bug
*/
namespace TypeScript.Reflection.Internal {

    export function isEnumeratorSignature(type: TypeInfo): boolean {
        let p = {
            "i": false,
            "sequence": false
        };

        for (let pname of type.property) {
            if (pname == "i") p.i = true;
            if (pname == "sequence") p.sequence = true;
        }

        if (!p.i || !p.sequence) {
            return false;
        }

        let m = {
            ElementAt: false,
            ToArray: false
        }

        for (let mname of type.methods) {
            if (mname == "ElementAt") m.ElementAt = true;
            if (mname == "ToArray") m.ToArray = true;
        }

        if (!m.ElementAt || !m.ToArray) {
            return false;
        }

        return true;
    }
}