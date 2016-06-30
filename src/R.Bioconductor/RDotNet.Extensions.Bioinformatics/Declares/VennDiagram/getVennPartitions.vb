Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNet.Extensions.VisualBasic
Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace VennDiagram

    ''' <summary>
    ''' Partitions a list into Venn regions.
    ''' 
    ''' If force.unique is FALSE, then there are two supported methods of grouping categories with duplicated elements in common. 
    ''' If hierarchical is FALSE, then any common elements are gathered into a pool. So if x &lt;- list(a = c(1,1,2,2,3,3), b=c(1,2,3,4,4,5), c=c(1,4)) then (b intersect c)/(a) would contain three 4's. Since the 4's are pooled, (b)/(a union c) contains no 4's. 
    ''' If hierachical is TRUE, then (b intersect c)/(a) would contain one 4.Then (b)/(a union c) cotains one 4.
    ''' </summary>
    <RFunc("get.venn.partitions")> Public Class getVennPartitions : Inherits vennBase

        ''' <summary>
        ''' A list of vectors.
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression
        ''' <summary>
        ''' A logical value. Should only unique values be considered?
        ''' </summary>
        ''' <returns></returns>
        <Parameter("force.unique")> Public Property forceUnique As Boolean = True
        ''' <summary>
        ''' A logical value. Should the elements in each region be returned?
        ''' </summary>
        ''' <returns></returns>
        <Parameter("keep.elements")> Public Property keepElements As Boolean = True
        ''' <summary>
        ''' A logical value. Changed the way overlapping elements are treated if force.unique is TRUE.
        ''' </summary>
        ''' <returns></returns>
        Public Property hierarchical As Boolean = False
    End Class
End Namespace