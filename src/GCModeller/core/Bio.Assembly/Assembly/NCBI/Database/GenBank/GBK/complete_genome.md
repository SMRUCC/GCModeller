判断一条 GenBank 记录是否为"完整组装的染色体基因组"，**不能只看单一字段，而需组合 `completeness`、accession 前缀（是否为 WGS）、`/chromosome` 限定符、DEFINITION 关键词等多重证据**。泛基因组分析需要的是有明确染色体定位、无 gap、非草图的记录，下面给出可直接落地的判定规则。
## 一、完整组装 vs 草图：各字段判断依据
INSDC/NCBI 对 WGS（Whole Genome Shotgun）项目的定义是"未完成的基因组或未完成的染色体"组装，其记录采用 4 字母（2019 年后改为 6 字母）WGS 前缀 + 版本号 + 重叠群编号的 accession 格式，例如 `AAAA02002744` 表示项目 AAAA 第 2 版第 002744 号 contig；而真正完整组装的染色体/质粒以普通 GenBank 记录（如 `NC_000913`、`U00096`）形式提交，一条记录对应一条染色体或一个质粒。
| 判断字段 | 位置 | 完整组装的典型取值 | 草图/不完整信号 | 可靠性 |
|---|---|---|---|---|
| Accession 格式 | LOCUS 行 | 2 字母+5/6 位数字（如 `U00096`、`AE000511`）或 RefSeq `NC_` 前缀 | WGS 格式：4/6 字母+版本+重叠群号（如 `AAAA02000001`、`JAAQRI01xxxxx`）；或 WGS master（如 `AAAA00000000`） | 高（最直接区分 WGS/非 WGS） |
| `completeness` | source 限定符 | `full` | `partial` | 高 |
| `/chromosome` | source 限定符 | 存在且非空（如 `/chromosome="1"`） | 缺失（contig/scaffold 通常没有 chromosome 限定） | 高 |
| DEFINITION 行 | 头部第 2 行 | 含 `complete genome` 或 `complete sequence`，且带 `chromosome X` 描述 | 仅含 `genome assembly`、`contig`、`scaffold` 等 | 中高 |
| LOCUS 拓扑 | 头部第 1 行 | 真细菌染色体多为 `DNA circular`（线粒体也常见 circular，需结合其他字段） | 不确定，仅辅助 | 低 |
| `assembly_gap` feature | FEATURES 表 | 无或极少 | 大量 `assembly_gap` feature | 高（有 gap 即非 complete） |
| `Assembly-Data` 结构化注释 | 结构化注释块 | `Finishing Goal: complete`（如有） | `Finishing Goal: standard draft` / 缺失 | 中（依赖提交者填写） |
| KEYWORDS | 头部第 5 行 | 可能含 `complete genome` | 多为空或含 WGS 相关词 | 中 |
对结构化注释：NCBI 的 GenomeAssembly-Data 结构化注释要求填写 Assembly Name、Assembly Method、Genome Coverage、Sequencing Technology 等字段，但 Finishing Goal（"complete" vs "standard draft"）属于 MIGS 合规字段而非强制字段，缺失较常见。
## 二、程序化判定流程
按以下优先级组合判断，任一高优先级条件命中即可定性：
```python
from Bio import SeqIO
import re
def is_complete_chromosome(gb_file):
    """判断 GenBank 记录是否为完整组装的染色体（非 WGS、非 contig/scaffold）"""
    record = SeqIO.read(gb_file, "genbank")
    
    # 1. Accession 格式判断（排除 WGS）
    # WGS master: XXXX00000000 或 XXXXXX000000000（4/6字母后接多个0）
    # WGS contig: XXXX0N000001 格式
    acc = record.id.split(".")[0]
    if re.match(r'^[A-Z]{4}0\d{7}$', acc) or re.match(r'^[A-Z]{6}0\d{9}$', acc):
        return False, "WGS_master"
    if re.match(r'^[A-Z]{4}\d{8}$', acc) or re.match(r'^[A-Z]{6}\d{10,}$', acc):
        # 进一步排除：WGS contig 的第5-6位通常是版本号01-99
        return False, "WGS_contig/scaffold"
    
    # 2. source feature 的 completeness 与 chromosome
    src = next((f for f in record.features if f.type == "source"), None)
    if src:
        q = src.qualifiers
        # 排除 partial
        if "partial" in q.get("completeness", [""])[0]:
            return False, "partial_record"
        # 排除无 chromosome 定位的（可能为未完成染色体或随机 contig）
        chr_name = q.get("chromosome", [""])[0]
        if not chr_name:
            # 兜底：查 DEFINITION 是否明确说 complete genome
            if "complete genome" not in record.description.lower():
                return False, "no_chromosome_no_complete_tag"
    
    # 3. DEFINITION 关键词
    desc = record.description.lower()
    if any(kw in desc for kw in ["contig", "scaffold", "draft", "shotgun"]):
        return False, "draft_keyword"
    if "complete genome" in desc or "complete sequence" in desc:
        return True, "complete_tagged"
    
    # 4. assembly_gap 数量检查
    gaps = sum(1 for f in record.features if f.type == "assembly_gap")
    if gaps > 0:
        return False, f"has_{gaps}_gaps"
    
    # 5. 结构化注释兜底（如果存在）
    sc = getattr(record, "annotations", {}).get("structured_comment", {})
    asm = sc.get("GenomeAssembly-Data", {}) or sc.get("Assembly-Data", {})
    if asm.get("Finishing Goal") == "complete":
        return True, "structured_comment_complete"
    if asm.get("Finishing Goal") in ("standard draft", "improved high-quality draft"):
        return False, f"structured_comment_{asm['Finishing Goal']}"
    
    return None, "undetermined"
# 批量筛选
if __name__ == "__main__":
    import glob
    for f in glob.glob("*.gb"):
        verdict, reason = is_complete_chromosome(f)
        if verdict is True:
            print(f"KEEP    {f}: {reason}")
        elif verdict is None:
            print(f"MANUAL  {f}: {reason}")
        else:
            print(f"EXCLUDE {f}: {reason}")
```
关于 accession 正则的说明：2019 年 1 月前 WGS 使用 4 字母前缀 + 8 位数字（版本 2 位 + contig 6 位），2019 年后改为 6 字母前缀 + 至少 9 位数字，master accession 后接多位 0。
## 三、NCBI 官方推荐路径与局限性
**如果输入是一批 GenBank 文件（.gbff）**，除上述单文件判定外，更稳妥的方法是结合 NCBI 的 Assembly 元数据：
- 通过 NCBI Datasets CLI（`datasets download genome accession XXXX --include genome`）或 Entrez API 查询该 accession 对应的 Assembly 记录，读取其 `assembly_level` 字段（`complete genome` / `chromosome` / `scaffold` / `contig`）以及 `submitter` 提供的 finishing goal。Assembly 数据库中 `assembly_level=Complete Genome` 表示"染色体序列无 gap"。
- 在 Entrez 检索时使用 `complete[prop]` 等属性过滤，或用 `"Finishing Goal"[Assembly] AND "complete"` 等查询语句筛选。
**局限性提示**：`Assembly-Data` / `GenomeAssembly-Data` 结构化注释并非所有记录都填写（尤其是 2014 年之前或第三方重提交的记录），KEYWORDS 和 DEFINITION 属于自由文本可能不规范，所以单一字段都有假阴性/假阳性风险，组合判定 + 最终与 Assembly 元数据交叉验证是最可靠的方案。
## 四、泛基因组分析的取数建议
针对泛基因组（如 Panaroo、Roary、Parsnp 等工具）的输入要求，选样时建议做三重过滤：
1. **先用 accession 正则排除 WGS 草图**（最硬的过滤条件，一步排除绝大多数 contig/scaffold 记录）。
2. **再用 `completeness=full` + `chromosome` 存在 + DEFINITION 含 `complete genome` 三条联合确认**，避免误杀 RefSeq 补充记录或非常规命名的完整基因组。
3. **对多组件物种**（一条染色体对应多个 GenBank 记录，如真核基因组每条染色体一条记录），需要按 BioProject/BioSample ID 聚合，把同一菌株/个体的所有染色体和质粒记录合并后再送入泛基因组流程；可通过 `/db_xref` 中的 BioProject 和 BioSample accession 串联。
**排除线粒体/质粒的额外信号**：source 限定符 `/organelle="mitochondrion"` 或 `/plasmid="pXXX"` 存在时，即使 completeness=full 也应从泛基因组染色体数据集中剔除，因为泛基因组分析通常只关注核基因组主染色体；这一规则可叠加到上面代码的 source feature 检查步骤中（在判断 chromosome 之前先排除 organelle/plasmid）。
