# ==============================================================================
# 1. 环境配置与包加载
# ==============================================================================

# 检查并安装所需的包
required_packages <- c("jsonlite", "ggplot2", "dplyr", "tidyr", "openxlsx", "stringr")
new_packages <- required_packages[!(required_packages %in% installed.packages()[,"Package"])]
if(length(new_packages)) install.packages(new_packages)

# 加载包
library(jsonlite)
library(ggplot2)
library(dplyr)
library(tidyr)
library(openxlsx)
library(stringr)

# ==============================================================================
# 2. 数据读取
# ==============================================================================

# 设置JSON文件路径 (请修改为你的实际文件路径)
json_file_path <- "F:\\datapool\\20260312\\20260322\\pangenome_result.json";

setwd(dirname(json_file_path));

# 检查文件是否存在
if (!file.exists(json_file_path)) {
  stop(paste("错误：找不到文件", json_file_path))
}

# 读取JSON数据
# flatten = TRUE 可以自动将部分嵌套结构拉平，但针对复杂的PAV矩阵我们需要后续手动处理
cat("正在读取JSON文件...\n")
pan_data <- fromJSON(json_file_path, flatten = FALSE)

# ==============================================================================
# 3. 数据处理 (转换为数据框)
# ==============================================================================

cat("正在处理数据结构...\n")

# -----------------------------------------
# 3.1 基因组基本信息
# -----------------------------------------
df_total_genes <- bind_rows(pan_data$TotalGenesInGenomes, .id = "GenomeName")
# 注意：VB中的Dictionary在JSON中可能被序列化为Key-Value列表，需转换
# 如果 TotalGenesInGenomes 在JSON中是对象，fromJSON通常转为命名列表或数据框
if(is.data.frame(df_total_genes) && "GenomeName" %in% colnames(df_total_genes)) {
  # 修正bind_rows可能产生的列名问题
  colnames(df_total_genes) <- c("GenomeName", "TotalGenes")
} else if (is.list(pan_data$TotalGenesInGenomes)) {
  df_total_genes <- data.frame(
    GenomeName = names(pan_data$TotalGenesInGenomes),
    TotalGenes = unlist(pan_data$TotalGenesInGenomes),
    row.names = NULL
  )
}

# -----------------------------------------
# 3.2 泛基因组曲线数据
# -----------------------------------------
df_curve <- as.data.frame(pan_data$PangenomeCurveData)

# -----------------------------------------
# 3.3 基因家族分类统计 (核心、壳、云等)
# -----------------------------------------
# 提取各类基因家族的数量
category_stats <- data.frame(
  Category = character(),
  Count = integer(),
  stringsAsFactors = FALSE
)

# 辅助函数：安全获取数组长度
get_length <- function(field) {
  if (!is.null(pan_data[[field]]) && length(pan_data[[field]]) > 0) {
    return(length(pan_data[[field]]))
  } else {
    return(0)
  }
}

category_stats <- rbind(category_stats, data.frame(Category = "Core", Count = get_length("CoreGeneFamilies")))
category_stats <- rbind(category_stats, data.frame(Category = "SoftCore", Count = get_length("SoftCoreGeneFamilies")))
category_stats <- rbind(category_stats, data.frame(Category = "Shell", Count = get_length("ShellGeneFamilies")))
category_stats <- rbind(category_stats, data.frame(Category = "Cloud", Count = get_length("CloudGeneFamilies")))
category_stats <- rbind(category_stats, data.frame(Category = "Specific", Count = get_length("SpecificGeneFamilies")))

# -----------------------------------------
# 3.4 PAV 矩阵处理 (重点)
# -----------------------------------------
# VB结构: Dictionary(Of String, Dictionary(Of String, Integer))
# JSON结构: Key为FamilyID, Value为 { GenomeA: count, GenomeB: count ... }
if (!is.null(pan_data$PAVMatrix) && length(pan_data$PAVMatrix) > 0) {
  # 将列表的列表转换为Data Frame
  # bind_rows 会自动对齐列名(基因组名)
  df_pav <- bind_rows(pan_data$PAVMatrix, .id = "FamilyID")
  
  # 将NA替换为0 (表示缺失)
  df_pav[is.na(df_pav)] <- 0
  
  # 将数据转换为长格式以便于绘图 (可选)
  # df_pav_long <- pivot_longer(df_pav, cols = -FamilyID, names_to = "Genome", values_to = "CopyNumber")
} else {
  df_pav <- data.frame(Message = "No PAV Matrix data found")
}

# -----------------------------------------
# 3.5 共线性区块
# -----------------------------------------
if (!is.null(pan_data$CollinearBlocks) && length(pan_data$CollinearBlocks) > 0) {
  df_collinear <- bind_rows(pan_data$CollinearBlocks)
  
  # OrthologyLinks 是一个数组列，为了Excel展示，我们将其转换为字符串
  if("OrthologyLinks" %in% colnames(df_collinear)) {
    df_collinear$OrthologyLinks <- sapply(df_collinear$OrthologyLinks, function(x) {
      if(is.list(x)) paste(sapply(x, paste, collapse=","), collapse="; ") else ""
    })
  }
} else {
  df_collinear <- data.frame(Message = "No Collinear Blocks found")
}

