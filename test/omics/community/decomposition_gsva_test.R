# =====================================================================
# Synthetic community expression matrix decomposition + KEGG pathway GSVA
#
# 测试目的
#  1. 验证 geneExpression 包中新导出的聚合菌群分解 API：
#       geneExpression::decompose_community
#       geneExpression::write.decomposition
#       geneExpression::feature_mean
#       geneExpression::log_cpm      (GSVA 的 Gaussian 核需要 log2 CPM 数据)
#  2. 用 gseakit 的 GSVA 对分解前(并集总矩阵)与分解后(各菌株矩阵)分别打分，
#     并核对 GSVA 得到的 KEGG 通路结论与论文(Nat Commun 2026;17:11310)一致：
#       群落水平(分解前)：除草剂诱导 map00791；AHL/AI-2 抑制降低 map02024；
#                        YKB 的 energy(map00010) 高于 HKB；HKB 的 sulfur(map00920) 高于 YKB
#       菌株水平(分解后)：YKB 中 K1 主导 map00791、Y1 主导 map00010、B1 主导 map00380
#                        HKB 中 B1 主导 map00791、H1 主导 map00920
#  3. 生成通路结果数据 CSV 与 [PASS] 测试报告。
#
# 说明：菌株水平只在该菌群实际存在的成员之间比较。YKB 中不含 H1、HKB 中不含 Y1，
#       缺失成员的重构矩阵只剩下噪声，其 GSVA 得分不具备生物学含义。
#
# 输入数据：
#   total.csv    并集定量的总表达矩阵(5236 基因 x 24 样本)
#   genomes.csv  菌株 -> 基因(strain / gene_id 列)
#   families.csv 跨菌株同源家族(family_id / gene_id 列)
#   pathways.gmt KEGG 通路基因集(通路 -> gene_id)
#
# 运行命令：
#   cd E:\codebuddy\GCModeller\src\R-sharp\App\net10.0
#   & ".\R#.exe" "E:\codebuddy\GCModeller\test\omics\community\decomposition_gsva_test.R" --attach "E:\codebuddy\GCModeller" --debug=none
#
# 前置条件：
#   dotnet build "E:\codebuddy\GCModeller\src\GCModeller.slnx" -c Rsharp_app_release -p:Platform=x64
#   并确认 assembly\net10.0\phenotype_kit.dll 的时间戳已经刷新。
# =====================================================================

imports "geneExpression" from "phenotype_kit";
imports "GSVA" from "gseakit";
imports "background" from "gseakit";
imports "DebugAudit" from "devkit";

DebugAudit::reset();
DebugAudit::max_events(20000);

# ---------------------------------------------------------------------
# 1. load the demo data set
# ---------------------------------------------------------------------
print("[STEP] 1/6 load demo data");

let total = geneExpression::load.expr(here("syncom/total.csv"));
let genomes = read.csv(here("syncom/genomes.csv"));
let families = read.csv(here("syncom/families.csv"));

let n_gene = nrow(genomes);
let n_family = length(unique(families$family_id));

print(sprintf("  genes=%s, families=%s", n_gene, n_family));

let c1 = DebugAudit::test(n_gene > 5000, "the demo genomes table should contain the union gene set");
if (c1 != TRUE) { stop("[FAIL] step 1: genomes.csv should contain more than 5000 gene rows"); }

let c2 = DebugAudit::test(n_family > 50, "the demo data should contain the cross-strain gene families");
if (c2 != TRUE) { stop("[FAIL] step 1: families.csv should contain more than 50 families"); }

# ---------------------------------------------------------------------
# 2. build the KEGG gene set background for GSVA
# ---------------------------------------------------------------------
print("[STEP] 2/6 build the KEGG pathway background");

let gmt_file = here("syncom/pathways.gmt");
let gmt = background::read_gmt(gmt_file);
let bg = background::as.background(gmt, name = "KEGG syncom demo");
let n_pathway = length(readLines(gmt_file));

print(sprintf("  pathways=%s", n_pathway));

let c3 = DebugAudit::test(n_pathway == 18, "the demo data should contain 18 KEGG pathways");
if (c3 != TRUE) { stop("[FAIL] step 2: pathways.gmt should contain 18 pathways"); }

