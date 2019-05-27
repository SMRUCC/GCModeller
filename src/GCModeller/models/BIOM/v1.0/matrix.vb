Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace v10

    ''' <summary>
    ''' BIOM json with integer matrix data
    ''' </summary>
    Public Class IntegerMatrix : Inherits Json(Of Integer)

        Public Overloads Shared Function LoadFile(path$) As IntegerMatrix
            Dim json$ = path.ReadAllText
            Dim biom As IntegerMatrix = JsonContract.EnsureDate(json, "date").LoadJSON(Of IntegerMatrix)
            Return biom
        End Function
    End Class

    ''' <summary>
    ''' BIOM json with double matrix data
    ''' </summary>
    Public Class FloatMatrix : Inherits Json(Of Double)

        Public Overloads Shared Function LoadFile(path$) As FloatMatrix
            Dim json$ = path.ReadAllText
            Dim biom As FloatMatrix = JsonContract.EnsureDate(json, "date").LoadJSON(Of FloatMatrix)
            Return biom
        End Function
    End Class

    ''' <summary>
    ''' BIOM json with string matrix data
    ''' </summary>
    Public Class StringMatrix : Inherits Json(Of String)

        Public Overloads Shared Function LoadFile(path$) As StringMatrix
            Dim json$ = path.ReadAllText
            Dim biom As StringMatrix = JsonContract.EnsureDate(json, "date").LoadJSON(Of StringMatrix)
            Return biom
        End Function
    End Class

    ''' <summary>
    ''' 在<see cref="matrix_type.sparse"/>和<see cref="matrix_type.dense"/>
    ''' 这两种布局的矩阵之间相互转换
    ''' </summary>
    Public Module MatrixConversion

        ''' <summary>
        ''' 目标是否是稀疏矩阵，需要转换为一个完整的矩阵对象
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="table"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function RequiredConvertToDenseMatrix(Of T As {IComparable(Of T), IEquatable(Of T), IComparable})(table As Json(Of T)) As Boolean
            Return Strings.LCase(table.matrix_type) = matrix_type.sparse
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="table"></param>
        ''' <param name="shape">``[rows, columns]``</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' if matrix_type is "sparse", 
        ''' 
        ''' ```
        ''' [[row, column, value],
        '''  [row, column, value],
        '''  ...
        ''' ]
        ''' ```
        ''' </remarks>
        <Extension>
        Public Function ToDenseMatrix(Of T)(table As T()(), shape As Integer()) As T()()

        End Function
    End Module
End Namespace