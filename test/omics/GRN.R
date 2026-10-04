require(GCModeller);

imports "geneExpression" from "phenotype_kit";
imports "TRN" from "cytoscape";
imports "WGCNA" from "TRNtoolkit";

let TF = read.table("K:\hsa_grn\Homo_sapiens_TF.txt", header = TRUE);

print(TF, max.print = 6);

let hsa = geneExpression::load.expr("K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv") 
|> batch_normalize() 
|> top_variance(50000)
;
let top_genes = geneExpression::dims(hsa)$feature_names;

print("top variance gene features:");
print(top_genes);
print("hsa TF vector:");
print(TF$Ensembl);

# write_bicor: 一次性计算 bicor 相关矩阵缓存（{repo}/bicor.dat + index.dat），
#   并在同一趟顺带缓存 WGCNA 模块划分结果（{repo}/modules.dat）。
#   可选参数 wgcna_opts 用于自定义 WGCNA blockwise 配置（默认 GPU + buildGraph = FALSE）。
write_bicor(hsa, repo = "Z:/hsa_mat");

let bicor = open_bicor("Z:/hsa_mat");
let opts = new("GRN_opts", minAbsCorrelation = 0.3,  enableGpu = TRUE);

# build_grn 的模块映射解析优先级：
#   ① 显式传入的 modules 参数（可由 open_modules("Z:/hsa_mat") 读取缓存）
#   ② 自动定位边车缓存 {storeFile}.modules（指纹校验：基因行序 + WGCNA 配置）
#   ③ 缓存未命中/指纹失效 → 现算一次 blockwise 并自动写回边车
# 因此第二次及以后的 build_grn 不再重复运行 WGCNA 计算，秒级完成；
# 更换阈值参数做过滤实验时，直接修改 opts 再调用 build_grn 即可。
# let modules = open_modules("Z:/hsa_mat");
let grn = WGCNA::build_grn(hsa, TF$Ensembl, 
        opts = opts |> string_links(string_db = "K:\hsa_grn\string-db"),
        bicor = bicor
);

