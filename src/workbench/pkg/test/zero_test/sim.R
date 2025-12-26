# ==============================================================================
# 1. 环境设置与辅助函数
# ==============================================================================

# 如果没有安装jsonlite，请先取消下面一行的注释进行安装
# install.packages("jsonlite")
library(jsonlite)

# 设置随机种子以保证结果可复现
set.seed(42)

# 辅助函数：生成生物学意义的名称前缀
generate_bio_names <- function() {
  # 基因簇相关的生物学主题（微生物发酵相关）
  cluster_prefixes <- c(
    "Glycolysis", "TCA_Cycle", "Ethanol_Production", "Acetate_Formation",
    "Heat_Shock", "Oxidative_Stress", "Nitrogen_Metabolism", "Sulfur_Assimilation",
    "Cell_Wall_Biosynthesis", "Ribosome_Biogenesis", "DNA_Replication", "RNA_Polymerase",
    "Fatty_Acid_Synth", "Amino_Acid_Degradation", "Sugar_Transport", "pH_Homeostasis"
  )
  
  # 样本分组相关的名称
  group_prefixes <- c(
    "Strain_Y203", "Strain_Y204", "Strain_Mutant_A", 
    "Wild_Type_Control", "High_Osmolarity", "Anaerobic_Condition"
  )
  
  return(list(cluster = cluster_prefixes, group = group_prefixes))
}

# ==============================================================================
# 2. 生成基因簇数据
# ==============================================================================

cat("正在生成基因簇数据...\n")

bio_names <- generate_bio_names()
num_clusters <- 50
cluster_info <- list()
all_gene_pool <- c() # 用于存储所有已生成的基因，以便处理重叠

for (i in 1:num_clusters) {
  # 随机选择一个生物学主题前缀
  theme <- sample(bio_names$cluster, 1)
  cluster_name <- paste0(sprintf("%02d", i),"-", theme)
  
  # 确定该簇的基因数量 (20 - 50)
  total_genes_in_cluster <- sample(20:50, 1)
  
  # 确定重叠基因数量 (0 - 9)
  # 只有当基因池足够大时才进行重叠
  if (length(all_gene_pool) > 0) {
    overlap_n <- sample(0:9, 1)
    overlap_n <- min(overlap_n, length(all_gene_pool)) # 防止超出范围
  } else {
    overlap_n <- 0
  }
  
  # 选取重叠基因
  overlap_genes <- c()
  if (overlap_n > 0) {
    overlap_genes <- sample(all_gene_pool, overlap_n)
  }
  
  # 生成新基因
  new_n <- total_genes_in_cluster - overlap_n
  # 生成新基因ID: 主题缩写_数字编号
  theme_abbr <- substr(theme, 1, 3) # 取前3个字母作为缩写
  # 使用一个不断增长的计数器来生成ID，防止ID重复
  start_idx <- length(all_gene_pool) + 1000 
  new_ids <- paste0(toupper(theme_abbr), "_", sprintf("%04d", start_idx:(start_idx + new_n - 1)))
  
  # 合并当前簇的基因
  current_genes <- c(new_ids, overlap_genes)
  # 随机打乱当前簇内基因顺序，避免重叠基因总排在前面
  current_genes <- sample(current_genes)
  
  # 更新基因簇信息
  cluster_info[[cluster_name]] <- current_genes
  
  # 更新全局基因池
  all_gene_pool <- unique(c(all_gene_pool, current_genes))
  
  cat(sprintf("生成簇 %s: 总数 %d, 新增 %d, 重叠 %d\n", 
              cluster_name, total_genes_in_cluster, new_n, overlap_n))
}

# 保存基因簇信息为JSON
json_output_file <- "gene_clusters.json"
write_json(cluster_info, json_output_file, pretty = TRUE, auto_unbox = TRUE)
cat(sprintf("基因簇信息已保存至: %s\n", json_output_file))

