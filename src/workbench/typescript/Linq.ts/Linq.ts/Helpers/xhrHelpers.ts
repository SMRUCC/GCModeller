module HttpHelpers {

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
    */
    export function GetAsyn(url: string, callback: (response: string, code: number) => void) {
        var http = new XMLHttpRequest();

        http.open("GET", url, true);
        http.onreadystatechange = function () {
            if (http.readyState == 4) {
                callback(http.responseText, http.status);
            }
        }
        http.send(null);
    }

    export function POST(
        url: string,
        postData: PostData,
        callback: (response: string, code: number) => void) {

        var http = new XMLHttpRequest();
        var data: any = postData.data;

        http.open('POST', url, true);
        // Send the proper header information along with the request
        http.setRequestHeader('Content-type', postData.type);
        // Call a function when the state changes.
        http.onreadystatechange = function () {
            if (http.readyState == 4) {
                callback(http.responseText, http.status);
            }
        }
        http.send(data);
    }

    /**
     * 使用multipart form类型的数据进行文件数据的上传操作
     * 
     * @param url 函数会通过POST方式将文件数据上传到这个url所指定的服务器资源位置
     * 
    */
    export function UploadFile(
        url: string,
        postData: PostData,
        callback: (response: string, code: number) => void) {

        var data = new FormData();

        data.append("File", postData.data);
        HttpHelpers.POST(url, <PostData>{
            type: postData.type,
            data: data
        }, callback);
    }

    export class PostData {

        /**
         * content type
        */
        public type: string;
        /**
         * 将要进行POST上传的数据包
        */
        public data: any;

        public toString(): string {
            return this.type;
        }
    }
}