Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class Disease : Implements INamedValue

        Public Property Category As String
        Public Property Comment As String
        Public Property Drug As KeyValuePair()
        Public Property Pathway As KeyValuePair()

        Public Property Entry As String Implements IKeyedEntity(Of String).Key
        Public Property Name As String

        Public Property Genes As TripleKeyValuesPair()
        Public Property Markers As TripleKeyValuesPair()
        Public Property OtherDBs As KeyValuePair()

        Public Property References As Reference()
        Public Property Description As String

        ''' <summary>
        ''' 从标签文本之中解析出人基因组的基因的编号
        ''' </summary>
        ''' <param name="s$"></param>
        ''' <returns></returns>
        Public Shared Function HumanGeneID(s$) As String
            Return s.GetStackValue("[", "]").Split(":"c).Last
        End Function
    End Class
End Namespace