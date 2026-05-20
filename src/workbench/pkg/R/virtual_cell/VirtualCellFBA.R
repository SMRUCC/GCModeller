#'
#' @param models a set of the virtual cell model CLR object to modelling of the FBA stoichiometric matrix
#' @param obj_rxns A character vector specifying the reaction flux IDs (a subset
#'   of the column names of \code{S_df}) to be maximized. If multiple reactions
#'   are provided, their combined flux is maximized.
#' @param temp temp dir for save the temp workspace files.
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
const VirtualCellFBA = function(models, obj_rxns, default_lb = -1000, default_ub = 1000, flux_bounds = list(), temp = "./.tmp/") {
    dir.create(temp);

    native_r(FBA_solver, list(
        S_df = models, 
        obj_rxns = obj_rxns, 
        outputdir = temp, 
        default_lb = default_lb, 
        default_ub = default_ub, 
        flux_bounds = flux_bounds
    ));
}