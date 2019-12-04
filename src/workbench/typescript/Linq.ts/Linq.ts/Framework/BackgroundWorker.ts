namespace Internal {

    export class BackgroundWorker {

        public static $workers = {};

        public static get hasWorkerFeature(): boolean {
            if (typeof (Worker) !== "undefined") {
                return true;
            } else {
                return false;
            }
        }

        /**
         * 加载所给定的脚本，并创建一个后台线程
         * 
         * 可以在脚本中使用格式为``{$name}``的占位符进行标注
         * 然后通过args参数来对这些占位符进行替换，从而达到参数传递的目的
         * 
         * @param script 脚本的url或者文本内容，这个主要是为了解决跨域创建后台线程的问题        
         * @param args 向脚本模板之中进行赋值操作的变量列表，这个参数应该是一个键值对对象
        */
        public static RunWorker(script: string, onMessage: Delegate.Sub, args: {} = null) {
            if (TypeScript.URLPatterns.isAPossibleUrlPattern(script)) {
                // 是一个url，则GET请求然后从文本创建
                // 进行url网络请求是一个异步的过程
                this.fetchWorker(script, args, url => BackgroundWorker.registerWorker(script, url, onMessage));
            } else {
                // 是一个字符串文本，则直接创建一个worker               
                // 注册一个新的工作线程
                this.registerWorker(md5(script), this.buildWorker(script, args), onMessage);
            }
        }

        private static registerWorker(script: string, url: string, onMessage: Delegate.Sub) {
            BackgroundWorker.$workers[script] = new Worker(url);
            BackgroundWorker.$workers[script].onmessage = onMessage;
        }

        private static buildWorker(scriptText: string, args: {}): string {
            let blob: Blob;

            // 进行字符串替换赋值
            if (!isNullOrUndefined(args)) {
                let value: string;
                let placeholder: string;

                for (var name in args) {
                    value = args[name];
                    placeholder = `{$${name}}`;

                    // 进行模板上的占位符字符串进行值替换
                    scriptText = scriptText.replace(placeholder, value);
                }
            }

            try {
                blob = new Blob([scriptText], { type: 'application/javascript' });
            } catch (e) {
                // Backwards-compatibility
                let webEngine: any = window;

                webEngine.BlobBuilder = webEngine.BlobBuilder ||
                    webEngine.WebKitBlobBuilder ||
                    webEngine.MozBlobBuilder;

                blob = webEngine.BlobBuilder();

                (<any>blob).append(scriptText);
                blob = (<any>blob).getBlob();
            }

            return URL.createObjectURL(blob);
        }

        /**
         * How to create a Web Worker from a string
         * 
         * > https://stackoverflow.com/questions/10343913/how-to-create-a-web-worker-from-a-string/10372280#10372280
        */
        private static fetchWorker(scriptUrl: string, args: {}, register: Delegate.Sub) {
            // get script text from server
            $ts.getText(scriptUrl, script => {
                let blobUrl: string = this.buildWorker(script, args);

                TypeScript.logging.log(`Build worker blob url: ${blobUrl}`, TypeScript.ConsoleColors.Gray);
                register(blobUrl);
            });
        }

        public static Stop(script: string) {
            BackgroundWorker.$workers[script].terminate();

            // removes worker object
            delete BackgroundWorker.$workers[script];
        }
    }
}