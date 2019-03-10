/// <reference path="../../../build/linq.d.ts" />

console.log('Hello world');

console.log($ts(["1", "2", "3"]).ElementType);

console.log($ts("#T5555").display("23333<sup>33333</sup>"));
console.log((<DOMEnumerator<HTMLElement>><any>$ts(".gg")).onClick((sender) => alert(sender.innerText)));

console.log("");


$ts.imports("javascriptStackTraceTest.js", () => {
    (<any>window).callsTop2();
});