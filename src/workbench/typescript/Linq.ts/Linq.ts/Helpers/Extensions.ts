/**
 * 通用数据拓展函数集合
*/
module DataExtensions {

    export function merge(obj: {}, ...args: {}[]) {
        var target: object;
        var key: string;

        for (var i = 0; i < args.length; i++) {
            target = args[i];

            for (key in target) {
                if (Object.prototype.hasOwnProperty.call(target, key)) {
                    obj[key] = target[key];
                }
            }
        }

        return obj;
    }

    export function arrayBufferToBase64(buffer: Array<number> | ArrayBuffer): string {
        var binary: string = '';
        var bytes = new Uint8Array(buffer);
        var len: number = bytes.byteLength;

        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }

        return window.btoa(binary);
    }

    export function toUri(data: DataURI): string {
        if (typeof data.data !== "string") {
            data.data = arrayBufferToBase64(data.data);
        }

        return `data:${data.mime_type};base64,${data.data}`;
    }

    /**
     * 将uri之中的base64字符串数据转换为一个byte数据流
    */
    export function uriToBlob(uri: string | DataURI): Blob {
        var mimeString: string;
        var buffer: ArrayBuffer;

        if (typeof uri == "string") {
            var base64: string = uri.split(',')[1];

            mimeString = uri.split(',')[0].split(':')[1].split(';')[0];
            buffer = base64ToBlob(base64);
        } else {
            mimeString = uri.mime_type;
            buffer = typeof uri.data == "string" ? base64ToBlob(uri.data) : uri.data;
        }

        return new Blob([buffer], {
            type: mimeString
        });
    }

    export function base64ToBlob(base64: string): ArrayBuffer {
        var byteString = window.atob(base64);
        var buffer = new ArrayBuffer(byteString.length);
        var intArray = new Uint8Array(buffer);

        for (var i = 0; i < byteString.length; i++) {
            intArray[i] = byteString.charCodeAt(i);
        }

        return buffer;
    }

    /**
     * @param fill 进行向量填充的初始值，可能不适用于引用类型，推荐应用于初始的基元类型
    */
    export function Dim<T>(len: number, fill: T = null): T[] {
        let vector: T[] = [];

        for (var i: number = 0; i < len; i++) {
            vector.push(fill);
        }

        return vector;
    }
}