# ==============================================================================
# 3. 生成表达矩阵
# ==============================================================================

cat("正在生成表达矩阵...\n")

# 定义样本分组
num_groups <- 6
groups <- bio_names$group[1:num_groups]
replicates_per_group <- 9
samples <- c()

# 构建样本名称列表和分组标签
group_labels <- c()
for (g in groups) {
  for (r in 1:replicates_per_group) {
    sample_name <- paste0(g, "_Rep", sprintf("%02d", r))
    samples <- c(samples, sample_name)
    group_labels <- c(group_labels, g)
  }
}

# --- 错误修正处 ---
# 错误原因：之前的代码写了两个 nrow 参数
# 修正：将第二个 nrow 改为 ncol
expr_matrix <- matrix(0, nrow = length(all_gene_pool), ncol = length(samples))
rownames(expr_matrix) <- all_gene_pool
colnames(expr_matrix) <- samples

# 为每个样本组分配显著高表达的基因簇 (3到10个)
group_active_clusters <- list()

for (g_idx in 1:num_groups) {
  group_name <- groups[g_idx]
  # 随机选择 3 到 10 个簇
  n_active <- sample(3:10, 1)
  active_cluster_names <- sample(names(cluster_info), n_active)
  
  group_active_clusters[[group_name]] <- active_cluster_names
  cat(sprintf("分组 [%s] 将高表达以下 %d 个簇: %s...\n", 
              group_name, n_active, paste(head(active_cluster_names, 2), collapse=",")))
}

# 填充表达矩阵 (基于二项分布)
# 假设测序深度 size = 100 (模拟归一化后的 Counts 或 CPM 类整数)
# 背景表达概率 p_base = 0.1
# 高表达概率 p_high = 0.6

size_binom <- 100
p_base <- 0.1
p_high <- 0.6

cat("开始计算样本表达值...\n")

# 获取所有高表达基因的集合
group_high_genes <- lapply(names(group_active_clusters), function(g_name) {
  active_clusts <- group_active_clusters[[g_name]]
  genes_in_active_clusts <- unlist(cluster_info[active_clusts])
  return(unique(genes_in_active_clusts))
})
names(group_high_genes) <- names(group_active_clusters)

# 遍历每个样本
for (s_col in 1:ncol(expr_matrix)) {
  current_group <- group_labels[s_col]
  
  # 获取当前组的高表达基因列表
  high_genes_set <- group_high_genes[[current_group]]
  
  # 向量化计算所有基因的表达概率
  # 如果基因在高表达集合中，概率为 p_high，否则为 p_base
  probs <- ifelse(rownames(expr_matrix) %in% high_genes_set, p_high, p_base)
  
  # 生成二项分布随机数
  # n = nrow(expr_matrix) 表示生成与基因数量相同的随机数
  expr_matrix[, s_col] <- rbinom(n = nrow(expr_matrix), size = size_binom, prob = probs)
}

# ==============================================================================
# 4. 保存数据
# ==============================================================================

# 保存表达矩阵为CSV
csv_output_file <- "expression_matrix.csv"
write.csv(expr_matrix, file = csv_output_file, quote = FALSE)
cat(sprintf("表达矩阵已保存至: %s\n", csv_output_file))

# 保存样本分组信息元数据
metadata <- data.frame(
  Sample = colnames(expr_matrix),
  Group = group_labels,
  stringsAsFactors = FALSE
)
write.csv(metadata, "sample_metadata.csv", row.names = FALSE)
cat("样本元数据已保存至: test_sample_metadata.csv\n")

cat("\n=== 数据生成完成 ===\n")
cat(sprintf("总基因数: %d\n", length(all_gene_pool)))
cat(sprintf("总簇数: %d\n", length(cluster_info)))
cat(sprintf("样本数: %d (Groups: %d, Reps: %d)\n", ncol(expr_matrix), num_groups, replicates_per_group))
