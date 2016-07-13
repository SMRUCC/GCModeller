---
title: MetaCycPathways
---

# MetaCycPathways
_namespace: [SMRUCC.genomics.Visualize.Cytoscape](N-SMRUCC.genomics.Visualize.Cytoscape.html)_

整理出代谢途径和相应的基因，对于基因个数少于5的代谢途径，其被合并至其他较大的SuperPathway之中去



### Methods

#### Performance
```csharp
SMRUCC.genomics.Visualize.Cytoscape.MetaCycPathways.Performance
```

> 
>  过程描述：
>  1. 获取所有的代谢途径的数据
>  2. 构建所有的反应对象与基因之间的相互联系
>  3. 根据Reaction-List属性值列表将基因与相应的代谢途径建立联系，最后输出数据
>  


