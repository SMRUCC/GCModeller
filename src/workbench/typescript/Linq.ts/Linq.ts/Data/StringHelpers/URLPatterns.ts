namespace TypeScript.URLPatterns {

    export const hostNamePattern: RegExp = /:\/\/(www[0-9]?\.)?(.[^/:]+)/i;

    /**
     * Regexp pattern for data uri string
    */
    export const uriPattern: RegExp = /data[:]\S+[/]\S+;base64,[a-zA-Z0-9/=+]/ig;
    /**
     * Regexp pattern for web browser url string
    */
    export const urlPattern: RegExp = /((https?)|(ftp))[:]\/{2}\S+\.[a-z]+[^ >"]*/ig;

    export function isFromSameOrigin(url: string): boolean {
        let URL = new TypeScript.URL(url);
        let origin1 = URL.origin.toLowerCase();
        let origin2 = window.location.origin.toLowerCase();

        return origin1 == origin2;
    }

    /**
     * 判断目标文本是否可能是一个url字符串
    */
    export function isAPossibleUrlPattern(text: string, pattern: RegExp = URLPatterns.urlPattern): boolean {
        var matches = text.match(pattern);

        if (isNullOrUndefined(matches)) {
            return false;
        }

        var match: string = matches[0];

        if (!Strings.Empty(match, true)) {
            return text.indexOf(match) == 0;
        } else {
            return false;
        }
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
}