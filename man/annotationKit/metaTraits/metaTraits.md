# metaTraits

toolkit for the microbial phenotype (Traitar) svm models
> 该工具集把生物表型的 SVM 训练与预测管线暴露给 R# 环境：
>  
>  1. 通过 R# 函数 ``load.phenotype_traits`` 读取表型定义表，
>     每一种表型按照其数据类型（boolean / categorical / numeric）对应一个
>     SVM 模型实例（C_SVC 或者 EPSILON_SVR）；
>  2. 通过 R# 函数 ``pfam.embedding`` 把各微生物基因组所预测的蛋白质组
>     Pfam 结构域组成嵌入为 per-genome 归一化的特征向量；
>  3. 通过 R# 函数 ``train.phenotype_models`` 训练出全部表型的模型，
>     并通过 ``save.trait_models`` 落盘为 json 模型仓库；
>  4. 通过 R# 函数 ``make_predicts`` 使用模型仓库预测新基因组的全部表型。

+ [load.meta_traits](metaTraits/load.meta_traits.1) 读取表型标注数据并按物种组织为表型谱
+ [load.trait_annotations](metaTraits/load.trait_annotations.1) 读取表型标注的原始表格（每一行是一个物种的一种表型标注）
+ [load.phenotype_traits](metaTraits/load.phenotype_traits.1) 读取表型定义表，每一种表型对应一个 SVM 模型实例
+ [read.pfam_proteomes](metaTraits/read.pfam_proteomes.1) 读取一个目录之下的全部微生物基因组的蛋白质组 Pfam 结构域注释
+ [pfam.embedding](metaTraits/pfam.embedding.1) 构建 Pfam 结构域的嵌入配置（词表 + 编码方式）
+ [phenotype.problem](metaTraits/phenotype.problem.1) 装配表型的训练数据集
+ [train.phenotype_models](metaTraits/train.phenotype_models.1) 训练全部表型所对应的 SVM 模型
+ [save.trait_models](metaTraits/save.trait_models.1) 把训练得到的表型模型仓库保存到指定目录
+ [load.trait_models](metaTraits/load.trait_models.1) 从 json 模型仓库目录之中加载全部表型的 SVM 模型
+ [make_predicts](metaTraits/make_predicts.1) 使用表型模型仓库预测基因组的全部表型
+ [phenotype_result](metaTraits/phenotype_result.1) 把预测结果合并上模型的元数据，生成最终的表型预测报告
+ [tune.trait_model](metaTraits/tune.trait_model.1) 对单个表型做超参数（C / gamma）的网格搜索
