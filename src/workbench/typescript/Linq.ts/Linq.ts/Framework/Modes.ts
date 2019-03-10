/**
 * 这个枚举选项的值会影响框架之中的调试器的终端输出行为
*/
enum Modes {
    /**
     * Framework debug level
     * (这个等级下会输出所有信息)
    */
    debug = 0,
    /**
     * development level
     * (这个等级下会输出警告信息)
    */
    development = 10,
    /**
     * production level      
     * (只会输出错误信息，默认等级)
    */
    production = 200
}