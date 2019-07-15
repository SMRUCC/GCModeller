/**
 * 通用数据拓展函数集合
*/
module DataExtensions {

    export function arrayBufferToBase64(buffer: Array<number>): string {
        var binary: string = '';
        var bytes = new Uint8Array(buffer);
        var len = bytes.byteLength;

        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }

        return window.btoa(binary);
    }

    /**
     * 将uri之中的base64字符串数据转换为一个byte数据流
    */
    export function uriToBlob(uri: string): Blob {
        var byteString = window.atob(uri.split(',')[1]);
        var mimeString = uri.split(',')[0].split(':')[1].split(';')[0]
        var buffer = new ArrayBuffer(byteString.length);
        var intArray = new Uint8Array(buffer);

        for (var i = 0; i < byteString.length; i++) {
            intArray[i] = byteString.charCodeAt(i);
        }

        return new Blob([buffer], { type: mimeString });
    }

    /**
     * 将URL查询字符串解析为字典对象，所传递的查询字符串应该是查询参数部分，即问号之后的部分，而非完整的url
     * 
     * @param queryString URL查询参数
     * @param lowerName 是否将所有的参数名称转换为小写形式？
     * 
     * @returns 键值对形式的字典对象
    */
    export function parseQueryString(queryString: string, lowerName: boolean = false): object {
        // stuff after # is not part of query string, so get rid of it
        // split our query string into its component parts
        var arr = queryString.split('#')[0].split('&');
        // we'll store the parameters here
        var obj = {};

        for (var i = 0; i < arr.length; i++) {
            // separate the keys and the values
            var a = arr[i].split('=');

            // in case params look like: list[]=thing1&list[]=thing2
            var paramNum = undefined;
            var paramName = a[0].replace(/\[\d*\]/, function (v) {
                paramNum = v.slice(1, -1);
                return '';
            });

            // set parameter value (use 'true' if empty)
            var paramValue: string = typeof (a[1]) === 'undefined' ? "true" : a[1];

            if (lowerName) {
                paramName = paramName.toLowerCase();
            }

            // if parameter name already exists
            if (obj[paramName]) {

                // convert value to array (if still string)
                if (typeof obj[paramName] === 'string') {
                    obj[paramName] = [obj[paramName]];
                }

                if (typeof paramNum === 'undefined') {
                    // if no array index number specified...
                    // put the value on the end of the array
                    obj[paramName].push(paramValue);
                } else {
                    // if array index number specified...
                    // put the value at that index number
                    obj[paramName][paramNum] = paramValue;
                }
            } else {
                // if param name doesn't exist yet, set it
                obj[paramName] = paramValue;
            }
        }

        return obj;
    }

    /**
     * 尝试将任意类型的目标对象转换为数值类型
     * 
     * @returns 一个数值
    */
    export function as_numeric(obj: any): number {
        return AsNumeric(obj)(obj);
    }

    /**
     * 因为在js之中没有类型信息，所以如果要取得类型信息必须要有一个目标对象实例
     * 所以在这里，函数会需要一个实例对象来取得类型值
    */
    export function AsNumeric<T>(obj: T): (x: T) => number {
        if (obj == null || obj == undefined) {
            return null;
        }

        if (typeof obj === 'number') {
            return x => <number><any>x;
        } else if (typeof obj === 'boolean') {
            return x => {
                if (<boolean><any>x == true) {
                    return 1;
                } else {
                    return -1;
                }
            }
        } else if (typeof obj == 'undefined') {
            return x => 0;
        } else if (typeof obj == 'string') {
            return x => {
                return Strings.Val(<string><any>x);
            }
        } else {
            // 其他的所有情况都转换为零
            return x => 0;
        }
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