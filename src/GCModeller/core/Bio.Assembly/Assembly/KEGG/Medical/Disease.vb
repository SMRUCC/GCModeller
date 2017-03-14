Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.Medical

    Public Class Disease : Implements INamedValue

        Public Property Entry As String Implements INamedValue.Key
        Public Property Names As String()
        Public Property Description As String
        Public Property Category As String
        Public Property Genes As String()
        Public Property Carcinogen As String()
        Public Property Pathogens As String()
        Public Property Markers As String()
        Public Property Drugs As String()

        ''' <summary>
        ''' 多行数据已经join过了的单行字符串
        ''' </summary>
        ''' <returns></returns>
        Public Property Comments As String
        Public Property DbLinks As DBLink()
        Public Property References As Reference()
        Public Property Env_factors As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