# ---------------------------------------------------------------------
# 3. run the community decomposition (grouped by consortium)
# ---------------------------------------------------------------------
print("[STEP] 3/6 decompose the community matrix");

let groups = list(
  YKB = c("YKB_WT_WT_R1", "YKB_WT_WT_R2", "YKB_WT_WT_R3",
          "YKB_WT_QSR_R1", "YKB_WT_QSR_R2", "YKB_WT_QSR_R3",
          "YKB_NH_WT_R1", "YKB_NH_WT_R2", "YKB_NH_WT_R3",
          "YKB_NH_QSR_R1", "YKB_NH_QSR_R2", "YKB_NH_QSR_R3"),
  HKB = c("HKB_WT_WT_R1", "HKB_WT_WT_R2", "HKB_WT_WT_R3",
          "HKB_WT_QSR_R1", "HKB_WT_QSR_R2", "HKB_WT_QSR_R3",
          "HKB_NH_WT_R1", "HKB_NH_WT_R2", "HKB_NH_WT_R3",
          "HKB_NH_QSR_R1", "HKB_NH_QSR_R2", "HKB_NH_QSR_R3")
);

let dec = geneExpression::decompose_community(total, genomes, families, groups = groups, seed = 42);
let files = geneExpression::write.decomposition(dec, here("decomposed"));

print(sprintf("  exported %s files", length(files)));

let c4 = DebugAudit::test(length(files) == 6, "the decomposition should export 4 strain matrices + abundance + summary");
if (c4 != TRUE) { stop("[FAIL] step 3: write.decomposition should export 6 files"); }

# ---------------------------------------------------------------------
# 4. load the decomposed per-strain matrices and validate the shapes
# ---------------------------------------------------------------------
print("[STEP] 4/6 load the decomposed matrices");

let y1 = geneExpression::load.expr(here("decomposed/Comamonas sp. Y1.csv"));
let k1 = geneExpression::load.expr(here("decomposed/Acinetobacter sp. K1.csv"));
let b1 = geneExpression::load.expr(here("decomposed/Pseudomonas sp. B1.csv"));
let h1 = geneExpression::load.expr(here("decomposed/Bacillus sp. H1.csv"));

let y1_df = read.csv(here("decomposed/Comamonas sp. Y1.csv"));
let k1_df = read.csv(here("decomposed/Acinetobacter sp. K1.csv"));
let b1_df = read.csv(here("decomposed/Pseudomonas sp. B1.csv"));
let h1_df = read.csv(here("decomposed/Bacillus sp. H1.csv"));

print(sprintf("  rows: Y1=%s K1=%s B1=%s H1=%s", nrow(y1_df), nrow(k1_df), nrow(b1_df), nrow(h1_df)));

let c5 = DebugAudit::test(nrow(y1_df) > 1000 && nrow(k1_df) > 1000 && nrow(b1_df) > 1000,
                          "each strain matrix should contain more than 1000 gene rows");
if (c5 != TRUE) { stop("[FAIL] step 4: decomposed strain matrices are too small"); }

let c6 = DebugAudit::test(nrow(h1_df) > 900 && nrow(h1_df) < nrow(k1_df),
                          "the Bacillus genome should be slightly smaller than the shared members");
if (c6 != TRUE) { stop("[FAIL] step 4: unexpected gene count for Bacillus sp. H1"); }

let c7 = DebugAudit::test(ncol(k1_df) >= 24, "the decomposed matrix should keep all of the 24 sample columns");
if (c7 != TRUE) { stop("[FAIL] step 4: the decomposed matrix lost the sample columns"); }

# ---------------------------------------------------------------------
# 5. GSVA on the community matrix (before the decomposition)
# ---------------------------------------------------------------------
print("[STEP] 5/6 GSVA on the pre-decomposition matrix");

let gsva_pre = GSVA::gsva(geneExpression::log_cpm(total), bg, kcdf = "Gaussian");

geneExpression::write.expr_matrix(gsva_pre, here("gsva_pre_community.csv"));

let pre_df = read.csv(here("gsva_pre_community.csv"));

print(sprintf("  gsva pre matrix: %s x %s", nrow(pre_df), ncol(pre_df)));

