/**
 * 包含有一个表示类型的mime_type属性以及存储数据的data属性
 * 用于生成一个data uri字符串
*/
interface DataURI {

    mime_type: string;
    /**
     * base64 string or array buffer blob
    */
    data: string | ArrayBuffer;
}