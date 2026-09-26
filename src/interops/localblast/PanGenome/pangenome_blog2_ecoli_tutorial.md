# 用 GCModeller 做泛基因组分析（二）：801 个大肠杆菌基因组实战

> 面向读者：具有一定 R 语言编程基础的生物信息学初学者、实验室里的微生物研究者。
> 本篇是系列的实战篇：从 GenBank 原始注释文件出发，用两条 R# 脚本跑完大肠杆菌的泛基因组分析，
> 并结合真实运行产出的结果文件逐项解读。算法原理（CD-HIT 聚类、PAV 矩阵、香农信息熵等）
> 请先阅读系列第一篇《用 GCModeller 做泛基因组分析（一）：算法原理与香农信息熵》。

---

## 1. 实战目标

我们要做的事情一句话就能说清：

> **拿 801 个大肠杆菌（*Escherichia coli*）基因组的 GenBank 注释文件，跑一遍 GCModeller 泛基因组分析流程，弄清楚：大肠杆菌这个物种的基因池有多大？哪些基因是大家都有的？哪些基因组"最不安分"？**

整个过程只需要两条 R# 脚本：

| 脚本 | 任务 | 输入 → 输出 |
|---|---|---|
| `pack_genes.R` | 数据准备 | `*.gbff` → `proteins.faa` + `genes.csv` |
| `cdhit_family.R` | 泛基因组分析 | 上两者 → `result.html` + 一组 CSV 统计表 |

这两条示例脚本随 GCModeller 一同提供（可在 GCModeller 安装包的示例脚本集中找到，也可以直接从本文复制使用），下面逐段精讲。

## 2. 准备工作

### 2.1 数据从哪里来

到 NCBI Assembly 数据库检索 *Escherichia coli*，筛选 **Complete Genome / Chromosome** 完成度的 assembly，批量下载它们的 GenBank 注释文件（`*.gbff`）。建议顺便下载 assembly 级别的汇总表，用于核对菌株来源、血清型等信息——后面的结果解读会用到。

把所有 `.gbff` 文件放进一个文件夹（名称与存放位置随意，下文统一以 `./gbff/` 指代）：

```
gbff/                       ← 存放几百上千个 *.gbff 文件
```

> **为什么要挑完成度高的基因组？** 草图（scaffold/contig）级别的组装会把完整基因错误地切断在 contig 边界上，让"基因缺失"的判断失真。泛基因组分析对注释完整性非常敏感。

### 2.2 软件

- GCModeller 运行时 + R# 解释器（本流程用到 `seqtoolkit`、`comparative_toolkit` 两个 R# 包）；
- 硬件上，几百个基因组的规模在普通工作站上即可完成（本次 801 基因组的实测：序列聚类约十几分钟，全流程分析数分钟内完成，结果归档约 168 MB）。

## 3. 第一步：`pack_genes.R` —— 从 GenBank 库导出分析原料

### 3.1 完整脚本

```r
require(GCModeller);

imports "GenBank" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

# step 1: prepare annotation data set

# R# ./pack_genes.R --genbank ./data

let src     = ?"--genbank" || stop("no genbank source was provided!");
let result  = ?"--out"     || file.path(src, "result");
let takes_n = ?"--takes"   || 10000;
let gbff    = load_genbanks( list.files(src,"*.gbff" ), extract_genomics = TRUE);
let table   = c();
let targets = open.fasta(file.path(result,"proteins.faa"), read=  FALSE);

for(let gb in gbff |> take(takes_n)) {
    print(accession_id(gb));

    # write protein sequence into the file stream
    table = c(table, as_tabular(gb));
    write.fasta(protein_seqs(gb, title = "<locus_tag>.<gb_asm_id>"),
        file = targets,
        filter.empty = TRUE);
}

# export gene table and flush the protein sequence stream
write.csv(table, file = file.path(result,"genes.csv"));
close(targets);
```

运行方式：

```
R# ./pack_genes.R --genbank ./gbff
```

### 3.2 逐段讲解

