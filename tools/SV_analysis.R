# ==============================================================================
# 1. 环境准备与数据加载
# ==============================================================================

# 加载必要的R包，如果没有安装请先安装
required_packages <- c("tidyverse", "pheatmap", "RColorBrewer", "patchwork")
for (pkg in required_packages) {
  if (!requireNamespace(pkg, quietly = TRUE)) install.packages(pkg)
  library(pkg, character.only = TRUE)
}

for(dir in c("Sevevo+AL8-78","Sevevo+AL8-78-4D")) {

  setwd(dir);

  # 设置文件路径
  file_path <- "limma+SV+PAV.csv"

  # 读取数据
  # check.names = FALSE 防止R自动修改列名（如PAV-AT23中的横杠）
  raw_data <- read.csv(file_path, header = TRUE, row.names = 1, check.names = FALSE, fileEncoding = "UTF-8")

  # 查看数据结构
  print("数据维度:")
  print(dim(raw_data))
  print("前几列预览:")
  print(head(raw_data[, 1:8]))

  # ==============================================================================
  # 2. 数据清洗与整理
  # ==============================================================================

  # 提取关键信息列
  # 注意：根据你的表格，第一列可能是行名，也可能是重复的ID列，这里做统一处理
  # 假设表格第一列是 GeneID，或者是空列，我们需要确认哪一列作为行名
  # 检查第一列是否全为空或者是ID，如果是ID则用作行名
  # if(names(raw_data)[1] == "" || names(raw_data)[1] %in% raw_data[,2]) {
  #   # 如果第一列没有名字或者与第二列相同，通常第一列是行名
  #   # 但read.csv读入时如果是空表头会自动填充X或行号
  #   # 这里保守处理：将FamilyID或第一列设为行名以便于后续画图
  #   if("FamilyID" %in% colnames(raw_data)){
  #     rownames(raw_data) <- make.names( raw_data$FamilyID, unique=TRUE)
  #   } else {
  #     rownames(raw_data) <- raw_data[,1]
  #   }
  # }

  # 填充SV Type中的空值，统一标记
  raw_data$Type[raw_data$Type %in% c("NULL", "None", "", NA)] <- "No SV"

  # 将logFC分类为 Up 和 Down (阈值可调整，这里假设你的Top100已经筛选过，但为了可视化做个标记)
  raw_data$Expression_Direction <- ifelse(raw_data$logFC > 0, "Up-regulated", "Down-regulated")

  # ==============================================================================
  # 3. 泛基因组与SV特征统计
  # ==============================================================================

  # 3.1 泛基因组分类统计
  cat_stats <- raw_data %>%
    group_by(Category) %>%
    summarise(Count = n()) %>%
    arrange(desc(Count))

  p1 <- ggplot(cat_stats, aes(x = reorder(Category, -Count), y = Count, fill = Category)) +
    geom_bar(stat = "identity") +
    theme_minimal() +
    labs(title = "Pan-genome Category Distribution", x = "Category", y = "Number of DEGs") +
    theme(axis.text.x = element_text(angle = 45, hjust = 1))

  # 3.2 SV类型统计
  sv_stats <- raw_data %>%
    group_by(Type) %>%
    summarise(Count = n())

  p2 <- ggplot(sv_stats, aes(x = reorder(Type, -Count), y = Count, fill = Type)) +
    geom_bar(stat = "identity") +
    theme_minimal() +
    labs(title = "Structural Variation (SV) Types", x = "SV Type", y = "Count") +
    theme(axis.text.x = element_text(angle = 45, hjust = 1))

  # 组合展示
  ggsave("Pan-genome-SV-category.pdf", p1 + p2 + plot_annotation(title = 'Global Statistics of Top 100 DEGs'), width = 36, height = 18, units = "cm");


  # ==============================================================================
  # 4. 转录组与SV/PAV关联分析
  # ==============================================================================

  # 4.1 分析SV类型与表达量的关系 (Boxplot)
  p3 <- ggplot(raw_data, aes(x = Type, y = logFC, fill = Type)) +
    geom_boxplot() +
    geom_jitter(width = 0.2, alpha = 0.5, size = 1) + # 添加散点
    theme_bw() +
    labs(title = "Log2FC Distribution by SV Type",
        subtitle = "Checking if specific SVs correlate with expression changes",
        x = "SV Type", y = "Log2 Fold Change") +
    theme(axis.text.x = element_text(angle = 45, hjust = 1)) +
    coord_flip() # 翻转坐标轴以便于阅读长标签

  ggsave("Pan-genome-SV-logfc.pdf",  p3, width = 18, height = 12, units = "cm")

  # 4.2 PAV矩阵可视化
  # 提取PAV列 (列名包含 "PAV-" 的列)
  pav_cols <- grep("PAV-", colnames(raw_data), value = TRUE)
  pav_matrix <- raw_data[, pav_cols]

  # 将数据转换为数值矩阵 (0/1)
  pav_matrix <- as.matrix(pav_matrix)
  mode(pav_matrix) <- "numeric"
  rownames(pav_matrix) <- raw_data$FamilyID # 设置行名

  # 绘制PAV热图
  # 如果基因数量较多（100个），热图会比较密集，这里调整了格子大小
  p4 <- pheatmap(pav_matrix, 
                cluster_rows = TRUE, 
                cluster_cols = TRUE,
                color = c("lightgrey", "steelblue"), # 0为灰色，1为蓝色
                main = "PAV Presence/Absence Pattern Across Varieties",
            legend_breaks = c(0, 1),
            legend_labels = c("Absence (0)", "Presence (1)"),
            show_rownames = FALSE, # 基因太多不显示行名，如果需要改为TRUE
            border_color = NA)

  ggsave("Pan-genome-PAV-heatmap.pdf", p4, width = 16, height = 20, units = "cm")

  # ==============================================================================
  # 5. 核心候选基因筛选
  # ==============================================================================

  # 筛选策略：
  # 1. 基因表达量变化显著 (Top100已经满足)
  # 2. 存在SV变异 (SV.Type != "No SV") 或者 存在PAV差异 (品种间PAV值不同)
  # 3. CNV逻辑：拷贝数增加通常导致高表达，拷贝数丢失导致低表达

  # 5.1 筛选有SV变异的基因
  sv_genes <- raw_data %>%
    filter(Type != "No SV") %>%
    select(FamilyID, logFC, SwissprotName, Description, Type, CopyNumber, Median, Category, Pathway)

  # 5.2 筛选CNV与表达方向一致的基因 (这是最可能的候选基因)
  # 逻辑：logFC > 0 且 CNV_Gain -> 可能由拷贝数增加导致高表达
  # 逻辑：logFC < 0 且 CNV_Loss -> 可能由拷贝数缺失导致低表达
  cnv_candidates <- raw_data %>%
    filter(Type == "CNV_Gain" & logFC > 0 | Type == "CNV_Loss" & logFC < 0) %>%
    mutate(Consistency = "Consistent") %>%
    select(FamilyID, logFC, SwissprotName, Type, CopyNumber, GenomeName, Description)

  # 5.3 筛选PAV特异基因 (在特定品种中特异存在/缺失)
  # 计算每个基因在不同品种中的PAV总和，如果不等于0或3（假设有3个品种），说明有差异
  raw_data$PAV_Sum <- rowSums(raw_data[, pav_cols], na.rm = TRUE)
  pav_candidates <- raw_data %>%
    filter(Category %in% c("Unique", "SoftCore") | (Type %in% c("PAV_Presence", "PAV_Absence"))) %>%
    select(FamilyID, logFC, SwissprotName, Type, Category, Description)

  # ==============================================================================
  # 6. 结果输出
  # ==============================================================================

  # 打印关键候选基因到控制台
  print("================================================================================")
  print("Core Candidate Genes with Consistent CNV-Expression Pattern (High Priority):")
  print(cnv_candidates)

  # 保存分析结果到CSV文件
  output_file <- "Integrated_Analysis_Results.csv"
  final_results <- raw_data %>%
    mutate(PAV_Pattern = apply(raw_data[, pav_cols], 1, paste0, collapse = "-")) %>%
    select(FamilyID, logFC, SwissprotName, Type, CopyNumber, Median, Category, PAV_Pattern, Description, Pathway)

  write.csv(final_results, output_file, row.names = FALSE)

  print(paste("Analysis complete. Results saved to:", output_file))

  # 保存筛选出的CNV一致性基因
  write.csv(cnv_candidates, "Candidate_CNV_Genes.csv", row.names = FALSE)
  write.csv(pav_candidates, "Candidate_PAV_Genes.csv", row.names = FALSE)

  setwd("..");
}
