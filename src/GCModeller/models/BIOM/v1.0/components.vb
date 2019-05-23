Imports Microsoft.VisualBasic.Serialization.JSON

Namespace v10

    ''' <summary>
    ''' ``&lt;string>`` Table type (a controlled vocabulary) acceptable values
    ''' </summary>
    Public Class types

        Public Const OTU_table As String = "OTU table"
        Public Const Pathway_table As String = "Pathway table"
        Public Const Function_table As String = "Function table"
        Public Const Ortholog_table As String = "Ortholog table"
        Public Const Gene_table As String = "Gene table"
        Public Const Metabolite_table As String = "Metabolite table"
        Public Const Taxon_table As String = "Taxon table"

        Public Const MatrixTypeSparse As String = "sparse"
        Public Const MatrixTypeDense As String = "dense"

        Public Const MatrixElTypeInt As String = "int"
        Public Const MatrixElTypeFloat As String = "float"
        Public Const MatrixElTypeUnicode As String = "unicode"
    End Class

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

    Public Class row
        Public Property id As String
        Public Property metadata As meta
    End Class

    Public Class meta
        Public Property taxonomy As String()
        Public Property KEGG_Pathways As String()
    End Class

    Public Class column
        Public Property id As String
        Public Property metadata As columnMeta
    End Class

    Public Class columnMeta
        Public Property BarcodeSequence As String
        Public Property LinkerPrimerSequence As String
        Public Property BODY_SITE As String
        Public Property Description As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace