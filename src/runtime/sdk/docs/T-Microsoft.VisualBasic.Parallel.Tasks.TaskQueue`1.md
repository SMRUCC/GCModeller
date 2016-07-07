---
title: TaskQueue`1
---

# TaskQueue`1
_namespace: [Microsoft.VisualBasic.Parallel.Tasks](N-Microsoft.VisualBasic.Parallel.Tasks.html)_





### Methods

#### Enqueue
```csharp
Microsoft.VisualBasic.Parallel.Tasks.TaskQueue`1.Enqueue(System.Func{`0},System.Action{`0})
```
这个函数只会讲任务添加到队列之中，而不会阻塞线程

|Parameter Name|Remarks|
|--------------|-------|
|handle|-|


#### Join
```csharp
Microsoft.VisualBasic.Parallel.Tasks.TaskQueue`1.Join(System.Func{`0})
```
函数会被插入一个队列之中，之后线程会被阻塞在这里直到函数执行完毕，这个主要是用来控制服务器上面的任务并发的

|Parameter Name|Remarks|
|--------------|-------|
|handle|-|

_returns: 假若本对象已经开始Dispose了，则为完成的任务都会返回Nothing_


