# 加载所需包
library(limma)
library(ggplot2)

# 1. 创建模拟数据（根据用户输入）
expr <- matrix(rnorm(1000 * 4, mean = 10), nrow = 1000, 
               dimnames = list(paste0("Gene", 1:1000), NULL))
expr[1:300, 3:4] <- expr[1:300, 3:4] + 120  # 前10个基因在处理组上调
expr[500:900, 1:2] <- expr[500:900, 1:2] + 320  # 前10个基因在处理组上调
groups <- c("Control", "Control", "Treat", "Treat")

# 2. 数据预处理
# 对数转换（避免负值）
expr_log <- log2(expr + 1)
# 标准化处理
expr_norm <- normalizeBetweenArrays(expr_log, method = "quantile")

colnames(expr_norm) = groups;

write.csv(expr_norm, file = "./expr_demo.csv");

# 3. 构建分组矩阵
design <- model.matrix(~0 + factor(groups))
colnames(design) <- c("Control", "Treat")
rownames(design) <- colnames(expr)

# 4. 构建比较矩阵
contrast_matrix <- makeContrasts(Treat_vs_Control = Treat - Control, levels = design)

# 5. 线性模型拟合
fit <- lmFit(expr_norm, design)
fit2 <- contrasts.fit(fit, contrast_matrix)
fit2 <- eBayes(fit2)

# 6. 提取差异表达结果
DEG_results <- topTable(fit2, 
                        number = Inf, 
                        adjust.method = "BH", 
                        sort.by = "P")

# 7. 标记显著差异基因
DEG_results$Significant <- ifelse(
  DEG_results$adj.P.Val < 0.05 & abs(DEG_results$logFC) > 1,
  ifelse(DEG_results$logFC > 1, "Up", "Down"),
  "Not Significant"
)

write.csv(DEG_results, file = "rlimma_degs.csv");

# 8. 可视化：火山图
print(ggplot(DEG_results, aes(x = logFC, y = -log10(P.Value), color = Significant)) +
  geom_point(alpha = 0.6) +
  scale_color_manual(values = c("Down" = "blue", "Not Significant" = "grey", "Up" = "red")) +
  geom_hline(yintercept = -log10(0.05), linetype = "dashed") +
  geom_vline(xintercept = c(-1, 1), linetype = "dashed") +
  labs(title = "差异表达基因火山图",
       x = "Log2 Fold Change (Treat vs Control)",
       y = "-Log10 P-value") +
  theme_bw());

# 9. 提取显著差异基因
significant_genes <- subset(DEG_results, adj.P.Val < 0.05 & abs(logFC) > 1)
upregulated <- significant_genes[significant_genes$logFC > 1, ]
downregulated <- significant_genes[significant_genes$logFC < -1, ]

# 输出结果
cat("显著上调基因数量:", nrow(upregulated), "\n")
cat("显著下调基因数量:", nrow(downregulated), "\n")
head(significant_genes[order(significant_genes$logFC, decreasing = TRUE), ])