**① 命令行参数。** `?"--genbank"` 读取命令行参数；`|| stop(...)` 是"没有参数就直接报错退出"的写法。`--out` 与 `--takes` 都给了默认值（输出到源文件夹下的 `result` 子目录、最多处理一万基因组）。

**② `load_genbanks(..., extract_genomics = TRUE)`**——这一步有个容易被忽略但非常关键的开关：打开它之后，加载器只保留**染色体级别的基因组序列**，自动过滤掉质粒、噬菌体等单独复制子。泛基因组分析关心的是物种核心染色体层面的基因组成，让成百上千个质粒混进来会把"家族存在率"搅得面目全非。

**③ 流式写出蛋白序列。** `open.fasta(..., read = FALSE)` 以**写入模式**打开一个 FASTA 文件流，循环里每处理完一个基因组就 `write.fasta` 追加进去，最后 `close` 落盘。这样做的好处是内存里永远只保留当前一个基因组的数据——几百万条蛋白序列也不会撑爆内存。

**④ FASTA 序列的命名模板 `"<locus_tag>.<gb_asm_id>"`。** 这是整个流程的"暗号系统"：

- `<locus_tag>`：基因在注释文件中的唯一编号（如 `b0001`）；
- `<gb_asm_id>`：该基因组在 NCBI 的 assembly 编号。

也就是说，每条蛋白序列的名字同时携带了**"我是哪个基因"** 和 **"我来自哪个基因组"** 两层信息。下游的共线性分析、家族成员定位、结构变异检测全部依靠解析这个名字来还原基因的基因组归属。

**⑤ `as_tabular(gb)` → `genes.csv`。** 把每个基因座的坐标（起始/终止位置、链方向、所属染色体、基因组编号）整理成表格。后续 `build_context` 靠这张表知道"每个基因长在哪个基因组的哪个位置"——这也是泛基因组曲线、共线性区块、结构变异定位的坐标来源。

跑完后 `result` 文件夹里多了两个文件：

```
result/
├── proteins.faa    ← 801 个基因组约 380 万条蛋白序列
└── genes.csv       ← 每个基因在基因组上的位置信息
```

## 4. 第二步：`cdhit_family.R` —— 泛基因组分析主流程

### 4.1 完整脚本

```r
require(GCModeller);
require(jsonlite);

# step 2: pan-genome analysis and report

# R# ./cdhit_family.R --dir ./data/result

imports "bioseq.fasta" from "seqtoolkit";
imports "kmers" from "seqtoolkit";
imports "pangenome" from "comparative_toolkit";

let dir as string = ?"--dir" || stop("no analysis data provided!");
let family_json   = file.path(dir, "cdhit-family.json");
let family_result = {
    if (file.exists(family_json )) {
        # use cache
        jsonlite::fromJSON(family_json , what = "cdhit-family");
    } else {
        # make protein family clustering
        let proteins = read.fasta(file.path(dir, "proteins.faa"));
        let cache_data = cdhit_clusters(proteins, identities = 0.6);

        # write the cache data
        writeLines(jsonlite::toJSON(cache_data$clusters),
            con = family_json );

        cache_data$clusters;
    }
};
# load gene annotation and gene family groups
# then run pan-genome analysis
let orth    = multiple_genome_alignment( family_groups(family_result));
let geneset = read_genetable(file.path(dir,"genes.csv"));
let context = build_context(geneset,soft_core_threshold = 0.8,genome_size = c(2000,8000), uniqueByAcc=TRUE);
let result  = pangenome::analysis(context, orth);

# save the analysis result
writeBin(result, con = file.path(dir, "result.zip"));

# export the result data files
let scatter = pangenome::scatter_set(result);

# genome gene counts table
write.csv(as.data.frame(scatter$stats), file = file.path(dir, "genome_stats.csv"));
# pav entropy PCA result
write.csv(as.data.frame(scatter$pca), file = file.path(dir, "pav_pca.csv"));
# pav entropy data
write.csv(as.data.frame(scatter$entropy), file = file.path(dir, "pav_entropy.csv"));

write.csv(genetic_distance(result), file = file.path(dir, "genetic_distance.csv"));
write.csv(pav_matrix(result), file = file.path(dir, "pav_matrix.csv"));
write.csv(curve_data(result), file = file.path(dir, "pangenome_curve.csv"));
write.csv(sv_table(result ), file = file.path(dir, "sv_table.csv" ));
write.csv(pav_table(result), file = file.path(dir,"pav_table.csv"));

# SV structural variation matrices: rows are gene families, columns are genomes
write.csv(sv_copy_number_matrix(result), file = file.path(dir, "sv_copy_number_matrix.csv"));
write.csv(sv_median_matrix(result), file = file.path(dir, "sv_median_matrix.csv"));

# gene family distribution percent matrix, by two calibers:
# "gene" = copy number based, "family" = family count based
write.csv(category_percent_matrix(result, by = "gene"), file = file.path(dir, "category_percent_gene.csv"));
write.csv(category_percent_matrix(result, by = "family"), file = file.path(dir, "category_percent_family.csv"));

# SV information entropy scatter data with the kmeans clustering result
write.csv(as.data.frame(sv_entropy(result)), file = file.path(dir, "sv_entropy.csv"));
# make export html report result
writeLines(report_html(result), con = file.path(dir,"result.html"));
```