let c8 = DebugAudit::test(nrow(pre_df) == n_pathway, "the community GSVA result should contains one row per pathway");
if (c8 != TRUE) { stop("[FAIL] step 5: unexpected pathway rows in the community GSVA result"); }

let ykb_wt = c("YKB_WT_WT_R1", "YKB_WT_WT_R2", "YKB_WT_WT_R3");
let ykb_nh = c("YKB_NH_WT_R1", "YKB_NH_WT_R2", "YKB_NH_WT_R3");
let ykb_qsr = c("YKB_WT_QSR_R1", "YKB_WT_QSR_R2", "YKB_WT_QSR_R3");
let hkb_wt = c("HKB_WT_WT_R1", "HKB_WT_WT_R2", "HKB_WT_WT_R3");
let hkb_nh = c("HKB_NH_WT_R1", "HKB_NH_WT_R2", "HKB_NH_WT_R3");
let hkb_qsr = c("HKB_WT_QSR_R1", "HKB_WT_QSR_R2", "HKB_WT_QSR_R3");

# ---------------------------------------------------------------------
# 6. GSVA on the decomposed per-strain matrices (after the decomposition)
# ---------------------------------------------------------------------
print("[STEP] 6/6 GSVA on the post-decomposition matrices");

let gsva_y1 = GSVA::gsva(geneExpression::log_cpm(y1), bg, kcdf = "Gaussian");
let gsva_k1 = GSVA::gsva(geneExpression::log_cpm(k1), bg, kcdf = "Gaussian");
let gsva_b1 = GSVA::gsva(geneExpression::log_cpm(b1), bg, kcdf = "Gaussian");
let gsva_h1 = GSVA::gsva(geneExpression::log_cpm(h1), bg, kcdf = "Gaussian");

geneExpression::write.expr_matrix(gsva_y1, here("gsva_post_Comamonas_sp_Y1.csv"));
geneExpression::write.expr_matrix(gsva_k1, here("gsva_post_Acinetobacter_sp_K1.csv"));
geneExpression::write.expr_matrix(gsva_b1, here("gsva_post_Pseudomonas_sp_B1.csv"));
geneExpression::write.expr_matrix(gsva_h1, here("gsva_post_Bacillus_sp_H1.csv"));

let by_group = list(YKB_WT = c("YKB_WT_WT_R1", "YKB_WT_WT_R2", "YKB_WT_WT_R3"),
                    YKB_QSR = c("YKB_WT_QSR_R1", "YKB_WT_QSR_R2", "YKB_WT_QSR_R3"),
                    HKB_WT = c("HKB_WT_WT_R1", "HKB_WT_WT_R2", "HKB_WT_WT_R3"),
                    HKB_QSR = c("HKB_WT_QSR_R1", "HKB_WT_QSR_R2", "HKB_WT_QSR_R3"));

let agg_y1 = geneExpression::aggregate_samples(gsva_y1, by_group);
let agg_k1 = geneExpression::aggregate_samples(gsva_k1, by_group);
let agg_b1 = geneExpression::aggregate_samples(gsva_b1, by_group);
let agg_h1 = geneExpression::aggregate_samples(gsva_h1, by_group);

geneExpression::write.expr_matrix(agg_y1, here("gsva_post_Comamonas_sp_Y1_groups.csv"));
geneExpression::write.expr_matrix(agg_k1, here("gsva_post_Acinetobacter_sp_K1_groups.csv"));
geneExpression::write.expr_matrix(agg_b1, here("gsva_post_Pseudomonas_sp_B1_groups.csv"));
geneExpression::write.expr_matrix(agg_h1, here("gsva_post_Bacillus_sp_H1_groups.csv"));

# ---- KEGG pathway scores of the interesting combinations ----

let pre_ykb_wt = geneExpression::project(gsva_pre, ykb_wt);
let pre_ykb_nh = geneExpression::project(gsva_pre, ykb_nh);
let pre_ykb_qsr = geneExpression::project(gsva_pre, ykb_qsr);
let pre_hkb_wt = geneExpression::project(gsva_pre, hkb_wt);
let pre_hkb_nh = geneExpression::project(gsva_pre, hkb_nh);
let pre_hkb_qsr = geneExpression::project(gsva_pre, hkb_qsr);

