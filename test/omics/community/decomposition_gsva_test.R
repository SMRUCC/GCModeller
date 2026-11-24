# =====================================================================
# Synthetic community expression matrix decomposition + KEGG pathway GSVA
#
# 测试目的
#  1. 验证 geneExpression 包中新导出的聚合菌群分解 API：
#       geneExpression::decompose_community
#       geneExpression::write.decomposition
#       geneExpression::feature_mean
#  2. 用 gseakit 的 GSVA 对分解前(并集总矩阵)与分解后(各菌株矩阵)分别打分，
#     并核对 GSVA 得到的 KEGG 通路结论与论文(Nat Commun 2026;17:11310)一致：
#       YKB: K1 主导 map00791(阿特拉津降解)、Y1 主导 map00010(糖酵解)、
#            B1 主导 map00380(色氨酸代谢)
#       HKB: B1 主导 map00791、H1 主导 map00920(硫代谢)
#  3. 生成通路结果数据 CSV 与 [PASS] 测试报告。
#
# 输入数据(由 test/R/syncom_demo_data.R 生成)：
#   src/GCModeller/analysis/HTS_matrix/test/data/syncom/
#     total.csv    并集定量的总表达矩阵(5236 基因 x 24 样本)
#     genomes.csv  菌株 -> 基因(strain / gene_id 列)
#     families.csv 跨菌株同源家族(family_id / gene_id 列)
#     design.csv   样本设计(sample / consortium / herbicide / qs)
#     pathways.gmt KEGG 通路基因集(通路 -> gene_id)
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

let report = c(
"# community decomposition + KEGG GSVA test report",
"# input : src/GCModeller/analysis/HTS_matrix/test/data/syncom",
"# output: test/omics/community",
""
);

# ---------------------------------------------------------------------
# 1. load the demo data set
# ---------------------------------------------------------------------
print("[STEP] 1/6 load demo data");

let total = geneExpression::load.expr("E:/codebuddy/GCModeller/src/GCModeller/analysis/HTS_matrix/test/data/syncom/total.csv");
let genomes = read.csv("E:/codebuddy/GCModeller/src/GCModeller/analysis/HTS_matrix/test/data/syncom/genomes.csv");
let families = read.csv("E:/codebuddy/GCModeller/src/GCModeller/analysis/HTS_matrix/test/data/syncom/families.csv");
let design = read.csv("E:/codebuddy/GCModeller/src/GCModeller/analysis/HTS_matrix/test/data/syncom/design.csv");

let n_gene = nrow(genomes);
let n_family = length(unique(families$family_id));
let n_sample = nrow(design);

print(sprintf("  genes=%s, families=%s, samples=%s", n_gene, n_family, n_sample));

let c1 = DebugAudit::test(n_gene > 5000, "the demo genomes table should contain the union gene set");
if (c1 != TRUE) { stop("[FAIL] step 1: genomes.csv should contain more than 5000 gene rows"); }

let c2 = DebugAudit::test(n_family > 50, "the demo data should contain the cross-strain gene families");
if (c2 != TRUE) { stop("[FAIL] step 1: families.csv should contain more than 50 families"); }

let c3 = DebugAudit::test(n_sample == 24, "the demo design should contain 24 samples");
if (c3 != TRUE) { stop("[FAIL] step 1: design.csv should contain 24 samples"); }

report = c(report, sprintf("[PASS] 1.1 loaded %s genes / %s families / %s samples", n_gene, n_family, n_sample));

# ---------------------------------------------------------------------
# 2. build the KEGG gene set background for GSVA
# ---------------------------------------------------------------------
print("[STEP] 2/6 build the KEGG pathway background");

let gmt_file = "E:/codebuddy/GCModeller/src/GCModeller/analysis/HTS_matrix/test/data/syncom/pathways.gmt";
let gmt = background::read_gmt(gmt_file);
let bg = background::as.background(gmt, name = "KEGG syncom demo");
let n_pathway = length(readLines(gmt_file));

print(sprintf("  pathways=%s", n_pathway));

let c4 = DebugAudit::test(n_pathway == 18, "the demo data should contain 18 KEGG pathways");
if (c4 != TRUE) { stop("[FAIL] step 2: pathways.gmt should contain 18 pathways"); }

report = c(report, sprintf("[PASS] 2.1 KEGG background built from %s pathways", n_pathway));

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

let files = geneExpression::write.decomposition(dec, "E:/codebuddy/GCModeller/test/omics/community/decomposed");

print(sprintf("  exported %s files", length(files)));

let c5 = DebugAudit::test(length(files) == 6, "the decomposition should export 4 strain matrices + abundance + summary");
if (c5 != TRUE) { stop("[FAIL] step 3: write.decomposition should export 6 files"); }

