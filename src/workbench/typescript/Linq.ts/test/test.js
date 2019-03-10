/// <reference path="../../../build/linq.d.ts" />
console.log('Hello world');
console.log($ts(["1", "2", "3"]).ElementType);
console.log($ts("#T5555").display("23333<sup>33333</sup>"));
console.log($ts(".gg").onClick(function (sender) { return alert(sender.innerText); }));
console.log("");
$ts.imports("javascriptStackTraceTest.js", function () {
    window.callsTop2();
});
//# sourceMappingURL=test.js.map