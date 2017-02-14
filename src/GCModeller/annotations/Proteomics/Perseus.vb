
Imports System.Web.Script.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' perseus output data csv
''' </summary>
Public Class Perseus

    <Column("Only identified by site")> Public Property OnlyIdentifiedBySite As String
    <Column("Reverse")> Public Property Reverse As String
    <Column("Potential contaminant")> Public Property Potential_contaminant As String
    <Column("Peptides")> Public Property Peptides As String
    <Column("Razor + unique peptides")> Public Property Razor_unique_peptides As String
    <Column("Unique peptides")> Public Property Unique_peptides As String
    <Column("Sequence coverage [%]")> Public Property Sequence_coverage As String
    <Column("Unique + razor sequence coverage [%]")> Public Property Unique_razor_sequence_coverage As String
    <Column("Unique sequence coverage [%]")> Public Property Unique_sequence_coverage As String
    <Column("Mol. weight [kDa]")> Public Property Molweight As String
    <Column("Q-value")> Public Property Qvalue As String
    <Column("Score")> Public Property Score As String
    <Column("Intensity")> Public Property Intensity As String
    <Column("MS/MS Count")> Public Property MSMSCount As String

    ''' <summary>
    ''' 蛋白质搜库的结果
    ''' </summary>
    ''' <returns></returns>
    <Collection("Protein IDs", ";")> Public Property ProteinIDs As String()
    <Collection("Majority protein IDs", ";")> Public Property Majority_proteinIDs As String()

    Public Property Data As Dictionary(Of String, String)

    Const Tag As String = "LFQ intensity"

    <ScriptIgnore>
    Public ReadOnly Property ExpressionValues As NamedValue(Of Double)()
        Get
            Return Data _
                .Where(Function(x) InStr(x.Key, Tag, CompareMethod.Text) = 1) _
                .Select(Function(x) New NamedValue(Of Double) With {
                    .Name = x.Key,
                    .Value = Val(x.Value)
                }) _
                .ToArray
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
