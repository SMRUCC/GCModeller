#Region "Microsoft.VisualBasic::a932304c2b841708e316761009cbedeb, G:/GCModeller/src/GCModeller/data/STRING//tsv/InteractExports.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 54
    '    Code Lines: 32
    ' Comment Lines: 16
    '   Blank Lines: 6
    '     File Size: 2.36 KB


    ' Class InteractExports
    ' 
    '     Properties: automated_textmining, coexpression, combined_score, database_annotated, experimentally_determined_interaction
    '                 gene_fusion, homology, neighborhood_on_chromosome, node1, node1_external_id
    '                 node1_string_internal_id, node2, node2_external_id, node2_string_internal_id, phylogenetic_cooccurrence
    ' 
    '     Function: ImportsTsv, ToString
    ' 
    ' /********************************************************************************/

#End Region

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
    Public Property node1 As String Implements IInteraction.source
    Public Property node2 As String Implements IInteraction.target
    Public Property node1_string_internal_id As String
    Public Property node2_string_internal_id As String

    ''' <summary>
    ''' 可以在uniprot注释数据之中的<see cref="entry.dbReferences"/>找得到``STRING``编号
    ''' </summary>
    ''' <returns></returns>
    Public Property node1_external_id As String
    ''' <summary>
    ''' 可以在uniprot注释数据之中的<see cref="entry.dbReferences"/>找得到``STRING``编号
    ''' </summary>
    ''' <returns></returns>
    Public Property node2_external_id As String
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
