#Region "Microsoft.VisualBasic::a296537eab16bcd9ad066394d77df10b, analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\PfsNET\Output\KEGGPhenotypes.vb"

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

    '     Class KEGGPhenotypes
    ' 
    '         Properties: [Class], Category, Percentage, Percentage2
    ' 
    '         Function: __assign, __weights, CalculateContributions, ExportCytoscape, PhenotypeAssociations
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
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
            Dim w As Double = data.Select(Function(x) x.weights).IteratesALL.Sum
            Dim w2 As Double = data.Select(Function(x) x.weight2).IteratesALL.Sum

            For Each phe As KEGGPhenotypes In data
                phe.Percentage = phe.weights.Sum / w * 100
                phe.Percentage2 = phe.weight2.Sum / w2 * 100
            Next

            Return data
        End Function

        Public Shared Function ExportCytoscape(data As IEnumerable(Of KEGGPhenotypes), saveDIR As String) As Boolean
            Dim Edges = (From item In data Select String.Format("""{0}"",{1},{2}", item.PhenotypePair.Split(CChar(".")).First, item.Category, 100 - item.PValue * 100)).ToArray
            Dim Nodes = {(From item In data Select String.Format("{0},Regulator", item.PhenotypePair.Split(CChar(".")).First) Distinct).ToArray,
                         (From item In data Select String.Format("""{0}"",CellPhenotype", item.Category) Distinct).ToArray}.ToVector
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
End Namespace
