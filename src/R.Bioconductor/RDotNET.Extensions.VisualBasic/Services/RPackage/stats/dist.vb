Imports Microsoft.VisualBasic.Serialization
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace stats

    ''' <summary>
    ''' Distance Matrix Computation
    ''' </summary>
    Public MustInherit Class distAPI : Inherits IRToken

        ''' <summary>
        ''' Distance Matrix Computation
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property Func(x As String) As String
            Get
                Dim api As distAPI = Me.ShadowsCopy
                Call api.__setX(x)
                Return api.RScript
            End Get
        End Property

        ''' <summary>
        ''' logical value indicating whether the diagonal of the distance matrix should be printed by print.dist.
        ''' </summary>
        ''' <returns></returns>
        Public Property diag As Boolean = False
        ''' <summary>
        ''' logical value indicating whether the upper triangle of the distance matrix should be printed by print.dist.
        ''' </summary>
        ''' <returns></returns>
        Public Property upper As Boolean = False

        Protected MustOverride Sub __setX(x As String)
    End Class

    ''' <summary>
    ''' This function computes and returns the distance matrix computed by using the specified distance measure to compute the distances between the rows of a data matrix.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    <RFunc("dist")> Public Class dist : Inherits distAPI

        ''' <summary>
        ''' a numeric matrix, data frame or "dist" object.
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression
        ''' <summary>
        ''' the distance measure to be used. This must be one of "euclidean", "maximum", "manhattan", "canberra", "binary" or "minkowski". Any unambiguous substring can be given.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Available distance measures are (written for two vectors x and y):
        ''' 
        ''' euclidean:
        ''' Usual distance between the two vectors (2 norm aka L_2), sqrt(sum((x_i - y_i)^2)).
        '''
        ''' maximum:
        ''' Maximum distance between two components Of x And y (supremum norm)
        '''
        ''' manhattan:
        ''' Absolute distance between the two vectors (1 norm aka L_1).
        ''' 
        ''' canberra:
        ''' sum(|x_i - y_i| / |x_i + y_i|). Terms with zero numerator And denominator are omitted from the sum And treated as if the values were missing.
        ''' 
        ''' This Is intended for non-negative values (e.g., counts): taking the absolute value Of the denominator Is a 1998 R modification To avoid negative distances.
        ''' 
        ''' binary:
        ''' (aka asymmetric binary): The vectors are regarded As binary bits, so non-zero elements are 'on’ and zero elements are ‘off’. 
        ''' The distance is the proportion of bits in which only one is on amongst those in which at least one is on.
        '''
        ''' minkowski:
        ''' The p norm, the pth root Of the sum Of the pth powers Of the differences Of the components.
        '''
        ''' Missing values are allowed, And are excluded from all computations involving the rows within which they occur. 
        ''' Further, When Inf values are involved, all pairs Of values are excluded When their contribution To the distance gave NaN Or NA. 
        ''' If some columns are excluded In calculating a Euclidean, Manhattan, Canberra Or Minkowski distance, the sum Is scaled up proportionally To the number Of columns used. 
        ''' If all pairs are excluded When calculating a particular distance, the value Is NA.
        ''' </remarks>
        Public Property method As String = "euclidean"
        ''' <summary>
        ''' The power of the Minkowski distance.
        ''' </summary>
        ''' <returns></returns>
        Public Property p As Integer = 2

        Protected Overrides Sub __setX(x As String)
            Me.x = New RExpression(x)
        End Sub
    End Class

    <RFunc("as.dist")> Public Class AsDist : Inherits distAPI

        ''' <summary>
        ''' An object with distance information to be converted to a "dist" object. For the default method, a "dist" object, or a matrix (of distances) or an object which can be coerced to such a matrix using as.matrix(). 
        ''' (Only the lower triangle of the matrix is used, the rest is ignored).
        ''' </summary>
        ''' <returns></returns>
        Public Property m As RExpression

        Protected Overrides Sub __setX(x As String)
            m = New RExpression(x)
        End Sub
    End Class

    ''' <summary>
    ''' ## S3 method for class 'dist'
    ''' </summary>
    <RFunc("print")> Public Class print : Inherits IRToken

        ''' <summary>
        ''' a numeric matrix, data frame or "dist" object.
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression
        ''' <summary>
        ''' logical value indicating whether the diagonal of the distance matrix should be printed by print.dist.
        ''' </summary>
        ''' <returns></returns>
        Public Property diag As RExpression = NULL
        ''' <summary>
        ''' logical value indicating whether the upper triangle of the distance matrix should be printed by print.dist.
        ''' </summary>
        ''' <returns></returns>
        Public Property upper As RExpression = NULL
        ''' <summary>
        ''' passed to format inside of print().
        ''' </summary>
        ''' <returns></returns>
        Public Property digits As RExpression = getOption("digits")
        Public Property justify As String = "none"
        ''' <summary>
        ''' further arguments, passed to other methods.
        ''' </summary>
        ''' <returns></returns>
        Public Property right As Boolean = True
    End Class
End Namespace