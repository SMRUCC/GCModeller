/**
 * 前端与后台之间的get/post的消息通信格式的简单接口抽象
*/
interface IMsg<T> {

    /**
     * 错误代码，一般使用零表示没有错误
    */
    code: number;
    /**
     * 消息的内容
     * 当code不等于零的时候，表示发生错误，则这个时候的错误消息将会以字符串的形式返回
    */
    info: string | T;

    url: string;
}