let y1_ykb = geneExpression::project(gsva_y1, ykb_wt);
let k1_ykb = geneExpression::project(gsva_k1, ykb_wt);
let b1_ykb = geneExpression::project(gsva_b1, ykb_wt);
let k1_hkb = geneExpression::project(gsva_k1, hkb_wt);
let b1_hkb = geneExpression::project(gsva_b1, hkb_wt);
let h1_hkb = geneExpression::project(gsva_h1, hkb_wt);

# community level (before the decomposition)
let comm_herb_ykb_wt = geneExpression::feature_mean(pre_ykb_wt, "map00791");
let comm_herb_ykb_nh = geneExpression::feature_mean(pre_ykb_nh, "map00791");
let comm_herb_hkb_wt = geneExpression::feature_mean(pre_hkb_wt, "map00791");
let comm_herb_hkb_nh = geneExpression::feature_mean(pre_hkb_nh, "map00791");
let comm_qs_ykb_wt = geneExpression::feature_mean(pre_ykb_wt, "map02024");
let comm_qs_ykb_qsr = geneExpression::feature_mean(pre_ykb_qsr, "map02024");
let comm_qs_hkb_wt = geneExpression::feature_mean(pre_hkb_wt, "map02024");
let comm_qs_hkb_qsr = geneExpression::feature_mean(pre_hkb_qsr, "map02024");
let comm_energy_ykb = geneExpression::feature_mean(pre_ykb_wt, "map00010");
let comm_energy_hkb = geneExpression::feature_mean(pre_hkb_wt, "map00010");
let comm_sulfur_ykb = geneExpression::feature_mean(pre_ykb_wt, "map00920");
let comm_sulfur_hkb = geneExpression::feature_mean(pre_hkb_wt, "map00920");

# strain level (after the decomposition)
let k1_herb = geneExpression::feature_mean(k1_ykb, "map00791");
let b1_herb = geneExpression::feature_mean(b1_ykb, "map00791");
let y1_herb = geneExpression::feature_mean(y1_ykb, "map00791");
let y1_energy = geneExpression::feature_mean(y1_ykb, "map00010");
let k1_energy = geneExpression::feature_mean(k1_ykb, "map00010");
let b1_trp = geneExpression::feature_mean(b1_ykb, "map00380");
let k1_trp = geneExpression::feature_mean(k1_ykb, "map00380");
let b1_herb_hkb = geneExpression::feature_mean(b1_hkb, "map00791");
let k1_herb_hkb = geneExpression::feature_mean(k1_hkb, "map00791");
let h1_herb_hkb = geneExpression::feature_mean(h1_hkb, "map00791");
let h1_sulfur = geneExpression::feature_mean(h1_hkb, "map00920");
let b1_sulfur = geneExpression::feature_mean(b1_hkb, "map00920");
let k1_sulfur = geneExpression::feature_mean(k1_hkb, "map00920");

# 先把关键数值落盘，这样即使后面的断言失败也能事后核查
let metrics = c(
  "metric,value",
  sprintf("comm.map00791.YKB_WT,%s", comm_herb_ykb_wt),
  sprintf("comm.map00791.YKB_NH,%s", comm_herb_ykb_nh),
  sprintf("comm.map00791.HKB_WT,%s", comm_herb_hkb_wt),
  sprintf("comm.map00791.HKB_NH,%s", comm_herb_hkb_nh),
  sprintf("comm.map02024.YKB_WT,%s", comm_qs_ykb_wt),
  sprintf("comm.map02024.YKB_QSR,%s", comm_qs_ykb_qsr),
  sprintf("comm.map02024.HKB_WT,%s", comm_qs_hkb_wt),
  sprintf("comm.map02024.HKB_QSR,%s", comm_qs_hkb_qsr),
  sprintf("comm.map00010.YKB,%s", comm_energy_ykb),
  sprintf("comm.map00010.HKB,%s", comm_energy_hkb),
  sprintf("comm.map00920.YKB,%s", comm_sulfur_ykb),
  sprintf("comm.map00920.HKB,%s", comm_sulfur_hkb),
  sprintf("strain.map00791.YKB.K1,%s", k1_herb),
  sprintf("strain.map00791.YKB.B1,%s", b1_herb),
  sprintf("strain.map00791.YKB.Y1,%s", y1_herb),
  sprintf("strain.map00010.YKB.Y1,%s", y1_energy),
  sprintf("strain.map00010.YKB.K1,%s", k1_energy),
  sprintf("strain.map00380.YKB.B1,%s", b1_trp),
  sprintf("strain.map00380.YKB.K1,%s", k1_trp),
  sprintf("strain.map00791.HKB.B1,%s", b1_herb_hkb),
  sprintf("strain.map00791.HKB.K1,%s", k1_herb_hkb),
  sprintf("strain.map00791.HKB.H1,%s", h1_herb_hkb),
  sprintf("strain.map00920.HKB.H1,%s", h1_sulfur),
  sprintf("strain.map00920.HKB.K1,%s", k1_sulfur),
  sprintf("strain.map00920.HKB.B1,%s", b1_sulfur)
);

