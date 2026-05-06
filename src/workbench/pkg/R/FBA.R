# =========================================================================
# 基于化学计量矩阵的 FBA (通量平衡分析) R脚本
# =========================================================================

const FBA_solver = function(S_df, obj_rxns) {
    library(lpSolve);
    library(dplyr);

    # =========================================================================
    # 2. 数据输入 (请根据实际情况修改路径和变量)
    # =========================================================================

    if (!is.data.frame(S_df)) {
      # 读取化学计量矩阵 CSV 文件
      # 假设第一列为代谢物名称(行名)，第一行为反应ID(列名)
      S_df <- read.csv(S_df, row.names = 1, check.names = FALSE);
    }

    # 目标反应ID集合 (即需要最大化流量的反应)
    # 示例：obj_rxns <- c("BIOMASS_Ecoli_core_w_GAM", "EX_glc__D_e")
    # obj_rxns <- c("你的目标反应ID1", "你的目标反应ID2") 

    # =========================================================================
    # 3. 数据预处理
    # =========================================================================

    # 将数据框转换为数值矩阵
    S <- as.matrix(S_df)

    # 获取反应ID和代谢物ID
    rxn_ids <- colnames(S)
    met_ids <- rownames(S)

    n_rxns <- length(rxn_ids)
    n_mets <- length(met_ids)

    # 验证目标反应是否在矩阵中
    missing_rxns <- obj_rxns[!obj_rxns %in% rxn_ids]
    if (length(missing_rxns) > 0) {
      stop(paste("错误：以下目标反应不在矩阵的反应列中:", paste(missing_rxns, collapse = ", ")))
    }

    # 构建目标函数向量 c (默认全为0，目标反应系数为1)
    # 如果有多个目标反应，等同于最大化它们流量之和
    c_vec <- rep(0, n_rxns)
    c_vec[rxn_ids %in% obj_rxns] <- 1

    # =========================================================================
    # 4. 设置约束条件
    # =========================================================================

    # 约束1: 稳态约束 S * v = 0
    # lpSolve 要求将约束写成 Ax <= b 的形式
    # 对于等式约束，方向设为 "="
    const_mat <- S
    const_dir <- rep("=", n_mets)
    const_rhs <- rep(0, n_mets)

    # 约束2: 反应流量边界约束 lb <= v <= ub
    # 【重要说明】CSV中通常不包含边界信息，这里设置默认边界：
    # 默认所有反应可逆: 下限 -1000, 上限 1000
    default_lb <- -1000
    default_ub <- 1000

    lower_bounds <- rep(default_lb, n_rxns)
    upper_bounds <- rep(default_ub, n_rxns)

    # 【进阶修改】如果你的模型中有不可逆反应，或者需要限制特定底物的摄入，
    # 你可以在此处修改 lower_bounds 和 upper_bounds。
    # 例如：限制葡萄糖摄入反应(EX_glc)最大摄入量为10：
    # ex_glc_idx <- which(rxn_ids == "EX_glc__D_e")
    # lower_bounds[ex_glc_idx] <- -10  # 摄入通常用负数表示

    # =========================================================================
    # 5. 求解 FBA 线性规划问题
    # =========================================================================

    cat("开始求解 FBA 模型...\n")

    fba_result <- lp(
      direction = "max",          # 最大化目标函数
      objective.in = c_vec,       # 目标函数系数
      const.mat = const_mat,      # 约束矩阵 S
      const.dir = const_dir,      # 约束方向 (=)
      const.rhs = const_rhs,      # 约束右侧值 (0)
      lower.bounds = lower_bounds,# 流量下限
      upper.bounds = upper_bounds # 流量上限
    )

    # =========================================================================
    # 6. 结果提取与输出
    # =========================================================================

    if (fba_result$status == 0) {
      cat("FBA 求解成功！\n")
      
      # 获取最优目标函数值 (最大通量)
      opt_obj_val <- fba_result$objval
      cat("目标反应最大流量:", opt_obj_val, "\n")
      
      # 获取所有反应的通量分布
      flux_dist <- fba_result$solution
      names(flux_dist) <- rxn_ids
      
      # 将结果整理为数据框
      result_df <- data.frame(
        ReactionID = rxn_ids,
        Flux = flux_dist,
        LowerBound = lower_bounds,
        UpperBound = upper_bounds,
        stringsAsFactors = FALSE
      )
      
      # 筛选出有实际流量的反应（绝对值 > 1e-6），方便查看
      active_flux_df <- result_df %>% filter(abs(Flux) > 1e-6)
      
      # 保存完整结果和活跃反应结果
      write.csv(result_df, "FBA_Full_Flux_Distribution.csv", row.names = FALSE)
      write.csv(active_flux_df, "FBA_Active_Flux_Distribution.csv", row.names = FALSE)
      
      cat("结果已保存至 'FBA_Full_Flux_Distribution.csv' 和 'FBA_Active_Flux_Distribution.csv'\n")
      
    } else {
      cat("FBA 求解失败！\n")
      cat("状态码:", fba_result$status, "\n")
      cat("可能原因: \n")
      cat("  1. 模型不可行 - 检查边界约束是否允许底物摄入和产物排出\n")
      cat("  2. 模型无界 - 检查是否遗漏了必要的不可逆反应约束(lower_bound=0)\n")
    }
}