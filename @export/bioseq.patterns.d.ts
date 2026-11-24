// export R# package module type define for javascript/typescript language
//
//    imports "bioseq.patterns" from "seqtoolkit";
//
// ref=seqtoolkit.patterns@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Tools for sequence patterns
 * 
*/
declare namespace bioseq.patterns {
   module as {
      /**
       * make the sequence graph embedding data of the given sequence collection
       * 
       * 
        * @param fasta a fasta sequence collection for make the sequence graph embedding, 
        *  which can be a [FastaFile](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile) object, a collection of the 
        *  [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) object, or a character vector of the raw 
        *  sequence data.
        * @param mol_type the molecule type of the input sequence data for select the graph 
        *  embedding algorithm.
        * 
        * + default value Is ``null``.
        * @param parallel run the graph embedding task in parallel on multiple cpu cores?
        * 
        * + default value Is ``false``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return the sequence graph embedding vector data is generates from different method 
        *  based on the **mol_type** data:
        *  
        *  + [SeqTypes.DNA](cref:F:SMRUCC.genomics.SequenceModel.SeqTypes.DNA): [Builder.DNAGraph()](cref:M:SMRUCC.genomics.Model.MotifGraph.Builder.DNAGraph(SMRUCC.genomics.SequenceModel.FASTA.FastaSeq))
        *  + [SeqTypes.Protein](cref:F:SMRUCC.genomics.SequenceModel.SeqTypes.Protein): [Builder.PolypeptideGraph()](cref:M:SMRUCC.genomics.Model.MotifGraph.Builder.PolypeptideGraph(SMRUCC.genomics.SequenceModel.FASTA.FastaSeq))
        *  + [SeqTypes.RNA](cref:F:SMRUCC.genomics.SequenceModel.SeqTypes.RNA): [Builder.RNAGraph()](cref:M:SMRUCC.genomics.Model.MotifGraph.Builder.RNAGraph(SMRUCC.genomics.SequenceModel.FASTA.FastaSeq))
      */
      function seq_graph(fasta: any, mol_type?: object, parallel?: boolean, env?: object): object;
   }
   module create {
      /**
       * create the sequence seeds data from the given fasta sequence 
       *  collection, and then save the generated seed data into the target seed 
       *  data file
       * 
       * 
        * @param fasta a fasta sequence collection for make the sequence seeds, which can be 
        *  a [FastaFile](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile) object, a collection of the 
        *  [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) object, or a character vector of the raw 
        *  sequence data.
        * @param saveto a [ScanFile](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.ScanFile) sequence seed data file object for save the 
        *  generated seed data, which is created by the ``open.seedFile`` api.
        * @param minw 
        * + default value Is ``8``.
        * @param maxw 
        * + default value Is ``20``.
        * @param seedingCutoff the similarity cutoff threshold value for build the seed clusters.
        * 
        * + default value Is ``0.95``.
        * @param scanMinW the minimum width of the seed scan region.
        * 
        * + default value Is ``6``.
        * @param scanCutoff the similarity score cutoff threshold value of the seed scan.
        * 
        * + default value Is ``0.8``.
        * @param significant_sites the minimum number of the significant sites for keep a seed.
        * 
        * + default value Is ``4``.
        * @param debug print the debug log message of the seed scan progress?
        * 
        * + default value Is ``false``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return the input [ScanFile](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.ScanFile) sequence seed data file object with 
        *  the generated seed data has been written into it.
      */
      function seeds(fasta: any, saveto: object, minw?: object, maxw?: object, seedingCutoff?: number, scanMinW?: object, scanCutoff?: number, significant_sites?: object, debug?: boolean, env?: object): any;
   }
   /**
    * find possible motifs of the given sequence collection
    * 
    * 
     * @param fasta a fasta sequence collection for make the motif discovery, which should 
     *  contains multiple sequence.
     * @param minw 
     * + default value Is ``8``.
     * @param maxw 
     * + default value Is ``20``.
     * @param nmotifs A number for limit the number of motif outputs:
     *  
     *  + negative integer/zero: no limits[default]
     *  + positive value: top motifs with score desc
     * 
     * + default value Is ``-1``.
     * @param noccurs 
     * + default value Is ``12``.
     * @param seedingCutoff the similarity cutoff threshold value for build the seed clusters.
     * 
     * + default value Is ``0.65``.
     * @param scanMinW the minimum width of the seed scan region.
     * 
     * + default value Is ``6``.
     * @param scanCutoff the similarity score cutoff threshold value of the seed scan.
     * 
     * + default value Is ``0.8``.
     * @param cleanMotif the motif cleaning threshold value for remove the low score motif 
     *  sites from the generated motif model.
     * 
     * + default value Is ``0.5``.
     * @param significant_sites the minimum number of the significant sites for keep a seed.
     * 
     * + default value Is ``4``.
     * @param seeds the pre-computed seed data for make the motifs, which can be a 
     *  [ScanFile](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.ScanFile) object that is created by the ``create.seeds`` 
     *  api, or a collection of the [HSP](cref:T:SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment.HSP) seed object. If this 
     *  parameter is not specified, then the seeds will be discovered from the 
     *  input sequence data automatically.
     * 
     * + default value Is ``null``.
     * @param debug print the debug log message of the motif discovery progress?
     * 
     * + default value Is ``false``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [SequenceMotif](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceMotif) motif object that 
     *  discovered from the given sequence collection, which is sorted by the 
     *  average motif site score in descending order.
   */
   function find_motifs(fasta: any, minw?: object, maxw?: object, nmotifs?: object, noccurs?: object, seedingCutoff?: number, scanMinW?: object, scanCutoff?: number, cleanMotif?: number, significant_sites?: object, seeds?: any, debug?: boolean, env?: object): object;
   /**
    * make a motif scan from the given sequence collection
    * 
    * 
     * @param seqs a fasta sequence collection for run the gibbs sampler motif discovery, 
     *  which can be a [FastaFile](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile) object, a collection of the 
     *  [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) object, or a character vector of the raw 
     *  sequence data.
     * @param width the motif width of the gibbs sampler. If this parameter is not 
     *  specified, then the motif width will be evaluated from the input 
     *  sequence data automatically as the 60% of the average sequence length.
     * 
     * + default value Is ``null``.
     * @param maxitr the maximum iteration number of the gibbs sampler.
     * 
     * + default value Is ``1000``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a [MSAMotif](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.MSAMotif) motif object that discovered from the given 
     *  sequence collection by the gibbs sampler;
     *  
     *  this function returns NULL if the input sequence data can not be cast 
     *  to a fasta sequence collection.
   */
   function gibbs_scan(seqs: any, width?: object, maxitr?: object, env?: object): object;
   module motif {
      /**
       * find the target loci match sites of the given sequence data based on 
       *  the motif PWM model
       * 
       * 
        * @param motif the motif PWM model for make the site scan, which could be a 
        *  [SequenceMotif](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceMotif) or a [MSAMotif](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.MSAMotif) object.
        * @param target a fasta sequence collection for make the motif site scan, which can be 
        *  a single [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) object, a [FastaFile](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile) 
        *  object, or a character vector of the raw sequence data.
        * @param cutoff 
        * + default value Is ``0.6``.
        * @param minW 
        * + default value Is ``8``.
        * @param identities the minimum identity threshold value of the accepted match sites.
        * 
        * + default value Is ``0.85``.
        * @param pvalue the maximum p-value threshold of the accepted match sites.
        * 
        * + default value Is ``0.05``.
        * @param parallel run the site scan task in parallel on multiple cpu cores?
        * 
        * + default value Is ``false``.
        * @param motif_name the motif name that will be recorded in the ``seeds`` property of the 
        *  match result. If this parameter is not specified, then the motif name 
        *  of the given PWM model object will be used.
        * 
        * + default value Is ``null``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a vector of the [MotifMatch](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifMatch) motif match result;
        *  
        *  this function returns a R# error message object if the given motif 
        *  model object or the target sequence data is not a supported data 
        *  model.
      */
      function find_sites(motif: any, target: any, cutoff?: number, minW?: number, identities?: number, pvalue?: number, parallel?: boolean, motif_name?: string, env?: object): object;
   }
   /**
    * convert the given sequence motif object as a regexp liked format 
    *  pattern string for do motif matches
    * 
    * 
     * @param motif a [SequenceMotif](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceMotif) motif object for convert as the pattern 
     *  string text.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the regexp liked format string for do motif matches
   */
   function motifString(motif: any, env?: object): string;
   module open {
      /**
       * open the sequence seed scan data file
       * 
       * 
        * @param file the file path of the seed scan data file, or a file stream object of 
        *  the target seed scan data file.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a [ScanFile](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.ScanFile) sequence seed data file object for read or 
        *  save the seed data in a stream manner, which can be used by the 
        *  ``pull.all_seeds`` api or the ``create.seeds`` api;
        *  
        *  this function returns a R# error message object if the given file can 
        *  not be opened for read/write.
      */
      function seedFile(file: any, env?: object): any;
   }
   module palindrome {
      /**
       * Search mirror palindrome sites for a given seed sequence
       * 
       * 
        * @param sequence the raw nucleotide sequence text for search the mirror palindrome 
        *  sites.
        * @param seed the seed sequence fragment text for search its mirror palindrome sites.
        * @return a vector of the [PalindromeLoci](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci) mirror palindrome site 
        *  loci object that found in the given sequence data.
      */
      function mirror(sequence: string, seed: string): object;
   }
   module plot {
      /**
       * Drawing the sequence logo just simply modelling this motif site 
       *  from the clustal multiple sequence alignment.
       * 
       * 
        * @param MSA the multiple sequence alignment data for drawing the sequence logo, 
        *  which can be a [MSAOutput](cref:T:SMRUCC.genomics.Analysis.SequenceAlignment.MSA.MSAOutput) alignment result object, a 
        *  [SequenceMotif](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceMotif) motif object, a [MSAMotif](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.MSAMotif) 
        *  gibbs sampler result, a [Probability](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Probability) PWM model object, or 
        *  a fasta sequence collection.
        * @param title the title text of the generated sequence logo graphics.
        * 
        * + default value Is ``''``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a [GraphicsData](cref:T:Microsoft.VisualBasic.Imaging.Driver.GraphicsData) graphics data object of the sequence 
        *  logo, which can be saved as a image file via the ``bitmap`` or 
        *  ``svg`` api.
      */
      function seqLogo(MSA: any, title?: string, env?: object): object;
   }
   module pull {
      /**
       * pull all of the sequence seed data from the given seed scan data file
       * 
       * 
        * @param seed a [ScanFile](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.ScanFile) sequence seed data file object that is opened 
        *  by the ``open.seedFile`` api.
        * @return a vector of the [HSP](cref:T:SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment.HSP) sequence seed object that loaded from 
        *  the given seed data file.
      */
      function all_seeds(seed: object): object;
   }
   module read {
      /**
       * read the xml motif data model output from the meme program
       * 
       * 
        * @param file the file path of the MEME suite xml format motif discovery output 
        *  document(``meme.xml``).
        * @return a [MEMEXml](cref:T:SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.XmlOutput.MEME.MEMEXml) meme document object model, which can be used 
        *  for extract the motif PWM model via the ``toPWM`` api.
      */
      function meme_xml(file: string): object;
      /**
       * read sequence motif json file.
       * 
       * > apply for search by [patterns.matchSites()](cref:M:seqtoolkit.patterns.matchSites(System.Object,System.Object,System.Double,System.Double,System.Double,System.Double,System.Boolean,System.String,SMRUCC.Rsharp.Runtime.Environment))
       * 
        * @param file the file path of the sequence motif json data document.
        * @return a vector of the [SequenceMotif](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceMotif) motif object that loaded 
        *  from the given json data file.
      */
      function motifs(file: string): object;
      /**
       * read the motif match scan result table file
       * 
       * 
        * @param file a file path to a csv format motif match scan result table file, which 
        *  is generated by the ``motif.find_sites`` api via the ``write.csv`` api.
        * @param tqdm read the table data in a lazy stream mode? if this parameter is TRUE, 
        *  then a lazy pipeline collection will be returned, which is helpful for 
        *  read a huge csv table file without loading all of the data rows into 
        *  the memory at once.
        * 
        * + default value Is ``false``.
        * @return a vector of the [MotifMatch](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifMatch) motif match scan result, or a 
        *  lazy pipeline collection of the [MotifMatch](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifMatch) object when 
        *  the ``tqdm`` parameter is TRUE.
      */
      function scans(file: string, tqdm?: boolean): object;
   }
   module scaffold {
      /**
       * analyses orthogonality of two DNA-Origami scaffold strands.
       *  Multiple criteria For orthogonality Of the two sequences can be specified
       *  to determine the level of orthogonality.
       * 
       * 
        * @param scaffolds a collection of the DNA-Origami scaffold nucleotide sequences for 
        *  evaluate the pairwise orthogonality, which can be a 
        *  [FastaFile](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile) object, a collection of the 
        *  [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) object, or a character vector of the raw 
        *  sequence data.
        * @param segment_len segment length
        * 
        * + default value Is ``7``.
        * @param is_linear scaffolds are not circular
        * 
        * + default value Is ``false``.
        * @param rev_compl also count reverse complementary sequences
        * 
        * + default value Is ``false``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a vector of the [Output](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.DNAOrigami.Output) orthogonality 
        *  evaluation result: one result element for each of the scaffold 
        *  sequence pair in the input sequence collection.
      */
      function orthogonality(scaffolds: any, segment_len?: object, is_linear?: boolean, rev_compl?: boolean, env?: object): object;
   }
   /**
    * create all of the possible seed sequence fragments of the given 
    *  alphabet base letters
    * 
    * 
     * @param size the seed length in chars, i.e. the ``k`` value of the k-mer seeds.
     * @param base a character value of the sequence alphabet letters, example as 
     *  ``ACGT`` for the nucleotide sequence.
     * @return a character vector of all of the possible combination of the seed 
     *  sequence fragments, and the size of the generated seed collection is 
     *  ``len(base) ^ size``.
   */
   function seeds(size: object, base: string): string;
   /**
    * split the motif matches result in parts by its gene source
    * 
    * 
     * @param matches the motif match result data for split, which can be a vector of the 
     *  [MotifMatch](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifMatch) object or a file path of the csv format motif 
     *  match result table.
     * @param gff a [GFFTable](cref:T:SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.GFFTable) genomics feature annotation table for map the 
     *  match result to its source gene feature. If this parameter is not 
     *  specified, then the match result will be grouped by the first token of 
     *  the match title.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a named list of the [MotifMatch](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifMatch) match result group: the 
     *  name of each list element is the gene source id, and the element value 
     *  is a vector of the [MotifMatch](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifMatch) object that belongs to the 
     *  corresponding gene source.
   */
   function split_match_source(matches: any, gff?: object, env?: object): any;
   /**
    * takes the top motif match sites by a set of the given filter threshold 
    *  values
    * 
    * 
     * @param sites a vector of the [MotifMatch](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifMatch) motif match scan result for 
     *  make the filter.
     * @param identities the minimum identity threshold value of the accepted motif matches.
     * 
     * + default value Is ``null``.
     * @param pvalue the maximum p-value threshold of the accepted motif matches.
     * 
     * + default value Is ``null``.
     * @param minW the minimum motif width of the accepted motif matches.
     * 
     * + default value Is ``null``.
     * @return a vector of the [MotifMatch](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifMatch) object that contains the 
     *  accepted motif match sites: the input motif matches will be passed 
     *  through each of the specified filter thresholds in turn, and only the 
     *  matches that satisfies all of the specified threshold conditions will 
     *  be kept in the returned result.
   */
   function top_sites(sites: object, identities?: object, pvalue?: object, minW?: object): object;
   /**
    * convert the meme document to motif PWM model object
    * 
    * 
     * @param meme a [MEMEXml](cref:T:SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.XmlOutput.MEME.MEMEXml) meme document object that is read by the 
     *  ``read.meme_xml`` api.
     * @return a vector of the PWM clr object.
   */
   function toPWM(meme: object): object;
   module view {
      /**
       * display the motif match sites on the given target sequence
       * 
       * 
        * @param sites a collection of the motif match site data([Site](cref:T:SMRUCC.genomics.GCModeller.Workbench.SeqFeature.Site)) for 
        *  display on the target sequence.
        * @param seq the target sequence data for display the motif match sites, which can 
        *  be a raw sequence text or a fasta sequence object.
        * @param deli the delimiter string for display the sequence fragment of each match 
        *  site.
        * 
        * + default value Is ``', '``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a character value of the formatted motif match site display text.
      */
      function sites(sites: any, seq: any, deli?: string, env?: object): string;
   }
}
