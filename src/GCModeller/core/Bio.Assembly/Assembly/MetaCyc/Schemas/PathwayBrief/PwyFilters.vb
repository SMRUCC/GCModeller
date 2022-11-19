#Region "Microsoft.VisualBasic::d14b358a2a70133bcf20e236b4e7a403, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\Schemas\PathwayBrief\PwyFilters.vb"

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

    '   Total Lines: 81
    '    Code Lines: 44
    ' Comment Lines: 30
    '   Blank Lines: 7
    '     File Size: 4.55 KB


    '     Module PwyFilters
    ' 
    '         Function: GenerateReport, Performance
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem

Namespace Assembly.MetaCyc.Schema.PathwayBrief

    ''' <summary>
    ''' 整理出代谢途径和相应的基因，对于基因个数少于5的代谢途径，其被合并至其他较大的SuperPathway之中去
    ''' </summary>
    ''' <remarks></remarks>
    Public Module PwyFilters

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
        Public Function Performance(MetaCyc As DatabaseLoadder) As Pathway()
            Dim Pathways As MetaCyc.File.DataFiles.Pathways =
                MetaCyc.GetPathways
            Dim AssignedRxnGeneLinks As Dictionary(Of String, String()) = New AssignGene(MetaCyc).Performance
            Dim GeneratePwy As System.Func(Of MetaCyc.File.DataFiles.Slots.Pathway, Pathway) =
                Function(pwyObj As MetaCyc.File.DataFiles.Slots.Pathway)
                    Dim pathway As Pathway = New Pathway With
                                             {
                                                 .Identifier = pwyObj.Identifier,
                                                 .SuperPathway = pwyObj.Types.IndexOf("Super-Pathways") > -1,
                                                 .MetaCycBaseType = pwyObj} '实例化一个返回对象
                    pathway.ReactionList = (From rxnId As String
                                            In pwyObj.ReactionList
                                            Where AssignedRxnGeneLinks.ContainsKey(rxnId)
                                            Select New NamedVector(Of String) With
                                                   {
                                                       .name = rxnId,
                                                       .vector = AssignedRxnGeneLinks(rxnId)}).ToArray     '获取反应对象列表
                    Return pathway
                End Function
            Dim Collection = (From pwy In Pathways Select GeneratePwy(pwy)).ToArray
            For i As Integer = 0 To Collection.Length - 1
                Dim pway = Collection(i)
                If pway.SuperPathway Then
                    pway.ContiansSubPathway = (From pwy In Collection Where pway.MetaCycBaseType.SubPathways.IndexOf(pwy.Identifier) > -1 Select pwy).ToArray
                End If
            Next
            Return Collection
        End Function

        Public Function GenerateReport(Pathways As Pathway(), Genes As File.DataFiles.Genes) As PathwayBrief()
            Dim LQuery = (From pwy In Pathways
                          Let item = New PathwayBrief With {
                              .EntryId = pwy.Identifier, .Description = pwy.MetaCycBaseType.CommonName,
                              .Is_Super_Pathway = pwy.SuperPathway,
                              .PathwayGenes = (From gene In Genes.Takes(pwy.AssociatedGenes) Select gene.Accession1).ToArray} Select item).ToArray
            Return LQuery
        End Function

        'Public Shared Function CreateGeneCollection(Pathways As Pathway(),
        '                                            Genes As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Genes,
        '                                            ProteinDomains As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        '    Dim List As List(Of String) = New List(Of String)

        '    For Each pwy In Pathways
        '        Dim GeneCollection = (From gene In Genes.Takes(pwy.AssociatedGenes) Select gene.Accession1).ToArray  '
        '        Call List.AddRange(GeneCollection)
        '    Next
        '    List = List.Distinct.AsList

        '    Dim File = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        '    Call File.AppendLine(New String() {"AccessionId", "Common_name", "Description", "Pfam_domains", "Sequence"})
        '    For Each Id In List
        '        Call File.AppendLine(ProteinDomains.FindAtColumn(KeyWord:=Id, Column:=0).First)
        '    Next
        '    Return File
        'End Function
    End Module
End Namespace
