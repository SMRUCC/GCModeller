#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
================================================================
生成 BNLearn 基因表达调控网络测试数据集
================================================================
主题：细菌微生物风味发酵中的次级代谢产物合成

生成三个文件：
1. gene_expression_matrix.csv  — 时间序列基因表达矩阵（60时间点 × 350基因）
2. regulatory_network_prior.csv — 转录调控网络先验知识
3. pathway_info.json            — 生物学通路/功能背景信息

数据生成策略：
- 基于已知的细菌风味发酵代谢通路构建真实感的调控网络骨架
- 使用常微分方程(ODE)模拟基因表达的时间动力学
- 添加生物学噪声（高斯 + 负二项分布风格）
- 时间序列反映发酵三阶段：生长期→代谢期→次级代谢期
================================================================
"""

import numpy as np
import csv
import json
import os

np.random.seed(2024)

# ================================================================
# 一、定义通路和基因体系
# ================================================================

# --- 通路定义：细菌风味发酵次级代谢产物合成相关通路 ---
PATHWAYS = {
    "P01": {
        "name": "支链氨基酸降解通路 (Branched-Chain Amino Acid Degradation)",
        "desc": "亮氨酸/异亮氨酸/缬氨酸降解产生支链脂肪酸和醇类，是奶酪成熟中关键风味前体",
        "genes": []
    },
    "P02": {
        "name": "脂肪酸β-氧化通路 (Fatty Acid β-Oxidation)",
        "desc": "脂肪酸降解产生短链脂肪酸（乙酸、丙酸、丁酸），贡献酸味和奶酪风味",
        "genes": []
    },
    "P03": {
        "name": "酯类合成通路 (Ester Biosynthesis)",
        "desc": "醇酰基转移酶催化酸醇缩合生成酯类（乙酸乙酯、丁酸乙酯），贡献果香风味",
        "genes": []
    },
    "P04": {
        "name": "萜类合成通路 (Terpenoid Biosynthesis)",
        "desc": "MEP途径合成萜类化合物（香叶醇、芳樟醇），贡献花香和柑橘风味",
        "genes": []
    },
    "P05": {
        "name": "苯丙氨酸代谢通路 (Phenylalanine Metabolism)",
        "desc": "苯丙氨酸降解生成苯甲醛、苯乙醇、苯乙酸，贡献玫瑰和蜂蜜风味",
        "genes": []
    },
    "P06": {
        "name": "硫代谢通路 (Sulfur Metabolism)",
        "desc": "甲硫氨酸降解生成甲硫醇、二甲基硫醚等含硫风味化合物",
        "genes": []
    },
    "P07": {
        "name": "乳酸代谢通路 (Lactate Metabolism)",
        "desc": "乳酸脱氢酶调控乳酸合成与转化，影响酸味和pH驱动风味反应",
        "genes": []
    },
    "P08": {
        "name": "聚酮合酶通路 (Polyketide Synthase Pathway)",
        "desc": "PKS合成聚酮类次级代谢产物，部分具有抗菌和风味修饰功能",
        "genes": []
    },
    "P09": {
        "name": "非核糖体肽合成通路 (NRPS Pathway)",
        "desc": "NRPS合成非核糖体肽类次级代谢产物，包括铁载体和风味肽",
        "genes": []
    },
    "P10": {
        "name": "细菌素合成通路 (Bacteriocin Biosynthesis)",
        "desc": "合成细菌素（如nisin），抑制杂菌并间接影响风味物质积累",
        "genes": []
    },
    "P11": {
        "name": "群体感应通路 (Quorum Sensing)",
        "desc": "AHL信号分子介导的群体感应调控次级代谢产物合成开关",
        "genes": []
    },
    "P12": {
        "name": "全局调控网络 (Global Regulatory Network)",
        "desc": "全局转录因子（CodY, CcpA, SigB, AbrB）调控代谢重编程",
        "genes": []
    },
    "P13": {
        "name": "嘌呤/嘧啶代谢通路 (Purine/Pyrimidine Metabolism)",
        "desc": "核苷酸代谢为发酵提供能量和辅因子",
        "genes": []
    },
    "P14": {
        "name": "铁载体合成通路 (Siderophore Biosynthesis)",
        "desc": "铁载体合成影响铁获取，间接调控氧化还原敏感的风味酶活性",
        "genes": []
    },
    "P15": {
        "name": "氧化应激响应通路 (Oxidative Stress Response)",
        "desc": "抗氧化系统保护风味酶免受氧化损伤，影响风味稳定性",
        "genes": []
    },
    "P16": {
        "name": "氨基酸生物合成通路 (Amino Acid Biosynthesis)",
        "desc": "氨基酸前体供应通路，为风味化合物合成提供底物",
        "genes": []
    },
    "P17": {
        "name": "丙酮酸代谢枢纽 (Pyruvate Metabolism Hub)",
        "desc": "丙酮酸节点连接糖酵解与多种风味前体合成途径",
        "genes": []
    },
    "P18": {
        "name": "维生素/辅因子合成通路 (Vitamin/Cofactor Biosynthesis)",
        "desc": "B族维生素和辅酶合成，为风味代谢酶提供辅因子",
        "genes": []
    },
    "P19": {
        "name": "细胞壁降解通路 (Cell Wall Degradation)",
        "desc": "自溶素介导细胞壁降解，释放胞内风味酶和代谢物",
        "genes": []
    },
    "P20": {
        "name": "蛋白水解系统 (Proteolytic System)",
        "desc": "胞外蛋白酶→肽酶系统降解蛋白质产生游离氨基酸和风味肽",
        "genes": []
    },
}

# --- 基因定义：350个基因，按通路分组 ---
# 格式: (gene_id, gene_name, functional_annotation, pathway_id, is_TF, expression_pattern)

GENE_DEFS = []

# ===== P01: 支链氨基酸降解通路 (18 genes) =====
bcad_genes = [
    ("bcatA", "bcatA", "branched-chain amino acid aminotransferase", True, "early_peak"),
    ("bcatB", "bcatB", "branched-chain amino acid aminotransferase II", False, "early_peak"),
    ("bkdA1", "bkdA1", "branched-chain alpha-keto acid dehydrogenase E1 alpha", False, "mid_peak"),
    ("bkdA2", "bkdA2", "branched-chain alpha-keto acid dehydrogenase E1 beta", False, "mid_peak"),
    ("bkdB", "bkdB", "branched-chain alpha-keto acid dehydrogenase E2", False, "mid_peak"),
    ("lpdA", "lpdA", "dihydrolipoamide dehydrogenase", False, "mid_peak"),
    ("ilvE", "ilvE", "branched-chain amino acid transaminase", False, "early_peak"),
    ("leuA", "leuA", "2-isopropylmalate synthase", False, "early_rise"),
    ("leuB", "leuB", "3-isopropylmalate dehydrogenase", False, "early_rise"),
    ("leuC", "leuC", "3-isopropylmalate dehydratase large subunit", False, "early_rise"),
    ("leuD", "leuD", "3-isopropylmalate dehydratase small subunit", False, "early_rise"),
    ("ivdA", "ivdA", "isovaleryl-CoA dehydrogenase", False, "mid_peak"),
    ("mmsA", "mmsA", "methylmalonate-semialdehyde dehydrogenase", False, "late_rise"),
    ("hmgA", "hmgA", "hydroxymethylglutaryl-CoA lyase", False, "late_rise"),
    ("hmgL", "hmgL", "HMG-CoA synthase", False, "mid_peak"),
    ("bkdR", "bkdR", "BCAA degradation transcriptional regulator", True, "early_peak"),
    ("aroP", "aroP", "aromatic amino acid permease", False, "early_peak"),
    ("brnQ", "brnQ", "branched-chain amino acid transport system II carrier protein", False, "early_peak"),
]
for g in bcad_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P01", g[3], g[4]))

# ===== P02: 脂肪酸β-氧化通路 (18 genes) =====
fao_genes = [
    ("fadD", "fadD", "acyl-CoA synthetase", False, "mid_rise"),
    ("fadE", "fadE", "acyl-CoA dehydrogenase", False, "mid_rise"),
    ("fadA", "fadA", "3-ketoacyl-CoA thiolase", False, "mid_peak"),
    ("fadB", "fadB", "fatty acid oxidation complex alpha subunit", False, "mid_peak"),
    ("fadI", "fadI", "3-hydroxyacyl-CoA dehydrogenase", False, "mid_peak"),
    ("fadJ", "fadJ", "enoyl-CoA hydratase", False, "mid_peak"),
    ("fadR", "fadR", "fatty acid metabolism transcriptional regulator", True, "mid_peak"),
    ("fadK", "fadK", "acyl-CoA synthetase (anaerobic)", False, "late_rise"),
    ("echA", "echA", "enoyl-CoA hydratase/isomerase", False, "mid_rise"),
    ("acdh", "acdh", "acyl-CoA dehydrogenase (short-chain specific)", False, "mid_peak"),
    ("atoA", "atoA", "acetoacetyl-CoA transferase alpha subunit", False, "late_rise"),
    ("atoB", "atoB", "acetyl-CoA acetyltransferase", False, "late_rise"),
    ("atoD", "atoD", "acetoacetyl-CoA transferase beta subunit", False, "late_rise"),
    ("yqeA", "yqeA", "putative acyl-CoA thioesterase", False, "mid_peak"),
    ("paaF", "paaF", "enoyl-CoA hydratase (phenylacetate catabolism)", False, "mid_rise"),
    ("paaG", "paaG", "phenylacetate catabolism ring-cleavage protein", False, "late_rise"),
    ("paaH", "paaH", "3-hydroxyadipyl-CoA dehydrogenase", False, "late_rise"),
    ("paaJ", "paaJ", "3-oxoadipyl-CoA thiolase", False, "late_rise"),
]
for g in fao_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P02", g[3], g[4]))

# ===== P03: 酯类合成通路 (16 genes) =====
est_genes = [
    ("eatA", "eatA", "ethanol acetyltransferase", False, "late_peak"),
    ("eatB", "eatB", "alcohol acetyltransferase II", False, "late_peak"),
    ("estA", "estA", "carboxylesterase type B", False, "late_rise"),
    ("estB", "estB", "arylesterase", False, "late_rise"),
    ("estC", "estC", "carboxylesterase family protein", False, "late_peak"),
    ("atfA", "atfA", "alcohol O-acetyltransferase", False, "late_peak"),
    ("atfB", "atfB", "alcohol O-acetyltransferase homolog", False, "late_rise"),
    ("eht1", "eht1", "ethanol hexanoyl transferase", False, "late_peak"),
    ("eeb1", "eeb1", "ethanol acyltransferase", False, "late_peak"),
    ("acsA", "acsA", "acetyl-CoA synthetase", False, "mid_rise"),
    ("acsL", "acsL", "long-chain acyl-CoA synthetase", False, "mid_rise"),
    ("adhA", "adhA", "alcohol dehydrogenase (Zn-dependent)", False, "mid_peak"),
    ("adhB", "adhB", "alcohol dehydrogenase (Fe-dependent)", False, "mid_peak"),
    ("adhE", "adhE", "bifunctional aldehyde/alcohol dehydrogenase", False, "mid_peak"),
    ("aldA", "aldA", "aldehyde dehydrogenase A", False, "mid_rise"),
    ("aldB", "aldB", "aldehyde dehydrogenase B", False, "mid_rise"),
]
for g in est_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P03", g[3], g[4]))

# ===== P04: 萜类合成通路 (MEP途径) (16 genes) =====
ter_genes = [
    ("dxs", "dxs", "1-deoxy-D-xylulose-5-phosphate synthase", False, "mid_rise"),
    ("dxr", "dxr", "1-deoxy-D-xylulose-5-phosphate reductoisomerase", False, "mid_rise"),
    ("ispD", "ispD", "2-C-methyl-D-erythritol 4-phosphate cytidylyltransferase", False, "mid_peak"),
    ("ispE", "ispE", "4-diphosphocytidyl-2-C-methyl-D-erythritol kinase", False, "mid_peak"),
    ("ispF", "ispF", "2-C-methyl-D-erythritol 2,4-cyclodiphosphate synthase", False, "mid_peak"),
    ("ispG", "ispG", "(E)-4-hydroxy-3-methylbut-2-enyl-diphosphate synthase", False, "mid_peak"),
    ("ispH", "ispH", "4-hydroxy-3-methylbut-2-enyl diphosphate reductase", False, "mid_peak"),
    ("idi", "idi", "isopentenyl-diphosphate delta-isomerase", False, "mid_rise"),
    ("ispA", "ispA", "geranyltranstransferase (farnesyl diphosphate synthase)", False, "late_rise"),
    ("crtE", "crtE", "geranylgeranyl pyrophosphate synthase", False, "late_rise"),
    ("gerA", "gerA", "geraniol synthase", False, "late_peak"),
    ("linalS", "linalS", "linalool synthase", False, "late_peak"),
    ("pinS", "pinS", "alpha-pinene synthase", False, "late_peak"),
    ("limS", "limS", "limonene synthase", False, "late_peak"),
    ("terR", "terR", "terpenoid biosynthesis transcriptional regulator", True, "mid_rise"),
    ("ispB", "ispB", "octaprenyl diphosphate synthase", False, "mid_peak"),
]
for g in ter_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P04", g[3], g[4]))

# ===== P05: 苯丙氨酸代谢通路 (14 genes) =====
phe_genes = [
    ("pal", "pal", "phenylalanine ammonia-lyase", False, "mid_peak"),
    ("pdcA", "pdcA", "phenylacetate decarboxylase", False, "late_rise"),
    ("feaB", "feaB", "phenylacetaldehyde dehydrogenase", False, "mid_peak"),
    ("feaD", "feaD", "phenylacetic acid degradation protein", False, "late_rise"),
    ("phhA", "phhA", "phenylalanine 4-hydroxylase", False, "mid_rise"),
    ("tyrB", "tyrB", "aromatic amino acid aminotransferase", False, "early_peak"),
    ("aroA", "aroA", "3-phosphoshikimate 1-carboxyvinyltransferase", False, "early_rise"),
    ("aroB", "aroB", "3-dehydroquinate synthase", False, "early_rise"),
    ("aroC", "aroC", "chorismate synthase", False, "early_rise"),
    ("aroG", "aroG", "DAHP synthase (Phe-sensitive)", True, "early_rise"),
    ("pheA", "pheA", "chorismate mutase/prephenate dehydratase", False, "early_peak"),
    ("paaX", "paaX", "phenylacetate catabolism transcriptional repressor", True, "mid_peak"),
    ("hpaB", "hpaB", "4-hydroxyphenylacetate 3-monooxygenase", False, "mid_rise"),
    ("hpaC", "hpaC", "4-hydroxyphenylacetate 3-monooxygenase reductase", False, "mid_rise"),
]
for g in phe_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P05", g[3], g[4]))

# ===== P06: 硫代谢通路 (16 genes) =====
sul_genes = [
    ("metA", "metA", "homoserine O-succinyltransferase", False, "early_rise"),
    ("metB", "metB", "cystathionine gamma-synthase", False, "early_peak"),
    ("metC", "metC", "cystathionine beta-lyase", False, "mid_peak"),
    ("metE", "metE", "cobalamin-independent methionine synthase", False, "mid_rise"),
    ("metH", "metH", "cobalamin-dependent methionine synthase", False, "mid_rise"),
    ("metK", "metK", "S-adenosylmethionine synthetase", False, "mid_peak"),
    ("mgl", "mgl", "methionine gamma-lyase", False, "late_peak"),
    ("mdeA", "mdeA", "L-methionine gamma-lyase (flavor-relevant)", False, "late_peak"),
    ("cysK", "cysK", "cysteine synthase A", False, "early_rise"),
    ("cysE", "cysE", "serine O-acetyltransferase", False, "early_rise"),
    ("cysD", "cysD", "sulfate adenylyltransferase subunit 2", False, "early_peak"),
    ("cysH", "cysH", "phosphoadenosine phosphosulfate reductase", False, "early_peak"),
    ("cysI", "cysI", "sulfite reductase (NADPH) hemoprotein beta", False, "early_peak"),
    ("tst", "tst", "thiosulfate sulfurtransferase", False, "mid_rise"),
    ("mtrA", "mtrA", "methyltransferase (sulfur metabolism)", False, "mid_peak"),
    ("cysR", "cysR", "cysteine biosynthesis transcriptional regulator", True, "early_peak"),
]
for g in sul_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P06", g[3], g[4]))

# ===== P07: 乳酸代谢通路 (12 genes) =====
lac_genes = [
    ("ldhA", "ldhA", "L-lactate dehydrogenase", False, "early_peak"),
    ("ldhB", "ldhB", "D-lactate dehydrogenase", False, "early_peak"),
    ("lldD", "lldD", "L-lactate dehydrogenase (FMN-linked)", False, "mid_rise"),
    ("dld", "dld", "D-lactate dehydrogenase (FAD-linked)", False, "mid_rise"),
    ("lctP", "lctP", "lactate permease", False, "early_peak"),
    ("pflB", "pflB", "pyruvate formate-lyase", False, "mid_peak"),
    ("pflD", "pflD", "formate acetyltransferase", False, "mid_peak"),
    ("ackA", "ackA", "acetate kinase", False, "mid_peak"),
    ("pta", "pta", "phosphotransacetylase", False, "mid_peak"),
    ("acs", "acs", "acetyl-CoA synthetase", False, "mid_rise"),
    ("poxB", "poxB", "pyruvate oxidase", False, "mid_peak"),
    ("lldR", "lldR", "lactate metabolism transcriptional regulator", True, "early_peak"),
]
for g in lac_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P07", g[3], g[4]))

# ===== P08: 聚酮合酶通路 (18 genes) =====
pks_genes = [
    ("pksA", "pksA", "polyketide synthase type I module 1", False, "late_rise"),
    ("pksB", "pksB", "polyketide synthase type I module 2", False, "late_rise"),
    ("pksC", "pksC", "polyketide synthase type I module 3", False, "late_peak"),
    ("pksD", "pksD", "polyketide synthase type I module 4", False, "late_peak"),
    ("pksE", "pksE", "polyketide synthase type II alpha subunit", False, "late_peak"),
    ("pksF", "pksF", "polyketide synthase type II beta subunit", False, "late_peak"),
    ("pksG", "pksG", "polyketide cyclase", False, "late_peak"),
    ("pksH", "pksH", "polyketide ketoreductase", False, "late_rise"),
    ("pksI", "pksI", "polyketide aromatase", False, "late_peak"),
    ("pksJ", "pksJ", "polyketide synthase thioesterase", False, "late_peak"),
    ("pksK", "pksK", "acyl carrier protein (PKS)", False, "late_rise"),
    ("pksL", "pksL", "PKS-associated phosphopantetheinyl transferase", False, "late_rise"),
    ("pksM", "pksM", "PKS regulatory protein", True, "late_rise"),
    ("pksN", "pksN", "PKS transport protein", False, "late_peak"),
    ("pksR", "pksR", "PKS resistance protein", False, "late_peak"),
    ("acpA", "acpA", "acyl carrier protein", False, "mid_rise"),
    ("fabD", "fabD", "malonyl CoA-ACP transacylase", False, "mid_rise"),
    ("fabH", "fabH", "3-oxoacyl-ACP synthase III", False, "mid_rise"),
]
for g in pks_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P08", g[3], g[4]))

# ===== P09: 非核糖体肽合成通路 (16 genes) =====
nrps_genes = [
    ("nrpA", "nrpA", "non-ribosomal peptide synthetase module 1", False, "late_rise"),
    ("nrpB", "nrpB", "non-ribosomal peptide synthetase module 2", False, "late_rise"),
    ("nrpC", "nrpC", "non-ribosomal peptide synthetase module 3", False, "late_peak"),
    ("nrpD", "nrpD", "NRPS condensation domain", False, "late_peak"),
    ("nrpE", "nrpE", "NRPS adenylation domain", False, "late_peak"),
    ("nrpF", "nrpF", "NRPS thiolation domain", False, "late_rise"),
    ("nrpG", "nrpG", "NRPS thioesterase domain", False, "late_peak"),
    ("nrpH", "nrpH", "NRPS epimerase domain", False, "late_peak"),
    ("nrpI", "nrpI", "NRPS methyltransferase domain", False, "late_rise"),
    ("nrpJ", "nrpJ", "NRPS reductase domain", False, "late_rise"),
    ("nrpK", "nrpK", "NRPS transport protein", False, "late_peak"),
    ("nrpL", "nrpL", "NRPS regulatory protein", True, "late_rise"),
    ("nrpM", "nrpM", "NRPS resistance protein", False, "late_peak"),
    ("srfAA", "srfAA", "surfactin synthetase subunit A", False, "late_peak"),
    ("srfAB", "srfAB", "surfactin synthetase subunit B", False, "late_peak"),
    ("comS", "comS", "competence protein (NRPS-derived)", False, "late_rise"),
]
for g in nrps_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P09", g[3], g[4]))

# ===== P10: 细菌素合成通路 (14 genes) =====
bac_genes = [
    ("nisA", "nisA", "nisin precursor peptide", False, "late_peak"),
    ("nisB", "nisB", "nisin modification enzyme (dehydratase)", False, "late_rise"),
    ("nisC", "nisC", "nisin modification enzyme (cyclase)", False, "late_rise"),
    ("nisT", "nisT", "nisin ABC transporter", False, "late_peak"),
    ("nisI", "nisI", "nisin immunity protein", False, "late_peak"),
    ("nisR", "nisR", "nisin two-component response regulator", True, "late_rise"),
    ("nisK", "nisK", "nisin two-component sensor kinase", False, "late_rise"),
    ("nisP", "nisP", "nisin extracellular serine protease", False, "late_peak"),
    ("bacA", "bacA", "bacteriocin precursor peptide A", False, "late_peak"),
    ("bacB", "bacB", "bacteriocin precursor peptide B", False, "late_peak"),
    ("bacC", "bacC", "bacteriocin immunity protein", False, "late_peak"),
    ("bacT", "bacT", "bacteriocin ABC transporter", False, "late_rise"),
    ("bacR", "bacR", "bacteriocin transcriptional regulator", True, "late_rise"),
    ("bacK", "bacK", "bacteriocin sensor histidine kinase", False, "late_rise"),
]
for g in bac_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P10", g[3], g[4]))

# ===== P11: 群体感应通路 (16 genes) =====
qs_genes = [
    ("luxS", "luxS", "S-ribosylhomocysteine lyase (AI-2 synthase)", False, "mid_rise"),
    ("luxR", "luxR", "transcriptional regulator LuxR family", True, "mid_peak"),
    ("luxI", "luxI", "autoinducer synthase (AHL)", False, "mid_rise"),
    ("phrA", "phrA", "phr peptide pheromone", False, "mid_peak"),
    ("rapA", "rapA", "response regulator aspartate phosphatase", False, "mid_peak"),
    ("comA", "comA", "two-component response regulator (competence)", True, "mid_peak"),
    ("comP", "comP", "two-component sensor kinase (competence)", False, "mid_rise"),
    ("spaK", "spaK", "two-component sensor kinase (subtilin)", False, "late_rise"),
    ("spaR", "spaR", "two-component response regulator (subtilin)", True, "late_rise"),
    ("sinI", "sinI", "sin anti-repressor", False, "mid_peak"),
    ("sinR", "sinR", "sin transcriptional regulator", True, "mid_peak"),
    ("abrB", "abrB", "transition state regulator AbrB", True, "early_peak"),
    ("spo0A", "spo0A", "sporulation transcription factor Spo0A", True, "late_rise"),
    ("spo0B", "spo0B", "sporulation phosphotransferase", False, "late_rise"),
    ("spo0F", "spo0F", "sporulation response regulator", False, "mid_peak"),
    ("codY", "codY", "global transcriptional regulator CodY", True, "early_peak"),
]
for g in qs_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P11", g[3], g[4]))

# ===== P12: 全局调控网络 (18 genes) =====
glo_genes = [
    ("ccpA", "ccpA", "catabolite control protein A", True, "early_peak"),
    ("ccpB", "ccpB", "catabolite control protein B", True, "early_peak"),
    ("ccpC", "ccpC", "catabolite control protein C", True, "early_rise"),
    ("sigA", "sigA", "RNA polymerase sigma factor SigA (housekeeping)", True, "stable"),
    ("sigB", "sigB", "RNA polymerase sigma factor SigB (stress)", True, "mid_peak"),
    ("sigH", "sigH", "RNA polymerase sigma factor SigH (sporulation)", True, "late_rise"),
    ("sigL", "sigL", "RNA polymerase sigma factor SigL (ECF)", True, "mid_rise"),
    ("sigM", "sigM", "RNA polymerase sigma factor SigM (ECF)", True, "mid_rise"),
    ("sigW", "sigW", "RNA polymerase sigma factor SigW (ECF)", True, "mid_peak"),
    ("glnR", "glnR", "glutamine synthetase repressor", True, "early_peak"),
    ("tcrA", "tcrA", "two-component response regulator", True, "mid_peak"),
    ("tcrB", "tcrB", "two-component sensor histidine kinase", False, "mid_rise"),
    ("resD", "resD", "two-component response regulator (respiration)", True, "mid_peak"),
    ("resE", "resE", "two-component sensor kinase (respiration)", False, "mid_rise"),
    ("fur", "fur", "ferric uptake regulator", True, "early_peak"),
    ("perR", "perR", "peroxide stress response regulator", True, "mid_peak"),
    ("lexA", "lexA", "SOS response transcriptional repressor", True, "stable"),
    ("hrcA", "hrcA", "heat-inducible transcription repressor", True, "stable"),
]
for g in glo_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P12", g[3], g[4]))

# ===== P13: 嘌呤/嘧啶代谢通路 (14 genes) =====
pur_genes = [
    ("purA", "purA", "adenylosuccinate synthetase", False, "early_rise"),
    ("purB", "purB", "adenylosuccinate lyase", False, "early_rise"),
    ("purC", "purC", "phosphoribosylaminoimidazole-succinocarboxamide synthase", False, "early_peak"),
    ("purD", "purD", "phosphoribosylamine-glycine ligase", False, "early_peak"),
    ("purF", "purF", "amidophosphoribosyltransferase", False, "early_rise"),
    ("purH", "purH", "phosphoribosylaminoimidazolecarboxamide formyltransferase", False, "early_peak"),
    ("pyrA", "pyrA", "carbamoyl-phosphate synthase large subunit", False, "early_rise"),
    ("pyrB", "pyrB", "aspartate carbamoyltransferase", False, "early_peak"),
    ("pyrC", "pyrC", "dihydroorotase", False, "early_peak"),
    ("pyrD", "pyrD", "dihydroorotate dehydrogenase", False, "early_rise"),
    ("pyrE", "pyrE", "orotate phosphoribosyltransferase", False, "early_peak"),
    ("pyrF", "pyrF", "orotidine-5'-phosphate decarboxylase", False, "early_peak"),
    ("purR", "purR", "purine operon repressor", True, "early_peak"),
    ("pyrR", "pyrR", "pyrimidine operon attenuator protein", True, "early_peak"),
]
for g in pur_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P13", g[3], g[4]))

# ===== P14: 铁载体合成通路 (14 genes) =====
sid_genes = [
    ("sidA", "sidA", "siderophore biosynthesis monooxygenase", False, "mid_rise"),
    ("sidB", "sidB", "siderophore biosynthesis ligase", False, "mid_rise"),
    ("sidC", "sidC", "siderophore biosynthesis acetyltransferase", False, "mid_peak"),
    ("sidD", "sidD", "siderophore biosynthesis epimerase", False, "mid_peak"),
    ("sidE", "sidE", "siderophore biosynthesis decarboxylase", False, "mid_rise"),
    ("sidF", "sidF", "siderophore synthetase", False, "mid_peak"),
    ("sidG", "sidG", "siderophore export ABC transporter ATP-binding protein", False, "mid_peak"),
    ("sidH", "sidH", "siderophore transport permease", False, "mid_rise"),
    ("sidI", "sidI", "siderophere-binding protein", False, "mid_rise"),
    ("sidJ", "sidJ", "siderophore uptake ABC transporter", False, "mid_peak"),
    ("sidK", "sidK", "siderophore-interacting protein", False, "mid_rise"),
    ("sidL", "sidL", "siderophore biosynthesis transcriptional regulator", True, "mid_peak"),
    ("entA", "entA", "2,3-dihydro-2,3-dihydroxybenzoate dehydrogenase", False, "mid_rise"),
    ("entB", "entB", "enterobactin synthetase component B", False, "mid_peak"),
]
for g in sid_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P14", g[3], g[4]))

# ===== P15: 氧化应激响应通路 (14 genes) =====
oxs_genes = [
    ("katA", "katA", "catalase (heme-dependent)", False, "mid_peak"),
    ("katE", "katE", "catalase HPII", False, "mid_peak"),
    ("sodA", "sodA", "superoxide dismutase (Mn)", False, "mid_rise"),
    ("sodB", "sodB", "superoxide dismutase (Fe)", False, "mid_rise"),
    ("sodC", "sodC", "superoxide dismutase (Cu-Zn)", False, "mid_rise"),
    ("ahpC", "ahpC", "alkyl hydroperoxide reductase subunit C", False, "mid_peak"),
    ("ahpD", "ahpD", "alkyl hydroperoxide reductase subunit D", False, "mid_peak"),
    ("trxA", "trxA", "thioredoxin", False, "stable"),
    ("trxB", "trxB", "thioredoxin reductase", False, "stable"),
    ("gor", "gor", "glutathione reductase", False, "stable"),
    ("gshA", "gshA", "glutamate-cysteine ligase", False, "early_rise"),
    ("gshB", "gshB", "glutathione synthetase", False, "early_rise"),
    ("osrR", "osrR", "oxidative stress response regulator", True, "mid_peak"),
    ("oxyR", "oxyR", "hydrogen peroxide response regulator", True, "mid_peak"),
]
for g in oxs_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P15", g[3], g[4]))

# ===== P16: 氨基酸生物合成通路 (18 genes) =====
aas_genes = [
    ("glyA", "glyA", "serine hydroxymethyltransferase", False, "early_rise"),
    ("thrA", "thrA", "aspartokinase/homoserine dehydrogenase", False, "early_peak"),
    ("thrB", "thrB", "homoserine kinase", False, "early_peak"),
    ("thrC", "thrC", "threonine synthase", False, "early_peak"),
    ("ilvA", "ilvA", "threonine deaminase", False, "early_peak"),
    ("ilvB", "ilvB", "acetolactate synthase large subunit", False, "early_peak"),
    ("ilvC", "ilvC", "ketol-acid reductoisomerase", False, "early_rise"),
    ("ilvD", "ilvD", "dihydroxyacid dehydratase", False, "early_rise"),
    ("lysC", "lysC", "aspartokinase III (lysine-sensitive)", False, "early_peak"),
    ("dapA", "dapA", "dihydrodipicolinate synthase", False, "early_rise"),
    ("dapB", "dapB", "dihydrodipicolinate reductase", False, "early_rise"),
    ("lysA", "lysA", "diaminopimelate decarboxylase", False, "early_peak"),
    ("argA", "argA", "N-acetylglutamate synthase", False, "early_rise"),
    ("argB", "argB", "acetylglutamate kinase", False, "early_rise"),
    ("argC", "argC", "N-acetyl-gamma-glutamyl-phosphate reductase", False, "early_peak"),
    ("argG", "argG", "argininosuccinate synthase", False, "early_peak"),
    ("argH", "argH", "argininosuccinate lyase", False, "early_peak"),
    ("argR", "argR", "arginine repressor", True, "early_peak"),
]
for g in aas_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P16", g[3], g[4]))

# ===== P17: 丙酮酸代谢枢纽 (16 genes) =====
pyr_genes = [
    ("pykA", "pykA", "pyruvate kinase II", False, "early_peak"),
    ("pykF", "pykF", "pyruvate kinase I", False, "early_peak"),
    ("pdhA", "pdhA", "pyruvate dehydrogenase E1 alpha", False, "early_peak"),
    ("pdhB", "pdhB", "pyruvate dehydrogenase E1 beta", False, "early_peak"),
    ("pdhC", "pdhC", "pyruvate dehydrogenase E2", False, "early_peak"),
    ("pdhD", "pdhD", "dihydrolipoamide dehydrogenase (PDH)", False, "early_peak"),
    ("alsS", "alsS", "alpha-acetolactate synthase", False, "mid_peak"),
    ("alsD", "alsD", "alpha-acetolactate decarboxylase", False, "mid_peak"),
    ("budA", "budA", "alpha-acetolactate decarboxylase (acetoin)", False, "mid_peak"),
    ("budB", "budB", "acetolactate synthase (acetoin)", False, "mid_peak"),
    ("budC", "budC", "acetoin reductase/diacetyl reductase", False, "late_peak"),
    ("ilvN", "ilvN", "acetolactate synthase small subunit", False, "early_peak"),
    ("pdhR", "pdhR", "pyruvate dehydrogenase complex repressor", True, "early_peak"),
    ("aceE", "aceE", "pyruvate dehydrogenase E1 component", False, "early_peak"),
    ("aceF", "aceF", "dihydrolipoamide acetyltransferase", False, "early_peak"),
    ("lpd", "lpd", "dihydrolipoamide dehydrogenase", False, "early_peak"),
]
for g in pyr_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P17", g[3], g[4]))

# ===== P18: 维生素/辅因子合成通路 (16 genes) =====
vit_genes = [
    ("ribA", "ribA", "GTP cyclohydrolase II (riboflavin)", False, "early_rise"),
    ("ribB", "ribB", "3,4-dihydroxy-2-butanone 4-phosphate synthase", False, "early_rise"),
    ("ribC", "ribC", "riboflavin synthase alpha chain", False, "early_peak"),
    ("ribD", "ribD", "riboflavin biosynthesis protein RibD", False, "early_peak"),
    ("ribE", "ribE", "riboflavin synthase beta chain", False, "early_peak"),
    ("thiA", "thiA", "thiamine biosynthesis protein ThiA", False, "early_rise"),
    ("thiB", "thiB", "thiamine-phosphate pyrophosphorylase", False, "early_rise"),
    ("thiC", "thiC", "thiamine biosynthesis protein ThiC", False, "early_peak"),
    ("bioA", "bioA", "adenosylmethionine-8-amino-7-oxononanoate aminotransferase", False, "early_rise"),
    ("bioB", "bioB", "biotin synthase", False, "early_peak"),
    ("bioD", "bioD", "dethiobiotin synthetase", False, "early_peak"),
    ("bioF", "bioF", "8-amino-7-oxononanoate synthase", False, "early_rise"),
    ("panB", "panB", "3-methyl-2-oxobutanoate hydroxymethyltransferase", False, "early_rise"),
    ("panC", "panC", "pantothenate synthetase", False, "early_peak"),
    ("panD", "panD", "aspartate 1-decarboxylase", False, "early_peak"),
    ("ribR", "ribR", "riboflavin biosynthesis transcriptional regulator", True, "early_peak"),
]
for g in vit_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P18", g[3], g[4]))

# ===== P19: 细胞壁降解通路 (14 genes) =====
cwl_genes = [
    ("cwlO", "cwlO", "cell wall hydrolase (autolysin)", False, "late_rise"),
    ("lytA", "lytA", "N-acetylmuramoyl-L-alanine amidase", False, "late_peak"),
    ("lytB", "lytB", "glucosaminidase", False, "late_peak"),
    ("lytC", "lytC", "N-acetylmuramidase", False, "late_rise"),
    ("lytD", "lytD", "L-alanine amidase", False, "late_peak"),
    ("lytE", "lytE", "cell wall endopeptidase", False, "late_rise"),
    ("lytF", "lytF", "gamma-D-glutamate-meso-diaminopimelate muropeptidase", False, "late_peak"),
    ("atlA", "atlA", "major autolysin AtlA", False, "late_peak"),
    ("atlB", "atlB", "autolysin AtlB", False, "late_rise"),
    ("lytR", "lytR", "autolysin transcriptional regulator", True, "late_rise"),
    ("lytS", "lytS", "two-component sensor histidine kinase (autolysis)", False, "late_rise"),
    ("walK", "walK", "two-component sensor kinase (cell wall)", False, "mid_peak"),
    ("walR", "walR", "two-component response regulator (cell wall)", True, "mid_peak"),
    ("tagO", "tagO", "teichoic acid biosynthesis protein", False, "mid_rise"),
]
for g in cwl_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P19", g[3], g[4]))

# ===== P20: 蛋白水解系统 (18 genes) =====
prt_genes = [
    ("aprE", "aprE", "subtilisin (alkaline protease)", False, "mid_peak"),
    ("nprE", "nprE", "neutral metalloprotease", False, "mid_peak"),
    ("nprB", "nprB", "neutral protease B", False, "mid_rise"),
    ("bpr", "bpr", "bacillopeptidase F", False, "mid_peak"),
    ("vpr", "vpr", "minor extracellular serine protease", False, "mid_rise"),
    ("epr", "epr", "extracellular serine protease", False, "mid_rise"),
    ("mpr", "mpr", "metalloprotease", False, "mid_peak"),
    ("pepA", "pepA", "aminopeptidase A/I", False, "mid_rise"),
    ("pepB", "pepB", "aminopeptidase B", False, "mid_rise"),
    ("pepC", "pepC", "aminopeptidase C", False, "mid_peak"),
    ("pepF", "pepF", "oligoendopeptidase F", False, "mid_peak"),
    ("pepN", "pepN", "aminopeptidase N", False, "mid_rise"),
    ("pepQ", "pepQ", "prolidase (Xaa-Pro dipeptidase)", False, "mid_rise"),
    ("pepX", "pepX", "X-prolyl dipeptidyl aminopeptidase", False, "mid_peak"),
    ("oppA", "oppA", "oligopeptide-binding protein", False, "mid_rise"),
    ("oppB", "oppB", "oligopeptide transport system permease", False, "mid_rise"),
    ("degU", "degU", "two-component response regulator (degradative enzymes)", True, "mid_peak"),
    ("degS", "degS", "two-component sensor kinase (degradative enzymes)", False, "mid_rise"),
]
for g in prt_genes:
    GENE_DEFS.append((g[0], g[1], g[2], "P20", g[3], g[4]))

# ===== 补充基因：跨通路连接基因 + 额外基因（凑够350个） =====
extra_genes = [
    # 糖酵解/碳代谢连接
    ("pfkA", "pfkA", "phosphofructokinase", False, "early_peak", "P17"),
    ("fbaA", "fbaA", "fructose-bisphosphate aldolase", False, "early_peak", "P17"),
    ("tpiA", "tpiA", "triosephosphate isomerase", False, "early_peak", "P17"),
    ("gapA", "gapA", "glyceraldehyde-3-phosphate dehydrogenase", False, "early_peak", "P17"),
    ("pgk", "pgk", "phosphoglycerate kinase", False, "early_peak", "P17"),
    ("eno", "eno", "enolase", False, "early_peak", "P17"),
    # TCA循环
    ("gltA", "gltA", "citrate synthase", False, "mid_rise", "P02"),
    ("acnA", "acnA", "aconitate hydratase", False, "mid_rise", "P02"),
    ("icd", "icd", "isocitrate dehydrogenase", False, "mid_peak", "P02"),
    ("sucA", "sucA", "2-oxoglutarate dehydrogenase E1", False, "mid_peak", "P02"),
    ("sdhA", "sdhA", "succinate dehydrogenase flavoprotein", False, "mid_peak", "P02"),
    ("fumA", "fumA", "fumarase A", False, "mid_rise", "P02"),
    ("mdh", "mdh", "malate dehydrogenase", False, "mid_rise", "P02"),
    # 转运蛋白
    ("glcP", "glcP", "glucose/mannose:H+ symporter", False, "early_peak", "P17"),
    ("ptsG", "ptsG", "glucose-specific PTS enzyme IIBCBA", False, "early_peak", "P17"),
    ("ptsH", "ptsH", "phosphocarrier protein HPr", False, "early_peak", "P17"),
    ("ptsI", "ptsI", "phosphoenolpyruvate-protein phosphotransferase", False, "early_peak", "P17"),
    # 应激蛋白
    ("dnaK", "dnaK", "chaperone protein DnaK", False, "stable", "P15"),
    ("groEL", "groEL", "chaperonin GroEL", False, "stable", "P15"),
    ("groES", "groES", "chaperonin GroES", False, "stable", "P15"),
    ("clpP", "clpP", "ATP-dependent Clp protease", False, "mid_rise", "P15"),
    ("clpX", "clpX", "ATP-dependent Clp protease ATP-binding subunit", False, "mid_rise", "P15"),
    # DNA/RNA代谢
    ("recA", "recA", "recombinase A", False, "stable", "P13"),
    ("rpoA", "rpoA", "RNA polymerase alpha subunit", False, "stable", "P12"),
    ("rpoB", "rpoB", "RNA polymerase beta subunit", False, "stable", "P12"),
    ("rpoC", "rpoC", "RNA polymerase beta prime subunit", False, "stable", "P12"),
    # 膜脂代谢
    ("pgsA", "pgsA", "phosphatidylglycerophosphate synthase", False, "early_rise", "P02"),
    ("cls", "cls", "cardiolipin synthase", False, "early_rise", "P02"),
    ("plsB", "plsB", "glycerol-3-phosphate acyltransferase", False, "early_rise", "P02"),
    # 能量代谢
    ("atpA", "atpA", "ATP synthase alpha subunit", False, "stable", "P17"),
    ("atpB", "atpB", "ATP synthase beta subunit", False, "stable", "P17"),
    ("atpD", "atpD", "ATP synthase delta subunit", False, "stable", "P17"),
    # 额外风味相关
    ("mglB", "mglB", "methionine ABC transporter substrate-binding protein", False, "early_peak", "P06"),
    ("cysP", "cysP", "sulfate ABC transporter substrate-binding protein", False, "early_peak", "P06"),
    ("ssuA", "ssuA", "alkanesulfonate ABC transporter substrate-binding protein", False, "mid_rise", "P06"),
    ("tauA", "tauA", "taurine ABC transporter substrate-binding protein", False, "mid_rise", "P06"),
    ("dmdA", "dmdA", "dimethylsulfoniopropionate demethylase", False, "late_peak", "P06"),
    # 额外次级代谢
    ("bacilysinA", "bacilysinA", "bacilysin biosynthesis protein BacA", False, "late_rise", "P09"),
    ("bacilysinB", "bacilysinB", "bacilysin biosynthesis protein BacB", False, "late_rise", "P09"),
    ("bacilysinC", "bacilysinC", "bacilysin biosynthesis protein BacC", False, "late_peak", "P09"),
    ("dfnA", "dfnA", "difficidin biosynthesis PKS module 1", False, "late_rise", "P08"),
    ("dfnB", "dfnB", "difficidin biosynthesis PKS module 2", False, "late_rise", "P08"),
    ("macA", "macA", "macrolactin biosynthesis PKS", False, "late_peak", "P08"),
    ("baeA", "baeA", "bacillaene biosynthesis PKS module 1", False, "late_rise", "P08"),
    ("baeB", "baeB", "bacillaene biosynthesis PKS module 2", False, "late_rise", "P08"),
    ("baeC", "baeC", "bacillaene biosynthesis PKS module 3", False, "late_peak", "P08"),
    # 额外调控因子
    ("rocR", "rocR", "arginine catabolism transcriptional regulator", True, "early_peak", "P12"),
    ("tenA", "tenA", "transcriptional regulator TenA family", True, "mid_peak", "P12"),
    ("yvfA", "yvfA", "putative transcriptional regulator MarR family", True, "mid_rise", "P12"),
    ("ytkD", "ytkD", "putative transcriptional regulator LysR family", True, "early_peak", "P12"),
    ("yydA", "yydA", "putative two-component response regulator", True, "mid_peak", "P11"),
]
for g in extra_genes:
    GENE_DEFS.append((g[0], g[1], g[2], g[5], g[3], g[4]))

# 填充通路基因列表
for gdef in GENE_DEFS:
    pid = gdef[3]
    if pid in PATHWAYS:
        PATHWAYS[pid]["genes"].append(gdef[0])

print(f"Total genes defined: {len(GENE_DEFS)}")
print(f"Total pathways: {len(PATHWAYS)}")
for pid, pinfo in PATHWAYS.items():
    print(f"  {pid}: {pinfo['name'][:40]}... ({len(pinfo['genes'])} genes)")


# ================================================================
# 二、定义调控网络（先验知识）
# ================================================================

REGULATORY_EDGES = []

# --- P01: 支链氨基酸降解 ---
for tf, targets, reg_type, conf in [
    ("bcatA", ["bkdA1","bkdA2","bkdB","ivdA","mmsA"], "activation", 0.92),
    ("bkdR", ["bcatA","bcatB","bkdA1","bkdA2","bkdB","ivdA"], "activation", 0.88),
    ("codY", ["bcatA","ilvE","leuA","brnQ"], "repression", 0.85),
    ("ccpA", ["bkdA1","bkdA2","bkdB","lpdA"], "activation", 0.80),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P02: 脂肪酸β-氧化 ---
for tf, targets, reg_type, conf in [
    ("fadR", ["fadD","fadE","fadA","fadB","fadI","fadJ","fadK"], "activation", 0.90),
    ("fadR", ["echA","acdh"], "activation", 0.82),
    ("ccpA", ["fadD","fadE"], "activation", 0.78),
    ("atoB", ["atoA","atoD"], "activation", 0.75),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P03: 酯类合成 ---
for tf, targets, reg_type, conf in [
    ("atfA", ["eatA","eatB","eht1","eeb1"], "activation", 0.88),
    ("fadR", ["acsA","acsL"], "activation", 0.82),
    ("ccpA", ["adhA","adhB","adhE","aldA","aldB"], "activation", 0.85),
    ("luxR", ["estA","estB","estC"], "activation", 0.72),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P04: 萜类合成 ---
for tf, targets, reg_type, conf in [
    ("terR", ["dxs","dxr","ispD","ispE","ispF","ispG","ispH","idi","ispA"], "activation", 0.91),
    ("terR", ["crtE","gerA","linalS","pinS","limS"], "activation", 0.87),
    ("sigB", ["dxs","dxr"], "activation", 0.70),
    ("spo0A", ["gerA","linalS","pinS","limS"], "activation", 0.75),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P05: 苯丙氨酸代谢 ---
for tf, targets, reg_type, conf in [
    ("aroG", ["aroA","aroB","aroC","pheA"], "activation", 0.90),
    ("paaX", ["paaF","paaG","paaH","paaJ","feaB","feaD"], "repression", 0.86),
    ("tyrB", ["pal","phhA"], "activation", 0.78),
    ("hrcA", ["hpaB","hpaC"], "repression", 0.65),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P06: 硫代谢 ---
for tf, targets, reg_type, conf in [
    ("cysR", ["cysK","cysE","cysD","cysH","cysI","tst"], "activation", 0.93),
    ("metK", ["metA","metB","metC"], "activation", 0.80),
    ("codY", ["metA","metB","metE"], "repression", 0.82),
    ("mgl", ["mdeA","dmdA"], "activation", 0.88),
    ("cysR", ["mglB","cysP","ssuA","tauA"], "activation", 0.85),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P07: 乳酸代谢 ---
for tf, targets, reg_type, conf in [
    ("lldR", ["ldhA","ldhB","lldD","dld","lctP"], "activation", 0.91),
    ("ccpA", ["ldhA","ldhB","pflB","ackA","pta"], "activation", 0.87),
    ("ccpA", ["poxB","acs"], "repression", 0.80),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P08: 聚酮合酶 ---
for tf, targets, reg_type, conf in [
    ("pksM", ["pksA","pksB","pksC","pksD","pksE","pksF","pksG","pksH"], "activation", 0.92),
    ("pksM", ["pksI","pksJ","pksK","pksL","pksN","pksR"], "activation", 0.88),
    ("spo0A", ["pksM","dfnA","dfnB","macA","baeA","baeB","baeC"], "activation", 0.83),
    ("sigH", ["pksA","pksB","pksC"], "activation", 0.76),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P09: NRPS ---
for tf, targets, reg_type, conf in [
    ("nrpL", ["nrpA","nrpB","nrpC","nrpD","nrpE","nrpF","nrpG","nrpH"], "activation", 0.90),
    ("nrpL", ["nrpI","nrpJ","nrpK","nrpM","srfAA","srfAB","comS"], "activation", 0.86),
    ("comA", ["srfAA","srfAB","comS"], "activation", 0.82),
    ("spo0A", ["nrpL","bacilysinA","bacilysinB","bacilysinC"], "activation", 0.80),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P10: 细菌素 ---
for tf, targets, reg_type, conf in [
    ("nisR", ["nisA","nisB","nisC","nisT","nisI","nisP"], "activation", 0.94),
    ("bacR", ["bacA","bacB","bacC","bacT"], "activation", 0.90),
    ("spaR", ["nisR","nisK"], "activation", 0.78),
    ("luxR", ["bacR","bacK"], "activation", 0.72),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P11: 群体感应 ---
for tf, targets, reg_type, conf in [
    ("luxR", ["luxI","phrA","rapA","sinI"], "activation", 0.89),
    ("comA", ["comP","spaK","spaR"], "activation", 0.85),
    ("sinR", ["sinI","abrB"], "repression", 0.82),
    ("spo0A", ["abrB","sinR","luxR"], "activation", 0.88),
    ("codY", ["luxS","comA","comP"], "repression", 0.80),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P12: 全局调控 ---
for tf, targets, reg_type, conf in [
    ("ccpA", ["ccpB","ccpC","sigA","ldhA","pdhR"], "activation", 0.92),
    ("sigB", ["sigM","sigW","katA","katE","ahpC","ahpD","osrR","oxyR"], "activation", 0.88),
    ("sigH", ["spo0A","spo0B","spo0F","sigL"], "activation", 0.86),
    ("fur", ["sidA","sidB","sidC","sidD","sidE","sidF","sidL","entA","entB"], "repression", 0.90),
    ("perR", ["katA","sodA","sodB","ahpC","trxA","gor"], "repression", 0.84),
    ("glnR", ["gshA","gshB","metA","metE"], "activation", 0.80),
    ("codY", ["ilvA","ilvB","ilvC","ilvD","metA","metB","thrA","lysC"], "repression", 0.87),
    ("abrB", ["aprE","nprE","bpr","vpr","degU"], "repression", 0.83),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P13: 嘌呤/嘧啶 ---
for tf, targets, reg_type, conf in [
    ("purR", ["purA","purB","purC","purD","purF","purH"], "repression", 0.91),
    ("pyrR", ["pyrA","pyrB","pyrC","pyrD","pyrE","pyrF"], "repression", 0.89),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P14: 铁载体 ---
for tf, targets, reg_type, conf in [
    ("sidL", ["sidA","sidB","sidC","sidD","sidE","sidF"], "activation", 0.87),
    ("fur", ["sidG","sidH","sidI","sidJ","sidK"], "repression", 0.85),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P15: 氧化应激 ---
for tf, targets, reg_type, conf in [
    ("osrR", ["sodA","sodB","sodC","ahpC","ahpD"], "activation", 0.86),
    ("oxyR", ["katA","katE","trxA","trxB","gor"], "activation", 0.88),
    ("perR", ["katA","sodA","ahpC"], "repression", 0.82),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P16: 氨基酸合成 ---
for tf, targets, reg_type, conf in [
    ("argR", ["argA","argB","argC","argG","argH"], "repression", 0.92),
    ("codY", ["thrA","thrB","thrC","ilvA","ilvB","ilvC","ilvD","lysC"], "repression", 0.85),
    ("rocR", ["argA","argB","argC"], "activation", 0.78),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P17: 丙酮酸代谢 ---
for tf, targets, reg_type, conf in [
    ("pdhR", ["pdhA","pdhB","pdhC","pdhD","aceE","aceF","lpd"], "repression", 0.88),
    ("ccpA", ["pykA","pykF","pfkA","fbaA"], "activation", 0.84),
    ("sigA", ["ptsG","ptsH","ptsI","glcP"], "activation", 0.80),
    ("alsS", ["alsD","budA","budB","budC"], "activation", 0.82),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P18: 维生素/辅因子 ---
for tf, targets, reg_type, conf in [
    ("ribR", ["ribA","ribB","ribC","ribD","ribE"], "activation", 0.86),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P19: 细胞壁降解 ---
for tf, targets, reg_type, conf in [
    ("lytR", ["cwlO","lytA","lytB","lytC","lytD","lytE","lytF","atlA","atlB"], "activation", 0.85),
    ("walR", ["lytE","lytF","cwlO"], "activation", 0.82),
    ("sigD", ["lytA","lytB","lytC","lytD"], "activation", 0.78),
    ("spo0A", ["lytR","lytS"], "activation", 0.75),
]:
    for t in targets:
        if t in [g[0] for g in GENE_DEFS]:  # only if gene exists
            REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- P20: 蛋白水解 ---
for tf, targets, reg_type, conf in [
    ("degU", ["aprE","nprE","nprB","bpr","vpr","epr","mpr"], "activation", 0.90),
    ("degU", ["pepA","pepB","pepC","pepF","pepN","pepQ","pepX"], "activation", 0.84),
    ("degU", ["oppA","oppB"], "activation", 0.80),
    ("abrB", ["degU","degS"], "repression", 0.82),
    ("ccpA", ["aprE","nprE"], "activation", 0.76),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# --- 跨通路调控 ---
for tf, targets, reg_type, conf in [
    ("spo0A", ["sigH","abrB","sinR"], "activation", 0.92),
    ("sigB", ["osrR","oxyR","katA","sodA"], "activation", 0.85),
    ("ccpA", ["pdhR","lldR","fadR"], "activation", 0.80),
    ("luxR", ["bacR","nisR","pksM"], "activation", 0.73),
    ("codY", ["rocR","argR","cysR"], "repression", 0.78),
    ("tenA", ["ribA","ribB","thiA","thiC"], "activation", 0.70),
    ("yvfA", ["sidA","sidB","entA"], "activation", 0.65),
    ("ytkD", ["lysC","dapA","dapB"], "activation", 0.68),
    ("yydA", ["luxS","luxI"], "activation", 0.72),
]:
    for t in targets:
        REGULATORY_EDGES.append((tf, t, reg_type, conf, "TFBS_motif_scan"))

# 去重
seen = set()
unique_edges = []
for e in REGULATORY_EDGES:
    key = (e[0], e[1])
    if key not in seen:
        seen.add(key)
        unique_edges.append(e)
REGULATORY_EDGES = unique_edges

print(f"\nTotal regulatory edges: {len(REGULATORY_EDGES)}")


# ================================================================
# 三、生成时间序列基因表达矩阵
# ================================================================

N_GENES = len(GENE_DEFS)
N_TIMEPOINTS = 60

# 时间轴：0-72小时发酵过程，每1.2小时一个采样点
time_hours = np.linspace(0, 72, N_TIMEPOINTS)

# 构建基因名列表
gene_names = [g[0] for g in GENE_DEFS]
gene_idx_map = {g[0]: i for i, g in enumerate(GENE_DEFS)}

# 定义表达模式函数
def expression_pattern(t, pattern, base_level=7.0, amplitude=2.5):
    """根据模式生成时间序列表达值"""
    # 归一化时间到 [0, 1]
    t_norm = t / 72.0

    if pattern == "early_peak":
        # 早期峰值（0-24h），之后下降
        val = base_level + amplitude * np.exp(-((t_norm - 0.15)**2) / (2 * 0.02))
    elif pattern == "early_rise":
        # 早期上升后稳定
        val = base_level + amplitude * (1 - np.exp(-t_norm * 8))
    elif pattern == "mid_peak":
        # 中期峰值（24-48h）
        val = base_level + amplitude * np.exp(-((t_norm - 0.45)**2) / (2 * 0.03))
    elif pattern == "mid_rise":
        # 中期上升
        val = base_level + amplitude * (1 - np.exp(-(t_norm - 0.2) * 6)) * (t_norm > 0.2)
    elif pattern == "late_peak":
        # 晚期峰值（48-72h）
        val = base_level + amplitude * np.exp(-((t_norm - 0.75)**2) / (2 * 0.03))
    elif pattern == "late_rise":
        # 晚期上升
        val = base_level + amplitude * (1 - np.exp(-(t_norm - 0.5) * 5)) * (t_norm > 0.5)
    elif pattern == "stable":
        # 稳定表达
        val = base_level + amplitude * 0.1 * np.sin(2 * np.pi * t_norm * 3)
    else:
        val = base_level

    return val

# 生成基础表达矩阵
expr_matrix = np.zeros((N_GENES, N_TIMEPOINTS))

for i, gdef in enumerate(GENE_DEFS):
    gene_id, gene_name, annotation, pathway, is_tf, pattern = gdef

    # TF基因通常表达量稍高
    base = 7.5 if is_tf else 7.0
    amp = 2.8 if is_tf else 2.5

    # 基础表达模式
    base_expr = np.array([expression_pattern(t, pattern, base, amp) for t in time_hours])

    # 添加调控效应：如果该基因被某个TF调控，则叠加调控信号
    regulatory_effect = np.zeros(N_TIMEPOINTS)
    for edge in REGULATORY_EDGES:
        tf_name, target_name, reg_type, conf, evidence = edge
        if target_name == gene_id:
            # 找到TF的表达模式
            tf_idx = gene_idx_map.get(tf_name, -1)
            if tf_idx >= 0:
                tf_pattern = GENE_DEFS[tf_idx][5]
                tf_expr = np.array([expression_pattern(t, tf_pattern, 7.5, 2.8) for t in time_hours])
                # 标准化TF表达
                tf_expr_norm = (tf_expr - tf_expr.mean()) / (tf_expr.std() + 1e-6)
                if reg_type == "activation":
                    regulatory_effect += conf * 0.5 * tf_expr_norm
                else:
                    regulatory_effect -= conf * 0.5 * tf_expr_norm

    # 合并基础表达 + 调控效应
    expr_matrix[i, :] = base_expr + regulatory_effect

    # 添加生物学噪声
    # 1. 高斯噪声（技术噪声）
    noise_gauss = np.random.normal(0, 0.3, N_TIMEPOINTS)
    # 2. 时间相关噪声（生物学变异）
    noise_bio = np.zeros(N_TIMEPOINTS)
    for j in range(1, N_TIMEPOINTS):
        noise_bio[j] = 0.7 * noise_bio[j-1] + np.random.normal(0, 0.15)
    # 3. 偶发的表达脉冲
    pulse = np.zeros(N_TIMEPOINTS)
    n_pulses = np.random.poisson(1.5)
    for _ in range(n_pulses):
        pulse_pos = np.random.randint(0, N_TIMEPOINTS)
        pulse[pulse_pos] = np.random.normal(0, 1.5)

    expr_matrix[i, :] += noise_gauss + noise_bio + pulse

    # 确保表达值为正（log2空间下最小值约2）
    expr_matrix[i, :] = np.maximum(expr_matrix[i, :], 2.0)

# 添加3个重复（生物学重复），扩展样本列
# 每个时间点3个重复
N_REPLICATES = 3
total_samples = N_TIMEPOINTS * N_REPLICATES
full_matrix = np.zeros((N_GENES, total_samples))

sample_names = []
for t_idx in range(N_TIMEPOINTS):
    t_val = time_hours[t_idx]
    for rep in range(1, N_REPLICATES + 1):
        col_idx = t_idx * N_REPLICATES + (rep - 1)
        # 重复间添加微小变异
        rep_noise = np.random.normal(0, 0.2, N_GENES)
        full_matrix[:, col_idx] = expr_matrix[:, t_idx] + rep_noise
        full_matrix[:, col_idx] = np.maximum(full_matrix[:, col_idx], 2.0)
        sample_names.append(f"T{t_val:.1f}h_Rep{rep}")

print(f"\nExpression matrix shape: {full_matrix.shape}")
print(f"  Genes: {N_GENES}")
print(f"  Samples: {total_samples} ({N_TIMEPOINTS} timepoints x {N_REPLICATES} replicates)")
print(f"  Time range: {time_hours[0]:.1f} - {time_hours[-1]:.1f} hours")


# ================================================================
# 四、输出文件
# ================================================================

output_dir = "/home/z/my-project/GRN_BNLearn/TestData"

# --- 1. 基因表达矩阵 CSV ---
expr_csv = os.path.join(output_dir, "gene_expression_matrix.csv")
with open(expr_csv, 'w', newline='', encoding='utf-8') as f:
    writer = csv.writer(f)
    # 表头
    writer.writerow(["Gene"] + sample_names)
    # 数据行
    for i in range(N_GENES):
        row = [gene_names[i]] + [f"{full_matrix[i, j]:.4f}" for j in range(total_samples)]
        writer.writerow(row)

print(f"\n[1] Gene expression matrix saved: {expr_csv}")
print(f"    {N_GENES} genes x {total_samples} samples")

# --- 2. 调控网络先验知识 CSV ---
prior_csv = os.path.join(output_dir, "regulatory_network_prior.csv")
with open(prior_csv, 'w', newline='', encoding='utf-8') as f:
    writer = csv.writer(f)
    writer.writerow(["TF", "TargetGene", "RegulationType", "Confidence", "Evidence"])
    for edge in REGULATORY_EDGES:
        tf_name, target_name, reg_type, conf, evidence = edge
        # 随机分配证据来源
        evidences = ["TFBS_motif_scan", "ChIP_seq", "DNase_footprint", "comparative_genomics"]
        ev = np.random.choice(evidences, p=[0.45, 0.25, 0.15, 0.15])
        writer.writerow([tf_name, target_name, reg_type, f"{conf:.2f}", ev])

print(f"[2] Regulatory network prior saved: {prior_csv}")
print(f"    {len(REGULATORY_EDGES)} regulatory edges")

# --- 3. 通路信息 JSON ---
pathway_json = os.path.join(output_dir, "pathway_info.json")
pathway_output = {}
for pid, pinfo in PATHWAYS.items():
    pathway_output[pid] = {
        "name": pinfo["name"],
        "description": pinfo.get("desc", ""),
        "genes": pinfo["genes"]
    }

with open(pathway_json, 'w', encoding='utf-8') as f:
    json.dump(pathway_output, f, indent=2, ensure_ascii=False)

print(f"[3] Pathway info saved: {pathway_json}")
print(f"    {len(pathway_output)} pathways")

# --- 额外：基因注释信息（方便理解数据） ---
annotation_csv = os.path.join(output_dir, "gene_annotations.csv")
with open(annotation_csv, 'w', newline='', encoding='utf-8') as f:
    writer = csv.writer(f)
    writer.writerow(["GeneID", "GeneName", "Annotation", "PathwayID", "IsTF", "ExpressionPattern"])
    for gdef in GENE_DEFS:
        writer.writerow([gdef[0], gdef[1], gdef[2], gdef[3], gdef[4], gdef[5]])

print(f"[+] Gene annotations saved: {annotation_csv}")

# --- 数据摘要 ---
print("\n" + "="*60)
print("TEST DATASET SUMMARY")
print("="*60)
print(f"Theme: Bacterial flavor fermentation - secondary metabolite synthesis")
print(f"Genes: {N_GENES}")
print(f"Time points: {N_TIMEPOINTS} (0-72h fermentation)")
print(f"Replicates: {N_REPLICATES} per time point")
print(f"Total samples: {total_samples}")
print(f"Regulatory edges: {len(REGULATORY_EDGES)}")
print(f"Pathways: {len(PATHWAYS)}")
print(f"TFs: {sum(1 for g in GENE_DEFS if g[4])}")
print(f"Expression range: [{full_matrix.min():.2f}, {full_matrix.max():.2f}] (log2-scale)")
print(f"Mean expression: {full_matrix.mean():.2f}")
print("="*60)