运行方式：

```
R# ./cdhit_family.R --dir ./gbff/result
```

### 4.2 逐段讲解

**① 蛋白家族聚类 + JSON 缓存。**

```r
let cache_data = cdhit_clusters(proteins, identities = 0.6);
```

`identities = 0.6` 表示序列同一性达到 60% 即归入同一家族——这是比较宽松的"家族"粒度，能把同源但分化较远的蛋白聚在一起（想得到更接近"直系同源"的严格粒度，可以调高到 0.9 以上，代价是家族数量增多）。

紧接着的 `if (file.exists(...))` 是一个实用技巧：首次运行把聚类结果写成 `cdhit-family.json` 缓存，之后无论重跑多少遍分析（比如调参数、改阈值），都直接读缓存，**不再重复几个小时级的序列聚类**。386 万条蛋白序列的聚类不是小活儿，这个缓存值得。

**② `family_groups` + `multiple_genome_alignment`。** 把聚类结果整理成"家族 → 成员基因列表"，再按基因组归并成同源配对数据。此时每条序列名里的 `<locus_tag>.<gb_asm_id>` 暗号就派上了用场——基因组归属全靠它解析。

**③ `build_context(...)`——三个重要参数。**

```r
build_context(geneset, soft_core_threshold = 0.8,
              genome_size = c(2000,8000), uniqueByAcc = TRUE)
```

| 参数 | 取值 | 含义 |
|---|---|---|
| `soft_core_threshold` | 0.8 | 出现比例 ≥ 80% 的家族算软核心基因（默认 0.95，这里放宽） |
| `genome_size` | c(2000, 8000) | **基因数过滤**：基因数不在这个区间的基因组直接剔除，防止退化基因组、异常组装或混进来的质粒富集样本拉偏全局统计 |
| `uniqueByAcc` | TRUE | 按 accession 去重，避免同一基因组的重复记录混入 |

**④ `pangenome::analysis(context, orth)`。** 一行代码触发完整分析流水线（内部依次执行）：PAV 矩阵构建 + 家族四分类 → 遗传距离 → 共线性区块 → 结构变异检测 → 泛基因组曲线 → 基因组统计/PCA/PAV 熵 → SV 信息熵 + KMeans 聚类。

**⑤ `writeBin(result, ...)`。** 把整个分析结果对象打包归档成 `result.zip`（本次实测约 168 MB）。之后在任何 R# 会话里用 `readBin.pangenome` 重新载入即可做后续分析，**不必重跑流程**。

**⑥ 结果导出全家福。** 脚本后半段就是把结果对象的各个组成部分导出为 CSV（每张表的含义见下一节），最后 `report_html` 生成自包含的可视化 HTML 报告。

## 5. 结果解读：801 个大肠杆菌基因组

下面所有数字均来自一次真实的 801 基因组分析运行所导出的结果文件夹（即 `--dir` 参数指向的输出目录）。先看规模总览：

