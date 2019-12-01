namespace HttpHelpers {

    export const contentTypes = {
        form: "multipart/form-data",
        /**
         * 请注意：如果是php服务器，则$_POST很有可能不会自动解析json数据，导致$_POST变量为空数组
         * 则这个时候会需要你在php文件之中手动处理一下$_POST变量：
         * 
         * ```php
         * $json  = file_get_contents("php://input");
         * $_POST = json_decode($json, true);
         * ```
        */
        json: "application/json",
        text: "text/plain",
        /**
         * 传统的表单post格式
        */
        www: "application/x-www-form-urlencoded"
    }

    export interface httpCallback {
        (response: string, code: number, contentType: string): void;
    }

    export function measureContentType(obj: any): string {
        if (obj instanceof FormData) {
            return contentTypes.form;
        } else if (typeof obj == "string") {
            return contentTypes.text;
        } else {
            // object类型都会被转换为json发送回服务器
            return contentTypes.json;
        }
    }

    /**
     * 这个函数只会返回200成功代码的响应内容，对于其他的状态代码都会返回null
     * (这个函数是同步方式的)
    */
    export function GET(url: string): string {
        var request = new XMLHttpRequest();

        // `false` makes the request synchronous
        request.open('GET', url, false);
        request.send(null);

        if (request.status === 200) {
            return request.responseText;
        } else {
            return null;
        }
    }

    /**
     * 使用异步调用的方式进行数据的下载操作
     * 
     * @param callback ``callback(http.responseText, http.status)``
    */
    export function GetAsyn(url: string, callback: httpCallback) {
        var http = new XMLHttpRequest();

        http.open("GET", url, true);
        http.onreadystatechange = function () {
            let contentType: string = this.getResponseHeader('content-type');

            if (isNullOrUndefined(contentType)) {
                contentType = this.getResponseHeader('Content-Type');
            }

            if (http.readyState == 4) {
                callback(http.response || http.responseText, http.status, contentType);
            }
        }
        http.send(null);
    }

    export function POST(
        url: string,
        postData: PostData,
        callback: (response: string, code: number) => void) {

        var http = new XMLHttpRequest();
        var data = postData.data;

        if (postData.type == contentTypes.json) {
            if (typeof data != "string") {
                data = JSON.stringify(data);
            }
        }

        http.open('POST', url, true);
        // Send the proper header information along with the request
        if (postData.sendContentType) {
            http.setRequestHeader('Content-Type', postData.type);
        }
        // Call a function when the state changes.
        http.onreadystatechange = function () {
            if (http.readyState == 4) {
                callback(http.responseText, http.status);
            }
        }
        http.send(<any>data);
    }

    /**
     * 使用multipart form类型的数据进行文件数据的上传操作
     * 
     * @param url 函数会通过POST方式将文件数据上传到这个url所指定的服务器资源位置
     * 
    */
    export function UploadFile(
        url: string,
        postData: File | Blob | string,
        fileName: string = null,
        callback: (response: string, code: number) => void) {

        var data = new FormData();

        if (postData instanceof File) {
            data.append("filename", (<File>postData).name);
            fileName = fileName || (<File>postData).name;
        } else {
            data.append("filename", fileName);
        }

        data.append("File", postData, fileName);

        HttpHelpers.POST(url, <PostData>{
            type: contentTypes.form,
            data: data
        }, callback);
    }

    /**
     * @param a 如果这个是一个无参数的函数, 则会求值之后再进行序列化
    */
    export function serialize<T extends {}>(a: T | Delegate.Func<T>, nullAsStringFactor: boolean = false): string {
        let sb: string[] = [];
        let value: any;

        if (typeof a == "function") {
            a = (<Delegate.Func<T>>a)();
        }

        for (let key of Object.keys(a)) {
            value = a[key];

            if (isNullOrUndefined(value)) {
                if (nullAsStringFactor && TypeScript.logging.outputEverything) {
                    console.warn(`${key} value is nothing!`);
                    value = "null";
                } else {
                    // skip
                    continue;
                }
            }

            sb.push(`${key}=${encodeURIComponent(value)}`);
        }

        return sb.join("&");
    }

    /**
     * 在这个数据包对象之中应该包含有
     * 
     * + ``type``属性，用来设置``Content-type``
     * + ``data``属性，可以是``formData``或者一个``object``
    */
    export class PostData {

        /**
         * content type
        */
        public type: string;
        /**
         * 将要进行POST上传的数据包
        */
        public data: FormData | object | string | Blob | File;
        public sendContentType: boolean = true;

        public toString(): string {
            return this.type;
        }
    }
}