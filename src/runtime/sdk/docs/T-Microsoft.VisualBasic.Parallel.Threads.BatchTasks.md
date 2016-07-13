---
title: BatchTasks
---

# BatchTasks
_namespace: [Microsoft.VisualBasic.Parallel.Threads](N-Microsoft.VisualBasic.Parallel.Threads.html)_





### Methods

#### BatchTask``1
```csharp
Microsoft.VisualBasic.Parallel.Threads.BatchTasks.BatchTask``1(System.Func{``0}[],System.Int32,System.Int32)
```
由于LINQ是分片段来执行的，当某个片段有一个线程被卡住之后整个进程都会被卡住，所以执行大型的计算任务的时候效率不太好，
 使用这个并行化函数可以避免这个问题，同时也可以自己手动控制线程的并发数

|Parameter Name|Remarks|
|--------------|-------|
|actions|-|
|numThreads|可以在这里手动的控制任务的并发数，这个数值小于或者等于零则表示自动配置线程的数量|
|TimeInterval|-|


#### BatchTask``2
```csharp
Microsoft.VisualBasic.Parallel.Threads.BatchTasks.BatchTask``2(System.Collections.Generic.IEnumerable{``0},System.Func{``0,``1},System.Int32,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|getTask|-|
|numThreads|可以在这里手动的控制任务的并发数，这个数值小于或者等于零则表示自动配置线程的数量，如果想要单线程，请将这个参数设置为1|
|TimeInterval|-|



