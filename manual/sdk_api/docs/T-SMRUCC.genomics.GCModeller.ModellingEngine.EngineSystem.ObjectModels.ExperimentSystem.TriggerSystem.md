---
title: TriggerSystem
---

# TriggerSystem
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.ExperimentSystem](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.ExperimentSystem.html)_

由于需要动态构建代码并进行编译，由于Trigger进行条件测试的时候需要动态引用子系统部件的实例，故而TriggerSystem需要在CellSystem初始化完毕之后才可以初始化



### Methods

#### Initialize
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.ExperimentSystem.TriggerSystem.Initialize
```

> 
>  vbc Code:
>  
>  Namespace TriggerConditionTest
>     Public Class ConditionTest
>          
>        Public Function TestCondition(Prefix1, Prefix2, ...) As Boolean
>           Return [strTriggerCondition]
>        End Function
>     End Class
>  End Namespace
>  


### Properties

#### _ComponentSource
列表中的对象的顺序按照@"P:SMRUCC.genomics.GCModeller.ModellingEngine.Prefix.PrefixTypeReference"中的对象的顺序进行排列