| 项目 | 数值 |
|---|---|
| 参与分析的基因组数 | **801** |
| 参与分析的蛋白编码基因总数 | 约 **380 万** |
| 聚出的基因家族总数（泛基因组大小） | **252,301** |
| 检出的结构变异（SV）事件 | **822,101** |
| 参与 SV 熵聚类分析的家族 | 1,282（经 5% 频率过滤） |

### 5.1 `pangenome_curve.csv`：一个教科书级的开放泛基因组

泛基因组曲线（蒙特卡洛 100 次模拟的平均）的关键采样点：

| 加入基因组数 | 泛基因组大小 | 核心基因（100%） | 软核心基因（≥80%） |
|---|---|---|---|
| 1 | 4,668 | 4,668 | 4,668 |
| 10 | 11,792 | 2,433 | 3,181 |
| 50 | 29,254 | 1,448 | 3,047 |
| 100 | 46,483 | 935 | 3,031 |
| 200 | 78,254 | 433 | 2,993 |
| 400 | 137,203 | 139 | 2,996 |
| 600 | 194,723 | 50 | 3,008 |
| **801** | **252,301** | **16** | **2,996** |

三个读数：

1. **泛基因组曲线到第 801 个基因组仍在陡峭上升**（平均每个新基因组还带来约 290 个新家族）——大肠杆菌是典型的**开放泛基因组**，基因池远未见底，这与文献中长期以来的结论一致，根源是活跃的水平基因转移（质粒、前噬菌体、毒力岛）。
2. **100% 核心基因坍缩到只剩 16 个**。别慌——这不是数据错误，而是"全员必修课"的判定在 801 个基因组面前过于苛刻：任何一个基因组少注释一个基因，该家族就出局。核心池的真实功能规模应该看软核心。
3. **软核心稳定在约 3,000 个家族**，不再随基因组数量衰减——这才是大肠杆菌" chromosome 骨架功能"的稳定底座：翻译、糖酵解、DNA 复制这些看家本领。

### 5.2 `category_percent_gene.csv`：每个基因组的基因构成

四类家族在每个基因组内部的平均占比（按基因数口径）：

| 类别 | 平均占比 |
|---|---|
| 核心基因 | 0.36% |
| 软核心基因 | **63.37%** |
| 壳基因 | 20.19% |
| 云基因 | 16.08% |

单看任意一个基因组：约 63% 的基因属于软核心（物种的"公共财产"），但有超过三分之一的基因（壳 + 云 ≈ 36%）属于附属基因组——这就是为什么两个大肠杆菌菌株的基因组成可以差出上千个基因。

> 脚本同时导出了 `category_percent_family.csv`（按家族数口径）。两种口径的差异在于多拷贝家族的权重：按基因数计，一个有 10 个拷贝的云基因家族占 10 个基因的份额；按家族数计它只算 1 票。对比两张表可以判断附属基因组的膨胀主要靠"新家族"还是靠"拷贝数扩张"。

### 5.3 `genome_stats.csv` + `pav_entropy.csv`：基因组可塑性排行榜

`genome_stats.csv` 记录每个基因组的基因数、特有基因数、核心占比。本次数据集的基因数跨度为 **2,434 – 5,948**，相差 2.4 倍。

`pav_entropy.csv` 就是第一篇博客 8.2 节讲的**存在/缺失均衡度熵**，本次全体基因组的熵值区间为 **0.054 – 0.107 nat**（平均 0.092）。看几个有代表性的样本：

| 基因组 | 熵 H（nat） | 存在家族数 | 特有基因占比 | 基因数 | 背景注释 |
|---|---|---|---|---|---|
| K-12 substr. MG1655 | **0.0541**（最低之一） | 2,421 | 2.5% | 2,434 | 分子生物学的"标准实验菌株" |
| K-12 substr. MDS42 | 0.0738 | 3,542 | 0.7% | 3,547 | 经系统性缺失非必需序列的**基因组精简株** |
| O26 str. RM10386 | 0.1053 | 5,522 | 6.6% | 5,845 | 致病血清型 |
| O157:H7 FRIK2069 | **0.1071**（最高） | 5,645 | 7.3% | 5,948 | 肠出血性大肠杆菌（EHEC） |

