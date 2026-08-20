---
name: CDHitFamilyExport
overview: 基于 CDHit 模块 FindSimilar 的聚类结果，在 ProteinMatrix 项目新建 CDHitFamilyExport.vb 导出模块，将聚类簇转换为 FamilyExports 与 SequenceCluster 两个 CSV 集合，范式对齐 Linclust/ClusterExporter.vb。
todos:
  - id: create-cdhit-export
    content: 新建 CDHitFamilyExport.vb，实现 ExportClusters 导出 FamilyExports 与 SequenceCluster 两个 CSV
    status: completed
  - id: verify-compile
    content: 在 ProteinMatrix 项目编译验证新模块与既有引用一致，无类型/命名空间错误
    status: completed
    dependencies:
      - create-cdhit-export
  - id: add-demo
    content: 参照 LinclustDemo.vb 在 test 下补充轻量 CDHit 导出调用示例
    status: completed
    dependencies:
      - create-cdhit-export
---

## 用户需求

基于现有 CDHit 聚类模块（kmer + CD-HIT 贪婪聚类）的聚类结果，导出基于 CDHit 方法产生的蛋白质家族信息，复用 FamilyExports.vb 中定义的 FamilyExports 与 SequenceCluster 两个数据集合。

## 产品概述

在 ProteinMatrix 项目中新增一个导出模块，消费 `CDHit.FindSimilar()` 返回的 `IEnumerable(Of SimilarHit)` 聚类结果，将每个簇转换为两条 CSV 文件：FamilyExports.csv（每簇一行，记录家族元信息）与 SequenceCluster.csv（每簇成员一行，记录序列与相似度打分），输出格式与字段完全对齐既有的 Linclust.ClusterExporter 实现。

## 核心功能

- 消费 CDHit 聚类结果（SimilarHit 集合）并补全代表序列为簇内第 1 个成员（成员数 = 1 + Similar.Count，代表 score 取簇内相似度最大值）。
- 反查原始 FastaSeq 序列内容，生成 FamilyExports 集合（family_id、members、representative、rep_seq）。
- 生成 SequenceCluster 集合（seq_title、family_id、score、seq），覆盖代表与所有相似成员。
- 以 UTF-8 写出 FamilyExports.csv 与 SequenceCluster.csv 到指定目录，目录缺失时自动创建；输入为空时安全返回空结果并打印日志。

## 技术栈

- 语言：Visual Basic (.NET, net10.0)
- 项目：GCModeller/analysis/ProteinTools/ProteinMatrix（命名空间 `SMRUCC.genomics.Model.MotifGraph.ProteinStructure`）
- 依赖复用：`SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`、`Microsoft.VisualBasic.Data.Framework.IO`（SaveTo 流式写出）、`Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm`（进度条），与既有 Linclust/ClusterExporter.vb 保持一致。
- 输入来源：`SMRUCC.genomics.Analysis.SequenceAlignment.CDHit` 与 `SimilarHit`（SequenceAlignment 项目已被 ProteinMatrix 引用，可直接使用）。

## 实现方案

### 总体策略

在 ProteinMatrix 项目新建独立模块 `CDHitFamilyExport.vb`，参照 `Linclust/ClusterExporter.vb` 的范式实现导出函数 `ExportClusters`。核心是从 CDHit 的 `SimilarHit` 结构（以序列 Title 为键）转换为 FamilyExports / SequenceCluster 集合（Linclust 以整数索引为键），通过 `seqs.ToDictionary(Function(s) s.Title)` 建立 Title→FastaSeq 反查表，替代 Linclust 的整数下标反查。

### 关键技术决策

- 导出逻辑放 ProteinMatrix 侧：因 SequenceAlignment 未反向引用 ProteinMatrix，而 FamilyExports/SequenceCluster 位于 ProteinMatrix；ProteinMatrix.vbproj 已引用 SequenceAlignment，可直接调用 CDHit，避免新增项目间反向依赖。
- 代表序列补全：CDHit.FindSimilar 中代表本身未写入 `Similar` 字典，导出时显式将 `SeqID` 作为第 1 个成员行，score 取 `Similar.Values.Max`（空簇记 0），与 Linclust 导出语义一致。
- 反查方式：使用 Title 字典而非整数索引，契合 CDHit 的数据结构（SimilarHit.SeqID / Similar 键均为 Title），避免引入索引对齐风险。
- 复用既有写出范式：`List(Of T).SaveTo(path, encoding:=Encoding.UTF8)`，严格遵循既有 CSV 写出路径，字段名直接映射属性名，无需额外 ColumnAttribute。

