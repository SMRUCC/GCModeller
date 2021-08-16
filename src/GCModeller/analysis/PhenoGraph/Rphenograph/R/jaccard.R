jaccard_coeff <- function(idx, n_threads = 8) {
    require(foreach);
    require(doParallel);

    nrow <- nrow(idx);
    ncol <- ncol(idx);

    cl = makeCluster(n_threads);

    registerDoParallel(cl);

    print(sprintf("run parallel for %s lines of index data evaluate jaccard coeff...", nrow));
    print(head(idx));
    
    idx     = as.data.frame(idx);
    idxRows = lapply(1:nrow, function(i) {
        as.vector(unlist(idx[i,, drop = TRUE]));
    })

    # for (int i = 0; i < nrow; i++) {
    parallelDataSet = foreach(i = 1:length(idxRows)) %dopar% {
        row   = idxRows[[i]];
        nodei = row;
        list = lapply(row, function(k) {
            nodej = idxRows[[k]];
            u = length(intersect(nodei, nodej));
            o = c(i, k, u/(2.0*ncol - u)/2);

            o;
        });
        weights = NULL;

        for(r in list) {
            weights = rbind(weights, r);
        }

        as.vector(t(weights[weights[, 3] > 0, ]));
    }

    stopCluster(cl);

    x = unlist(parallelDataSet);
    weights = t(matrix(x, nrow = 3));
    weights;
}