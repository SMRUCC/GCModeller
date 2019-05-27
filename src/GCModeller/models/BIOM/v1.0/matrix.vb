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
End Namespace