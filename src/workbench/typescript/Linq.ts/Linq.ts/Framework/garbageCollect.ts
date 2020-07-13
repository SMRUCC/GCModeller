namespace TypeScript {

    /**
     * https://github.com/natewatson999/js-gc
    */
    export module garbageCollect {

        /**
         * try to do garbageCollect by invoke this function
        */
        export const handler: Delegate.Func<any> = getHandler();

        function getHandler() {
            if (typeof (<any>window).require === "function") {
                let require = (<any>window).require;

                try {
                    require("v8").setFlagsFromString('--expose_gc');

                    if ((<any>window).global != null) {
                        let global = (<any>window).global;

                        if (typeof global.gc == "function") {
                            return global.gc
                        }
                    }

                    var vm = require("vm");

                    if (vm != null) {
                        if (typeof vm.runInNewContext == "function") {
                            var k = vm.runInNewContext("gc");
                            return function () {
                                k();
                                return;
                            };
                        }
                    }
                } catch (e) { }

            }
            if (typeof window !== 'undefined') {
                if ((<any>window).CollectGarbage) {
                    return (<any>window).CollectGarbage;
                }
                if ((<any>window).gc) {
                    return (<any>window).gc;
                }
                if ((<any>window).opera) {
                    if ((<any>window).opera.collect) {
                        return (<any>window).opera.collect;
                    }
                }
                //if ((<any>window).QueryInterface) {
                //    return (<any>window).QueryInterface((<any>Components).interfaces.nsIInterfaceRequestor).getInterface((<any>Components).interfaces.nsIDOMWindowUtils).garbageCollect;
                //}
            }
            //if (typeof ProfilerAgent !== 'undefined') {
            //    if (ProfilerAgent.collectGarbage) {
            //        return ProfilerAgent.collectGarbage;
            //    }
            //}
            if (typeof (<any>window).global !== 'undefined') {
                let global = (<any>window).global;

                if (global.gc) {
                    return global.gc;
                }
            }


            //if (typeof Duktape == 'object') {
            //    if (typeof Duktape.gc) {
            //        return Duktape.gc;
            //    }
            //}
            //if (typeof Packages !== 'undefined') {
            //    if (typeof Packages.java !== 'undefined') {
            //        if (Packages.java.lang) {
            //            if (Packages.java.lang) {
            //                if (Packages.java.lang.System) {
            //                    return Packages.java.lang.System.gc;
            //                }
            //            }
            //        }
            //    }
            //}
            //if (typeof java !== 'undefined') {
            //    if (java.lang) {
            //        if (java.lang) {
            //            if (java.lang.System) {
            //                return java.lang.System.gc;
            //            }
            //        }
            //    }
            //}
            //if (typeof Java !== 'undefined') {
            //    if (java.type) {
            //        return Java.type('java.lang.System').gc;
            //    }
            //}
            //if (typeof CollectGarbage == "function") {
            //    return CollectGarbage;
            //}
            //if (typeof jerry_gc == "function") {
            //    return jerry_gc;
            //}

            return function () {
                if (TypeScript.logging.outputEverything) {
                    console.log("There is no gc handler could be found currently....");
                }
            };
        }
    }
}