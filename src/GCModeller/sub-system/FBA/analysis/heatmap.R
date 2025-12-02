# 加载必要的包
library(ggplot2)
library(pheatmap)
library(reshape2)

# 创建通量分布热图
create_flux_heatmap <- function(flux_data, output_file = "flux_heatmap.png") {
  # 数据重塑为矩阵格式
  flux_matrix <- reshape2::dcast(flux_data, reaction_id ~ pathway, value.var = "flux_value")
  flux_matrix[is.na(flux_matrix)] <- 0
  
  # 设置热图颜色方案
  color_palette <- colorRampPalette(c("#313695", "#4575B4", "#74AD81"))
  
  # 生成热图
  pheatmap(
    as.matrix(flux_matrix[-1]),  # 移除反应ID列
    col = color_palette(6),
    scale = "row",
    cluster_rows = FALSE,
    cluster_cols = FALSE,
    main = "代谢反应通量分布热图",
    xlab = "代谢通路",
    ylab = "反应ID",
    margins = c(5, 10),
    fontsize = 10,
    filename = output_file,
    width = 10,
    height = 8
  )
}

# 使用示例数据
example_flux_data <- data.frame(
  reaction_id = c("R1", "R2", "R3", "R4", "R5", "R6", "R7", "R8"),
  flux_value = c(18.5, 12.3, 8.7, 15.2, 22.1, 9.8, 25.6, 5.4),
  pathway = c("Glycolysis", "Glycolysis", "TCA", "TCA", "PPP", "AA", "AA", "AA"),
  subsystem = c("Carbohydrate", "Carbohydrate", "Energy", "Energy", "Energy", "Lipid", "Lipid","Lipid")
)

print(example_flux_data);

# 生成热图
create_flux_heatmap(example_flux_data)

# 关键反应识别：红色高亮区域表示高通量反应，可能是代谢瓶颈或关键调控点
# 通路活性分析：比较不同通路的整体颜色强度，评估各代谢途径的相对活性
# 代谢状态评估：通量分布模式可反映细胞在不同条件下的代谢状态变化