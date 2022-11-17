#Region "Microsoft.VisualBasic::e4296622d60d186264856fafb6625a92, Bio.Repository\KEGG\KEGGOrthology\OrganismModel.vb"

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

    ' Class OrganismModel
    ' 
    '     Properties: genome, KoFunction, organism
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateModel, EnumerateModules, GetGenbankSource, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism

''' <summary>
''' The <see cref="Pathway"/> repository
''' </summary>
''' 
<XmlType("organism", [Namespace]:="http://www.genome.jp/kegg/pathway.html")>
Public Class OrganismModel : Inherits XmlDataModel

    <XmlElement("genome")>
    Public Property organism As OrganismInfo
    ''' <summary>
    ''' 基因组是由代谢途径功能模块所构成的
    ''' </summary>
    ''' <returns></returns>
    <XmlArray("modules", [Namespace]:="http://www.genome.jp/kegg/pathway.html")>
    Public Property genome As Pathway()

    <XmlNamespaceDeclarations()>
    Public xmlns As New XmlSerializerNamespaces

    ''' <summary>
    ''' ``[geneID => KO]`` maps
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property KoFunction As Dictionary(Of String, String)
        Get
            Return genome.Select(Function(pathway As Pathway)
                                     Return pathway.genes
                                 End Function) _
                         .IteratesALL _
                         .GroupBy(Function(gene) gene.KO) _
                         .ToDictionary(Function(gene) gene.Key,
                                       Function(gene)
                                           Return gene.First.description
                                       End Function)
        End Get
    End Property

    Sub New()
        Call xmlns.Add("gcmodeller", LICENSE.GCModeller)
        ' Call xmlns.Add("kegg_pathway", "http://www.genome.jp/kegg/pathway.html")
    End Sub

    Public Overrides Function ToString() As String
        Return organism.ToString
    End Function

    ''' <summary>
    ''' Get NCBI genbank reference sequence assembly name
    ''' </summary>
    ''' <returns></returns>
    Public Function GetGenbankSource() As String
        Try
            Return organism _
                .DataSource _
                .Where(Function(d)
                           Return d.name.TextEquals("genbank") OrElse
                                  d.name.TextEquals("RefSeq")
                       End Function) _
                .First _
                .text _
                .Split("/"c) _
                .Last
        Catch ex As Exception
            Call App.LogException(ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 从KEGG的代谢途径下载文件夹加载零散的文件数据构成这个整体数据模型
    ''' </summary>
    ''' <param name="handle"></param>
    ''' <returns></returns>
    Public Shared Function CreateModel(handle As String) As OrganismModel
        If handle.FileExists AndAlso handle.ExtensionSuffix.TextEquals("xml") Then
            Return handle.LoadXml(Of OrganismModel)
        Else
            Dim organism As OrganismInfo = (handle & "/kegg.json").LoadJSON(Of OrganismInfo)
            Dim model As Pathway() = (ls - l - r - "*.Xml" <= handle) _
                .Select(AddressOf LoadXml(Of Pathway)) _
                .ToArray

            Return New OrganismModel With {
                .genome = model,
                .organism = organism
            }
        End If
    End Function

    ''' <summary>
    ''' 这个函数会自动判断数据的来源进行加载
    ''' </summary>
    ''' <param name="handle">
    ''' 可以同时兼容<see cref="OrganismModel"/>或者<see cref="Pathway"/>文件集合这两种类型的数据源
    ''' </param>
    ''' <returns></returns>
    Public Shared Function EnumerateModules(handle As String) As IEnumerable(Of Pathway)
        If handle.FileExists AndAlso handle.ExtensionSuffix.TextEquals("Xml") Then
            Return handle.LoadXml(Of OrganismModel).genome
        Else
            Return (ls - l - r - "*.Xml" <= handle) _
                .Select(AddressOf LoadXml(Of Pathway))
        End If
    End Function
End Class
