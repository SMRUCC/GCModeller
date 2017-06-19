Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.Abstract
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.STRING.StringDB.Tsv

''' <summary>
''' 对于<see cref="LinkAction"/>和<see cref="linksDetail"/>而言，都是从ftp服务器上面下载的结果数据
''' 这个tsv文件则是搜索蛋白质网络结果之后的export下载数据的文件格式读取对象
''' </summary>
''' <remarks>这个数据模型是使用STRING的蛋白编号作为节点编号的</remarks>
Public Class InteractExports
    Implements IInteraction

    ''' <summary>
    ''' <see cref="UniprotXML"/>之中的外部数据库的编号引用
    ''' </summary>
    Public Const STRING$ = NameOf(InteractExports.[STRING])

    <Column("#node1")>
    Public Property node1 As String
    Public Property node2 As String
    Public Property node1_string_internal_id As String
    Public Property node2_string_internal_id As String

    ''' <summary>
    ''' 可以在uniprot注释数据之中的<see cref="entry.dbReferences"/>找得到``STRING``编号
    ''' </summary>
    ''' <returns></returns>
    Public Property node1_external_id As String Implements IInteraction.source
    ''' <summary>
    ''' 可以在uniprot注释数据之中的<see cref="entry.dbReferences"/>找得到``STRING``编号
    ''' </summary>
    ''' <returns></returns>
    Public Property node2_external_id As String Implements IInteraction.target
    Public Property neighborhood_on_chromosome As String
    Public Property gene_fusion As String
    Public Property phylogenetic_cooccurrence As String
    Public Property homology As String
    Public Property coexpression As String
    Public Property experimentally_determined_interaction As String
    Public Property database_annotated As String
    Public Property automated_textmining As String
    Public Property combined_score As Double

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function ImportsTsv(tsv$) As InteractExports()
        Return DataImports.ImportsTsv(Of InteractExports)(tsv.ReadAllLines)
    End Function
End Class