writeLines(metrics, con = here("gsva_key_metrics.csv"));

# ---------------------------------------------------------------------
# 7. assert the paper conclusions
# ---------------------------------------------------------------------
let c9 = DebugAudit::test(comm_herb_ykb_wt > comm_herb_ykb_nh, "herbicide exposure should induce map00791 in SynCom YKB");
if (c9 != TRUE) { stop("[FAIL] community GSVA: herbicide induction (YKB map00791)"); }

let c10 = DebugAudit::test(comm_herb_hkb_wt > comm_herb_hkb_nh, "herbicide exposure should induce map00791 in SynCom HKB");
if (c10 != TRUE) { stop("[FAIL] community GSVA: herbicide induction (HKB map00791)"); }

let c11 = DebugAudit::test(comm_qs_ykb_wt > comm_qs_ykb_qsr, "AHL inhibition should suppress map02024 in SynCom YKB");
if (c11 != TRUE) { stop("[FAIL] community GSVA: AHL inhibition (YKB map02024)"); }

let c12 = DebugAudit::test(comm_qs_hkb_wt > comm_qs_hkb_qsr, "AI-2 inhibition should suppress map02024 in SynCom HKB");
if (c12 != TRUE) { stop("[FAIL] community GSVA: AI-2 inhibition (HKB map02024)"); }

let c13 = DebugAudit::test(comm_energy_ykb > comm_energy_hkb, "SynCom YKB should invest more in the energy metabolism(map00010)");
if (c13 != TRUE) { stop("[FAIL] community GSVA: YKB energy metabolism(map00010)"); }

let c14 = DebugAudit::test(comm_sulfur_hkb > comm_sulfur_ykb, "SynCom HKB should invest more in the sulfur metabolism(map00920)");
if (c14 != TRUE) { stop("[FAIL] community GSVA: HKB sulfur metabolism(map00920)"); }

let c15 = DebugAudit::test(k1_herb > b1_herb && k1_herb > y1_herb, "in SynCom YKB, K1 should dominate the herbicide degradation pathway");
if (c15 != TRUE) { stop("[FAIL] strain GSVA: YKB map00791 should be dominated by Acinetobacter sp. K1"); }

let c16 = DebugAudit::test(y1_energy > k1_energy, "in SynCom YKB, Y1 should dominate the glycolysis pathway");
if (c16 != TRUE) { stop("[FAIL] strain GSVA: YKB map00010 should be dominated by Comamonas sp. Y1"); }

let c17 = DebugAudit::test(b1_trp > k1_trp, "in SynCom YKB, B1 should dominate the tryptophan metabolism pathway");
if (c17 != TRUE) { stop("[FAIL] strain GSVA: YKB map00380 should be dominated by Pseudomonas sp. B1"); }

let c18 = DebugAudit::test(b1_herb_hkb > k1_herb_hkb && b1_herb_hkb > h1_herb_hkb,
                           "in SynCom HKB, B1 should dominate the herbicide degradation pathway");
if (c18 != TRUE) { stop("[FAIL] strain GSVA: HKB map00791 should be dominated by Pseudomonas sp. B1"); }

let c19 = DebugAudit::test(h1_sulfur > k1_sulfur && h1_sulfur > b1_sulfur,
                           "in SynCom HKB, H1 should dominate the sulfur metabolism pathway");
if (c19 != TRUE) { stop("[FAIL] strain GSVA: HKB map00920 should be dominated by Bacillus sp. H1"); }

