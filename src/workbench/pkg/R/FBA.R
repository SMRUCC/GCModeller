# =========================================================================
# 基于化学计量矩阵的 FBA (通量平衡分析) R脚本
# =========================================================================

#' Flux Balance Analysis (FBA) Solver in Native R
#'
#' Solves a Flux Balance Analysis (FBA) problem by formulating it as a linear
#' programming problem using the stoichiometric matrix and solving it with
#' the \code{lpSolve} package. Results, including full and active flux
#' distributions, are exported to the specified output directory.
#'
#' @param S_df A stoichiometric matrix of the reaction network. It can be either
#'   a data.frame (where rows are metabolites and columns are reactions) or a
#'   file path to a CSV file. If a file path is provided, the first column is
#'   assumed to contain metabolite names (rownames) and the first row to contain
#'   reaction IDs (colnames).
#' @param obj_rxns A character vector specifying the reaction flux IDs (a subset
#'   of the column names of \code{S_df}) to be maximized. If multiple reactions
#'   are provided, their combined flux is maximized.
#' @param outputdir A character string specifying the directory path for exporting
#'   FBA result files. Defaults to the current directory (\code{"./"}).
#' @param default_lb A numeric value specifying the default lower bound for every
#'   metabolic reaction flux. Defaults to \code{-1000} (assumes reversible
#'   reactions by default).
#' @param default_ub A numeric value specifying the default upper bound for every
#'   metabolic reaction flux. Defaults to \code{1000}.
#' @param flux_bounds A named list used to tweak specific reaction flux bounds. The
#'   name of each element should be the reaction ID, and the value should be a
#'   numeric vector of length 2 containing the lower and upper bounds:
#'   \code{list("flux_id" = c(lb, ub))}.
#'
#' @details The function formulates the standard FBA problem:
#'   \itemize{
#'     \item Maximize: \code{sum(obj_rxns fluxes)}
#'     \item Subject to: \code{S * v = 0} (Steady-state constraint)
#'     \item And: \code{lb <= v <= ub} (Flux bounds)
#'   }
#'
#'   If the solver succeeds, it writes the following files to \code{outputdir}:
#'   \itemize{
#'     \item \code{FBA_Full_Flux_Distribution.csv}: All reactions and their calculated fluxes.
#'     \item \code{FBA_Active_Flux_Distribution.csv}: Reactions with flux absolute value > 1e-6.
#'     \item \code{ObjectiveFlux.txt}: The maximum objective value.
#'     \item \code{FBA_Result.json}: The raw result object from \code{lpSolve} in JSON format.
#'   }
#'   If the solver fails, it prints the status code and common troubleshooting tips to the console.
#'
#' @return Invisible \code{NULL}. The function is called for its side effects
#'   (console output and file generation).
#'
#' @examples
#' \dontrun{
#' # Assuming you have a stoichiometric matrix data frame 'S_mat'
#' # where colnames are reaction IDs and rownames are metabolites
#'
#' # Define objective reactions
#' objectives <- c("BIOMASS_Ecoli_core_w_GAM")
#'
#' # Define specific bounds for glucose uptake (e.g., max uptake = 10)
#' my_bounds <- list("EX_glc__D_e" = c(-10, 1000))
#'
#' # Run FBA
#' FBA_solver(
#'   S_df = S_mat,
#'   obj_rxns = objectives,
#'   outputdir = "./fba_results",
#'   default_lb = -1000,
#'   default_ub = 1000,
#'   flux_bounds = my_bounds
#' )
#' }
#'
#' @import lpSolve dplyr jsonlite
#' @export
const FBA_solver = function(S_df, obj_rxns, outputdir = "./", default_lb = -1000, default_ub = 1000, flux_bounds = list()) {
    library(lpSolve);
    library(dplyr);
    library(jsonlite);

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
    let S <- as.matrix(S_df);

    # 获取反应ID和代谢物ID
    let rxn_ids <- colnames(S);
    let met_ids <- rownames(S);

    n_rxns <- length(rxn_ids);
    n_mets <- length(met_ids);

    # 验证目标反应是否在矩阵中
    let missing_rxns <- obj_rxns[!(obj_rxns in rxn_ids)];

    if (length(missing_rxns) > 0) {
      stop(paste("错误：以下目标反应不在矩阵的反应列中:", paste(missing_rxns, collapse = ", ")));
    }

    # 构建目标函数向量 c (默认全为0，目标反应系数为1)
    # 如果有多个目标反应，等同于最大化它们流量之和
    let c_vec <- rep(0, n_rxns);
    c_vec[rxn_ids in obj_rxns] <- 1;

    # =========================================================================
    # 4. 设置约束条件
    # =========================================================================

    # 约束1: 稳态约束 S * v = 0
    # lpSolve 要求将约束写成 Ax <= b 的形式
    # 对于等式约束，方向设为 "="
    let const_mat <- S;
    let const_dir <- rep("=", n_mets);
    let const_rhs <- rep(0, n_mets);

    # 约束2: 反应流量边界约束 lb <= v <= ub
    # 【重要说明】CSV中通常不包含边界信息，这里设置默认边界：
    # 默认所有反应可逆: 下限 -1000, 上限 1000
    # let default_lb <- -1000;
    # let default_ub <- 1000;

    let lower_bounds <- rep(default_lb, n_rxns);
    let upper_bounds <- rep(default_ub, n_rxns);

    # 【进阶修改】如果你的模型中有不可逆反应，或者需要限制特定底物的摄入，
    # 你可以在此处修改 lower_bounds 和 upper_bounds。
    # 例如：限制葡萄糖摄入反应(EX_glc)最大摄入量为10：
    # ex_glc_idx <- which(rxn_ids == "EX_glc__D_e")
    # lower_bounds[ex_glc_idx] <- -10  # 摄入通常用负数表示
    for(let id in names(flux_bounds)) {
      let idx = which(rxn_ids == id);
      let bound_vals = flux_bounds[[id]];

      lower_bounds[idx] <- bound_vals[1];
      upper_bounds[idx] <- bound_vals[2];
    }

    # =========================================================================
    # 5. 求解 FBA 线性规划问题
    # =========================================================================

    cat("开始求解 FBA 模型...\n");

    let fba_result <- lp(
      direction = "max",          # 最大化目标函数
      objective.in = c_vec,       # 目标函数系数
      const.mat = const_mat,      # 约束矩阵 S
      const.dir = const_dir,      # 约束方向 (=)
      const.rhs = const_rhs,      # 约束右侧值 (0)
      lower.bounds = lower_bounds,# 流量下限
      upper.bounds = upper_bounds # 流量上限
    );

    # =========================================================================
    # 6. 结果提取与输出
    # =========================================================================

    if (fba_result$status == 0) {
      cat("FBA 求解成功！\n");
      
      # 获取最优目标函数值 (最大通量)
      let opt_obj_val <- fba_result$objval;
      cat("目标反应最大流量:", opt_obj_val, "\n");
      
      # 获取所有反应的通量分布
      let flux_dist <- fba_result$solution;
      names(flux_dist) <- rxn_ids;
      
      # 将结果整理为数据框
      let result_df <- data.frame(
        ReactionID = rxn_ids,
        Flux = flux_dist,
        LowerBound = lower_bounds,
        UpperBound = upper_bounds,
        stringsAsFactors = FALSE
      );
      
      # 筛选出有实际流量的反应（绝对值 > 1e-6），方便查看
      let active_flux_df <- result_df %>% filter(abs(Flux) > 1e-6);
      
      # 保存完整结果和活跃反应结果
      write.csv(result_df, file.path(outputdir, "FBA_Full_Flux_Distribution.csv"), row.names = FALSE);
      write.csv(active_flux_df, file.path(outputdir, "FBA_Active_Flux_Distribution.csv"), row.names = FALSE);
      writeLines(as.character(opt_obj_val), con = file.path(outputdir, "ObjectiveFlux.txt"));
      writeLines(jsonlite::toJSON(fba_result), con = file.path(outputdir, "FBA_Result.json"));

      cat("结果已保存至 'FBA_Full_Flux_Distribution.csv' 和 'FBA_Active_Flux_Distribution.csv'\n");
      
    } else {
      cat("FBA 求解失败！\n");
      cat("状态码:", fba_result$status, "\n");
      cat("可能原因: \n");
      cat("  1. 模型不可行 - 检查边界约束是否允许底物摄入和产物排出\n");
      cat("  2. 模型无界 - 检查是否遗漏了必要的不可逆反应约束(lower_bound=0)\n");
    }

    invisible(NULL);
}