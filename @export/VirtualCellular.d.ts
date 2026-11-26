// export R# package module type define for javascript/typescript language
//
//    imports "VirtualCellular" from "biosystem";
//
// ref=biosystem.virtualCellularTools@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * VirtualCellular: 基于 CellularAI 的虚拟细胞模型与虚拟扰动实验
 * 
 * > 模块内的每个 api 只做**一件事**，可以按需组合；计算与文件 IO 严格分离：
 * >  
 * > 
 * > 
 * >  
 * > - 数据加载：`load_expr`（表达矩阵 + 元数据 → `ExpressionStream`）
 * > - 纯计算：`model.build`（装配模型，不落盘）、`as_perturbation`（包装成扰动实验对象）、
 * >  `overexpress_genes`（联合过表达）
 * > - 纯 IO：`model.save` / `model.open` / `export.de` / `export.summary`
 * > - 查询：`model.genes` / `model.perturbations`
 * > 
 * > 
 * >  
 * > 
 * > 
 * >  `as_perturbation` 返回的对象实现了 [InsilicoPerturbationExperiment](cref:T:SMRUCC.genomics.BioSystem.InsilicoPerturbationExperiment)，
 * >  因此可以直接交给 `intervention` 包已有的 `knockouts` / `overexpress` /
 * >  `knockdown` / `make_exports` 复用，不必在本模块里重复实现扰动与批量导出。
*/
declare namespace VirtualCellular {
   /**
    * 把模型包装成虚拟扰动实验对象（纯计算，不做任何 IO）。
    * 
    * 
     * @param model 由 `model.build` 或 `model.open` 得到的模型
     * @param cells 默认合成细胞数；`intervention` 包的 api 未显式指定时使用
     * 
     * + default value Is ``400``.
     * @param seed 随机种子
     * 
     * + default value Is ``0``.
     * @return 实现了 [InsilicoPerturbationExperiment](cref:T:SMRUCC.genomics.BioSystem.InsilicoPerturbationExperiment) 的 [CellularAIPerturbation](cref:T:SMRUCC.genomics.Analysis.VirtualCellular.Perturbation.CellularAIPerturbation)，
     *  可直接传给 `intervention::overexpress` / `knockouts` / `knockdown`。
   */
   function as_perturbation(model: object, cells?: object, seed?: object): object;
   module export {
      /**
       * 导出差异表达表（纯 IO）。
       * 
       * 
        * @param result 一次扰动结果
        * @param path 输出 CSV 路径
      */
      function de(result: object, path: string): string;
      /**
       * 导出 Top 变化清单（纯 IO）。
       * 
       * 
        * @param result 一次扰动结果
        * @param path 输出 CSV 路径
        * @param top_n 导出前多少个基因，默认 200
        * 
        * + default value Is ``200``.
      */
      function summary(result: object, path: string, top_n?: object): string;
   }
   /**
    * 
    * 
     * @param expr 表达矩阵 CSV（行=基因，列=细胞）
     * @param meta 细胞元数据 CSV（需含 ID / patient_id / stage / type 列）
     * @param stages 只保留这些期别的细胞；`NULL` 表示全量
     * 
     * + default value Is ``null``.
     * @param limit 每个期别最多保留的细胞数；0 表示不限制
     * 
     * + default value Is ``0``.
   */
   function load_expr(expr: string, meta: string, stages?: string, limit?: object): object;
   module model {
      /**
       * 由加载好的表达数据装配虚拟细胞模型（纯计算，不写任何文件）。
       * 
       * 
        * @param exp 由 `load_expr` 得到的表达数据（含对照矩阵与细胞元数据）
        * @param opt CellularAI 参数；`NULL` 使用默认值
        * 
        * + default value Is ``null``.
        * @param seed 随机种子（写入模型清单，供后续扰动复现）
        * 
        * + default value Is ``0``.
        * @return 装配好的 [VirtualCellModel](cref:T:SMRUCC.genomics.Analysis.VirtualCellular.Model.VirtualCellModel)；需要保存时另行调用 `model.save`
      */
      function build(exp: object, opt?: object, seed?: object): object;
      /**
       * 模型的基因轴。
       * 
       * 
      */
      function genes(model: object): string;
      /**
       * 打开一个已保存的模型包（纯 IO）。
       * 
       * 
        * @param path 模型包路径（*.zip）
      */
      function open(path: string): object;
      /**
       * 模型的伪扰动轴（期别对比 / 患者对比 / 高表达签名）。
       * 
       * 
      */
      function perturbations(model: object): string;
      /**
       * 把模型保存为模型包（纯 IO）。
       * 
       * 
        * @param output 模型包路径（*.zip）
      */
      function save(model: object, output: string): string;
   }
   /**
    * 联合过表达（接口只定义了单基因，多基因场景走这个入口）。
    * 
    * 
     * @param experiment 由 `as_perturbation` 得到的实验对象
     * @param genes 同时过表达的基因集合
     * @param cells 合成细胞数；0 表示使用实验对象的默认值
     * 
     * + default value Is ``0``.
   */
   function overexpress_genes(experiment: object, genes: string, cells?: object): object;
   module perturbation {
      /**
       * 显著变化的基因（纯查询，无 IO）。
       * 
       * 
        * @param result 一次扰动结果
      */
      function significant(result: object): string;
   }
}
