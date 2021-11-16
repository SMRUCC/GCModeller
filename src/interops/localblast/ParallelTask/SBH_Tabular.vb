#Region "Microsoft.VisualBasic::30ccabd1b2233a27ec9a531e66bec680, localblast\ParallelTask\SBH_Tabular.vb"

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

    '     Module SBH_Tabular
    ' 
    '         Function: CreateFromBBHOrthologous
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast

Namespace NCBIBlastResult

    <Package("SBH_Tabular",
                        Publisher:="xie.guigang@gmail.com",
                        Category:=APICategories.UtilityTools)>
    Public Module SBH_Tabular

        ''' <summary>
        ''' BBH的文件夹
        ''' </summary>
        ''' <param name="QueryID"></param>
        ''' <param name="sbhDIR">Directory path which it contains the sbh result data.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Alignment.Table.From.bbh.Orthologous")>
        Public Function CreateFromBBHOrthologous(QueryID As String, sbhDIR As String, <Parameter("Query.Info")> queryInfo As IEnumerable(Of GeneTable)) As AlignmentTable
            Dim Entries = (From path As KeyValuePair(Of String, String)
                           In sbhDIR.LoadSourceEntryList({"*.csv"})
                           Let Log As AlignEntry = LogNameParser(path.Value)
                           Select ID = path.Key,
                               LogEntry = Log,
                               besthitData = path.Value.LoadCsv(Of BestHit)(False).ToArray).ToArray
            Dim querySide = From entry In Entries
                            Where String.Equals(entry.LogEntry.QueryName, QueryID, StringComparison.OrdinalIgnoreCase)
                            Select entry '得到Query的比对方向的数据
            Dim hitSide = (From entry In Entries
                           Where String.Equals(entry.LogEntry.HitName, QueryID, StringComparison.OrdinalIgnoreCase)
                           Select entry).ToArray
            Dim BBH = (From query
                       In querySide.AsParallel
                       Let subject = query.LogEntry.SelectEquals(hitSide, Function(Entry) Entry.LogEntry)
                       Let bbhData As BiDirectionalBesthit() =
                           (LinqAPI.Exec(Of BiDirectionalBesthit) <= From __bbh As BiDirectionalBesthit
                                                                     In BBHParser.GetBBHTop(
                                                                         query.besthitData,
                                                                         subject.besthitData)
                                                                     Where __bbh.isMatched
                                                                     Select __bbh)
                       Select query.ID,
                           query.LogEntry,
                           bbhData).ToArray

            Dim queryDict As Dictionary(Of GeneTable) = queryInfo.ToDictionary
            Dim hits As HitRecord() =
                LinqAPI.Exec(Of HitRecord) <= From genome
                                              In BBH
                                              Select From gene As BiDirectionalBesthit
                                                     In genome.bbhData
                                                     Let QueryGene As GeneTable = queryDict(gene.QueryName)
                                                     Select New HitRecord With {
                                                         .Identity = gene.Identities,
                                                         .QueryStart = QueryGene.left,
                                                         .QueryEnd = QueryGene.right,
                                                         .SubjectIDs = genome.LogEntry.HitName
                                                     }
            Dim Table As New AlignmentTable With {
                .Database = sbhDIR,
                .Hits = hits.ToArray,
                .Program = "BBH",
                .Query = QueryID,
                .RID = Now.ToString
            }
            Return Table
        End Function
    End Module
End Namespace
