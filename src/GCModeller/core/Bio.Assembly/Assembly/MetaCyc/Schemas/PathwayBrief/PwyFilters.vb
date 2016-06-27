Imports System.Text
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic

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
                                            Select New Key_strArrayValuePair With
                                                   {
                                                       .Key = rxnId,
                                                       .Value = AssignedRxnGeneLinks(rxnId)}).ToArray     '获取反应对象列表
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
        '                                            Genes As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Genes,
        '                                            ProteinDomains As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        '    Dim List As List(Of String) = New List(Of String)

        '    For Each pwy In Pathways
        '        Dim GeneCollection = (From gene In Genes.Takes(pwy.AssociatedGenes) Select gene.Accession1).ToArray  '
        '        Call List.AddRange(GeneCollection)
        '    Next
        '    List = List.Distinct.ToList

        '    Dim File = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        '    Call File.AppendLine(New String() {"AccessionId", "Common_name", "Description", "Pfam_domains", "Sequence"})
        '    For Each Id In List
        '        Call File.AppendLine(ProteinDomains.FindAtColumn(KeyWord:=Id, Column:=0).First)
        '    Next
        '    Return File
        'End Function
    End Module
End Namespace