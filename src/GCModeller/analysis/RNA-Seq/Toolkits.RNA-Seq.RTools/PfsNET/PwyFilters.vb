Imports System.Text
Imports Microsoft.VisualBasic.DataVisualization.DocumentFormat.Csv.Reflection

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
                                             .UniqueId = pwyObj.UniqueId,
                                             .SuperPathway = pwyObj.Types.IndexOf("Super-Pathways") > -1,
                                             .MetaCycBaseType = pwyObj} '实例化一个返回对象
                pathway.ReactionList = (From rxnId As String
                                        In pwyObj.ReactionList
                                        Where AssignedRxnGeneLinks.ContainsKey(rxnId)
                                        Select New LANS.SystemsBiology.Assembly.ComponentModel.Key_strArrayValuePair With
                                               {
                                                   .Key = rxnId,
                                                   .Value = AssignedRxnGeneLinks(rxnId)}).ToArray     '获取反应对象列表
                Return pathway
            End Function
        Dim Collection = (From pwy In Pathways Select GeneratePwy(pwy)).ToArray
        For i As Integer = 0 To Collection.Count - 1
            Dim pway = Collection(i)
            If pway.SuperPathway Then
                pway.ContiansSubPathway = (From pwy In Collection Where pway.MetaCycBaseType.SubPathways.IndexOf(pwy.UniqueId) > -1 Select pwy).ToArray
            End If
        Next
        Return Collection
    End Function

    <System.Xml.Serialization.XmlType("pwy")> Public Class Pathway : Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.IAccessionIdEnumerable
        Public Property MetaCycBaseType As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Pathway
        Public Property UniqueId As String Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.IAccessionIdEnumerable.UniqueId
        ''' <summary> 
        ''' 本代谢途径是否为一个超途径
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SuperPathway As Boolean
        Public Property ReactionList As LANS.SystemsBiology.Assembly.ComponentModel.Key_strArrayValuePair()
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
                Return String.Format("{0}, {1} reactions and {2} sub-pathways", UniqueId, ReactionList.Count, ContiansSubPathway.Count)
            Else
                Return String.Format("{0}, {1} reactions.", UniqueId, ReactionList.Count)
            End If
        End Function
    End Class

    Public Shared Function GenerateReport(Pathways As Pathway(), Genes As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Genes) As PathwayBrief()
        Dim LQuery = (From pwy In Pathways
                      Let item = New PathwayBrief With {
                          .UniqueId = pwy.UniqueId, .Description = pwy.MetaCycBaseType.CommonName,
                          .Is_Super_Pathway = pwy.SuperPathway,
                          .Associated_gene_list = (From gene In Genes.Takes(pwy.AssociatedGenes) Select gene.Accession1).ToArray} Select item).ToArray
        Return LQuery
    End Function

    Public Class PathwayBrief
        : Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.IAccessionIdEnumerable

        Public Property UniqueId As String Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.IAccessionIdEnumerable.UniqueId
        Public Property Description As String
        <Column("Is_Super_Pathway?")> Public Property Is_Super_Pathway As Boolean
        Public Property Associated_gene_counts As Integer
            Get
                If Associated_gene_list.IsNullOrEmpty Then
                    Return 0
                End If
                Return Associated_gene_list.Count
            End Get
            Set(value As Integer)
                'DO_NOTHING
            End Set
        End Property

        Public Property Associated_gene_list As String()
    End Class

    Public Shared Function CreateGeneCollection(Pathways As Pathway(),
                                                Genes As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Genes,
                                                ProteinDomains As Microsoft.VisualBasic.DataVisualization.DocumentFormat.Csv.File) As Microsoft.VisualBasic.DataVisualization.DocumentFormat.Csv.File
        Dim List As List(Of String) = New List(Of String)

        For Each pwy In Pathways
            Dim GeneCollection = (From gene In Genes.Takes(pwy.AssociatedGenes) Select gene.Accession1).ToArray  '
            Call List.AddRange(GeneCollection)
        Next
        List = List.Distinct.ToList

        Dim File = New Microsoft.VisualBasic.DataVisualization.DocumentFormat.Csv.File
        Call File.AppendLine(New String() {"AccessionId", "Common_name", "Description", "Pfam_domains", "Sequence"})
        For Each Id In List
            Call File.AppendLine(ProteinDomains.FindAtColumn(KeyWord:=Id, Column:=0).First)
        Next
        Return File
    End Function
End Class
