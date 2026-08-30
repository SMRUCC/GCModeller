# ProteinClustering — 蛋白质序列无监督聚类构建蛋白质家族

VB.NET (.NET 10) 实现，纯 BCL，运行于 Linux，支持 16GB 内存环境下处理 100GB 级 FASTA 文件。

## 核心算法

### 整体架构

```
┌─────────────────────────────────────────────────────────────────────┐
│                    ProteinClustering Pipeline                        │
├──────────────┬──────────────────────────────────────────────────────┤
│  Phase 1     │ 流式读取 FASTA → 分配 int ID → 重格式化 FASTA       │
│  索引+建库   │ → diamond makedb 构建 DIAMOND DB → 删除重格式化文件  │
├──────────────┼──────────────────────────────────────────────────────┤
│  Phase 2     │ 创建 N×4 字节内存映射文件，初始化为 -1（未初始化）   │
│  初始化DSU   │ MemoryMappedFile 支持 OS 分页，活跃集 ~1-2GB         │
├──────────────┼──────────────────────────────────────────────────────┤
│  Phase 3     │ 流式读取 FASTA → 写 chunk_NNNN.fasta (50万条/chunk) │
│  分块比对    │ → diamond blastp 比对 → 流式解析 TSV → DSU.Union      │
│  +聚类       │ → 删除 chunk 临时文件 → 循环至全部处理完毕           │
├──────────────┼──────────────────────────────────────────────────────┤
│  Phase 4     │ 流式读取 FASTA → 按位置分配 ID → DSU.Find → 家族ID  │
│  输出家族    │ → families.tsv (序列头→家族ID)                      │
│              │ → family_summary.tsv (家族ID→成员数)                 │
└──────────────┴──────────────────────────────────────────────────────┘
```

### 关键算法：内存映射并查集 (UnionFind.vb)

```
公式/概念                          → 代码位置
─────────────────────────────────────────────────────
DSU parent array: P[x]             → _accessor.Read/Write(offset=x*4)
Sentinel: -1 = uninitialized       → 文件初始化填充 0xFFFFFFFF
Find(x): 路径压缩                   → Find() 方法，O(α(N)) 均摊
  while P[x] ≠ x: P[x] = P[P[x]]    → 路径压缩循环
  x = P[x]                          → 向根移动
Union(x,y): P[ry] = rx             → Union() 方法
Cache: 4096-entry direct-mapped    → _cacheKeys/_cacheVals 数组
```

### 关键算法：分块 DIAMOND 比对

```
diamond blastp \
  --query chunk.fasta \
  --db protein_db.dmnd \
  --out chunk.tsv \
  --outfmt 6 qseqid sseqid pident qcovhsp scovhsp \  ← 只输出需要的字段
  --id {min_identity}          ← DIAMOND 级预过滤
  --query-cover {min_coverage}  ← 查询覆盖度预过滤
  --threads {threads} \
  --block-size {block_size}     ← 控制 DIAMOND 内存 (~2GB for 0.5)
  --max-target-seqs 100000000   ← 不限制命中数
  --tmpdir {tmpdir}
  --no-self-hits                ← 跳过自比对（如 DIAMOND 支持）
```

## 内存分析 (16GB 物理内存)

| 组件 | 内存占用 | 说明 |
|------|---------|------|
| DSU (内存映射) | ~1-2GB 活跃 | OS 按需分页，路径压缩使树很浅 |
| 当前 chunk | ~150MB | 50万序列 × ~300AA × 1字节 |
| DIAMOND 进程 | ~2-4GB | block-size=0.5 |
| .NET 运行时 | ~500MB | GC + JIT + 框架 |
| **合计** | **~4-7GB** | 16GB 充裕 |

## 磁盘需求

| 文件 | 大小 | 生命周期 |
|------|------|---------|
| 原始 FASTA | ~100GB | 全程只读 |
| 重格式化 FASTA | ~100GB | Phase 1 后删除 |
| DIAMOND DB | ~50GB | Phase 1-3 |
| DSU 文件 | ~8GB (1B序列) | Phase 2-4 |
| chunk 临时文件 | ~1-5GB/chunk | 用完即删 |
| 输出 TSV | ~10-50GB | 最终产物 |
| **峰值** | **~270GB** | 建议[SYSTEM_NOTE: Content compressed. Read the full version if needed.]tory<int> rootToFamily) | 将DSU根重编号为顺序家族ID |
| WriteFamiliesWithDSU | 流式读取FASTA + DSU.Find → 输出 families.tsv |

## 文件→论文算法映射

本实现不基于单一论文，而是综合以下方法：

| 算法概念 | 来源 | 代码文件 |
|----------|------|---------|
| Union-Find (DSU) | Tarjan (1975) | `UnionFind.vb` |
| 路径压缩 | Tarjan & van Leeuwen (1984) | `UnionFind.vb: Find()` |
| 蛋白质序列聚类 | CD-HIT (Li & Godzik 2006) | 分块策略 |
| DIAMOND 比对 | Buchfink et al. (2015) | `DiamondRunner.vb` |
| 单连锁聚类 (transitive closure) | Jardine & Sibson (1971) | DSU.Union 实现 |