这个排行榜完美演示了熵这把尺子的生物学分辨率：

- **熵最低的正是实验室驯化株**。K-12 MG1655 在实验室条件下传代数十年，丢失了大量自然环境才需要的附属基因；MDS42 更是人为敲掉了约 0.86 Mb 非必需序列的"减肥版"基因组。它们的 p 值（存在比例 2421/252301 ≈ 0.0096）极度偏离 0.5，熵自然最低。
- **熵最高的都是致病菌株**。O157:H7 携带大量毒力岛、前噬菌体和 O 抗原合成基因簇（特有基因占比 7.3%，是 K-12 的近三倍），把存在比例往 0.5 的方向推了一把，熵值显著走高。

> 在同一数据集内部做横向比较时，由于 p 远小于 0.5，熵值大小基本与"基因组携带的家族数"单调相关；但熵同时编码了缺失比例信息，且是单一的连续型指标——**按血清型/宿主来源/分离部位分组画熵的箱线图**，或把它作为 GWAS 的量化表型，都是直接的用法。

### 5.4 `genetic_distance.csv`：801 × 801 的 Jaccard 距离矩阵

全体基因组两两之间的 Jaccard 距离（第一篇博客第 6 节），矩阵均值 **0.424**，最大 **0.882**——任意两个大肠杆菌菌株平均有约四分之一的家族组成差异，最极端的基因组对之间甚至没有一半的家族相同。这张矩阵可以直接喂给 R 的层次聚类或 NMDS，按血清型着色后通常能看到清晰的分型结构。

### 5.5 `sv_table.csv` + `sv_entropy.csv`：结构变异与家族进化模式

`sv_table.csv` 共 **822,101** 条结构变异事件，类型分布：

| SV 类型 | 数量 | 说明 |
|---|---|---|
| PAV_Presence | 233,949 | 特有基因获得 |
| PAV_Absence | 261,897 | 高频家族的缺失 |
| CNV_Gain | 4,975 | 拷贝数扩张 |
| CNV_Loss | 880 | 拷贝数收缩 |
| Collinearity_Break | 320,400 | 跨染色体共线性断裂（易位信号） |

PAV 类事件占据绝对主导（约 60 万条），说明大肠杆菌种内的结构多样性主要由**基因的有无**驱动；真正的拷贝数变异只有约 6 千条——剂量变异是相对罕见的事件。

接下来是本流程最具特色的分析：**SV 信息熵散点 + KMeans 聚类**（原理见第一篇 8.4 节）。`sv_entropy.csv` 包含 1,282 个有足够 SV 数据的家族（出现频率 ≥ 5% 已过滤），聚成 4 簇。把导出的 CSV 按簇求均值，可以还原每个簇的进化画像：

| 簇 | 家族数 | 平均 H<sub>cn</sub> | 平均 H<sub>med</sub> | 进化画像 |
|---|---|---|---|---|
| 0 | 356 | 0.017 | 0.472 | 拷贝数高度一致，SV 多态性略低于平均 → 偏**僵化/保守型** |
| 1 | 487 | 0.030 | **0.646** | 剂量稳定，但 SV 有/无接近五五开（注意 ln2 ≈ 0.693，0.646 已逼近二状态分布的熵上限！）→ **结构微调型**方向 |
| 2 | 368 | 0.049 | 0.276 | 两维都低 → **僵化/保守型** |
| 3 | 71 | **3.43** | 0.572 | 拷贝数熵比其他簇高两个数量级 → **混沌/快速进化型** |

几个值得咀嚼的观察：

- **cluster 1 的 $H_{med} \approx 0.65$** 意味着这些家族的 SV 状态在群体中几乎对半开——一半基因组有这个 SV、一半没有。这种"高度摇摆"的家族大概率与菌株特异的适应性性状相关，是功能研究的好候选。
- **cluster 3 只有 71 个家族，但 $H_{cn}$ 高达 3.4**：它们的拷贝数在 0 到多拷贝之间剧烈波动（转座元件、重复序列相关家族的典型指纹）。这 71 个家族值得逐个拎出来在基因组上定位，看是否富集在已知的重组热点。
- 本次数据集没有出现典型的"剂量调谐型"（高 $H_{cn}$ + 低 $H_{med}$）簇——大肠杆菌的 SV 进化主要不是靠"统一结构的剂量调节"，这与它以水平转移（整段基因的获得/丢失）而非串联拷贝数微调为主的基因组演化方式相符。