report = c(report, sprintf("[PASS] 3.1 decomposition exported %s files", length(files)));

# ---------------------------------------------------------------------
# 4. load the decomposed per-strain matrices and validate the shapes
# ---------------------------------------------------------------------
print("[STEP] 4/6 load the decomposed matrices");

let y1 = geneExpression::load.expr("E:/codebuddy/GCModeller/test/omics/community/decomposed/Comamonas sp. Y1.csv");
let k1 = geneExpression::load.expr("E:/codebuddy/GCModeller/test/omics/community/decomposed/Acinetobacter sp. K1.csv");
let b1 = geneExpression::load.expr("E:/codebuddy/GCModeller/test/omics/community/decomposed/Pseudomonas sp. B1.csv");
let h1 = geneExpression::load.expr("E:/codebuddy/GCModeller/test/omics/community/decomposed/Bacillus sp. H1.csv");

let y1_df = read.csv("E:/codebuddy/GCModeller/test/omics/community/decomposed/Comamonas sp. Y1.csv");
let k1_df = read.csv("E:/codebuddy/GCModeller/test/omics/community/decomposed/Acinetobacter sp. K1.csv");
let b1_df = read.csv("E:/codebuddy/GCModeller/test/omics/community/decomposed/Pseudomonas sp. B1.csv");
let h1_df = read.csv("E:/codebuddy/GCModeller/test/omics/community/decomposed/Bacillus sp. H1.csv");

print(sprintf("  rows: Y1=%s K1=%s B1=%s H1=%s", nrow(y1_df), nrow(k1_df), nrow(b1_df), nrow(h1_df)));

let c6 = DebugAudit::test(nrow(y1_df) > 1000 && nrow(k1_df) > 1000 && nrow(b1_df) > 1000, "each strain matrix should contain more than 1000 gene rows");
if (c6 != TRUE) { stop("[FAIL] step 4: decomposed strain matrices are too small"); }

let c7 = DebugAudit::test(nrow(h1_df) > 900 && nrow(h1_df) < nrow(k1_df), "the H1 genome should be slightly smaller than the shared members");
if (c7 != TRUE) { stop("[FAIL] step 4: unexpected gene count for Bacillus sp. H1"); }

let c8 = DebugAudit::test(ncol(k1_df) >= 24, "the decomposed matrix should keep all of the 24 sample columns");
if (c8 != TRUE) { stop("[FAIL] step 4: the decomposed matrix lost the sample columns"); }

report = c(report, sprintf("[PASS] 4.1 decomposed matrices: Y1=%s K1=%s B1=%s H1=%s genes", nrow(y1_df), nrow(k1_df), nrow(b1_df), nrow(h1_df)));

# ---------------------------------------------------------------------
# 5. GSVA on the community matrix (before the decomposition)
# ---------------------------------------------------------------------
print("[STEP] 5/6 GSVA on the pre-decomposition matrix");

let gsva_pre = GSVA::gsva(total, bg);

geneExpression::write.expr_matrix(gsva_pre, "E:/codebuddy/GCModeller/test/omics/community/gsva_pre_community.csv");

let pre_df = read.csv("E:/codebuddy/GCModeller/test/omics/community/gsva_pre_community.csv");

print(sprintf("  gsva pre matrix: %s x %s", nrow(pre_df), ncol(pre_df)));

let c9 = DebugAudit::test(nrow(pre_df) == n_pathway, "the community GSVA result should contains one row per pathway");
if (c9 != TRUE) { stop("[FAIL] step 5: unexpected pathway rows in the community GSVA result"); }

report = c(report, sprintf("[PASS] 5.1 community GSVA matrix: %s pathways x %s samples", nrow(pre_df), ncol(pre_df)));

# ---------------------------------------------------------------------
# 6. GSVA on the decomposed per-strain matrices (after the decomposition)
# ---------------------------------------------------------------------
print("[STEP] 6/6 GSVA on the post-decomposition matrices");

let gsva_y1 = GSVA::gsva(y1, bg);
let gsva_k1 = GSVA::gsva(k1, bg);
let gsva_b1 = GSVA::gsva(b1, bg);
let gsva_h1 = GSVA::gsva(h1, bg);

geneExpression::write.expr_matrix(gsva_y1, "E:/codebuddy/GCModeller/test/omics/community/gsva_post_Comamonas_sp_Y1.csv");
geneExpression::write.expr_matrix(gsva_k1, "E:/codebuddy/GCModeller/test/omics/community/gsva_post_Acinetobacter_sp_K1.csv");
geneExpression::write.expr_matrix(gsva_b1, "E:/codebuddy/GCModeller/test/omics/community/gsva_post_Pseudomonas_sp_B1.csv");
geneExpression::write.expr_matrix(gsva_h1, "E:/codebuddy/GCModeller/test/omics/community/gsva_post_Bacillus_sp_H1.csv");

