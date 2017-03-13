Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' The data model of the genes in the human genome.(人类基因组之中的基因模型)    
    ''' </summary>
    <XmlRoot("HumanGene")> Public Class Hsa_gene : Implements INamedValue

        Public Property Entry As String Implements IKeyedEntity(Of String).Key
        Public Property GeneName As String
        Public Property Definition As KeyValuePair
        Public Property Pathway As KeyValuePair()
        Public Property Disease As KeyValuePair()
        Public Property DrugTarget As KeyValuePair()
        ' Public Property Motif As NamedCollection(Of String)
        ' Public Property [Structure] As NamedCollection(Of String)
        Public Property Position As String
        Public Property AA As String
        Public Property NT As String
        Public Property OtherDBs As KeyValuePair()
        Public Property Modules As KeyValuePair()

        ''' <summary>
        ''' Split of the <see cref="GeneName"/> property value
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MyNames As String()
            Get
                Return GeneName.Replace("'", "").Trim.StringSplit(",\s+")
            End Get
        End Property

        Public ReadOnly Property MyPositions As String()
            Get
                Return Position.StringSplit("\s+and\s+")
            End Get
        End Property

        ''' <summary>
        ''' + 两个都匹配，则返回2
        ''' + 匹配任意一个，则返回1
        ''' + 匹配不上任何一个，则返回0
        ''' </summary>
        ''' <param name="pos$"></param>
        ''' <param name="symbol$"></param>
        ''' <returns></returns>
        Public Function Match(pos$, symbol$) As Integer
            Dim n%

            n += If(MatchAnyPosition(pos), 1, 0)
            n += If(MatchAnyName(symbol), 1, 0)

            Return n
        End Function

        Public Function MatchAnyPosition(pos$) As Boolean
            Return Not MyPositions _
                .Where(Function(l) l.TextEquals(pos)) _
                .FirstOrDefault _
                .StringEmpty
        End Function

        ''' <summary>
        ''' 目标输入的基因名称符号能够匹配上这个基因对象的任意一个名称
        ''' </summary>
        ''' <param name="symbol$"></param>
        ''' <returns></returns>
        Public Function MatchAnyName(symbol$) As Boolean
            Return Not MyNames _
                .Where(Function(s$) s.TextEquals(symbol)) _
                .FirstOrDefault _
                .StringEmpty
        End Function

        Public Overrides Function ToString() As String
            Return Definition.ToString
        End Function
    End Class
End Namespace