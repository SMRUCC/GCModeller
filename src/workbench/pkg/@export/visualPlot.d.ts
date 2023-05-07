declare namespace visualPlot {
   module dem {
      /**
        * @param upName default value is ``'up'``.
        * @param downName default value is ``'down'``.
        * @param size default value is ``'2700,2400'``.
        * @param padding default value is ``'padding:200px 350px 250px 200px;'``.
        * @param title default value is ``'Statistic of Differently Expressed Metabolite'``.
        * @param headN default value is ``60``.
        * @param ppi default value is ``300``.
        * @param env default value is ``null``.
      */
      function barplot(sample:any, upName?:string, downName?:string, size?:any, padding?:any, title?:string, headN?:object, ppi?:object, env?:object): any;
   }
   module classchange {
      /**
        * @param size default value is ``'3000,2400'``.
        * @param padding default value is ``'padding:250px 150px 300px 300px;'``.
        * @param bg default value is ``'white'``.
        * @param colorSet default value is ``'Set1:c9'``.
        * @param showLabel default value is ``false``.
        * @param radius default value is ``'15,50'``.
        * @param xlab default value is ``'X'``.
        * @param orderByClass default value is ``null``.
        * @param env default value is ``null``.
      */
      function plot(genes:any, size?:any, padding?:any, bg?:any, colorSet?:any, showLabel?:boolean, radius?:any, xlab?:string, orderByClass?:object, env?:object): any;
   }
   /**
     * @param size default value is ``'2400,2700'``.
     * @param padding default value is ``'padding:250px 150px 300px 300px;'``.
     * @param bg default value is ``'white'``.
     * @param colors default value is ``null``.
     * @param pvalue default value is ``0.05``.
     * @param level default value is ``1.5``.
     * @param title default value is ``'volcano plot'``.
     * @param env default value is ``null``.
   */
   function volcano(genes:any, size?:any, padding?:any, bg?:any, colors?:any, pvalue?:number, level?:number, title?:string, env?:object): any;
   module kegg {
      /**
        * @param top default value is ``10``.
        * @param sort default value is ``true``.
        * @param valueCut default value is ``-1``.
        * @param env default value is ``null``.
      */
      function category_profile(profiles:any, top?:object, sort?:boolean, valueCut?:number, env?:object): any;
      module enrichment {
         /**
           * @param size default value is ``'3800,2600'``.
           * @param padding default value is ``'padding:300px 1000px 300px 200px;'``.
           * @param unenrichColor default value is ``'LightGray'``.
           * @param themeColors default value is ``'Set1:c8'``.
           * @param alpha default value is ``0.75``.
           * @param displays default value is ``5``.
           * @param serialTopn default value is ``false``.
           * @param bubbleRadius default value is ``[7,40]``.
           * @param heatmap default value is ``false``.
           * @param bubbleStyle default value is ``false``.
           * @param top_samples default value is ``16``.
           * @param ppi default value is ``300``.
           * @param env default value is ``null``.
         */
         function bubbles(profiles:any, size?:any, padding?:any, unenrichColor?:string, themeColors?:string, alpha?:number, displays?:object, serialTopn?:boolean, bubbleRadius?:any, heatmap?:boolean, bubbleStyle?:boolean, top_samples?:object, ppi?:object, env?:object): any;
      }
   }
   /**
   */
   function erase(profiles:object, catalogs:string): any;
   module go {
      module enrichment {
         /**
           * @param size default value is ``'2000,1600'``.
           * @param pvalue default value is ``0.05``.
           * @param topN default value is ``10``.
           * @param R default value is ``'10,50'``.
         */
         function bubbles(profiles:object, goDb:object, size?:string, pvalue?:number, topN?:object, R?:string): any;
      }
   }
   module category_profiles {
      /**
        * @param title default value is ``'KEGG Orthology Profiling'``.
        * @param axis_title default value is ``'Number Of Proteins'``.
        * @param size default value is ``'2300,2000'``.
        * @param tick default value is ``-1``.
        * @param colors default value is ``'#E41A1C,#377EB8,#4DAF4A,#984EA3,#FF7F00,#CECE00'``.
        * @param dpi default value is ``300``.
        * @param format default value is ``'F2'``.
        * @param env default value is ``null``.
      */
      function plot(profiles:object, title?:string, axis_title?:string, size?:any, tick?:number, colors?:any, dpi?:object, format?:string, env?:object): any;
   }
   module sample {
      /**
        * @param horizontal default value is ``true``.
        * @param sampleNames default value is ``null``.
        * @param labelFontCSS default value is ``'font-style: normal; font-size: 16; font-family: Bookman Old Style;'``.
      */
      function color_bend(g:object, layout:object, geneExpression:object, horizontal?:boolean, sampleNames?:string, labelFontCSS?:string): any;
   }
   /**
     * @param colorSet default value is ``'RdYlGn:c8'``.
     * @param levels default value is ``25``.
     * @param env default value is ``null``.
   */
   function color_bends(matrix:object, colorSet?:string, levels?:object, env?:object): any;
   module plot {
      /**
        * @param size default value is ``'2400,2700'``.
        * @param padding default value is ``'padding:250px 150px 300px 300px;'``.
        * @param bg default value is ``'white'``.
        * @param colorSet default value is ``'PiYG:c8'``.
        * @param levels default value is ``25``.
        * @param clusterLabelStyle default value is ``'font-style: normal; font-size: 20; font-family: Bookman Old Style;'``.
        * @param legendTitleStyle default value is ``'font-style: normal; font-size: 10; font-family: Microsoft YaHei;'``.
        * @param legendTickStyle default value is ``'font-style: normal; font-size: 10; font-family: Microsoft YaHei;'``.
        * @param axisTickCSS default value is ``'font-style: normal; font-size: 12; font-family: Segoe UI;'``.
        * @param axisLabelCSS default value is ``'font-style: normal; font-size: 12; font-family: Microsoft YaHei;'``.
        * @param driver default value is ``null``.
        * @param ppi default value is ``300``.
        * @param env default value is ``null``.
      */
      function expression_patterns(matrix:object, size?:any, padding?:any, bg?:any, colorSet?:string, levels?:object, clusterLabelStyle?:string, legendTitleStyle?:string, legendTickStyle?:string, axisTickCSS?:string, axisLabelCSS?:string, driver?:object, ppi?:object, env?:object): any;
      /**
        * @param kmeans_n default value is ``3``.
        * @param size default value is ``'2400,2700'``.
        * @param padding default value is ``'padding:250px 150px 300px 300px;'``.
        * @param bg default value is ``'white'``.
        * @param colorSet default value is ``'red,blue,green'``.
        * @param viewAngle default value is ``[30,60,-56.25]``.
        * @param viewDistance default value is ``2500``.
        * @param qDisplay default value is ``0.9``.
        * @param prefix default value is ``'Cluster: #'``.
        * @param axisFormat default value is ``'CMeans dimension #%s'``.
        * @param showHull default value is ``true``.
        * @param hullAlpha default value is ``150``.
        * @param hullBspline default value is ``3``.
        * @param env default value is ``null``.
      */
      function cmeans3D(matrix:object, kmeans_n?:object, size?:any, padding?:any, bg?:any, colorSet?:string, viewAngle?:any, viewDistance?:number, qDisplay?:number, prefix?:string, axisFormat?:string, showHull?:boolean, hullAlpha?:object, hullBspline?:number, env?:object): any;
   }
}