let group_wt = list(
  YKB_WT = c("YKB_WT_WT_R1", "YKB_WT_WT_R2", "YKB_WT_WT_R3"),
  YKB_QSR = c("YKB_WT_QSR_R1", "YKB_WT_QSR_R2", "YKB_WT_QSR_R3"),
  HKB_WT = c("HKB_WT_WT_R1", "HKB_WT_WT_R2", "HKB_WT_WT_R3"),
  HKB_QSR = c("HKB_WT_QSR_R1", "HKB_WT_QSR_R2", "HKB_WT_QSR_R3")
);

let agg_k1 = geneExpression::aggregate_samples(gsva_k1, group_wt);
let agg_b1 = geneExpression::aggregate_samples(gsva_b1, group_wt);

geneExpression::write.expr_matrix(agg_k1, "E:/codebuddy/GCModeller/test/omics/community/gsva_post_Acinetobacter_sp_K1_groups.csv");
geneExpression::write.expr_matrix(agg_b1, "E:/codebuddy/GCModeller/test/omics/community/gsva_post_Pseudomonas_sp_B1_groups.csv");

# ---- biological conclusions from the GSVA pathway scores ----

let ykb_wt = c("YKB_WT_WT_R1", "YKB_WT_WT_R2", "YKB_WT_WT_R3");
let hkb_wt = c("HKB_WT_WT_R1", "HKB_WT_WT_R2", "HKB_WT_WT_R3");

let k1_ykb = geneExpression::project(gsva_k1, ykb_wt);
let b1_ykb = geneExpression::project(gsva_b1, ykb_wt);
let y1_ykb = geneExpression::project(gsva_y1, ykb_wt);
let k1_hkb = geneExpression::project(gsva_k1, hkb_wt);
let b1_hkb = geneExpression::project(gsva_b1, hkb_wt);
let h1_hkb = geneExpression::project(gsva_h1, hkb_wt);

# --- YKB: K1 > B1 > Y1 on the herbicide degradation pathway (map00791)
let k1_herb = geneExpression::feature_mean(k1_ykb, "map00791");
let b1_herb = geneExpression::feature_mean(b1_ykb, "map00791");
let y1_herb = geneExpression::feature_mean(y1_ykb, "map00791");

print(sprintf("  YKB map00791: K1=%s B1=%s Y1=%s", k1_herb, b1_herb, y1_herb));

let c10 = DebugAudit::test(k1_herb > b1_herb && b1_herb > y1_herb, "in SynCom YKB, K1 should dominate the herbicide degradation pathway");
if (c10 != TRUE) { stop("[FAIL] step 6: in YKB, K1 should have the highest map00791 score"); }

report = c(report, sprintf("[PASS] 6.1 YKB map00791 (herbicide degradation): K1=%s > B1=%s > Y1=%s", k1_herb, b1_herb, y1_herb));

# --- YKB: Y1 > K1 on the glycolysis pathway (map00010)
let y1_carb = geneExpression::feature_mean(y1_ykb, "map00010");
let k1_carb = geneExpression::feature_mean(k1_ykb, "map00010");

print(sprintf("  YKB map00010: Y1=%s K1=%s", y1_carb, k1_carb));

let c11 = DebugAudit::test(y1_carb > k1_carb, "in SynCom YKB, Y1 should dominate the glycolysis pathway");
if (c11 != TRUE) { stop("[FAIL] step 6: in YKB, Y1 should have the highest map00010 score"); }

report = c(report, sprintf("[PASS] 6.2 YKB map00010 (glycolysis): Y1=%s > K1=%s", y1_carb, k1_carb));

# --- YKB: B1 > K1 on the tryptophan metabolism pathway (map00380)
let b1_trp = geneExpression::feature_mean(b1_ykb, "map00380");
let k1_trp = geneExpression::feature_mean(k1_ykb, "map00380");

print(sprintf("  YKB map00380: B1=%s K1=%s", b1_trp, k1_trp));

let c12 = DebugAudit::test(b1_trp > k1_trp, "in SynCom YKB, B1 should dominate the tryptophan metabolism pathway");
if (c12 != TRUE) { stop("[FAIL] step 6: in YKB, B1 should have the highest map00380 score"); }

report = c(report, sprintf("[PASS] 6.3 YKB map00380 (tryptophan metabolism): B1=%s > K1=%s", b1_trp, k1_trp));

