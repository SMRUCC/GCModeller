// export R# package module type define for javascript/typescript language
//
//    imports "sampleInfo" from "phenotype_kit";
//
// ref=phenotype_kit.DEGSample@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * GCModeller DEG experiment analysis designer toolkit
 * 
 * > This R# package module provides the toolkit for create and manipulate the 
 * >  experiment sample information data(@``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo``), which is the 
 * >  experiment design data of the different expression analysis:
 * >  
 * >  + create the sample information data: ``sampleInfo``, 
 * >    ``guess.sample_groups``, ``sampleinfo.text.groups``, ``read.sampleinfo``;
 * >  + manipulate the sample group data: ``design``, ``sample_groups``, 
 * >    ``shuffle_groups``, ``group.colors``, ``sampleinfo_gsub``, ``sampleId``;
 * >  + build the analysis model for run the different expression analysis: 
 * >    ``make.analysis``, ``make.MLdataset``.
 * >  
 * >  The sample information data object in R# environment can be saved as a csv 
 * >  table file via the ``write.sampleinfo`` api, or be converted to a data frame 
 * >  via the ``as.data.frame`` api.
*/
declare namespace sampleInfo {
   /**
    * Create new analysis design sample info via formula
    * 
    * > the sample information data(the ``ID``, ``sample_name``, ``color``, 
    * >  ``shape``, ``batch`` and ``injectionOrder`` property) of the merged samples 
    * >  will be kept as is, only the ``sample_info`` group label will be replaced 
    * >  with the new group label.
    * >  
    * >  the design formula expression is a lazy expression, so that the sample group 
    * >  label in the formula is not required to be a R# symbol.
    * 
     * @param sampleinfo the sample information data, which can be a vector of the 
     *  @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` object or a pipeline object that produces a set of 
     *  the sample information data.
     * @param designs a tuple list of the experiment design formula: the slot key of the list is 
     *  the label of the new sample group and the slot value is a formula 
     *  expression that describes the merge of the original sample groups, example 
     *  as ``list(A = B + C + D)`` means that the sample groups ``B``, ``C`` and 
     *  ``D`` will be merged into a new sample group ``A``.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a new vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` object: the sample groups that 
     *  are described in the given design formula will be replaced with the new 
     *  generated sample groups, and the other sample groups that are not 
     *  referenced in the design formula will be kept as is;
     *  
     *  this function returns a R# error message object if the input data can not 
     *  be cast to a collection of the sample information data, or the given design 
     *  formula is invalid.
   */
   function design(sampleinfo: any, designs?: object, env?: object): object;
   module group {
      /**
       * get/set the group colors
       * 
       * > the colors of the color set will be assigned to the sample groups in a 
       * >  loop manner, so that the color set is not required to have the same size 
       * >  as the sample group numbers.
       * >  
       * >  this api can be used as a property setter in R# environment: the color of 
       * >  each sample group can be overwritten via the value assign syntax:
       * >  
       * >  ```r
       * >  group.colors(samples) <- "Set1:c8";
       * >  ```
       * 
        * @param sampleinfo a vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` sample information data.
        * @param colorSet a new color set for assign to each sample group, which can be a character 
        *  vector of the html color code or the color palette name, the ``Paper`` 
        *  color set will be used if this color set parameter can not be recognized.
        *  
        *  if this parameter is not specified, then this function works as a getter: 
        *  the current color of each sample group will be returned.
        * 
        * + default value Is ``null``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return this function returns a tuple list of the color of each sample group when 
        *  the ``colorSet`` parameter is not specified(the slot key of the list is the 
        *  sample group label and the slot value is the html color code of the 
        *  corresponding sample group), otherwise the input sample information 
        *  collection that the color of each sample group has been modified will be 
        *  returned.
      */
      function colors(sampleinfo: object, colorSet?: any, env?: object): object;
   }
   module guess {
      /**
       * try to parse the sampleInfo data from the
       *  sample labels
       * 
       * 
        * @param sample_names a character vector of the sample labels, the sample group information will 
        *  be guessed from the common tag prefix of these sample labels, example as 
        *  the sample labels ``iBAQ-AAA-1``, ``iBAQ-AAA-2``, ``iBAQ-BBB-1`` will be 
        *  grouped as the ``AAA`` and ``BBB`` groups.
        * @param maxDepth extends the group label to the max depth? if this parameter is FALSE, then 
        *  only the first different tag token will be used as the group label, 
        *  otherwise the group label will be extended until the last common tag token.
        * 
        * + default value Is ``false``.
        * @param raw_list returns the group result as a raw tuple list object(the slot key of the 
        *  list is the group label and the slot value is a character vector of the 
        *  sample label)? if this parameter is FALSE, then a vector of the 
        *  @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` object will be returned.
        * 
        * + default value Is ``true``.
        * @return a tuple list of the guessed sample groups, or a vector of the 
        *  @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` object when the ``raw_list`` parameter is FALSE.
        *  
        *  the generated @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` object will be assigned with a 
        *  default color from the ``Paper`` color set, and the ``shape`` property is 
        *  set as ``circle``, the ``batch`` property is set as 1 and the 
        *  ``injectionOrder`` property is the index order of the sample in the 
        *  generated sample collection.
      */
      function sample_groups(sample_names: object, maxDepth?: boolean, raw_list?: boolean): object|object;
   }
   module make {
      /**
       * create the different expression analysis design of the control vs treatment
       * 
       * > the samples of the control group will be placed at the first in the 
       * >  generated analysis design object, and the samples of the treatment group 
       * >  will be placed after the control group, so that the order of the sample 
       * >  groups in the generated analysis design object is ``control vs treatment``.
       * >  
       * >  the generated analysis design object can be used by the limma analysis 
       * >  api(``limma``) or the t-test analysis api(``deg.t.test``) for run the 
       * >  different expression analysis.
       * 
        * @param sampleinfo a vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` sample information data.
        * @param control the sample group label of the control group.
        * @param treatment the sample group label of the treatment(the experiment) group.
        * @return a @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.DataAnalysis`` analysis design object that only contains the 
        *  samples of the given control group and treatment group, the other sample 
        *  groups in the input sample information data will be ignored.
      */
      function analysis(sampleinfo: object, control: string, treatment: string): object;
      /**
       * create the machine learning dataset from the gene expression matrix and the 
       *  sample group information
       * 
       * > the sample data that its id is not exists in the expression matrix will be 
       * >  skipped with a warning message.
       * 
        * @param x a gene expression matrix object, the gene feature rows of this matrix will 
        *  be used as the data features of the generated dataset.
        * @param sampleinfo a vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` sample information data: the 
        *  ``ID`` property of the sample data should be matched with the sample columns 
        *  of the given expression matrix, and the ``sample_info`` property of the 
        *  sample data will be used as the class label of the generated dataset 
        *  entities.
        * @return a vector of the @``T:Microsoft.VisualBasic.DataMining.KMeans.EntityClusterModel`` data entity: the ``ID`` 
        *  property is the sample id, the ``Cluster`` property is the sample group 
        *  label and the ``Properties`` property is the expression value of each gene 
        *  feature in the corresponding sample.
      */
      function MLdataset(x: object, sampleinfo: object): any;
   }
   module read {
      /**
       * Read the sampleinfo data table from a given csv file
       * 
       * 
        * @param file the file path of the sample information table file.
        * @param tsv is the target table file a TSV format table file? by default is FALSE means 
        *  that the target table file is a CSV format table file.
        * 
        * + default value Is ``false``.
        * @param exclude_groups a character vector of the sample group label for exclude from the loaded 
        *  sample information data.
        * 
        * + default value Is ``null``.
        * @param id_makenames rename the sample id via the generic make names function? this parameter is 
        *  helpful for make the sample id as a valid R# symbol name.
        * 
        * + default value Is ``false``.
        * @return a vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` object that is loaded from the 
        *  given table file.
        *  
        *  NOTE: the first column of the table file will be used as the ``ID`` 
        *  property of the generated sample information data, and the sample data rows 
        *  that the ``ID`` or the ``sample_info`` data is empty will be removed 
        *  automatically with a warning message.
      */
      function sampleinfo(file: string, tsv?: boolean, exclude_groups?: string, id_makenames?: boolean): object;
   }
   /**
    * group the sample information data by the sample group label
    * 
    * 
     * @param x a vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` sample information data.
     * @return a tuple list of the sample groups: the slot key of the list is the sample 
     *  group label and the slot value is a vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` 
     *  object that belongs to the corresponding sample group, the sample groups in 
     *  the generated list object are sorted by the group label in ascending order.
   */
   function sample_groups(x: object): object;
   /**
    * Get sample id collection from a speicifc sample data groups
    * 
    * 
     * @param sampleinfo the sample information data, which can be a vector of the 
     *  @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` object or a pipeline object that produces a set of 
     *  the sample information data.
     * @param groups a character vector of the sample group label for get the sample id list.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a character vector of the sample id that belongs to the given sample groups, 
     *  the sample id of all of the given sample groups will be merged into a single 
     *  character vector in the order of the given group label vector;
     *  
     *  this function returns a R# error message object if the input data can not be 
     *  cast to a collection of the sample information data.
   */
   function sampleId(sampleinfo: any, groups: string, env?: object): string;
   module sampleinfo {
      module text {
         /**
          * Create sampleInfo table from text files
          * 
          * > only the ``*.txt`` files in the given directory will be scanned, an empty 
          * >  sample collection will be returned if there is no text file in the target 
          * >  directory.
          * 
           * @param dir a directory path that contains a set of the text files: each text file is a 
           *  sample group and the file basename is used as the sample group label, each 
           *  line in the text file is a sample id of the corresponding sample group.
           * @return a vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` sample information data that is 
           *  created from the text files in the given directory: the ``ID`` and the 
           *  ``sample_name`` property of the generated sample data is the sample id, the 
           *  ``sample_info`` property is the file basename and the ``injectionOrder`` 
           *  property is the index order of the sample in the generated sample 
           *  collection.
         */
         function groups(dir: string): object;
      }
   }
   /**
    * create ``sample_info`` data table
    * 
    * 
     * @param ID the sample id in the raw data files
     * @param sample_info the sample group information.
     * @param sample_name the sample name label for display, this character vector could be nothing, 
     *  then the generated sample display name will be replaced with the input sample id
     * 
     * + default value Is ``null``.
     * @param color the color of each sample, this parameter could be nothing, then the color 
     *  of the generated sample data will not be assigned.
     * 
     * + default value Is ``null``.
     * @param batch the experiment batch id of each sample, the default batch id of each sample 
     *  is 1.
     * 
     * + default value Is ``null``.
     * @param inject_order the sample injection order of each sample, the default injection order of 
     *  each sample is its index order in the input sample id vector.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` sample information data, the 
     *  ``shape`` property of the generated sample data is set as ``circle``;
     *  
     *  this function returns NULL if the input sample id vector or the sample group 
     *  information vector is nothing, or a R# error message object if the size of 
     *  the input vectors is not agreed with each other: the size of the 
     *  ``sample_name`` should be equals to the size of the ``ID``, and the size of 
     *  the ``sample_info`` should be 1(a single group label for all samples) or 
     *  equals to the size of the ``ID``.
   */
   function sampleInfo(ID: string, sample_info: string, sample_name?: string, color?: string, batch?: object, inject_order?: object, env?: object): object;
   /**
    * do text replace of the sample group label
    * 
    * > this api is helpful for merge the sample groups in a simple manner: replace 
    * >  the different group label text as a common group label text, then all of 
    * >  these sample data will be merged into the same sample group.
    * 
     * @param sampleinfo the sample information data, which can be a vector of the 
     *  @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` object or a pipeline object that produces a set of 
     *  the sample information data.
     * @param find a character vector of the text pattern for search in the sample group label 
     *  of each sample data.
     * @param replace_as the text for replace all of the found text pattern in the sample group 
     *  label.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a new vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` object that the ``sample_info`` 
     *  group label of each sample data has been replaced;
     *  
     *  this function returns a R# error message object if the input data can not be 
     *  cast to a collection of the sample information data.
   */
   function sampleinfo_gsub(sampleinfo: any, find: any, replace_as: string, env?: object): object;
   /**
    * shuffle the sample group order in a random manner
    * 
    * > unlike the ``sample_groups`` api, which sorts the sample groups by the group 
    * >  label in ascending order, this function shuffles the sample group order in a 
    * >  random manner, which is helpful for the random color assignment or the 
    * >  permutation test of the sample groups.
    * 
     * @param x a vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` sample information data.
     * @return a tuple list of the sample groups in a random order: the slot key of the 
     *  list is the sample group label and the slot value is a vector of the 
     *  @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` object that belongs to the corresponding sample 
     *  group.
   */
   function shuffle_groups(x: object): object;
   module write {
      /**
       * save sampleinfo data as csv file
       * 
       * > You also can save the sampleinfo data directly via the ``write.csv`` function.
       * 
        * @param sampleinfo a vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` sample information data for save 
        *  into the target csv table file.
        * @param file the file path of the generated sample information csv table file.
        * @return a boolean value for indicates that the sample information data has been 
        *  saved into the target file successfully or not.
      */
      function sampleinfo(sampleinfo: object, file: string): boolean;
   }
}
