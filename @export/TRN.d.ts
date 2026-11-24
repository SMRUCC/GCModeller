// export R# package module type define for javascript/typescript language
//
//    imports "TRN" from "cytoscape";
//
// ref=cytoscape_toolkit.TRN@cytoscape, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Transcription Regulation Network Builder Tools
 * 
*/
declare namespace TRN {
   module fpkm {
      /**
        * @param cutoff default value Is ``0.65``.
      */
      function connections(fpkm: object, cutoff?: number): object;
   }
   /**
    * re-open the persisted bicor correlation store that was written by `write_bicor`
    * 
    * > the returned store supports fast point queries (gene1, gene2 -> correlation
    * >  and p-value), neighborhood queries (gene + threshold -> all correlated genes)
    * >  and full-library streaming edge filters, all without loading the matrix
    * >  into memory.
    * 
     * @param repo the data repository directory that was passed to `write_bicor`
     *  (reads `{repo}/bicor.dat`, or `{repo}/pearson.dat` when the
     *  store was written with `corKind = "pearson"`, and
     *  `{repo}/index.dat`).
     * @param cache the hot-row LRU cache capacity in rows. Each cached row of a N = 50,000
     *  matrix costs about 200 KB of memory. The default value 2048 means at most
     *  about 400 MB of the hot row cache.
     * 
     * + default value Is ``2048``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return an opened [CorrelationMatrixStore](cref:T:Microsoft.VisualBasic.Math.Matrix.CorrelationMatrixStore) object. Pass it to
     *  `build_grn` via the `bicor` parameter to skip the expensive
     *  correlation matrix re-computation.
   */
   function open_bicor(repo: string, cache?: object, env?: object): object;
   /**
    * read the WGCNA module map sidecar cache that was written by `write_bicor`
    * 
    * > pass the returned object to `build_grn` via the `modules` parameter
    * >  to skip the WGCNA blockwise module detection. Note that the cache carries a
    * >  fingerprint (gene id row order + WGCNA configuration); when it does not match
    * >  the current data, `build_grn` will ignore it and rebuild automatically.
    * 
     * @param repo the same data repository directory that was passed to `write_bicor`
     *  (reads `{repo}/modules.dat`).
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the cached module map object, or Nothing when the cache file does not exist
     *  (in that case `build_grn` will fall back to running the WGCNA blockwise
     *  module detection and write the cache automatically).
   */
   function open_modules(repo: string, env?: object): object;
   /**
    * compute the signed bicor correlation matrix once and persist it into a
    *  random-access correlation store for repeated threshold filtering experiments.
    * 
    * > computing the full N×N correlation matrix of a large expression matrix is
    * >  very expensive (hours for 50,000 genes), while all downstream network
    * >  generations are just threshold filters over this matrix. So this function
    * >  performs the expensive computation once and persists two artifacts:
    * >  1. the raw unfiltered signed correlation matrix (open it later via
    * >  `open_bicor`);
    * >  2. the WGCNA module map sidecar cache `{repo}/modules.dat` (consumed by
    * >  `build_grn`, so the WGCNA blockwise module detection is not re-run).
    * >  After this one-time computation, every different correlation threshold
    * >  experiment only costs a fast filtering over the cached matrix.
    * 
     * @param x the gene expression matrix (genes in rows, samples in columns) that the
     *  robust biweight midcorrelation (bicor) is computed from. Should be
     *  batch-normalized and variance-filtered already (see
     *  `batch_normalize` and `top_variance`).
     * @param repo the data repository directory. Two files are written into it:
     *  `{repo}/{corKind}.dat` (the row blocks of the correlation matrix,
     *  named `bicor.dat` or `pearson.dat` after the correlation kind)
     *  and `{repo}/index.dat` (the gene table + row directory). The WGCNA
     *  module map sidecar cache is written as `{repo}/modules.dat`.
     * @param type the on-disk encoding of the correlation values:
     *  [CorrelationEncodings.Float32](cref:F:Microsoft.VisualBasic.Math.Matrix.CorrelationEncodings.Float32) (4 bytes per value, ~1e-7 precision,
     *  default) or [CorrelationEncodings.QuantizedInt16](cref:F:Microsoft.VisualBasic.Math.Matrix.CorrelationEncodings.QuantizedInt16) (2 bytes per value,
     *  ~3e-5 precision, about half of the storage size).
     * 
     * + default value Is ``null``.
     * @param wgcnaOpts optional WGCNA blockwise configuration used by the module detection that
     *  runs in the same pass. Nothing (default) uses the default configuration with
     *  the GPU backend enabled and the network graph materialization disabled.
     * 
     * + default value Is ``null``.
     * @param corKind the correlation estimator used for the matrix computation:
     *  [CorrelationKind.Bicor](cref:F:SMRUCC.genomics.Analysis.HTS.WGCNA.CorrelationKind.Bicor) (robust biweight midcorrelation,
     *  default) or [CorrelationKind.Pearson](cref:F:SMRUCC.genomics.Analysis.HTS.WGCNA.CorrelationKind.Pearson). The kind is not stored
     *  in the index but implied by the data file name.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return TRUE when the correlation store (and the module map cache) is written successfully.
   */
   function write_bicor(x: object, repo: string, type?: object, wgcnaOpts?: object, corKind?: object, env?: object): any;
}
