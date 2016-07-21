Imports System.Reflection
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
        Dim schema = EcardValue.Schema
        Dim values As EcardValue() = tokens.ToArray(
            Function(x) EcardValue.[New](x, schema))

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

    Public Shared ReadOnly Property Schema As Dictionary(Of String, Action(Of EcardValue, String))
        Get
            Return New Dictionary(Of String, Action(Of EcardValue, String))(__schema)
        End Get
    End Property

    Shared ReadOnly __schema As Dictionary(Of String, Action(Of EcardValue, String))

    Private Shared Function __getSchema() As Dictionary(Of String, Action(Of EcardValue, String))
        Dim type As Type = GetType(EcardValue)
        Dim ps = type.Schema(PropertyAccessibilityControls.ReadWrite)
        Dim result = ps.ToDictionary(
            Function(x) x.Key.Replace("_", " "),
            Function(x) __setValue(x.Value))
        Return result
    End Function

    Private Shared Function __setValue(x As PropertyInfo) As Action(Of EcardValue, String)
        Return Sub(o, value) Call x.SetValue(o, value)
    End Function

    Friend Shared Function [New](token As Dictionary(Of String, String()), Schema As Dictionary(Of String, Action(Of EcardValue, String))) As EcardValue
        Dim value As New EcardValue

        For Each x In token

        Next

        Return value
    End Function
End Class