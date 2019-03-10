module Cookies {

    /**
     * Cookie 不存在，函数会返回空字符串
    */
    export function getCookie(cookiename: string): string {
        // Get name followed by anything except a semicolon
        var cookie: string = document.cookie;
        var cookiestring = RegExp("" + cookiename + "[^;]+").exec(cookie);
        var value: string;

        // Return everything after the equal sign, 
        // or an empty string if the cookie name not found
        if (!!cookiestring) {
            value = cookiestring.toString().replace(/^[^=]+./, "");
        } else {
            value = "";
        }

        return decodeURIComponent(value);
    }

    /**
     * 将cookie设置为过期，进行cookie的删除操作
    */
    export function delCookie(name: string): void {
        var cval: string = getCookie(name);
        var exp: Date = new Date();

        exp.setTime(exp.getTime() - 1);

        if (cval != null) {
            var expires: string = (<any>exp).toGMTString();

            expires = `${name}=${cval};expires=${expires}`;
            document.cookie = expires;
        }
    }
}