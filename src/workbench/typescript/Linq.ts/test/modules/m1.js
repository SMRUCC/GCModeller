function sleep(d){
    for(var t = Date.now();Date.now() - t <= d;);
}

console.log("running script 1");

// 等待5秒钟才会加载下一个脚本
sleep(1000);

console.log("job complete, load next script");