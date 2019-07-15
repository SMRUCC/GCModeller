/// <reference path="../../Data/Encoder/Base64.ts" />

namespace HttpHelpers {

    /**
     * Javascript动态加载帮助函数
    */
    export class Imports {

        /**
         * 发生加载错误的脚本，例如404，脚本文件不存在等错误
        */
        private errors: string[];
        private jsURL: string[];
        private i: number = 0;

        /**
         * 当脚本执行的时候抛出异常的时候是否继续执行下去？
        */
        private onErrorResumeNext: boolean = false;
        private echo: boolean = false;

        /**
         * @param modules javascript脚本文件的路径集合
         * @param onErrorResumeNext On Error Resume Next Or Just Break
        */
        public constructor(modules: string | string[], onErrorResumeNext: boolean = false, echo: boolean = true) {
            if (typeof modules == "string") {
                this.jsURL = [modules];
            } else {
                this.jsURL = modules;
            }

            this.errors = [];
            this.onErrorResumeNext = onErrorResumeNext;
            this.echo = echo;
        }

        private nextScript(): string {
            var url: string = this.jsURL[this.i++];
            return url;
        }

        /**
         * 开始进行异步的脚本文件加载操作
         * 
         * @param callback 在所有指定的脚本文件都完成了加载操作之后所调用的异步回调函数
        */
        doLoad(callback: () => void = DoNothing): void {
            var url: string = this.nextScript();

            if (Strings.Empty(url, true)) {
                // 已经加载完所有的脚本了
                // 执行callback
                callback();
            } else {
                HttpHelpers.GetAsyn(url, (script, code) => this.doExec(url, script, code, callback));
            }
        }

        /**
         * 完成向服务器的数据请求操作之后
         * 加载代码文本
        */
        private doExec(url: string, script: string, code: number, callback: () => void): void {
            switch (code) {
                case 200:
                    try {
                        eval.apply(window, [script]);
                    } catch (ex) {
                        if (this.onErrorResumeNext) {
                            if (TypeScript.logging.outputWarning) {
                                console.warn(url);
                                console.warn(ex);
                            }
                            this.errors.push(url);
                        } else {
                            throw ex;
                        }
                    } finally {
                        if (this.echo) {
                            console.log("script loaded: ", url);
                        }
                    }

                    break;
                default:
                    if (this.echo) {
                        console.error("ERROR: script not loaded: ", url);
                    }
                    this.errors.push(url);
            }

            this.doLoad(callback);
        }

        /**
         * @param script 这个函数可以支持base64字符串格式的脚本的动态加载
         * @param context 默认是添加在当前文档窗口环境之中
        */
        public static doEval(script: string, callback?: () => void, context: Window = window): void {
            if (Base64.isValidBase64String(script)) {
                script = Base64.decode(script);
            }

            eval.apply(context, [script]);

            if (callback) {
                callback();
            }
        }

        /**
         * 得到相对于当前路径而言的目标脚本全路径
        */
        public static getFullPath(url: string): string {
            var location: string = $ts.location.path;

            if (url.charAt(0) == "/") {
                // 是一个绝对路径
                return url;
            } else {

            }

            console.log(location);
        }
    }
}