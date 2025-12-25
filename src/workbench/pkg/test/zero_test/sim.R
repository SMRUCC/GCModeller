# 加载必要的库
if (!require("jsonlite")) install.packages("jsonlite")
library(jsonlite)

# 设置随机种子以保证结果可复现
set.seed(42)

# ==========================================
# 1. 参数设置与命名定义
# ==========================================

# 生物学主题：微生物发酵
# 定义10个基因簇的名称
cluster_names <- c(
  "Glycolysis_Pathway",           # 糖酵解途径
  "Pentose_Phosphate_Pathway",    # 磷酸戊糖途径
  "TCA_Cycle",                    # 三羧酸循环
  "Ethanol_Fermentation",         # 乙醇发酵
  "Lactate_Production",           # 乳酸生成
  "Acetate_Formation",           # 乙酸形成
  "Butanol_Synthesis",           # 丁醇合成
  "Amino_Acid_Biosynthesis",     # 氨基酸生物合成
  "Fatty_Acid_Metabolism",       # 脂肪酸代谢
  "Stress_Response_Chaperones"   # 压力反应伴侣蛋白
)

# 定义9个样本分组的名称
group_names <- c(
  "High_Glucose",      # 高葡萄糖
  "Low_Glucose",       # 低葡萄糖
  "Anaerobic_Cond",    # 厌氧条件
  "Microaerophilic",   # 微好氧
  "High_Temp",         # 高温
  "Acidic_pH",         # 酸性pH
  "Nitrogen_Limited",  # 氮限制
  "Phosphate_Limited", # 磷限制
  "Stationary_Phase"   # 稳定期
)

# 模拟参数
n_clusters <- 10
n_groups <- 9
n_reps <- 9
lib_size <- 1000000  # 模拟的测序深度，即二项分布的size参数

# ==========================================
# 2. 生成基因簇
# ==========================================

cat("正在生成基因簇...\n")

all_genes <- c()      # 存储所有生成的唯一基因ID
clusters_list <- list() # 存储簇的结构信息

for (i in 1:n_clusters) {
  # 每个簇的基因数量在 30 到 90 之间
  n_genes <- sample(30:120, 1)
  
  # 生成新基因ID (例如 GENE_00001)
  # 确保ID不重复
  current_new_genes_count <- n_genes
  overlap_genes <- c()
  
  # 如果不是第一个簇，生成重叠
  if (i > 1) {
    # 随机决定重叠数量 0 到 9
    n_overlap <- sample(0:9, 1)
    
    if (n_overlap > 0 && length(all_genes) >= n_overlap) {
      # 从已存在的基因中随机选取重叠基因
      overlap_indices <- sample(length(all_genes), n_overlap)
      overlap_genes <- all_genes[overlap_indices]
    }
  }
  
  # 剩余部分生成全新的基因
  n_new_genes_to_create <- n_genes - length(overlap_genes)
  
  # 生成新基因ID
  start_id <- length(all_genes) + 1
  new_gene_ids <- paste0("GENE_", sprintf("%05d", start_id:(start_id + n_new_genes_to_create - 1)))
  
  # 组合当前簇的基因
  current_cluster_genes <- c(overlap_genes, new_gene_ids)
  
  # 更新全局基因列表
  all_genes <- c(all_genes, new_gene_ids)
  
  # 保存簇信息
  clusters_list[[cluster_names[i]]] <- sort(current_cluster_genes)
}

cat("生成完毕。总基因数:", length(all_genes), "\n")

# 保存基因簇信息为JSON
write_json(clusters_list, "gene_clusters.json", pretty = TRUE)
cat("基因簇信息已保存至 gene_clusters.json\n")

# ==========================================
# 3. 设计样本与差异表达模式
# ==========================================

cat("正在设计样本表达模式...\n")

# 生成样本名称
samples <- paste0(rep(group_names, each = n_reps), "_Rep_", 1:n_reps)
sample_info <- data.frame(Sample = samples, Group = rep(group_names, each = n_reps))

# 定义每个分组显著高表达的基因簇 (每个分组 2-3 个簇)
group_to_clusters <- list()

for (grp in group_names) {
  # 随机选择 2 到 3 个簇索引
  n_active_clusters <- sample(2:3, 1)
  active_indices <- sample(1:n_clusters, n_active_clusters)
  group_to_clusters[[grp]] <- cluster_names[active_indices]
}

# ==========================================
# 4. 生成表达矩阵
# ==========================================

cat("正在基于二项分布生成表达矩阵...\n")

# 初始化矩阵
expr_matrix <- matrix(0L, nrow = length(all_genes), ncol = length(samples))
rownames(expr_matrix) <- all_genes
colnames(expr_matrix) <- samples

# 设定基准表达概率
base_prob <- 0.0001  # 背景表达水平
high_prob_factor <- 20 # 高表达倍率 (即 prob * 20)

for (j in 1:ncol(expr_matrix)) {
  current_sample <- colnames(expr_matrix)[j]
  current_group <- sample_info$Group[sample_info$Sample == current_sample]
  
  # 获取该样本高表达的簇
  active_clusters <- group_to_clusters[[current_group]]
  
  # 获取这些簇中的基因ID
  active_genes <- c()
  for (cl in active_clusters) {
    active_genes <- c(active_genes, clusters_list[[cl]])
  }
  active_genes <- unique(active_genes)
  
  # 为每个基因设定二项分布的概率
  # 默认基准概率
  probs <- rep(base_prob, nrow(expr_matrix))
  
  # 对高表达基因增加概率
  # 稍微加入一些随机扰动，使高表达基因之间也有差异，不完全相同
  idx <- match(active_genes, rownames(expr_matrix))
  probs[idx] <- base_prob * high_prob_factor * runif(length(idx), 0.8, 1.2)
  
  # 确保概率不超过1
  probs[probs > 1] <- 1
  
  # 使用 rbinom 生成 Counts
  # size = lib_size (模拟总reads数), prob = 表达概率
  expr_matrix[, j] <- rbinom(n = nrow(expr_matrix), size = lib_size, prob = probs)
}

cat("表达矩阵生成完毕。\n")

# ==========================================
# 5. 保存文件
# ==========================================

# 转置矩阵以符合常规格式 (行=基因, 列=样本)
expr_df <- as.data.frame(expr_matrix)

# 保存为CSV
write.csv(expr_df, "expression_matrix.csv", row.names = TRUE)
cat("表达矩阵已保存至 expression_matrix.csv\n")

# 打印一些统计信息
cat("\n=== 数据集摘要 ===\n")
cat("基因总数:", nrow(expr_df), "\n")
cat("样本总数:", ncol(expr_df), "\n")
cat("分组数量:", n_groups, "\n")
cat("每组重复数:", n_reps, "\n")
cat("每组高表达基因簇数: 2-3\n")

# 打印各分组对应的高表达簇
cat("\n=== 各样本分组的高表达基因簇映射 ===\n")
for (grp in group_names) {
  cat(grp, " -> ", paste(group_to_clusters[[grp]], collapse = ", "), "\n")
}
