// export R# package module type define for javascript/typescript language
//
//    imports "visualPlot" from "visualkit";
//
// ref=visualkit.visualPlot@visualkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * package module for biological analysis data visualization
 * 
*/
declare namespace visualPlot {
   module category_profiles {
      /**
       * Do plot of the given catalog profiles data
       * 
       * 
        * @param profiles -
        * @param title -
        * 
        * + default value Is ``'KEGG Orthology Profiling'``.
        * @param axis_title -
        * 
        * + default value Is ``'Number Of Proteins'``.
        * @param size the size of the image
        * 
        * + default value Is ``'2300,2000'``.
        * @param tick axis ticks, default value -1 for auto generated.
        * 
        * + default value Is ``-1``.
        * @param colors the color schema name
        * 
        * + default value Is ``'#E41A1C,#377EB8,#4DAF4A,#984EA3,#FF7F00,#CECE00'``.
        * @param dpi 
        * + default value Is ``300``.
        * @param format 
        * + default value Is ``'F2'``.
        * @param env 
        * + default value Is ``null``.
      */
      function plot(profiles: object, title?: string, axis_title?: string, size?: any, tick?: number, colors?: any, dpi?: object, format?: string, env?: object): object;
   }
   module classchange {
      /**
       * 
       * 
        * @param genes -
        * @param size -
        * 
        * + default value Is ``'3000,2400'``.
        * @param padding -
        * 
        * + default value Is ``'padding:250px 150px 300px 300px;'``.
        * @param bg -
        * 
        * + default value Is ``'white'``.
        * @param colorSet -
        * 
        * + default value Is ``'Set1:c9'``.
        * @param showLabel -
        * 
        * + default value Is ``false``.
        * @param radius -
        * 
        * + default value Is ``'15,50'``.
        * @param xlab -
        * 
        * + default value Is ``'X'``.
        * @param orderByClass -
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function plot(genes: any, size?: any, padding?: any, bg?: any, colorSet?: any, showLabel?: boolean, radius?: any, xlab?: string, orderByClass?: object, env?: object): any;
   }
   /**
    * map gene expressin data to color bends
    * 
    * 
     * @param matrix -
     * @param colorSet 
     * + default value Is ``'RdYlGn:c8'``.
     * @param levels -
     * 
     * + default value Is ``25``.
     * @param env 
     * + default value Is ``null``.
   */
   function color_bends(matrix: object, colorSet?: string, levels?: object, env?: object): object;
   module dem {
      /**
       * 
       * 
        * @param sample DAM data between sample group comparisions, this parameter 
        *  has two data format:
        *  
        *  1. dataframe - should contains at least two data fields: up and down, 
        *     field values of up and down is the DAM counts number. row data in
        *     dataframe is the sample comparision groups
        *  2. list - elements in the list is the sample comparision groups and 
        *     each element slot values should be a tuple list which contains the 
        *     DAM id vector, and each id vector is named up and down.
        * @param upName -
        * 
        * + default value Is ``'up'``.
        * @param downName -
        * 
        * + default value Is ``'down'``.
        * @param size -
        * 
        * + default value Is ``'2700,2400'``.
        * @param padding -
        * 
        * + default value Is ``'padding:200px 350px 250px 200px;'``.
        * @param title -
        * 
        * + default value Is ``'Statistic of Differently Expressed Metabolite'``.
        * @param headN -
        * 
        * + default value Is ``60``.
        * @param ppi -
        * 
        * + default value Is ``300``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function barplot(sample: any, upName?: string, downName?: string, size?: any, padding?: any, title?: string, headN?: object, ppi?: object, env?: object): any;
   }
   /**
   */
   function erase(profiles: object, catalogs: string): object;
   module go {
      module enrichment {
         /**
          * plot of the Go enrichment in bubble plot style
          * 
          * 
           * @param profiles -
           * @param goDb -
           * @param size -
           * 
           * + default value Is ``'2000,1600'``.
           * @param pvalue -
           * 
           * + default value Is ``0.05``.
           * @param topN -
           * 
           * + default value Is ``10``.
           * @param R -
           * 
           * + default value Is ``'10,50'``.
         */
         function bubbles(profiles: object, goDb: object, size?: string, pvalue?: number, topN?: object, R?: string): any;
      }
   }
   module kegg {
      /**
       * Create catalog profiles data for KEGG pathway 
       *  enrichment result its data visualization.
       * 
       * 
        * @param profiles kegg map id mapping to the profiles data
        * @param top -
        * 
        * + default value Is ``10``.
        * @param sort 
        * + default value Is ``true``.
        * @param valueCut 
        * + default value Is ``-1``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function category_profile(profiles: any, top?: object, sort?: boolean, valueCut?: number, env?: object): object;
      module enrichment {
         /**
          * plot kegg enrichment result in bubble plot
          * 
          * 
           * @param profiles A dataframe object(or a set of the dataframe object in tuple list) 
           *  that contains the kegg pathway enrichment data, data fields is 
           *  required for generates the bubble data:
           *  
           *  + ``Raw p``: pvalue of the enrichment term
           *  + ``Impact``: x axis value
           *  + ``Hits``: the molecule hits number in current enrichment term
           *  + ``pathway``: the kegg pathway id
           * @param size -
           * 
           * + default value Is ``'3800,2600'``.
           * @param padding -
           * 
           * + default value Is ``'padding:300px 1000px 300px 200px;'``.
           * @param unenrichColor -
           * 
           * + default value Is ``'LightGray'``.
           * @param themeColors 
           * + default value Is ``'Set1:c8'``.
           * @param alpha 
           * + default value Is ``0.75``.
           * @param displays 
           * + default value Is ``5``.
           * @param serialTopn 
           * + default value Is ``false``.
           * @param bubbleRadius 
           * + default value Is ``[7,40]``.
           * @param heatmap 
           * + default value Is ``false``.
           * @param bubbleStyle 
           * + default value Is ``false``.
           * @param top_samples 
           * + default value Is ``16``.
           * @param ppi -
           * 
           * + default value Is ``300``.
           * @param env -
           * 
           * + default value Is ``null``.
         */
         function bubbles(profiles: any, size?: any, padding?: any, unenrichColor?: string, themeColors?: string, alpha?: number, displays?: object, serialTopn?: boolean, bubbleRadius?: any, heatmap?: boolean, bubbleStyle?: boolean, top_samples?: object, ppi?: object, env?: object): any;
      }
   }
   module plot {
      /**
       * 
       * 
        * @param matrix -
        * @param kmeans_n 
        * + default value Is ``3``.
        * @param size -
        * 
        * + default value Is ``'2400,2700'``.
        * @param padding -
        * 
        * + default value Is ``'padding:250px 150px 300px 300px;'``.
        * @param bg -
        * 
        * + default value Is ``'white'``.
        * @param colorSet 
        * + default value Is ``'red,blue,green'``.
        * @param viewAngle -
        * 
        * + default value Is ``[30,60,-56.25]``.
        * @param viewDistance 
        * + default value Is ``2500``.
        * @param qDisplay quantile value for display the gene labels
        * 
        * + default value Is ``0.9``.
        * @param prefix 
        * + default value Is ``'Cluster: #'``.
        * @param axisFormat 
        * + default value Is ``'CMeans dimension #%s'``.
        * @param showHull 
        * + default value Is ``true``.
        * @param hullAlpha 
        * + default value Is ``150``.
        * @param hullBspline 
        * + default value Is ``3``.
        * @param env 
        * + default value Is ``null``.
      */
      function cmeans3D(matrix: object, kmeans_n?: object, size?: any, padding?: any, bg?: any, colorSet?: string, viewAngle?: any, viewDistance?: number, qDisplay?: number, prefix?: string, axisFormat?: string, showHull?: boolean, hullAlpha?: object, hullBspline?: number, env?: object): any;
      /**
       * Visualize of the gene expression patterns across different sample groups.
       * 
       * 
        * @param matrix -
        * @param size -
        * 
        * + default value Is ``'2400,2700'``.
        * @param padding -
        * 
        * + default value Is ``'padding:250px 150px 300px 300px;'``.
        * @param bg -
        * 
        * + default value Is ``'white'``.
        * @param colorSet color set for visualize the cmeans membership
        * 
        * + default value Is ``'PiYG:c8'``.
        * @param levels 
        * + default value Is ``25``.
        * @param clusterLabelStyle 
        * + default value Is ``'font-style: normal; font-size: 20; font-family: Bookman Old Style;'``.
        * @param legendTitleStyle 
        * + default value Is ``'font-style: normal; font-size: 10; font-family: Microsoft YaHei;'``.
        * @param legendTickStyle 
        * + default value Is ``'font-style: normal; font-size: 10; font-family: Microsoft YaHei;'``.
        * @param axisTickCSS 
        * + default value Is ``'font-style: normal; font-size: 12; font-family: Segoe UI;'``.
        * @param axisLabelCSS 
        * + default value Is ``'font-style: normal; font-size: 12; font-family: Microsoft YaHei;'``.
        * @param driver 
        * + default value Is ``null``.
        * @param ppi 
        * + default value Is ``300``.
        * @param env 
        * + default value Is ``null``.
      */
      function expression_patterns(matrix: object, size?: any, padding?: any, bg?: any, colorSet?: string, levels?: object, clusterLabelStyle?: string, legendTitleStyle?: string, legendTickStyle?: string, axisTickCSS?: string, axisLabelCSS?: string, driver?: object, ppi?: object, env?: object): any;
   }
   module sample {
      /**
        * @param horizontal default value Is ``true``.
        * @param sampleNames default value Is ``null``.
        * @param labelFontCSS default value Is ``'font-style: normal; font-size: 16; font-family: Bookman Old Style;'``.
      */
      function color_bend(g: object, layout: object, geneExpression: object, horizontal?: boolean, sampleNames?: string, labelFontCSS?: string): ;
   }
   /**
    * volcano plot of the different expression result
    * 
    * 
     * @param genes -
     * @param size -
     * 
     * + default value Is ``'2400,2700'``.
     * @param padding -
     * 
     * + default value Is ``'padding:250px 150px 300px 300px;'``.
     * @param bg -
     * 
     * + default value Is ``'white'``.
     * @param colors -
     * 
     * + default value Is ``null``.
     * @param pvalue -
     * 
     * + default value Is ``0.05``.
     * @param level -
     * 
     * + default value Is ``1.5``.
     * @param title 
     * + default value Is ``'volcano plot'``.
     * @param env 
     * + default value Is ``null``.
   */
   function volcano(genes: any, size?: any, padding?: any, bg?: any, colors?: any, pvalue?: number, level?: number, title?: string, env?: object): any;
}
