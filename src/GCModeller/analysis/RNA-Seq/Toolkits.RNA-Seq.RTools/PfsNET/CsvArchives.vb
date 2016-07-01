#Region "Microsoft.VisualBasic::29863c6e4ff789940b256f7ba9b4931e, ..\GCModeller\analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\PfsNET\CsvArchives.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic
Imports System.Text
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.AnalysisTools.CellularNetwork.PFSNet
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly

Namespace PfsNET.TabularArchives

    Public Class KEGGPhenotypes : Inherits SubNETCsvObject
        Public Property [Class] As String
        Public Property Category As String

        ''' <summary>
        ''' Sigma(<see cref="KEGGPhenotypes.weight2"></see>)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("Contribution(%)")> Public Property Percentage As Double
        <Column("Contribution2(%)")> Public Property Percentage2 As Double

        Public Shared Function PhenotypeAssociations(Result As SubNETCsvObject(), KEGGPathways As KEGG.Archives.Csv.Pathway()) As KEGGPhenotypes()
            Dim Dict As Dictionary(Of String, KEGG.Archives.Csv.Pathway) =
                KEGGPathways.ToDictionary(Of String)(Function(pathway) pathway.EntryId)

            Dim LQuery = (From item As SubNETCsvObject In Result
                          Let Id As String = item.UniqueId
                          Let Pathway As KEGG.Archives.Csv.Pathway = Dict(Id)
                          Select __assign(item, Pathway)).ToArray
            Return LQuery
        End Function

        Private Shared Function __assign(net As SubNETCsvObject, pathway As KEGG.Archives.Csv.Pathway) As KEGGPhenotypes
            Dim phen As KEGGPhenotypes = net.Copy(Of KEGGPhenotypes)()
            Dim [Class] As String = pathway.Class
            Dim Category As String = pathway.Category
            phen.Class = [Class]
            phen.Category = Category
            Return phen
        End Function

        Public Shared Function CalculateContributions(data As IEnumerable(Of KEGGPhenotypes)) As KEGGPhenotypes()
            Dim LQuery = (From item In data Select item Group item By item.Category Into Group).ToArray
            Dim ChunkBuffer = (From item In LQuery Select CalculationWeights(item.Group.ToArray)).ToArray.MatrixToVector     '计算贡献率权重比
            Return ChunkBuffer
        End Function

        ''' <summary>
        ''' 计算同一种分类的
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function CalculationWeights(data As KEGGPhenotypes()) As KEGGPhenotypes()
            Dim w As Double = (From item In data Select item.weights).ToArray.MatrixToVector.Sum
            Dim w2 As Double = (From item In data Select item.weight2).ToArray.MatrixToVector.Sum

            For Each item In data
                item.Percentage = item.weights.Sum / w * 100
                item.Percentage2 = item.weight2.Sum / w2 * 100
            Next

            Return data
        End Function

        Public Shared Function ExportCytoscape(data As IEnumerable(Of KEGGPhenotypes), saveDIR As String) As Boolean
            Dim Edges = (From item In data Select String.Format("""{0}"",{1},{2}", item.PhenotypePair.Split(CChar(".")).First, item.Category, 100 - item.PValue * 100)).ToArray
            Dim Nodes = {(From item In data Select String.Format("{0},Regulator", item.PhenotypePair.Split(CChar(".")).First) Distinct).ToArray,
                         (From item In data Select String.Format("""{0}"",CellPhenotype", item.Category) Distinct).ToArray}.MatrixToVector
            Dim Csv As StringBuilder = New StringBuilder(1024)
            Call Csv.AppendLine("fromNode,toNode,weight")
            For Each Line In Edges
                Call Csv.AppendLine(Line)
            Next

            Call Csv.ToString.SaveTo(saveDIR & "/Edges.csv")
            Call Csv.Clear()

            Call Csv.AppendLine("UniqueId,NodeType")
            For Each Line In Nodes
                Call Csv.AppendLine(Line)
            Next
            Call Csv.ToString.SaveTo(saveDIR & "/Nodes.csv")

            Return True
        End Function
    End Class

    Public Class KEGGPhenotypeDenormalizeData

        Public Property [Class] As String
        Public Property Category As String
        ''' <summary>
        ''' 使用结果文件名来表示
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PhenotypePair As String
        Public Property UniqueId As String
        Public Property PathwayDescription As String
        Public Property Statistics As Double
        Public Property PValue As Double
        Public Property GeneId As String
        Public Property GeneFunction As String

        Public Shared Function Denormalize(phenotypeData As Generic.IEnumerable(Of KEGGPhenotypes), PttDir As String) As KEGGPhenotypeDenormalizeData()
            Dim ChunkBuffer As List(Of KEGGPhenotypeDenormalizeData) = New List(Of KEGGPhenotypeDenormalizeData)
            Dim PTT As New PTTDbLoader(PttDir)

            For Each phen As KEGGPhenotypes In phenotypeData
                Dim funcs As KEGGPhenotypeDenormalizeData() = (From locus As String In phen.SignificantGeneObjects
                                                               Let gFuncs As KEGGPhenotypeDenormalizeData = Denormalize(PTT, locus, phen)
                                                               Select gFuncs).ToArray
                Call ChunkBuffer.AddRange(funcs)
            Next

            Return ChunkBuffer.ToArray
        End Function

        Private Shared Function Denormalize(PTT As PTTDbLoader, locus As String, phen As KEGGPhenotypes) As KEGGPhenotypeDenormalizeData
            Dim dnData As KEGGPhenotypeDenormalizeData = New KEGGPhenotypeDenormalizeData
            Dim PttGene = PTT(locus)

            dnData.GeneId = locus
            dnData.GeneFunction = PttGene.Product
            dnData.Category = phen.Category
            dnData.Class = phen.Class
            dnData.PathwayDescription = phen.Description
            dnData.PhenotypePair = phen.PhenotypePair
            dnData.PValue = phen.PValue
            dnData.Statistics = phen.Statistics
            dnData.UniqueId = phen.UniqueId

            Return dnData
        End Function
    End Class

    ''' <summary>
    ''' 从pfsnet的R输出之中直接解析出来的pfsnet的计算结果
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SubNETCsvObject

        ''' <summary>
        ''' 使用结果文件名来表示
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PhenotypePair As String
        Public Property UniqueId As String
        Public Property Description As String
        Public Property n As Integer
        <Column("flag")> Public Property Flag As Boolean
        Public Property Statistics As Double
        Public Property PValue As Double
        Public Property SignificantGeneObjects As String()

        Public Property weights As Double()
        Public Property weight2 As Double()
        Public Property SubNET_Vector As Double()
        '   Public Property Vectors As String()

        Public Overrides Function ToString() As String
            Return String.Format("{0} --> {1}", UniqueId, PhenotypePair)
        End Function

        Protected Friend Function Copy(Of T As SubNETCsvObject)() As T
            Dim Copied As T = Activator.CreateInstance(Of T)()
            Copied.Description = Description
            Copied.Flag = Flag
            Copied.n = n
            Copied.PhenotypePair = PhenotypePair
            Copied.PValue = PValue
            Copied.SignificantGeneObjects = SignificantGeneObjects
            Copied.Statistics = Statistics
            Copied.SubNET_Vector = SubNET_Vector
            Copied.UniqueId = UniqueId
            Copied.weight2 = weight2
            Copied.weights = weights

            Return Copied
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PfsNETResult">结果文件的XML文件名</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateObject(PfsNETResult As String, PathwayBrief As Dictionary(Of String, PathwayBrief)) As SubNETCsvObject()
            Dim ResultSet = PfsNETResult.LoadXml(Of PfsNET())()
            Dim PhenotypeName As String = PfsNETResult.Replace("\", "/").Split(CChar("/")).Last.ToLower.Replace(".xml", "")
            Dim LQuery = (From ElementItem In ResultSet Select CreateObject(ElementItem, PhenotypeName, PathwayBrief)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ResultSet">集合之中的数据必须都是同一个表型类别之下的</param>
        ''' <param name="PhenotypeName"></param>
        ''' <param name="PathwayBrief"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateObject(ResultSet As DataStructure.PFSNetGraph(),
                                            PhenotypeName As String,
                                            PathwayBrief As Dictionary(Of String, PathwayBrief),
                                            [Class] As String) As SubNETCsvObject()

            Dim Chunk = (From graphItem As SMRUCC.genomics.AnalysisTools.CellularNetwork.PFSNet.DataStructure.PFSNetGraph In ResultSet
                         Let subNET = New NetDetails With
                                      {
                                          .Nodes = (From node In graphItem.Nodes Select node.Name).ToArray,
                                          .weight = New Vector With {.Elements = (From node In graphItem.Nodes Select node.weight).ToArray},
                                          .weight2 = New Vector With {.Elements = (From node In graphItem.Nodes Select node.weight2).ToArray},
                                          .Pvalue = graphItem.pvalue,
                                          .statistics = graphItem.statistics}
                         Select New PfsNET With
                                {
                                    .Class = [Class], .Flag = True,
                                    .n = graphItem.Length,
                                    .Identifier = graphItem.Id,
                                    .SubNET = subNET}).ToArray
            Dim LQuery = (From ElementItem In Chunk Select CreateObject(ElementItem, PhenotypeName, PathwayBrief)).ToArray
            Return LQuery
        End Function

        Public Shared Function CreateObject(ResultSet As PfsNET(),
                                            PhenotypeName As String,
                                            PathwayBrief As Dictionary(Of String, PathwayBrief)) As SubNETCsvObject()

            Dim LQuery = (From ElementItem In ResultSet Select CreateObject(ElementItem, PhenotypeName, PathwayBrief)).ToArray
            Return LQuery
        End Function

        Private Shared Function CreateObject(XmlElement As PfsNET,
                                             PhenotypeName As String,
                                             PathwayBrief As Dictionary(Of String, PathwayBrief)) As SubNETCsvObject

            Dim CsvObject As SubNETCsvObject = New SubNETCsvObject With {.PhenotypePair = PhenotypeName}
            CsvObject.Flag = XmlElement.Flag
            CsvObject.n = XmlElement.n
            CsvObject.PValue = XmlElement.SubNET.Pvalue
            CsvObject.SignificantGeneObjects = XmlElement.SubNET.Nodes
            CsvObject.Statistics = XmlElement.SubNET.statistics
            CsvObject.SubNET_Vector = XmlElement.SubNET.Vector.Elements
            CsvObject.UniqueId = XmlElement.Identifier
            CsvObject.weight2 = XmlElement.SubNET.weight2.Elements
            CsvObject.weights = XmlElement.SubNET.weight.Elements
            ' CsvObject.Vectors = (From Vector In XmlElement.Vectors Select String.Join(", ", Vector.Elements)).ToArray
            CsvObject.Description = PathwayBrief(CsvObject.UniqueId).Description

            Return CsvObject
        End Function
    End Class
End Namespace