> 提示：报告页面中的簇象限标签（保守型/剂量调谐型/混沌型/结构微调型）由簇均值与全局均值自动比较生成；CSV 中的簇编号经过确定性重排，同一份数据重跑分析得到的编号不会漂移。

### 5.6 `result.html`：一键生成的可视化报告

最后 `report_html(result)` 产出的自包含 HTML 报告（本次约 1.8 MB）内置以下交互图表，方便不做编程的协作者直接浏览：

- **分析概述**：基因组数、家族数、四分类汇总；
- **基因家族分类**：核心/软核心/壳/云的饼图与占比条形图；
- **泛基因组曲线**：泛基因组/核心/软核心三条曲线；
- **PAV 矩阵分析**：基因组三维散点（PAV 熵 × 特有基因占比 × 核心基因占比）+ PCA 散点；
- **结构变异检测**：SV 事件统计与 **SV 信息熵散点图（含 KMeans 着色）**；
- **共线性分析**与**遗传距离分析**；
- **基因家族详情**：逐家族的 PAV 明细检索。

## 6. 参数调节建议与常见问题

**Q1：`identities = 0.6` 太宽松/太严格怎么办？**
0.6 得到的是"蛋白家族"粒度，适合泛基因组结构分析。若关心直系同源级别的一对一关系（比如做物种树），建议提高到 0.9 以上，或改走 BBH（双向最佳命中）路线——`analysis` 接口同时兼容两种输入。

**Q2：`soft_core_threshold` 取 0.8 还是 0.95？**
基因组数量大（数百以上）时，0.95 的核心集合容易受个别基因组的注释缺失扰动，0.8 更稳健；基因组数量少（几十个）时可以直接用 0.95 甚至看 100% 核心。

**Q3：分析报"家族存在率"异常，先检查什么？**
- 是否用 `extract_genomics = TRUE` 过滤了质粒？
- 是否设置了 `genome_size` 过滤区间（本例 c(2000, 8000)）？
- FASTA 序列名是否保持 `<locus_tag>.<gb_asm_id>` 模板？名字解析失败会导致基因组归属错乱。

**Q4：重跑流程每次都要重新聚类吗？**
不需要。`cdhit-family.json` 是聚类缓存；`result.zip` 是全部分析结果的归档，用 `readBin.pangenome` 载入后可以直接取任何结果数据。

**Q5：换一个物种要改脚本吗？**
不用。流程对任何细菌物种通用——替换 `--genbank` 指向的文件库即可。基因组数量、家族规模变化后，建议同步复核 `genome_size` 过滤区间与软核心阈值。

## 7. 结语

回顾整个实战流程，GCModeller 把泛基因组分析压缩成了两条脚本、一次点击式的命令：

```
R# ./pack_genes.R     --genbank ./gbff           # ① 导出序列与注释
R# ./cdhit_family.R   --dir     ./gbff/result    # ② 聚类 + 全流程分析 + 报告
```

801 个大肠杆菌基因组告诉我们：这是一个开放泛基因组——软核心约 3,000 个家族构成稳定的物种骨架，其余 25 万个家族在群体中以壳与云的形态流动；实验室驯化株与致病株在香农熵的尺子上相距近一倍；而 SV 双熵聚类把基因家族按进化模式分成了保守、摇摆与混沌等截然不同的类群。

把同一套流程放到你的物种上——病原菌的毒力因子分析、耐药菌的附属基因组普查、环境菌株的适应潜力评估——不过是换一个数据文件夹的事情。

**系列导航**

- 第一篇：《用 GCModeller 做泛基因组分析（一）：算法原理与香农信息熵》
- 第二篇：本文
