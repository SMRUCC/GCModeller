可以通过 GenBank 文件中的 **source feature 判断分子类型**，但由于 source 下的 `/mol_type` 限定符对于核基因组、质粒和细胞器基因组往往都返回 `genomic DNA`，单纯依赖它会失效，实践中需要将 `/organelle`、`/plasmid`、`/chromosome` 限定符与 LOCUS/DEFINITION 行的关键词组合判断。
## 一、各字段的判断依据
GenBank flat file 中与分子来源相关的字段分散在 LOCUS 头部、DEFINITION 行、KEYWORDS 行以及 FEATURES 表的 source feature 限定符中，各自的可靠性不同：
| 判断字段 | 位置 | 典型取值 / 示例 | 判断含义 | 可靠性 |
|---|---|---|---|---|
| `/organelle` | source 限定符 | `mitochondrion`、`mitochondrion:kinetoplast`、`plastid:chloroplast`、`plastid:apicoplast`、`hydrogenosome`、`nucleomorph`、`chromatophore`、`nitroplast` | 存在即判定为细胞器基因组（线粒体/叶绿体等） | 高（官方受控词表） |
| `/plasmid` | source 限定符 | `/plasmid="pPCP1"`、`/plasmid="C-589"` | 存在即判定为质粒 | 高 |
| `/chromosome` | source 限定符 | `/chromosome="IX"`、`/chromosome="1"` | 存在（且无 /organelle、/plasmid）时倾向核基因组 | 中 |
| `/mol_type` | source 限定符 | `genomic DNA`、`genomic RNA`、`mRNA` 等 | **仅区分 DNA/RNA，不区分核/质粒/细胞器**（官方明确指出 organelle 和 plasmid DNA 也用 `genomic DNA`） | 低（对分子定位无区分度） |
| LOCUS 行 KEYWORDS | 头部第 5 行 | 常为 `.`（空），老记录可能出现 `mitochondrion`、`chloroplast`、`plasmid` 等 | 关键词提示，作为兜底 | 中（非必填，新记录多为空） |
| DEFINITION 行 | 头部第 2 行 | `Homo sapiens mitochondrion, complete genome`、`... plasmid pPCP1, complete sequence` | 自由文本关键词匹配 | 中（命名规范，但无强约束） |
| LOCUS 行 molecule/topology | 头部第 1 行 | `DNA circular`、`DNA linear` | 环形拓扑可提示质粒或某些细胞器基因组，但不能单独定性 | 低（仅辅助） |
INSDC 官方文档对 source feature 的强制限定符只有 `/organism` 和 `/mol_type`，而 `/organelle`、`/plasmid`、`/chromosome` 均为**可选限定符**，因此依赖单一字段必然存在漏检。
## 二、程序化判断流程与示例代码
推荐的判断优先级为：**source 限定符（结构化）→ DEFINITION/KEYWORDS 关键词（自由文本兜底）→ mol_type/topology（辅助）**。使用 Biopython 的 `SeqIO` 解析后，source feature 通常是 `record.features[0]`，其限定符可通过 `feature.qualifiers` 字典访问。
```python
from Bio import SeqIO
def classify_genbank_record(gb_file):
    """判断 GenBank 记录是核基因组、质粒还是细胞器基因组"""
    record = SeqIO.read(gb_file, "genbank")
    
    # 1. 提取 source feature（通常为第一个 feature）
    src = next((f for f in record.features if f.type == "source"), None)
    if src is None:
        return "unknown", {}
    
    q = src.qualifiers
    # 2. 提取 LOCUS/DEFINITION 头部信息作为兜底
    header_text = " ".join([
        str(record.annotations.get("keywords", "")),
        record.description,  # DEFINITION 行
    ]).lower()
    
    # 3. 按优先级判断
    # 优先级 1: /organelle 限定符（最权威）
    organelle = q.get("organelle", [""])[0].lower()
    if organelle:
        if "mitochondrion" in organelle or "kinetoplast" in organelle:
            return "mitochondrion", {"organelle": organelle}
        if "plastid" in organelle or "chloroplast" in organelle:
            return "chloroplast", {"organelle": organelle}
        return f"organelle:{organelle}", {"organelle": organelle}
    
    # 优先级 2: /plasmid 限定符
    plasmid = q.get("plasmid", [""])[0]
    if plasmid:
        return "plasmid", {"plasmid": plasmid}
    
    # 优先级 3: /chromosome 限定符（无 organelle/plasmid 时倾向核基因组）
    chromosome = q.get("chromosome", [""])[0]
    if chromosome:
        return "nuclear_genome", {"chromosome": chromosome}
    
    # 优先级 4: DEFINITION/KEYWORDS 关键词兜底
    for kw, label in [
        ("mitochondrion", "mitochondrion"),
        ("mito", "mitochondrion"),
        ("chloroplast", "chloroplast"),
        ("plastid", "chloroplast"),
        ("kinetoplast", "mitochondrion"),
        ("apicoplast", "chloroplast"),
    ]:
        if kw in header_text:
            return label, {"source": "header_keyword"}
    for kw in ["plasmid"]:
        if kw in header_text:
            return "plasmid", {"source": "header_keyword"}
    
    # 优先级 5: mol_type 仅作为辅助（无法区分核/质粒/细胞器）
    mol_type = q.get("mol_type", [""])[0]
    return "unknown_likely_nuclear", {
        "mol_type": mol_type,
        "topology": record.annotations.get("topology", ""),
    }
# 批量处理示例
if __name__ == "__main__":
    import glob
    for f in glob.glob("*.gb"):
        label, detail = classify_genbank_record(f)
        print(f"{f}: {label} | detail={detail}")
```
代码中 `record.annotations` 的 `keywords`、`source`、`organism`、`topology`、`molecule_type` 等字段分别对应 GenBank flat file 头部的 KEYWORDS、SOURCE、ORGANISM、LOCUS 行信息。
## 三、边界情况与注意事项
实际使用中需要注意以下几点，否则容易误判：
**`/mol_type` 不提供分子定位信息。** INSDC 官方注释明确写道："the value 'genomic DNA' does not imply that the molecule is nuclear (e.g. organelle and plasmid DNA should be described using 'genomic DNA')"，即核基因组、质粒、线粒体、叶绿体的 mol_type 都可能是 `genomic DNA`，必须依赖其他限定符。
**可选限定符可能缺失。** `/organelle`、`/plasmid`、`/chromosome` 并非强制，尤其在早期提交或第三方重注释的记录中可能完全缺失，此时需回退到 DEFINITION/KEYWORDS 的关键词匹配。
**可能出现组合情况。** 例如线粒体质粒会同时带 `/organelle="mitochondrion"` 和 `/plasmid="..."` 两个限定符，此时应根据业务需求决定优先级——若目的是区分"线粒体 vs 质粒"，通常以 `/organelle` 为准；若关注"是否为质粒序列"，则 `/plasmid` 的存在即可作为判定依据。
**环形拓扑不能单独判定为质粒。** LOCUS 行的 `circular` 也见于某些线粒体基因组（如动物 mtDNA）、叶绿体基因组以及部分细菌染色体，因此仅能作为辅助信号。
**多 source feature 情况。** 一条记录理论上可以有多个 source feature（例如跨物种载体序列），此时第一个 source feature 不一定代表目标分子，应遍历所有 source feature 并检查覆盖全序列的那一个。
**RefSeq 与原始 GenBank 记录的差异。** RefSeq 记录（`NC_`、`NC_` 开头）的注释通常更规范，而直接提交的原始记录（`A~Z` 开头的登录号）头部关键词使用更随意，兜底匹配时建议使用不区分大小写的子串匹配而非精确等值比较。
综合而言，将 source 限定符作为第一优先级、头部字段关键词作为第二优先级的组合策略，能在绝大多数场景下稳定区分核基因组、质粒与细胞器基因组；对于仍无法判定的记录，返回 `unknown` 并辅以人工核查是最稳妥的处理方式。
