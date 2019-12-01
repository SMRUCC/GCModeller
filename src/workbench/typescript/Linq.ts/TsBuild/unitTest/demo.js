var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
/// <reference path="../../../../build/linq.d.ts" />
$ts(function () {
    Router.AddAppHandler(new demo.app.pages.printTest());
    Router.RunApp();
});
// demo test for the js parser
var demo;
(function (demo) {
    var app;
    (function (app) {
        var pages;
        (function (pages) {
            /**
             * A demo web app for print hello world on console
            */
            var printTest = /** @class */ (function (_super) {
                __extends(printTest, _super);
                function printTest() {
                    return _super !== null && _super.apply(this, arguments) || this;
                }
                Object.defineProperty(printTest.prototype, "appName", {
                    get: function () {
                        return "index";
                    },
                    enumerable: true,
                    configurable: true
                });
                ;
                printTest.prototype.init = function () {
                    console.log('Hello world');
                };
                return printTest;
            }(Bootstrap));
            pages.printTest = printTest;
        })(pages = app.pages || (app.pages = {}));
    })(app = demo.app || (demo.app = {}));
})(demo || (demo = {}));
//# sourceMappingURL=demo.js.map