# --- HKB: B1 > K1 > H1 on the herbicide degradation pathway (map00791)
let b1_herb_hkb = geneExpression::feature_mean(b1_hkb, "map00791");
let k1_herb_hkb = geneExpression::feature_mean(k1_hkb, "map00791");
let h1_herb_hkb = geneExpression::feature_mean(h1_hkb, "map00791");

print(sprintf("  HKB map00791: B1=%s K1=%s H1=%s", b1_herb_hkb, k1_herb_hkb, h1_herb_hkb));

let c13 = DebugAudit::test(b1_herb_hkb > k1_herb_hkb && k1_herb_hkb > h1_herb_hkb, "in SynCom HKB, B1 should dominate the herbicide degradation pathway");
if (c13 != TRUE) { stop("[FAIL] step 6: in HKB, B1 should have the highest map00791 score"); }

report = c(report, sprintf("[PASS] 6.4 HKB map00791 (herbicide degradation): B1=%s > K1=%s > H1=%s", b1_herb_hkb, k1_herb_hkb, h1_herb_hkb));

# --- HKB: H1 > B1 > K1 on the sulfur metabolism pathway (map00920)
let h1_sulfur = geneExpression::feature_mean(h1_hkb, "map00920");
let b1_sulfur = geneExpression::feature_mean(b1_hkb, "map00920");
let k1_sulfur = geneExpression::feature_mean(k1_hkb, "map00920");

print(sprintf("  HKB map00920: H1=%s B1=%s K1=%s", h1_sulfur, b1_sulfur, k1_sulfur));

let c14 = DebugAudit::test(h1_sulfur > b1_sulfur && b1_sulfur > k1_sulfur, "in SynCom HKB, H1 should dominate the sulfur metabolism pathway");
if (c14 != TRUE) { stop("[FAIL] step 6: in HKB, H1 should have the highest map00920 score"); }

report = c(report, sprintf("[PASS] 6.5 HKB map00920 (sulfur metabolism): H1=%s > B1=%s > K1=%s", h1_sulfur, b1_sulfur, k1_sulfur));

# --- QS inhibition: AHL knockdown suppresses the K1 herbicide program in YKB
let ykb_qsr = c("YKB_WT_QSR_R1", "YKB_WT_QSR_R2", "YKB_WT_QSR_R3");
let k1_ykb_qsr = geneExpression::project(gsva_k1, ykb_qsr);
let k1_herb_qsr = geneExpression::feature_mean(k1_ykb_qsr, "map00791");

print(sprintf("  YKB map00791 K1: WT=%s QSR=%s", k1_herb, k1_herb_qsr));

let c15 = DebugAudit::test(k1_herb > k1_herb_qsr, "AHL inhibition should suppress the K1 herbicide degradation program in YKB");
if (c15 != TRUE) { stop("[FAIL] step 6: AHL inhibition should lower the K1 map00791 score"); }

report = c(report, sprintf("[PASS] 6.6 YKB map00791 K1: WT=%s > QSR=%s (AHL inhibition)", k1_herb, k1_herb_qsr));

# --- HKB: AI-2 inhibition suppresses the B1 herbicide program
let hkb_qsr = c("HKB_WT_QSR_R1", "HKB_WT_QSR_R2", "HKB_WT_QSR_R3");
let b1_hkb_qsr = geneExpression::project(gsva_b1, hkb_qsr);
let b1_herb_qsr = geneExpression::feature_mean(b1_hkb_qsr, "map00791");

print(sprintf("  HKB map00791 B1: WT=%s QSR=%s", b1_herb_hkb, b1_herb_qsr));

let c16 = DebugAudit::test(b1_herb_hkb > b1_herb_qsr, "AI-2 inhibition should suppress the B1 herbicide degradation program in HKB");
if (c16 != TRUE) { stop("[FAIL] step 6: AI-2 inhibition should lower the B1 map00791 score"); }

report = c(report, sprintf("[PASS] 6.7 HKB map00791 B1: WT=%s > QSR=%s (AI-2 inhibition)", b1_herb_hkb, b1_herb_qsr));

# ---------------------------------------------------------------------
# 7. write the report
# ---------------------------------------------------------------------
let failures = DebugAudit::failure_count();

report = c(report, "", sprintf("summary: %s assertion(s), %s failure(s)", 16, failures));
report = c(report, "[PASS] all of the community decomposition + KEGG GSVA checks passed");

writeLines(report, con = "E:/codebuddy/GCModeller/test/omics/community/test_report.txt");
DebugAudit::save_report("E:/codebuddy/GCModeller/test/omics/community/debug_audit_report.json");

if (failures != 0) {
    stop("[FAIL] unexpected DebugAudit failures");
}

print("[PASS] community decomposition + KEGG GSVA test finished");
