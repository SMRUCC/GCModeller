#Region "Microsoft.VisualBasic::706158ae30b5db318065fb8d0545516a, CLI_tools\c2\Workflows\RegulationNetwork\PwyFilters.vb"

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

    ' Class PwyFilters
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateGeneCollection, GenerateReport, Performance
    '     Class Pathway
    ' 
    '         Properties: AssociatedGenes, ContiansSubPathway, Identifier, MetaCycBaseType, ReactionList
    '                     SuperPathway
    ' 
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

''' <summary>
''' 整理出代谢途径和相应的基因，对于基因个数少于5的代谢途径，其被合并至其他较大的SuperPathway之中去
''' </summary>
''' <remarks></remarks>
Public Class PwyFilters

    Dim MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder

    Sub New(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder)
        Me.MetaCyc = MetaCyc
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 过程描述：
    ''' 1. 获取所有的代谢途径的数据
    ''' 2. 构建所有的反应对象与基因之间的相互联系
    ''' 3. 根据Reaction-List属性值列表将基因与相应的代谢途径建立联系，最后输出数据
    ''' </remarks>
    Public Function Performance() As Pathway()
        Dim Pathways As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Pathways =
            MetaCyc.GetPathways
        Dim AssignedRxnGeneLinks As Dictionary(Of String, String()) = New AssignGene(MetaCyc).Performance
        Dim GeneratePwy As System.Func(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Pathway, Pathway) =
            Function(pwyObj As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Pathway)
                Dim pathway As Pathway = New Pathway With
                                         {
                                             .Identifier = pwyObj.Identifier,
                                             .SuperPathway = pwyObj.Types.IndexOf("Super-Pathways") > -1,
                                             .MetaCycBaseType = pwyObj} '实例化一个返回对象
                pathway.ReactionList = (From rxnId As String
                                        In pwyObj.ReactionList
                                        Where AssignedRxnGeneLinks.ContainsKey(rxnId)
                                        Select New LANS.SystemsBiology.ComponentModel.Key_strArrayValuePair With
                                               {
                                                   .Key = rxnId,
                                                   .Value = AssignedRxnGeneLinks(rxnId)}).ToArray     '获取反应对象列表
                Return pathway
            End Function
        Dim Collection = (From pwy In Pathways Select GeneratePwy(pwy)).ToArray
        For i As Integer = 0 To Collection.Count - 1
            Dim pway = Collection(i)
            If pway.SuperPathway Then
                pway.ContiansSubPathway = (From pwy In Collection Where pway.MetaCycBaseType.SubPathways.IndexOf(pwy.Identifier) > -1 Select pwy).ToArray
            End If
        Next
        Return Collection
    End Function

    <System.Xml.Serialization.XmlType("pwy")> Public Class Pathway : Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.IDEnumerable
        Public Property MetaCycBaseType As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Pathway
        Public Property Identifier As String Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.IDEnumerable.Identifier
        ''' <summary> 
        ''' 本代谢途径是否为一个超途径
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SuperPathway As Boolean
        Public Property ReactionList As LANS.SystemsBiology.ComponentModel.Key_strArrayValuePair()
        ''' <summary>
        ''' 本代谢途径所包含的的亚途径
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ContiansSubPathway As Pathway()

        Public ReadOnly Property AssociatedGenes As String()
            Get
                Dim List As List(Of String) = New List(Of String)
                For Each rxn In ReactionList
                    Call List.AddRange(rxn.Value)
                Next
                If SuperPathway Then
                    For Each pwy In ContiansSubPathway
                        Call List.AddRange(pwy.AssociatedGenes)
                    Next
                End If
                Return (From s As String In List Select s Distinct Order By s Ascending).ToArray
            End Get
        End Property

        Public Overrides Function ToString() As String
            If SuperPathway Then
                Return String.Format("{0}, {1} reactions and {2} sub-pathways", Identifier, ReactionList.Count, ContiansSubPathway.Count)
            Else
                Return String.Format("{0}, {1} reactions.", Identifier, ReactionList.Count)
            End If
        End Function
    End Class

    Public Shared Function GenerateReport(Pathways As Pathway(), Genes As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Genes) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        Dim File As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        File.AppendLine(New String() {"UniqueId", "Description", "Is_Super_Pathway?", "Associated_gene_counts", "Associated_gene_list"})
        For Each pwy In Pathways
            Dim row As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject = New DocumentFormat.Csv.DocumentStream.RowObject
            Call row.AddRange(New String() {pwy.Identifier, pwy.MetaCycBaseType.CommonName, pwy.SuperPathway})
            Dim GeneCollection = (From gene In Genes.Takes(pwy.AssociatedGenes) Select gene.Accession1).ToArray  '
            Call row.Add(GeneCollection.Count)
            Call sBuilder.Clear()
            For Each Id As String In GeneCollection
                Call sBuilder.Append(Id & "; ")
            Next
            Call row.Add(sBuilder.ToString)
            Call File.AppendLine(row)
        Next

        Return File
    End Function

    Public Shared Function CreateGeneCollection(Pathways As Pathway(),
                                                Genes As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Genes,
                                                ProteinDomains As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        Dim List As List(Of String) = New List(Of String)

        For Each pwy In Pathways
            Dim GeneCollection = (From gene In Genes.Takes(pwy.AssociatedGenes) Select gene.Accession1).ToArray  '
            Call List.AddRange(GeneCollection)
        Next
        List = List.Distinct.ToList

        Dim File = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        Call File.AppendLine(New String() {"AccessionId", "Common_name", "Description", "Pfam_domains", "Sequence"})
        For Each Id In List
            Call File.AppendLine(ProteinDomains.FindAtColumn(KeyWord:=Id, Column:=0).First)
        Next
        Return File
    End Function
End Class
