Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace gplots

    ''' <summary>
    ''' Plot a Venn diagrams for up to 5 sets
    ''' </summary>
    ''' <remarks>
    ''' data should be either a named list of vectors containing character string names ("GeneAABBB", "GeneBBBCY", .., "GeneXXZZ")
    ''' or indexes of group intersections (1, 2, .., N), or a data frame containing indicator variables (TRUE, FALSE, TRUE, ..)
    ''' for group intersectionship. Group names will be taken from the component list element or column names.
    '''
    ''' Invisibly returns an object of class "venn", containing a matrix of all possible sets of groups, and the observed count of
    ''' items belonging to each The fist column contains observed counts, subsequent columns contain 0-1 indicators of group
    ''' intersectionship.
    ''' </remarks>
    <RFunc(NameOf(venn))> Public Class venn : Inherits IRToken

        ''' <summary>
        ''' Either a list list containing vectors of names or indices of group intersections,
        ''' or a data frame containing boolean indicators of group intersectionship (see below)
        ''' </summary>
        ''' <returns></returns>
        Public Property data As RExpression
        ''' <summary>
        ''' Subset of valid name/index elements. Values ignore values in codedata not
        ''' in this list will be ignored. Use NA to use all elements of data (the default).
        ''' </summary>
        ''' <returns></returns>
        Public Property universe As RExpression = NA
        ''' <summary>
        ''' Character scaling of the smallest group counts
        ''' </summary>
        ''' <returns></returns>
        Public Property small As Double = 0.7
        ''' <summary>
        ''' Logical flag indicating whether the internal group label should be displayed
        ''' </summary>
        ''' <returns></returns>
        Public Property showSetLogicLabel As Boolean = False
        ''' <summary>
        ''' Logical flag indicating whether unobserved groups should be omitted.
        ''' </summary>
        ''' <returns></returns>
        Public Property simplify As Boolean = False
        ''' <summary>
        ''' Logical flag indicating whether the plot should be displayed. If false, simply returns the group count matrix.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("show.plot")> Public Property showPlot As Boolean = True
        ''' <summary>
        ''' Logical flag indicating if the returned object should have the attribute "individuals.in.intersections"
        ''' featuring for every set a list of individuals that are assigned to it.
        ''' </summary>
        ''' <returns></returns>
        Public Property intersections As Boolean = True
    End Class

    ''' <summary>
    ''' ## S3 method for class '<see cref="venn"/>'
    ''' </summary>
    <RFunc(NameOf(plot))> Public Class plot : Inherits IRToken
        ''' <summary>
        ''' Either a list list containing vectors of names or indices of group intersections,
        ''' or a data frame containing boolean indicators of group intersectionship (see below)
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression
        Public Property y As RExpression
        ''' <summary>
        ''' Character scaling of the smallest group counts
        ''' </summary>
        ''' <returns></returns>
        Public Property small As Double = 0.7
        ''' <summary>
        ''' Logical flag indicating whether the internal group label should be displayed
        ''' </summary>
        ''' <returns></returns>
        Public Property showSetLogicLabel As Boolean = False
        ''' <summary>
        ''' Logical flag indicating whether unobserved groups should be omitted.
        ''' </summary>
        ''' <returns></returns>
        Public Property simplify As Boolean = False
    End Class
End Namespace