GSEA（Gene Set Enrichment Analysis）是一种用于确定预先定义的基因集是否在某一生物学状态（例如，肿瘤与正常组织）中显示出统计学上显著的富集的方法。以下是使用R语言进行GSEA富集分析的基本步骤。以下是一个简化的示例过程：

### 步骤1：准备数据

首先，你需要一个基因表达矩阵和一个预先定义的基因集。这里我们假设你已经有了这些数据。

```r
# 假设gene_expression是一个行代表基因，列代表样本的矩阵
gene_expression <- matrix(rnorm(100), nrow=10)
# 假设gene_sets是一个列表，每个元素是一个基因集
gene_sets <- list(
  pathway1 = c("gene1", "gene2", "gene3"),
  pathway2 = c("gene4", "gene5", "gene6")
)
# 假设每个样本都有一个分数，代表它们的表型，比如肿瘤与正常
phenotype_scores <- c(-1, -1, 1, 1, -1, 1, 1, -1, -1, 1)
```

### 步骤2：排序基因

根据表型分数对基因进行排序。

```r
# 计算每个基因的平均表达量
gene_means <- rowMeans(gene_expression)
# 结合基因的均值和表型分数进行排序
sorted_indices <- order(phenotype_scores * gene_means, decreasing = TRUE)
sorted_genes <- rownames(gene_expression)[sorted_indices]
```

### 步骤3：运行GSEA

对每个基因集运行GSEA算法。
```r
gsea_results <- lapply(names(gene_sets), function(pathway) {
  genes_in_pathway <- gene_sets[[pathway]]
  enrichment_score <- 0
  max_score <- 0
  
  for (gene in sorted_genes) {
    if (gene %in% genes_in_pathway) {
      enrichment_score <- enrichment_score + 1
    } else {
      enrichment_score <- enrichment_score - 1
    }
    if (abs(enrichment_score) > abs(max_score)) {
      max_score <- enrichment_score
    }
  }
  
  return(max_score)
})
names(gsea_results) <- names(gene_sets)
```

### 步骤4：解释结果

得到的`gsea_results`将包含每个基因集的富集分数。
```r
print(gsea_results)
```

这里的示例非常简化，实际的GSEA分析会涉及更多的统计步骤，包括标准化、多重假设检验校正等。在实际情况中，我们通常会使用专门的R包来进行GSEA分析，因为它们提供了更完整、更准确的实现，以及更易于理解的输出格式。

计算GSEA富集分析的p值涉及到统计测试，通常需要模拟或置换测试来评估富集分数的显著性。下面我将演示如何使用R的基础函数进行一个简单的置换测试来计算GSEA富集分析的p值。
我们将重复以下步骤：

1. 对基因进行排序，但这次我们将根据随机化的表型分数进行排序。
2. 计算随机化数据集的富集分数。
3. 重复步骤1和2多次以建立一个富集分数的分布。
4. 比较原始富集分数与随机化富集分数的分布，以计算p值。

以下是具体的R代码示例：
```r
# 假设我们已经有了基因表达矩阵、基因集和表型分数
# gene_expression <- matrix(rnorm(100), nrow=10)
# gene_sets <- list(...)
# phenotype_scores <- c(...)
# 定义一个函数来计算富集分数
calculate_enrichment_score <- function(sorted_genes, gene_set) {
  enrichment_score <- 0
  for (gene in sorted_genes) {
    if (gene %in% gene_set) {
      enrichment_score <- enrichment_score + 1
    } else {
      enrichment_score <- enrichment_score - 1
    }
  }
  return(abs(enrichment_score))
}
# 定义一个函数来进行置换测试
perform_permutation_test <- function(gene_expression, gene_sets, phenotype_scores, n_permutations = 1000) {
  # 计算原始数据的富集分数
  gene_means <- rowMeans(gene_expression)
  sorted_indices <- order(phenotype_scores * gene_means, decreasing = TRUE)
  sorted_genes <- rownames(gene_expression)[sorted_indices]
  original_scores <- sapply(names(gene_sets), function(pathway) {
    calculate_enrichment_score(sorted_genes, gene_sets[[pathway]])
  })
  
  # 初始化一个向量来存储置换后的富集分数
  permuted_scores <- matrix(NA, nrow = n_permutations, ncol = length(gene_sets))
  colnames(permuted_scores) <- names(gene_sets)
  
  # 进行置换测试
  for (i in 1:n_permutations) {
    permuted_phenotype <- sample(phenotype_scores)
    sorted_indices_permuted <- order(permuted_phenotype * gene_means, decreasing = TRUE)
    sorted_genes_permuted <- rownames(gene_expression)[sorted_indices_permuted]
    permuted_scores[i, ] <- sapply(names(gene_sets), function(pathway) {
      calculate_enrichment_score(sorted_genes_permuted, gene_sets[[pathway]])
    })
  }
  
  # 计算每个基因集的p值
  p_values <- apply(permuted_scores, 2, function(scores) {
    sum(scores >= original_scores) / n_permutations
  })
  
  return(p_values)
}
# 运行置换测试
p_values <- perform_permutation_test(gene_expression, gene_sets, phenotype_scores, n_permutations = 1000)
# 打印p值
print(p_values)
```
在这个示例中，`perform_permutation_test`函数会进行1000次置换测试，计算每个基因集的富集分数，并与原始数据集的富集分数进行比较，从而计算出每个基因集的p值。
请注意，这个示例是非常简化的，并且在实际应用中，GSEA的p值计算通常会更复杂，可能需要考虑更多因素，如基因集的大小、基因表达矩阵的标准化等。此外，这个示例也没有进行多重假设检验校正，这在实际分析中是必不可少的。