### 性能与可靠性

- 复杂度：单次遍历 `clusters` 与每个簇的成员字典，O(N+M)（N=簇数，M=总成员数）；反查为 O(1) 字典查找，无 N+1 或重复遍历。
- 大对象堆看护：仅保存 Title→FastaSeq 引用（不复制序列内容），与 ClusterExporter 注释中"不复制序列内容"的约束一致，控制 LOH 压力。
- 边界处理：clusters 或 seqs 为 Nothing / 空时返回 (Nothing, Nothing) 并打印提示；反查缺失 Title 时跳过该成员并打印警告，避免 KeyError。
- 向后兼容：不修改 CDHit.vb、SimilarHit.vb、FamilyExports.vb，纯新增模块，blast radius 最小。

## 实现要点（执行细节）

- 入口签名仿 `ExportClusters(seqs As FastaSeq(), clusters As IEnumerable(Of SimilarHit), outputDir As String) As (familyCsv$, sequenceCsv$)`。
- family_id 采用 1-based 编号 `family_{i+1}`，与同仓库 FamilyCluster / Linclust 约定一致。
- FamilyExports.members = If(cluster.IsUniqued, 1, 1 + cluster.Similar.Count)。
- SequenceCluster.score：代表行取 `If(cluster.Similar.Values.Any, cluster.Similar.Values.Max, 0)`；成员行取 `cluster.Similar(memberTitle)`。
- 写出前 `Directory.CreateDirectory(outputDir)`；使用 Tqdm 进度条包裹簇循环（对齐 ClusterExporter）。
- Imports 与 ClusterExporter 对齐：Microsoft.VisualBasic、Data.Framework.IO、Data.Framework.IO.Linq、SMRUCC.genomics.SequenceModel.FASTA、System.Text。

## 架构设计

### 数据流

`CDHit.Setup(seqs).FindSimilar(threshold)` → `IEnumerable(Of SimilarHit)` → `CDHitFamilyExport.ExportClusters(seqs, clusters, outputDir)` → 反查 Title 字典 → 构建 `List(Of FamilyExports)` 与 `List(Of SequenceCluster)` → `SaveTo` 写出两个 CSV。

### 组件关系

- `CDHit`（SequenceAlignment，输入源，不改动）
- `FamilyExports` / `SequenceCluster`（ProteinMatrix 数据模型，不改动）
- `CDHitFamilyExport`（ProteinMatrix，新增导出适配器，对齐 `Linclust.ClusterExporter` 范式）

## 目录结构

```
g:\GCModeller\src\GCModeller\analysis\ProteinTools\ProteinMatrix\
└── CDHitFamilyExport.vb   # [NEW] CDHit 聚类结果导出模块。实现 ExportClusters(seqs, clusters, outputDir) 函数：
                            #   - 建立 Title→FastaSeq 反查字典；
                            #   - 遍历 SimilarHit 簇，补全代表为第1成员；
                            #   - 生成 List(Of FamilyExports) 与 List(Of SequenceCluster)；
                            #   - 写出 FamilyExports.csv 与 SequenceCluster.csv (UTF-8)；
                            #   - 处理空输入与缺失 Title 边界。Imports/范式对齐 Linclust/ClusterExporter.vb。
```

## 关键代码结构（接口级）

```
' g:\GCModeller\src\GCModeller\analysis\ProteinTools\ProteinMatrix\CDHitFamilyExport.vb
Public Module CDHitFamilyExport

    ''' <summary>
    ''' 将 CDHit.FindSimilar 返回的聚类结果导出为 FamilyExports.csv 与 SequenceCluster.csv。
    ''' </summary>
    ''' <param name="seqs">参与聚类的原始序列数组（用于按 Title 反查序列内容）</param>
    ''' <param name="clusters">CDHit.FindSimilar(threshold) 返回的聚类簇</param>
    ''' <param name="outputDir">导出目录（不存在时自动创建）</param>
    ''' <returns>(FamilyExports.csv 路径, SequenceCluster.csv 路径)；输入为空返回 (Nothing, Nothing)</returns>
    Public Function ExportClusters(seqs As FastaSeq(),
                                   clusters As IEnumerable(Of SimilarHit),
                                   outputDir As String) As (familyCsv$, sequenceCsv$)
End Function

End Module
```