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
