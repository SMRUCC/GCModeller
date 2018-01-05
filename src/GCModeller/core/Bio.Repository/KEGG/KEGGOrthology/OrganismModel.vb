Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism

''' <summary>
''' The <see cref="Pathway"/> repository
''' </summary>
Public Class OrganismModel

    Public Property organism As OrganismInfo
    ''' <summary>
    ''' 基因组是由代谢途径功能模块所构成的
    ''' </summary>
    ''' <returns></returns>
    Public Property genome As Pathway()

    Public Overrides Function ToString() As String
        Return organism.ToString
    End Function

    ''' <summary>
    ''' 从KEGG的代谢途径下载文件夹加载零散的文件数据构成这个整体数据模型
    ''' </summary>
    ''' <param name="directory"></param>
    ''' <returns></returns>
    Public Shared Function CreateModel(directory As String) As OrganismModel
        Dim organism As OrganismInfo = (directory & "/kegg.json").LoadObject(Of OrganismInfo)
        Dim model As Pathway() = (ls - l - r - "*.Xml" <= directory) _
            .Select(AddressOf LoadXml(Of Pathway)) _
            .ToArray

        Return New OrganismModel With {
            .genome = model,
            .organism = organism
        }
    End Function

    Public Shared Function EnumerateModules(handle As String) As IEnumerable(Of Pathway)
        If handle.FileExists AndAlso handle.ExtensionSuffix.TextEquals("Xml") Then
            Return handle.LoadXml(Of OrganismModel).genome
        Else
            Return (ls - l - r - "*.Xml" <= handle) _
                .Select(AddressOf LoadXml(Of Pathway))
        End If
    End Function
End Class
