# Kernel
_namespace: [SMRUCC.genomics.Analysis.SSystem.Kernel](./index.md)_

The simulation system kernel.



### Methods

#### __innerTicks
```csharp
SMRUCC.genomics.Analysis.SSystem.Kernel.Kernel.__innerTicks(System.Int32)
```
The kernel loop.(内核循环, 会在这里更新数学表达式计算引擎的环境变量)

#### Break
```csharp
SMRUCC.genomics.Analysis.SSystem.Kernel.Kernel.Break
```
中断执行

#### Run
```csharp
SMRUCC.genomics.Analysis.SSystem.Kernel.Kernel.Run(SMRUCC.genomics.Analysis.SSystem.Script.Model,System.Double,System.Action{Microsoft.VisualBasic.Data.csv.DocumentStream.DataSet})
```
Run a compiled model.(运行一个已经编译好的模型文件)

|Parameter Name|Remarks|
|--------------|-------|
|Model|-|



### Properties

#### __engine
模拟器的数学计算引擎
#### Channels
Alter the system state.(方程，也就是生化反应过程)
#### dataSvr
Data collecting
#### Kicks
Object that action the disturbing.(生物扰动实验)
#### Precision
整个引擎的计算精度
#### RuntimeTicks
Gets the system run time ticks
#### Vars
Store the system state.(变量，也就是生化反应底物)
