# Equation
_namespace: [SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels](./index.md)_





### Methods

#### Elapsed
```csharp
SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.Equation.Elapsed(Microsoft.VisualBasic.Mathematical.Expression)
```
执行一次数学运算，然后使用当前所更新的变量值更新表达式计算引擎内部的变量值

|Parameter Name|Remarks|
|--------------|-------|
|engine|-|


#### Evaluate
```csharp
SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.Equation.Evaluate
```
Evaluate the expression value of the property @``P:SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.Equation.Expression``.
 (计算@``P:SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.Equation.Expression``属性表达式的值)

#### Set
```csharp
SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.Equation.Set(SMRUCC.genomics.Analysis.SSystem.Kernel.Kernel)
```
Set up the simulation kernel.
 (设置模拟核心)

|Parameter Name|Remarks|
|--------------|-------|
|k|-|



### Properties

#### Expression
使用代谢底物的UniqueID属性值作为数值替代的表达式
#### Value
The node states in the current network state.
#### Var
The target that associated with this channel.
 (与本计算通道相关联的目标对象)
