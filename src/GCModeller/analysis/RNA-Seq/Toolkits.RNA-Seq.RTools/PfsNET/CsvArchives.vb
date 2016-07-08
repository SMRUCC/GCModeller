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

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.PFSNet
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel

Namespace PfsNET.TabularArchives

    Public Class KEGGPhenotypes : Inherits SubNetTable
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

        Public Shared Function PhenotypeAssociations(Result As SubNetTable(), KEGGPathways As KEGG.Archives.Csv.Pathway()) As KEGGPhenotypes()
            Dim Dict As Dictionary(Of String, KEGG.Archives.Csv.Pathway) =
                KEGGPathways.ToDictionary(Of String)(Function(pathway) pathway.EntryId)

            Dim LQuery As KEGGPhenotypes() =
                LinqAPI.Exec(Of KEGGPhenotypes) <= From subNet As SubNetTable
                                                   In Result
                                                   Let Id As String = subNet.UniqueId
                                                   Let Pathway As KEGG.Archives.Csv.Pathway = Dict(Id)
                                                   Select __assign(subNet, Pathway)
            Return LQuery
        End Function

        Private Shared Function __assign(net As SubNetTable, pathway As KEGG.Archives.Csv.Pathway) As KEGGPhenotypes
            Dim phen As KEGGPhenotypes = net.Copy(Of KEGGPhenotypes)()
            Dim [Class] As String = pathway.Class
            Dim Category As String = pathway.Category
            phen.Class = [Class]
            phen.Category = Category
            Return phen
        End Function

        Public Shared Function CalculateContributions(data As IEnumerable(Of KEGGPhenotypes)) As KEGGPhenotypes()
            Dim LGroup = From phe As KEGGPhenotypes
                         In data
                         Select phe
                         Group phe By phe.Category Into Group
            Dim out As KEGGPhenotypes() = LinqAPI.Exec(Of KEGGPhenotypes) <=
 _
                From g
                In LGroup
                Select __weights(g.Group.ToArray)  ' 计算贡献率权重比

            Return out
        End Function

        ''' <summary>
        ''' 计算同一种分类的
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function __weights(data As KEGGPhenotypes()) As KEGGPhenotypes()
            Dim w As Double = data.Select(Function(x) x.weights).MatrixAsIterator.Sum
            Dim w2 As Double = data.Select(Function(x) x.weight2).MatrixAsIterator.Sum

            For Each phe As KEGGPhenotypes In data
                phe.Percentage = phe.weights.Sum / w * 100
                phe.Percentage2 = phe.weight2.Sum / w2 * 100
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

        Public Shared Function Denormalize(phenotypeData As IEnumerable(Of KEGGPhenotypes), PTT_DIR As String) As KEGGPhenotypeDenormalizeData()
            Dim bufs As New List(Of KEGGPhenotypeDenormalizeData)
            Dim PTT As New PTTDbLoader(PTT_DIR)

            For Each phen As KEGGPhenotypes In phenotypeData
                Dim funcs As KEGGPhenotypeDenormalizeData() =
                    LinqAPI.Exec(Of KEGGPhenotypeDenormalizeData) <=
 _
                        From locus As String
                        In phen.SignificantGeneObjects
                        Let gFuncs As KEGGPhenotypeDenormalizeData = Denormalize(PTT, locus, phen)
                        Select gFuncs

                bufs += funcs
            Next

            Return bufs.ToArray
        End Function

        Private Shared Function Denormalize(PTT As PTTDbLoader, locus As String, phen As KEGGPhenotypes) As KEGGPhenotypeDenormalizeData
            Dim dnData As New KEGGPhenotypeDenormalizeData
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
    Public Class SubNetTable

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

        Public Overrides Function ToString() As String
            Return String.Format("{0} --> {1}", UniqueId, PhenotypePair)
        End Function

        Protected Friend Function Copy(Of T As SubNetTable)() As T
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
        Public Shared Function CreateObject(PfsNETResult As String, PathwayBrief As Dictionary(Of String, PathwayBrief)) As SubNetTable()
            Dim ResultSet = PfsNETResult.LoadXml(Of PfsNET())()
            Dim PhenotypeName As String = PfsNETResult.BaseName
            Dim LQuery As SubNetTable() =
                LinqAPI.Exec(Of SubNetTable) <= From x As PfsNET
                                                In ResultSet
                                                Select __creates(x, PhenotypeName, PathwayBrief)
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
                                            [Class] As String) As SubNetTable()

            Dim result = From graph As DataStructure.PFSNetGraph
                         In ResultSet
                         Let subNET = New NetDetails With {
                             .Nodes = (From node In graph.Nodes Select node.Name).ToArray,
                             .weight = New Vector With {
                                .x = (From node In graph.Nodes Select node.weight).ToArray
                             },
                             .weight2 = New Vector With {
                                .x = (From node In graph.Nodes Select node.weight2).ToArray
                             },
                             .Pvalue = graph.pvalue,
                             .statistics = graph.statistics
                         }
                         Select New PfsNET With {
                             .Class = [Class],
                             .Flag = True,
                             .n = graph.Length,
                             .Identifier = graph.Id,
                             .SubNET = subNET
                         }
            Dim LQuery As SubNetTable() =
                LinqAPI.Exec(Of SubNetTable) <= From x As PfsNET
                                                In result
                                                Select __creates(x, PhenotypeName, PathwayBrief)
            Return LQuery
        End Function

        Public Shared Function CreateObject(ResultSet As PfsNET(),
                                            PhenotypeName As String,
                                            PathwayBrief As Dictionary(Of String, PathwayBrief)) As SubNetTable()

            Dim LQuery As SubNetTable() =
                LinqAPI.Exec(Of SubNetTable) <= From x As PfsNET
                                                In ResultSet
                                                Select __creates(x, PhenotypeName, PathwayBrief)
            Return LQuery
        End Function

        Private Shared Function __creates(net As PfsNET,
                                          PhenotypeName As String,
                                          PathwayBrief As Dictionary(Of String, PathwayBrief)) As SubNetTable

            Dim tbl As New SubNetTable With {
                .PhenotypePair = PhenotypeName
            }
            tbl.Flag = net.Flag
            tbl.n = net.n
            tbl.PValue = net.SubNET.Pvalue
            tbl.SignificantGeneObjects = net.SubNET.Nodes
            tbl.Statistics = net.SubNET.statistics
            tbl.SubNET_Vector = net.SubNET.Vector.x
            tbl.UniqueId = net.Identifier
            tbl.weight2 = net.SubNET.weight2.x
            tbl.weights = net.SubNET.weight.x
            tbl.Description = PathwayBrief(tbl.UniqueId).Description

            Return tbl
        End Function
    End Class
End Namespace