# -----------------------------------------
# 3.6 结构变异 (SV)
# -----------------------------------------
if (!is.null(pan_data$StructuralVariations) && length(pan_data$StructuralVariations) > 0) {
  df_sv <- bind_rows(pan_data$StructuralVariations)
  # 将基因数组转换为字符串
  if("RelatedGenes" %in% colnames(df_sv)) {
    df_sv$RelatedGenes <- sapply(df_sv$RelatedGenes, paste, collapse="; ")
  }
} else {
  df_sv <- data.frame(Message = "No Structural Variations found")
}

# ==============================================================================
# 4. 导出 Excel
# ==============================================================================

cat("正在导出Excel文件...\n")

output_excel_path <- "PanGenome_Analysis_Report.xlsx"
wb <- createWorkbook()

addWorksheet(wb, "Summary")
writeData(wb, "Summary", df_total_genes)
writeData(wb, "Summary", category_stats, startRow = nrow(df_total_genes) + 3)

addWorksheet(wb, "PAV_Matrix")
writeData(wb, "PAV_Matrix", df_pav)

addWorksheet(wb, "Pangenome_Curve")
writeData(wb, "Pangenome_Curve", df_curve)

addWorksheet(wb, "Collinear_Blocks")
writeData(wb, "Collinear_Blocks", df_collinear)

addWorksheet(wb, "Structural_Variations")
writeData(wb, "Structural_Variations", df_sv)

# 保存文件
saveWorkbook(wb, output_excel_path, overwrite = TRUE)
cat(paste("Excel文件已保存至:", output_excel_path, "\n"))

# ==============================================================================
# 5. 数据可视化
# ==============================================================================

cat("正在生成可视化图表...\n")

# -----------------------------------------
# 图1: 泛基因组曲线
# -----------------------------------------
if(nrow(df_curve) > 0) {
  # 数据转换为长格式以便绘图
  curve_long <- df_curve %>%
    select(GenomeCount, TotalGenes, CoreGenes) %>%
    pivot_longer(cols = c(TotalGenes, CoreGenes), names_to = "Type", values_to = "GeneCount")
  
  p1 <- ggplot(curve_long, aes(x = GenomeCount, y = GeneCount, color = Type)) +
    geom_line(size = 1.2) +
    geom_point(size = 3) +
    scale_color_manual(values = c("TotalGenes" = "#E41A1C", "CoreGenes" = "#377EB8")) +
    labs(title = "Pangenome Growth Curve",
         x = "Number of Genomes Added",
         y = "Gene Count") +
    theme_bw() +
    theme(plot.title = element_text(hjust = 0.5, face = "bold"))
  
  ggsave("Plot_01_Pangenome_Curve.png", p1, width = 8, height = 6)
}

# -----------------------------------------
# 图2: 基因家族分类饼图或柱状图
# -----------------------------------------
if(nrow(category_stats) > 0 && sum(category_stats$Count) > 0) {
  # 过滤掉数量为0的类别
  cat_stats_filtered <- category_stats %>% filter(Count > 0)
  
  p2 <- ggplot(cat_stats_filtered, aes(x = reorder(Category, -Count), y = Count, fill = Category)) +
    geom_bar(stat = "identity", width = 0.7) +
    geom_text(aes(label = Count), vjust = -0.5) +
    scale_fill_brewer(palette = "Set2") +
    labs(title = "Gene Family Category Distribution",
         x = "Category",
         y = "Number of Gene Families") +
    theme_bw() +
    theme(plot.title = element_text(hjust = 0.5, face = "bold"),
          legend.position = "none") # 不需要图例，x轴已有标签
  
  ggsave("Plot_02_Category_Distribution.png", p2, width = 8, height = 6)
}

# -----------------------------------------
# 图3: 结构变异类型统计
# -----------------------------------------
if(nrow(df_sv) > 0 && "Type" %in% colnames(df_sv)) {
  sv_counts <- df_sv %>%
    group_by(Type) %>%
    summarise(Count = n())
  
  p3 <- ggplot(sv_counts, aes(x = reorder(Type, -Count), y = Count, fill = Type)) +
    geom_bar(stat = "identity") +
    geom_text(aes(label = Count), vjust = -0.5) +
    labs(title = "Structural Variation Types Distribution",
         x = "SV Type",
         y = "Count") +
    theme_bw() +
    theme(axis.text.x = element_text(angle = 45, hjust = 1),
          plot.title = element_text(hjust = 0.5, face = "bold"),
          legend.position = "none")
  
  ggsave("Plot_03_SV_Distribution.png", p3, width = 8, height = 6)
}

# -----------------------------------------
# 图4: 基因组基因总数柱状图
# -----------------------------------------
if(nrow(df_total_genes) > 0) {
  p4 <- ggplot(df_total_genes, aes(x = reorder(GenomeName, -TotalGenes), y = TotalGenes)) +
    geom_bar(stat = "identity", fill = "steelblue") +
    geom_text(aes(label = TotalGenes), vjust = -0.5) +
    labs(title = "Total Gene Count per Genome",
         x = "Genome",
         y = "Total Genes") +
    theme_bw() +
    theme(axis.text.x = element_text(angle = 45, hjust = 1),
          plot.title = element_text(hjust = 0.5, face = "bold"))
  
  ggsave("Plot_04_Genome_Sizes.png", p4, width = 10, height = 6)
}

cat("分析完成！\n")
