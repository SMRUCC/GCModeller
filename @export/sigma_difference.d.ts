// export R# package module type define for javascript/typescript language
//
//    imports "sigma_difference" from "comparative_toolkit";
//
// ref=comparative_toolkit.SigmaDifference@comparative_toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Comparative genomics API module of the Karlin, Campbell & Mrazek (1998)
 *  ``Comparative DNA Analysis Across Diverse Genomes`` algorithm suite.
 * 
 *  This R# package exports:
 * 
 *  1. the pairwise / batch sliding window ``delta*`` difference calculations;
 *  2. the partition-based genome homogeneity measurements (``dnaA``-``gyrB`` ruler);
 *  3. the codon usage / CAI compilation;
 *  4. the full set of the 1998 paper's analysis modules:
 *     the ``rho*`` genome signature, the ``tau*`` tetranucleotide relative
 *     abundance, the r-scan word distribution statistics, the site-specific
 *     codon signature, the ``B(F|C)`` codon bias difference, the 2D threshold
 *     alien gene detection, the sliding window ``delta*`` island profiling and
 *     the ``(C-G)/(C+G)`` strand composition asymmetry.
 * 
*/
declare namespace sigma_difference {
   module alien {
      module genes {
         /**
          * Build the codon usage profile of the ribosomal protein gene set (the
          *  highly expressed reference set RP of the 2D threshold method).
          * 
          * 
           * @param genes all CDS gene sequences of the genome under study
           * @param pattern the ribosomal protein product name matching pattern in the fasta header
           * 
           * + default value Is ``'ribosomal protein'``.
           * @return the ribosomal protein codon usage profile, or Nothing when no
           *  ribosomal protein gene was matched
         */
         function rp_reference(genes: object, pattern?: string): object;
         /**
          * Identify the alien (laterally transferred) genes with the 2D threshold
          *  method (the paper's ``B. subtilis`` thresholds by default):
          * 
          *  B(g|all) > 0.42 and B(g|RP) > 0.45: alien gene - unlike both the average
          *  host gene and the host highly expressed genes;
          * 
          *  B(g|all) > 0.42 but B(g|RP) < 0.45: highly expressed host gene.
          * 
          * 
           * @param genes all CDS gene sequences of the genome under study
           * @param rpPattern the ribosomal protein product name matching pattern;
           *  pass an empty string to skip the B(g|RP) axis
           * 
           * + default value Is ``'ribosomal protein'``.
           * @param biasAll the B(g|all) threshold, default = paper value 0.42
           * 
           * + default value Is ``0.42``.
           * @param biasRP the B(g|RP) threshold, default = paper value 0.45
           * 
           * + default value Is ``0.45``.
           * @return the classification prediction of every long gene
         */
         function scan(genes: object, rpPattern?: string, biasAll?: number, biasRP?: number): object;
      }
   }
   /**
    * Compile the codon usage table of every species gene collection.
    *  The reference set of each species is its own gene collection
    *  (for a real high-expression reference set use
    *  [ToolsAPI.BuildCAIReference()](cref:M:SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.ToolsAPI.BuildCAIReference(SMRUCC.genomics.SequenceModel.FASTA.FastaFile,SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.GeneticCodes,System.String)) with the ribosomal protein genes).
    * 
    * > Output layout:
    * > 
    * >  SpeciesID, CAI, CUBIAS_LIST
    * >  src1 ...
    * >  src2 ...
    * 
     * @param genes the directory of the ``*.fasta`` / ``*.fsa`` species gene collections
     * @return the compiled codon usage csv document
   */
   function cai_bias_dataset(genes: string): object;
   /**
    * Compile the CAI w weight table of one reference gene collection and export
    *  the codon usage csv table. (legacy batch compilation entry)
    * 
    * 
     * @param genes the reference gene collection of one species
   */
   function cai_bias_table(genes: object): object;
   module codon {
      module bias_B {
         /**
          * The ``B(F|C)`` codon usage bias difference between two gene classes (the
          *  paper's formula [1]):
          * 
          * ```
          *  B(F|C) = SUM_a p_a(F) SUM_{(x,y,z)->a} | f(x,y,z) - c(x,y,z) |
          *  ```
          * 
          *  the per-codon absolute difference weighted by the amino acid frequency of
          *  the query class F. Note that this measurement is asymmetric: F is the
          *  query object and C is the reference ruler.
          * 
          * 
           * @param f the query codon usage profile (gene family F)
           * @param c the reference codon usage profile (class C)
           * @return the B(F|C) value
         */
         function FC(f: object, c: object): number;
         /**
          * The ``B(g|C)`` codon usage bias difference of one single gene g against the
          *  reference class C.
          * 
          * 
           * @param gene the query CDS gene sequence
           * @param reference the reference codon usage profile (class C)
           * @return the B(g|C) value
         */
         function gC(gene: object, reference: object): number;
      }
      /**
       * Build the site-specific codon signature profile of a gene collection:
       *  the odds ratios ``rhoXY(1,2) / rhoYZ(2,3) / rhoXZ(1,3)`` within the codons
       *  and ``rhoZW(3,4)`` at the codon junctions, distinguished from the global
       *  genome signature.
       * 
       * 
        * @param genes the CDS gene sequences of the gene collection
        * @param name the profile name
        * 
        * + default value Is ``null``.
        * @return the codon signature profile
      */
      function signature_profile(genes: object, name?: string): object;
      /**
       * Build the codon usage frequency profile c(x, y, z) of a gene collection
       *  (the reference class C of the ``B(F|C)`` codon bias measurement).
       * 
       * 
        * @param genes the CDS gene sequences of the gene collection
        * @param name the profile name
        * 
        * + default value Is ``null``.
        * @param excludeStopCodons exclude the stop codons from the frequency total
        * 
        * + default value Is ``true``.
        * @return the codon usage profile
      */
      function usage_profile(genes: object, name?: string, excludeStopCodons?: boolean): object;
   }
   module compile {
      /**
       * Compile a set of pairwise ``delta*`` profile csv files (all created from the
       *  same query genome against different subjects) into one merged matrix csv
       *  document. The output file names cannot be changed by this requirement.
       * 
       * 
        * @param source the source directory that contains the profile csv files
        * @param saveCsv the merged matrix csv output file path
        * @return true when the merged document has been saved
      */
      function delta_query(source: string, saveCsv: string): boolean;
   }
   /**
    * The ``delta*`` measure of difference between two sequences f and g (from
    *  different organisms or from different regions of the same genome): the
    *  average absolute dinucleotide relative abundance difference
    * 
    * ```
    *  delta*(f, g) = (1/16) SUM |rho*XY(f) - rho*XY(g)|
    *  ```
    * 
    *  where the sum extends over all 16 dinucleotides.
    * 
    * 
     * @param f the query sequence
     * @param g the subject sequence
     * @return the delta* value with the similarity level description attached as its unit
   */
   function delta_star(f: object, g: object): object;
   /**
    * Using the DNA segment between the ``dnaA`` and ``gyrB`` genes as the reference
    *  ruler for the genome homogeneity measurement.
    * 
    * 
     * @param nt a fasta sequence object or an NCBI genbank database object
     * @param context the optional PTT gene table of the fasta sequence input
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return the ``dnaA``-``gyrB`` ruler segment sequence
   */
   function dnaA_gyrB(nt: any, context?: object, env?: object): object;
   module genome {
      /**
       * The sliding window ``delta*`` profile of the genome against its own global
       *  average genome signature: the alien DNA that has not yet been ameliorated
       *  shows a signature deviating from the host average and therefore appears as
       *  peaks on the curve (e.g. the pathogenicity islands).
       * 
       * 
        * @param genome the genome nucleotide sequence
        * @param windowSize the sliding window size in bp, default = the paper's 50kb contig size
        * 
        * + default value Is ``50000``.
        * @param stepSize the sampling step in bp
        * 
        * + default value Is ``5000``.
        * @return the profile rows ordered by the site position
      */
      function delta_star_islands(genome: object, windowSize?: object, stepSize?: object): object;
      /**
       * Sliding window ``delta*`` profile: the delta-difference between each local
       *  window of the genome and the comparison sequence.
       * 
       *  PERFORMANCE: implemented with the incremental window count matrix
       *  (O(1) update per slide + O(16) distance per sample), and the comparison
       *  sequence signature is built only once.
       * 
       * 
        * @param genome the query genome sequence
        * @param compare the comparison genome sequence
        * @param windowsSize the sliding window size in bp, default 1kb
        * 
        * + default value Is ``1000``.
        * @return profile rows ordered by the site position, an array of [WindowDelta](cref:T:SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.WindowDelta)
      */
      function delta_star_profile(genome: object, compare: object, windowsSize?: object): object;
      /**
       * The ``(C - G) / (C + G)`` sliding window profile of the genome strand
       *  composition asymmetry: the leading strand favors purines (G > C), so the
       *  curve flips its sign at the replication origin oriC.
       * 
       * 
        * @param genome the genome nucleotide sequence
        * @param windowSize the sliding window size in bp, default 10kb
        * 
        * + default value Is ``10000``.
        * @param stepSize the sampling step in bp, default 1kb
        * 
        * + default value Is ``1000``.
        * @return the skew profile rows ordered by the site position
      */
      function gc_skew(genome: object, windowSize?: object, stepSize?: object): object;
      /**
       * The ``rho*`` genome signature profile: the double-strand symmetrized
       *  dinucleotide relative abundance ``rho*XY = fXY / (fX fY)`` of all 16
       *  dinucleotides, each reported with its six-level significance symbol
       *  (``---`` / ``--`` / ``-`` / ``+`` / ``++`` / ``+++``, thresholds 0.50 / 0.70 /
       *  0.78 / 1.23 / 1.30 / 1.50).
       * 
       * 
        * @param nt the genome nucleotide sequence
        * @return the ``{XY, rho* [symbol]}`` profile table
      */
      function signature_profile(nt: object): object;
   }
   /**
    * Measure the homogeneity property of the genome partition segments against
    *  the ``dnaA``-``gyrB`` ruler segment of the reference genomes in batch.
    * 
    * 
     * @param PartitionData the partition segments of all of the genomes
     * @param RuleSource The original GenBank download data directory which should contains the ``*.ptt``
     *  file for parsing the ruler segment between the ``dnaA`` and ``gyrB`` genes and
     *  the ``*.fna`` file for parsing the genome nucleotide fasta sequence.
     * @return the data frame with one ruler distance column per reference genome
   */
   function measure_homogeneity(PartitionData: object, RuleSource: string): object;
   module Partition {
      module Similarity {
         /**
          * Calculate the pairwise ``delta*`` homogeneity matrix between all of the
          *  genome partition segments (the values are reported as ``delta* x 1000``).
          * 
          * 
           * @param data the partition segments of all of the genomes
           * @return the data frame with one ``Delta(i, j)`` column per pair
         */
         function Calculates(data: object): object;
      }
   }
   module partition_data {
      /**
       * Create the chromosome partitioning data from the two-way BLAST best hit
       *  data: for every partition tag, locate the corresponding ORFs on every
       *  subject genome and cut the partition nucleotide segments.
       * 
       * 
        * @param besthit the two-way BLAST best hit data of the genome pairs
        * @param partitions the partition definitions of the query genome
        * @param allCDSInfo the CDS gene table of all of the genomes
        * @param faDIR the directory of the subject genome fasta files
        * @return the partition nucleotide segments of every subject genome
      */
      function create(besthit: object, partitions: object, allCDSInfo: object, faDIR: string): object;
   }
   module Partitions {
      /**
       * Create the partition data from a raw data frame: the start / stop columns
       *  define the segment loci on the given genome sequence.
       * 
       * 
        * @param PartitionRaw the raw partition data frame
        * @param TagCol the column name of the partition tag
        * @param StartTag the column name of the segment start loci
        * @param StopTag the column name of the segment stop loci
        * @param Nt the genome sequence
        * @return the created partition data collection
      */
      function Creates(PartitionRaw: object, TagCol: string, StartTag: string, StopTag: string, Nt: object): object;
   }
   module read {
      module csv {
         /**
          * Load the partitioning data collection from its csv document.
          * 
          * 
           * @param path the csv file path
           * @return the partitioning data collection
         */
         function genome_partition_data(path: string): object;
      }
      /**
       * Load one sliding window ``delta*`` profile from its csv document.
       * 
       * 
        * @param path the csv file path of one [WindowDelta](cref:T:SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.WindowDelta) profile
        * @return the profile rows ordered by the site position
      */
      function site_delta(path: string): object;
   }
   module Read {
      module Csv {
         /**
          * Load the chromosome partitioning entries from their csv document.
          * 
          * 
           * @param path the csv file path
           * @return the chromosome partitioning entries
         */
         function Chromsome_Partitioning(path: string): object;
      }
   }
   module rendering_merge {
      /**
       * Merge the exported partition-level ``delta*`` profile csv files with the
       *  two-way BLAST best hit rendering data.
       * 
       * 
        * @param source the source directory that contains the profile csv files
        * @param query the gene objects of the query genome
        * @param render_source the xml file of the [SpeciesBesthit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models.SpeciesBesthit) rendering data
        * @param saveto the merged csv output file path
        * @param samples the row sampling step (1 = keep every row)
        * 
        * + default value Is ``1``.
        * @return true when the merged csv document has been saved
      */
      function delta_source(source: string, query: object, render_source: string, saveto: string, samples?: object): boolean;
   }
   module replication_origin {
      /**
       * Locate the candidate replication origin (oriC) and terminus positions from
       *  the GC skew profile: the sign-flip sites of the cumulative skew curve.
       *  A single bidirectional replication origin produces one sign flip at oriC;
       *  genomes with many origins (e.g. some archaea) show no significant
       *  asymmetry at all.
       * 
       * 
        * @param skew the GC skew profile of [SigmaDifference.GenomeGcSkew()](cref:M:comparative_toolkit.SigmaDifference.GenomeGcSkew(SMRUCC.genomics.SequenceModel.FASTA.FastaSeq,System.Int32,System.Int32))
        * @return the window site positions where the skew changes sign
      */
      function predict(skew: object): object;
   }
   module seq {
      /**
       * Create a delta* distance matrix for a given sequence collection.
       * 
       * 
        * @param seqs the sequence collection
        * @return the distance matrix
      */
      function dist(seqs: any): object;
   }
   module sigma_diff {
      /**
       * Compare one query genome against a directory of subject genomes: calculate
       *  the sliding window ``delta*`` profile of the query against every subject and
       *  export each profile as a csv file, then compile all of the profiles into one
       *  merged matrix csv document.
       * 
       * 
        * @param query the query genome fasta file path
        * @param sbjDIR the directory of the subject genome fasta files
        * @param EXPORT the csv output directory
        * @param windowsSize the sliding window size in bp, default 1kb
        * 
        * + default value Is ``1000``.
        * @return true when the compiled matrix has been saved
      */
      function query(query: string, sbjDIR: string, EXPORT: string, windowsSize?: object): boolean;
   }
   module tau_star {
      /**
       * The ``tau*`` tetranucleotide relative abundance profile of all 256
       *  tetranucleotides.
       * 
       * 
        * @param nt the genome nucleotide sequence
        * @return the ``{word, tau*}`` dictionary (only the present words are included)
      */
      function profile(nt: object): object;
      /**
       * List the rare and the frequent tetranucleotides of the ``tau*`` profile
       *  against the given thresholds (the restriction avoidance candidates).
       * 
       * 
        * @param nt the genome nucleotide sequence
        * @param rareBelow words with ``tau*`` below this value are rare
        * 
        * + default value Is ``0.78``.
        * @param frequentAbove words with ``tau*`` above this value are frequent
        * 
        * + default value Is ``1.23``.
        * @return a dictionary with the ``rare`` and ``frequent`` word lists
      */
      function rare_frequent(nt: object, rareBelow?: number, frequentAbove?: number): object;
   }
   module word {
      /**
       * Count the absolute occurrences of one oligonucleotide word in the genome
       *  sequence (e.g. the CTAG rarity report).
       * 
       * 
        * @param nt the genome nucleotide sequence
        * @param word the oligonucleotide word to be counted
        * @return the number of occurrences
      */
      function count(nt: object, word: string): object;
      /**
       * Locate all occurrences of one oligonucleotide word in the genome sequence
       *  (exact match, 0-based start positions).
       * 
       * 
        * @param genome the genome nucleotide sequence
        * @param word the oligonucleotide word to be located
        * @return the 0-based start positions of the word occurrences
      */
      function locate(genome: object, word: string): object;
      /**
       * Run the r-scan significance test on the spatial distribution of one
       *  oligonucleotide word (Dembo-Karlin 1992): detect the clustering, the
       *  overdispersion or the even spacing patterns against the random (Poisson)
       *  spacing expectation.
       * 
       * 
        * @param genome the genome nucleotide sequence
        * @param word the oligonucleotide word under study
        * @param RMax the maximum r of the left-tail r-scan table; increase this
        *  value when the word copy number is large (e.g. the USS / HIP1 elements)
        * 
        * + default value Is ``5``.
        * @return the r-scan result with the pattern interpretation
      */
      function rscan(genome: object, word: string, RMax?: object): object;
   }
   module write {
      module csv {
         /**
          * Save the partitioning data collection as a csv document.
          * 
          * 
           * @param dat the partitioning data collection
           * @param saveto the csv output file path
           * @return true when the csv document has been saved
         */
         function genome_partition_data(dat: object, saveto: string): boolean;
      }
   }
}
