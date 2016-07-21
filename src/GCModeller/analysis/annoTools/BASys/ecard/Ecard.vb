Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class Ecard : Inherits ClassObject

    Public Property Name As NamedValue(Of String)
    Public Property Values As EcardValue()
    Public Property Keys As String()

    Public Overrides Function ToString() As String
        Return Name.GetJson
    End Function

    Public Shared Function Parser(path As String) As Ecard
        Dim tag As NamedValue(Of String) = Nothing
        Dim tokens = ecardParser.ParseFile(path, tag).ToArray
        Dim keys As String() = tokens.Select(
            Function(x) x.Keys).MatrixAsIterator.Distinct.ToArray
        Dim values As EcardValue() = tokens.ToArray(AddressOf EcardValue.[New])

        Return New Ecard With {
            .Name = tag,
            .Values = values,
            .Keys = keys
        }
    End Function
End Class

Public Class EcardValue

    Public Property NAME As String
    Public Property TYPE As String
    Public Property [CDATE] As String
    Public Property MDATE As String
    Public Property QUALITY As String
    Public Property EVIDENCE As String
    Public Property SOURCE As String
    Public Property PARAMETER As String
    Public Property VALUE As String
    Public Property CATEGORY As String
    Public Property AUTHORS As String
    Public Property TITLE As String
    Public Property JOURNAL As String
    Public Property MEDLINE As String
    Public Property PUBMED As String
    Public Property RP As String
    Public Property COMMENT As String
    Public Property CLASSIFICATION As String
    Public Property SPECIES As String
    Public Property GENUS As String
    Public Property COMMON_NAME As String
    Public Property BINOMIAL As String
    Public Property [VARIANT] As String
    Public Property SUBSPECIES As String
    Public Property NCBI_TAXID As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function [New](token As Dictionary(Of String, String())) As EcardValue

    End Function
End Class