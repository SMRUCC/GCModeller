Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace base.Extremes

    ''' <summary>
    ''' Returns the (parallel) maxima and minima of the input values.
    ''' </summary>
    ''' <remarks>
    ''' max and min return the maximum or minimum of all the values present in their arguments, as integer if all are logical or integer, as double if all are numeric, and character otherwise.
    ''' If na.rm Is False an NA value In any Of the arguments will cause a value Of NA To be returned, otherwise NA values are ignored.
    ''' The minimum And maximum Of a numeric empty Set are +Inf And -Inf (In this order!) which ensures transitivity, e.g., min(x1, min(x2)) == min(x1, x2). For numeric x max(x) == -Inf And min(x) == +Inf whenever length(x) == 0 (after removing missing values If requested). However, pmax And pmin Return NA If all the parallel elements are NA even For na.rm = True.
    ''' pmax And pmin take one Or more vectors (Or matrices) as arguments And return a single vector giving the 'parallel’ maxima (or minima) of the vectors. The first element of the result is the maximum (minimum) of the first elements of all the arguments, the second element of the result is the maximum (minimum) of the second elements of all the arguments and so on. Shorter inputs (of non-zero length) are recycled if necessary. Attributes (see attributes: such as names or dim) are copied from the first argument (if applicable).
    ''' pmax.int And pmin.int are faster internal versions only used when all arguments are atomic vectors And there are no classes: they drop all attributes. (Note that all versions fail For raw And complex vectors since these have no ordering.)
    ''' max And min are generic functions: methods can be defined For them individually Or via the Summary group generic. For this To work properly, the arguments ... should be unnamed, And dispatch Is On the first argument.
    ''' By definition the min/max Of a numeric vector containing an NaN Is NaN, except that the min/max Of any vector containing an NA Is NA even If it also contains an NaN. Note that max(NA, Inf) == NA even though the maximum would be Inf whatever the missing value actually Is.
    ''' Character versions are sorted lexicographically, And this depends On the collating sequence Of the locale In use: the help For 'Comparison’ gives details. The max/min of an empty character vector is defined to be character NA. (One could argue that as "" is the smallest character element, the maximum should be "", but there is no obvious candidate for the minimum.)
    ''' </remarks>
    Public MustInherit Class extremes : Inherits IRToken

        ''' <summary>
        ''' numeric or character arguments (see Note).
        ''' </summary>
        ''' <remarks>
        ''' ‘Numeric’ arguments are vectors of type integer and numeric, and logical (coerced to integer). For historical reasons, NULL is accepted as equivalent to integer(0).
        ''' pmax And pmin will also work on classed objects with appropriate methods for comparison, Is.na And rep (if recycling of arguments Is needed).
        ''' </remarks>
        ''' <returns></returns>
        <Parameter("...", ValueTypes.List, False, True)>
        Public Property x As RExpression()

        ''' <summary>
        ''' a logical indicating whether missing values should be removed.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("na.rm")>
        Public Property NArm As Boolean = False
    End Class

    <RFunc("max")>
    Public Class max : Inherits extremes

        Sub New(ParamArray x As String())
            Me.x = x.ToArray(Function(s) New RExpression(s))
        End Sub
    End Class

    <RFunc("min")>
    Public Class min : Inherits extremes

        Sub New(ParamArray x As String())
            Me.x = x.ToArray(Function(s) New RExpression(s))
        End Sub
    End Class

    <RFunc("pmax")>
    Public Class pmax : Inherits extremes

        Sub New(ParamArray x As String())
            Me.x = x.ToArray(Function(s) New RExpression(s))
        End Sub
    End Class

    <RFunc("pmin")>
    Public Class pmin : Inherits extremes

        Sub New(ParamArray x As String())
            Me.x = x.ToArray(Function(s) New RExpression(s))
        End Sub
    End Class

    <RFunc("pmax.int")>
    Public Class pmaxInt : Inherits extremes

        Sub New(ParamArray x As String())
            Me.x = x.ToArray(Function(s) New RExpression(s))
        End Sub
    End Class

    <RFunc("pmin.int")>
    Public Class pminInt : Inherits extremes

        Sub New(ParamArray x As String())
            Me.x = x.ToArray(Function(s) New RExpression(s))
        End Sub
    End Class
End Namespace
