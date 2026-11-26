# VirtualCellular

VirtualCellular: 基于 CellularAI 的虚拟细胞模型与虚拟扰动实验
> 模块内的每个 api 只做**一件事**，可以按需组合；计算与文件 IO 严格分离：
>  
> 
> 
>  
> - 数据加载：`load_expr`（表达矩阵 + 元数据 → `ExpressionStream`）
> - 纯计算：`model.build`（装配模型，不落盘）、`as_perturbation`（包装成扰动实验对象）、
>  `overexpress_genes`（联合过表达）
> - 纯 IO：`model.save` / `model.open` / `export.de` / `export.summary`
> - 查询：`model.genes` / `model.perturbations`
> 
> 
>  
> 
> 
>  `as_perturbation` 返回的对象实现了 [InsilicoPerturbationExperiment](cref:T:SMRUCC.genomics.BioSystem.InsilicoPerturbationExperiment)，
>  因此可以直接交给 `intervention` 包已有的 `knockouts` / `overexpress` /
>  `knockdown` / `make_exports` 复用，不必在本模块里重复实现扰动与批量导出。

+ [load_expr](VirtualCellular/load_expr.1) 
+ [model.build](VirtualCellular/model.build.1) 由加载好的表达数据装配虚拟细胞模型（纯计算，不写任何文件）。
+ [model.save](VirtualCellular/model.save.1) 把模型保存为模型包（纯 IO）。
+ [model.open](VirtualCellular/model.open.1) 打开一个已保存的模型包（纯 IO）。
+ [model.genes](VirtualCellular/model.genes.1) 模型的基因轴。
+ [model.perturbations](VirtualCellular/model.perturbations.1) 模型的伪扰动轴（期别对比 / 患者对比 / 高表达签名）。
+ [as_perturbation](VirtualCellular/as_perturbation.1) 把模型包装成虚拟扰动实验对象（纯计算，不做任何 IO）。
+ [overexpress_genes](VirtualCellular/overexpress_genes.1) 联合过表达（接口只定义了单基因，多基因场景走这个入口）。
+ [perturbation.significant](VirtualCellular/perturbation.significant.1) 显著变化的基因（纯查询，无 IO）。
+ [export.de](VirtualCellular/export.de.1) 导出差异表达表（纯 IO）。
+ [export.summary](VirtualCellular/export.summary.1) 导出 Top 变化清单（纯 IO）。
