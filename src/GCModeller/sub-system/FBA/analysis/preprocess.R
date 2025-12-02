# 将FBA结果转换为数据框
flux_data <- data.frame(
  reaction_id = c("R1", "R2", "R3", "R4", "R5"),
  flux_value = c(15.2, 8.7, 12.3, 3.5, 20.1),
  pathway = c("Glycolysis", "TCA", "PPP", "AA"),
  subsystem = c("Carbohydrate", "Energy", "Lipid", "Nucleotide")
)

# 对通量值进行标准化处理
flux_data$flux_scaled <- scale(flux_data$flux_value, center = TRUE, scale = TRUE)
flux_data$flux_abs <- abs(flux_data$flux_value)

# 按代谢通路分组
pathway_order <- c("Glycolysis", "TCA", "PPP", "AA")