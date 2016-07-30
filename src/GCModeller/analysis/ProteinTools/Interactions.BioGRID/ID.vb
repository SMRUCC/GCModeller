Imports System.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' <see cref="ID.Type"/>:<see cref="ID.sId"/>
''' </summary>
Public Class ID

    Public Const Type_NCBI_Locus As String = "entrez gene/locuslink"
    Public Const Type_biogridID As String = "biogrid"

    Public Property Type As Types
    Public Property sId As String

    Sub New()
    End Sub

    Sub New(type As String, id As String)
        Me.Type = StringTypes.TryGetValue(type, Types.Unknown)
        Me.sId = id
    End Sub

    ''' <summary>
    ''' <see cref="ID.Type"/>:<see cref="ID.sId"/>
    ''' </summary>
    ''' <param name="raw"></param>
    Sub New(raw As String)
        Dim value As NamedValue(Of String) = raw.GetTagValue(":")

        Me.sId = value.x
        Me.Type = StringTypes.TryGetValue(value.Name, Types.Unknown)
    End Sub

    Public Shared ReadOnly Property StringTypes As Dictionary(Of String, ID.Types) =
        Enums(Of Types)().ToDictionary(Function(x) x.Description)

    ''' <summary>
    ''' 编号的数据库来源
    ''' </summary>
    Public Enum Types
        Unknown = -1
        ''' <summary>
        ''' <see cref="ID.Type_NCBI_Locus"/>
        ''' </summary>
        <Description(ID.Type_NCBI_Locus)>
        NCBI
        ''' <summary>
        ''' <see cref="ID.Type_biogridID"/>
        ''' </summary>
        <Description(ID.Type_biogridID)>
        BioGrid
    End Enum

    Public Overrides Function ToString() As String
        Return $"{Type.Description}:{sId}"
    End Function

    Public Shared Function FieldParser(raw As String) As ID()
        Dim tokens As String() = raw.Split("|"c)
        Return tokens.ToArray(Function(s) New ID(s))
    End Function
End Class
