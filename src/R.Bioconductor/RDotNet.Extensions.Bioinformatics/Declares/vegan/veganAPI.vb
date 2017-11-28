Namespace vegan

    ' http://blog.csdn.net/sinat_38163598/article/details/72825742

    ''' <summary>
    ''' ### Community Ecology Package: Ordination, Diversity and Dissimilarities
    ''' 
    ''' The vegan package provides tools for descriptive community ecology. It has most basic functions of diversity analysis, 
    ''' community ordination and dissimilarity analysis. Most of its multivariate tools can be used for other data types as 
    ''' well.
    ''' 
    ''' The functions in the vegan package contain tools for diversity analysis, ordination methods and tools for the analysis 
    ''' of dissimilarities. Together with the labdsv package, the vegan package provides most standard tools of descriptive 
    ''' community analysis. Package ade4 provides an alternative comprehensive package, and several other packages complement 
    ''' vegan and provide tools for deeper analysis in specific fields. Package BiodiversityR provides a GUI for a large subset 
    ''' of vegan functionality.
    ''' 
    ''' The vegan package Is developed at GitHub (https://github.com/vegandevs/vegan/). GitHub provides up-to-date information 
    ''' And forums for bug reports.
    ''' 
    ''' Most important changes In vegan documents can be read With news(package="vegan") And vignettes can be browsed With 
    ''' browseVignettes("vegan"). The vignettes include a vegan FAQ, discussion On design decisions, Short introduction To 
    ''' ordination And discussion On diversity methods. A tutorial Of the package at http://cc.oulu.fi/~jarioksa/opetus/metodi/vegantutor.pdf 
    ''' provides a more thorough introduction to the package.
    ''' 
    ''' To see the preferable citation of the package, type citation("vegan").
    ''' </summary>
    Public Module veganAPI

        Public Enum Models
            [global]
            local
            linear
            hybrid
        End Enum

        ''' <summary>
        ''' Global and Local Non-metric Multidimensional Scaling and Linear and Hybrid Scaling
        ''' 
        ''' Function implements Kruskal's (1964a,b) non-metric multidimensional scaling (NMDS) using monotone regression and 
        ''' primary (“weak”) treatment of ties. In addition to traditional global NMDS, the function implements local NMDS, 
        ''' linear and hybrid multidimensional scaling.
        ''' </summary>
        ''' <param name="dist$">Input dissimilarities.</param>
        ''' <param name="y$">Starting configuration. A random configuration will be generated if this is missing.</param>
        ''' <param name="k#">Number of dimensions. NB., the number of points n should be n > 2*k + 1, and preferably higher in non-metric MDS.</param>
        ''' <param name="model">MDS model: "global" is normal non-metric MDS with a monotone regression, "local" is non-metric MDS with separate regressions for each point, "linear" uses linear regression, and "hybrid" uses linear regression for dissimilarities below a threshold in addition to monotone regression. See Details.</param>
        ''' <param name="threshold#">Dissimilarity below which linear regression is used alternately with monotone regression.</param>
        ''' <param name="maxit#">Maximum number of iterations.</param>
        ''' <param name="weakties">Use primary or weak tie treatment, where equal observed dissimilarities are allowed to have different fitted values. if FALSE, then secondary (strong) tie treatment is used, and tied values are not broken.</param>
        ''' <param name="stress#">Use stress type 1 or 2 (see Details).</param>
        ''' <param name="scaling">Scale final scores to unit root mean squares.</param>
        ''' <param name="pc">Rotate final scores to principal components.</param>
        ''' <param name="smin#">Convergence criteria: iterations stop when stress drops below smin, scale factor of the gradient drops below sfgrmin, or stress ratio between two iterations goes over sratmax (but is still &lt; 1).</param>
        ''' <param name="sfgrmin#">Convergence criteria: iterations stop when stress drops below smin, scale factor of the gradient drops below sfgrmin, or stress ratio between two iterations goes over sratmax (but is still &lt; 1).</param>
        ''' <param name="sratmax#">Convergence criteria: iterations stop when stress drops below smin, scale factor of the gradient drops below sfgrmin, or stress ratio between two iterations goes over sratmax (but is still &lt; 1).</param>
        ''' <param name="additionals"></param>
        ''' <returns></returns>
        Public Function monoMDS(dist$, y$, Optional k# = 2, Optional model As Models = Models.global,
                                Optional threshold# = 0.8, Optional maxit# = 200, Optional weakties As Boolean = True, Optional stress# = 1,
                                Optional scaling As Boolean = True, Optional pc As Boolean = True, Optional smin# = 0.0001, Optional sfgrmin# = 0.0000001,
                                Optional sratmax# = 0.99999, Optional additionals As Dictionary(Of String, String) = Nothing) As String

        End Function
    End Module
End Namespace