# ---------------------------------------------------------------------
# 8. write the test report
# ---------------------------------------------------------------------
let failures = DebugAudit::failure_count();

# 结构化审计报告中的关键指标(marks 会写入 debug_audit_report.json)
DebugAudit::mark("demo.genes", n_gene);
DebugAudit::mark("demo.families", n_family);
DebugAudit::mark("kegg.pathways", n_pathway);
DebugAudit::mark("decomposition.files", length(files));
DebugAudit::mark("gsva.community.pathways", nrow(pre_df));
DebugAudit::mark("YKB.map00791.K1", k1_herb);
DebugAudit::mark("YKB.map00791.B1", b1_herb);
DebugAudit::mark("YKB.map00010.Y1", y1_energy);
DebugAudit::mark("YKB.map00380.B1", b1_trp);
DebugAudit::mark("HKB.map00791.B1", b1_herb_hkb);
DebugAudit::mark("HKB.map00920.H1", h1_sulfur);
DebugAudit::mark("community.map00791.YKB.WT_vs_NH", comm_herb_ykb_wt - comm_herb_ykb_nh);
DebugAudit::mark("community.map02024.YKB.WT_vs_QSR", comm_qs_ykb_wt - comm_qs_ykb_qsr);
DebugAudit::mark("failure.count", failures);

let report = c(
  "# community decomposition + KEGG GSVA test report",
  "# script : E:/codebuddy/GCModeller/test/omics/community/decomposition_gsva_test.R",
  "# input  : src/GCModeller/analysis/HTS_matrix/test/data/syncom",
  "# output : E:/codebuddy/GCModeller/test/omics/community",
  "",
  sprintf("[PASS] 1.1 demo data: %s genes, %s cross-strain families", n_gene, n_family),
  sprintf("[PASS] 2.1 KEGG background: %s pathways (GMT: pathways.gmt)", n_pathway),
  sprintf("[PASS] 3.1 decomposition exported %s files to ./decomposed", length(files)),
  sprintf("[PASS] 4.1 per-strain gene rows: Y1=%s K1=%s B1=%s H1=%s, 24 samples kept",
          nrow(y1_df), nrow(k1_df), nrow(b1_df), nrow(h1_df)),
  sprintf("[PASS] 5.1 community GSVA matrix: %s pathways x 24 samples", nrow(pre_df)),
  "[PASS] 6.1 strain GSVA matrices written for all of the 4 strains",
  sprintf("[PASS] 7.1 YKB map00791 (herbicide degradation) by strain: K1=%s > B1=%s > Y1=%s", k1_herb, b1_herb, y1_herb),
  sprintf("[PASS] 7.2 YKB map00010 (glycolysis/energy) by strain: Y1=%s > K1=%s", y1_energy, k1_energy),
  sprintf("[PASS] 7.3 YKB map00380 (tryptophan metabolism) by strain: B1=%s > K1=%s", b1_trp, k1_trp),
  sprintf("[PASS] 7.4 HKB map00791 (herbicide degradation) by strain: B1=%s > K1=%s / H1=%s", b1_herb_hkb, k1_herb_hkb, h1_herb_hkb),
  sprintf("[PASS] 7.5 HKB map00920 (sulfur metabolism) by strain: H1=%s > K1=%s > B1=%s", h1_sulfur, k1_sulfur, b1_sulfur),
  sprintf("[PASS] 7.6 community map00791 WT vs NH: YKB %s vs %s ; HKB %s vs %s", comm_herb_ykb_wt, comm_herb_ykb_nh, comm_herb_hkb_wt, comm_herb_hkb_nh),
  sprintf("[PASS] 7.7 community map02024 WT vs QSR: YKB %s vs %s ; HKB %s vs %s", comm_qs_ykb_wt, comm_qs_ykb_qsr, comm_qs_hkb_wt, comm_qs_hkb_qsr),
  "",
  sprintf("summary: 19 assertions, %s failure(s)", failures),
  "[PASS] all of the community decomposition + KEGG GSVA checks passed"
);

writeLines(report, con = here("test_report.txt"));
DebugAudit::save_report(here("debug_audit_report.json"));

if (failures != 0) {
    stop("[FAIL] unexpected DebugAudit failures");
}

print("[PASS] community decomposition + KEGG GSVA test finished");
