// export R# package module type define for javascript/typescript language
//
//    imports "metaTraits" from "annotationKit";
//
// ref=annotationKit.metaTraitsTool@annotationKit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * toolkit for the microbial phenotype (Traitar) svm models
 * 
 * > 该工具集把生物表型的 SVM 训练与预测管线暴露给 R# 环境：
 * >  
 * >  1. 通过 R# 函数 ``load.phenotype_traits`` 读取表型定义表，
 * >     每一种表型按照其数据类型（boolean / categorical / numeric）对应一个
 * >     SVM 模型实例（C_SVC 或者 EPSILON_SVR）；
 * >  2. 通过 R# 函数 ``pfam.embedding`` 把各微生物基因组所预测的蛋白质组
 * >     Pfam 结构域组成嵌入为 per-genome 归一化的特征向量；
 * >  3. 通过 R# 函数 ``train.phenotype_models`` 训练出全部表型的模型，
 * >     并通过 ``save.trait_models`` 落盘为 json 模型仓库；
 * >  4. 通过 R# 函数 ``make_predicts`` 使用模型仓库预测新基因组的全部表型。
*/
declare namespace metaTraits {
   module load {
      /**
       * 读取表型标注数据并按物种组织为表型谱
       * 
       * 
        * @param file ncbi/gtdb species summary tsv 表文件
        * @param env 
        * + default value Is ``null``.
      */
      function meta_traits(file: string, env?: object): object;
      /**
       * 读取表型定义表，每一种表型对应一个 SVM 模型实例
       * 
       * 
        * @param file phenotype_traits_and_types.csv，其中定义了每一种表型的数据类型
        *  （boolean / categorical / numeric (continuous)）
        * @param env 
        * + default value Is ``null``.
      */
      function phenotype_traits(file: string, env?: object): object;
      /**
       * 读取表型标注的原始表格（每一行是一个物种的一种表型标注）
       * 
       * 
        * @param file ncbi/gtdb species summary tsv 表文件
        * @param env 
        * + default value Is ``null``.
      */
      function trait_annotations(file: any, env?: object): object;
      /**
       * 从 json 模型仓库目录之中加载全部表型的 SVM 模型
       * 
       * 
        * @param repo 模型仓库目录
        * @param env 
        * + default value Is ``null``.
      */
      function trait_models(repo: string, env?: object): object;
   }
   /**
    * 使用表型模型仓库预测基因组的全部表型
    * 
    * 
     * @param models 模型仓库
     * @param pfams 待预测基因组的蛋白质组 Pfam 注释：
     *  传入单个基因组的 PfamString 向量时返回该基因组的预测结果；
     *  传入 list（R# 函数 ``read.pfam_proteomes`` 的输出）或者目录路径时，
     *  返回以基因组名为键的预测结果列表
     * @param env 
     * + default value Is ``null``.
   */
   function make_predicts(models: object, pfams: any, env?: object): object;
   module pfam {
      /**
       * 构建 Pfam 结构域的嵌入配置（词表 + 编码方式）
       * 
       * 
        * @param pfams 可以是 R# 函数 ``read.pfam_proteomes`` 的输出，也可以直接是
        *  一个包含各微生物基因组 Pfam.csv 的目录路径
        * @param minGenomes 词表剪枝阈值：Pfam 结构域至少要在这么多个基因组之中出现才会被保留，默认为 1（不剪枝）
        * 
        * + default value Is ``1``.
        * @param encoding 编码方式：``normalized`` 表示按基因组内的最大出现次数做归一化（默认），
        *  ``binary`` 表示退化为 0/1 的 one-hot 编码
        * 
        * + default value Is ``'normalized'``.
        * @param env 
        * + default value Is ``null``.
      */
      function embedding(pfams: any, minGenomes?: object, encoding?: string, env?: object): object;
   }
   module phenotype {
      /**
       * 装配表型的训练数据集
       * 
       * 
        * @param traits 表型定义表
        * @param annotations 表型标注数据
        * @param pfams 各微生物基因组的蛋白质组 Pfam 注释（list 或者目录路径）
        * @param embedding Pfam 嵌入配置
        * @param env 
        * + default value Is ``null``.
        * @return 行 = 微生物基因组样本，topic = 表型名的训练数据集
      */
      function problem(traits: any, annotations: any, pfams: any, embedding: object, env?: object): object;
   }
   /**
    * 把预测结果合并上模型的元数据，生成最终的表型预测报告
    * 
    * 
     * @param predicts 预测结果
     * @param models 模型仓库
     * @param dataset 训练数据集，提供该参数时会额外计算每一个表型的关键 Pfam 结构域特征
     * 
     * + default value Is ``null``.
     * @param topN 关键特征的数量上限
     * 
     * + default value Is ``20``.
     * @param env 
     * + default value Is ``null``.
   */
   function phenotype_result(predicts: any, models: object, dataset?: any, topN?: object, env?: object): object;
   module read {
      /**
       * 读取一个目录之下的全部微生物基因组的蛋白质组 Pfam 结构域注释
       * 
       * 
        * @param dir 每一个子目录是一个微生物基因组，目录名即物种名，
        *  其中包含一个 Pfam.csv 文件
        * @param env 
        * + default value Is ``null``.
        * @return 一个以基因组名为键的 list，每一个元素是该基因组的蛋白质组 Pfam 注释
      */
      function pfam_proteomes(dir: string, env?: object): object;
   }
   module save {
      /**
       * 把训练得到的表型模型仓库保存到指定目录
       * 
       * 
        * @param models 模型仓库
        * @param dir 输出目录，每一种表型会被写为一个独立的 json 文件
        * @param sampleCount 训练集的样本数量
        * 
        * + default value Is ``0``.
        * @param verbose 
        * + default value Is ``true``.
        * @param env 
        * + default value Is ``null``.
      */
      function trait_models(models: object, dir: string, sampleCount?: object, verbose?: boolean, env?: object): object;
   }
   module train {
      /**
       * 训练全部表型所对应的 SVM 模型
       * 
       * 
        * @param dataset 由 [metaTraitsTool.phenotype_problem()](cref:M:annotationKit.metaTraitsTool.phenotype_problem(System.Object,System.Object,System.Object,SMRUCC.genomics.Analysis.metaTraits.Traitar.PfamEmbedding,SMRUCC.Rsharp.Runtime.Environment)) 装配出来的训练数据集
        * @param kernel 核函数类型：rbf（默认）/ linear / poly / sigmoid
        * 
        * + default value Is ``'rbf'``.
        * @param nrfold 交叉验证的折数
        * 
        * + default value Is ``5``.
        * @param tune 网格搜索的作用范围：``regression`` 表示只对数值型表型做搜索（默认，开销很小），
        *  ``none`` 表示全部使用默认参数，``all`` 表示对全部表型做搜索（分类型表型
        *  在部分网格点上收敛极慢，耗时可能达到分钟级）
        * 
        * + default value Is ``'regression'``.
        * @param verbose 是否输出训练日志
        * 
        * + default value Is ``true``.
        * @param env 
        * + default value Is ``null``.
      */
      function phenotype_models(dataset: object, kernel?: string, nrfold?: object, tune?: string, verbose?: boolean, env?: object): object;
   }
   module tune {
      /**
       * 对单个表型做超参数（C / gamma）的网格搜索
       * 
       * 
        * @param dataset 训练数据集
        * @param trait 目标表型
        * @param kernel 
        * + default value Is ``'rbf'``.
        * @param nrfold 交叉验证折数
        * 
        * + default value Is ``5``.
        * @param env 
        * + default value Is ``null``.
      */
      function trait_model(dataset: object, trait: object, kernel?: string, nrfold?: object, env?: object): object;
   }
}
