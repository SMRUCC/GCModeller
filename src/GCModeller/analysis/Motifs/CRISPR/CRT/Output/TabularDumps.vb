#Region "Microsoft.VisualBasic::66f4422542f9d954a507593678bf27b8, analysis\Motifs\CRISPR\CRT\Output\TabularDumps.vb"

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

    '     Module TabularDumps
    ' 
    '         Function: __isLocatedInConserved, BatchExportCsv, BatchTrimConserved, Export, (+2 Overloads) RemoveConserved
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models

Namespace Output

    <[Namespace]("CRT.Csv_Export")>
    Public Module TabularDumps

        ''' <summary>
        ''' 将保守的区域删除
        ''' </summary>
        ''' <param name="besthit"></param>
        ''' <param name="CDSInfo"></param>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("conserved.removes")>
        Public Function RemoveConserved(besthit As SpeciesBesthit, CDSInfo As IEnumerable(Of GeneTable), data As GenomeScanResult) As GenomeScanResult
            Dim ORF = (From pro As GeneTable
                       In CDSInfo
                       Select pro
                       Group By pro.locus_id Into Group) _
                            .ToDictionary(Function(x) x.locus_id,
                                          Function(x)
                                              Return x.Group.First
                                          End Function)

            Return RemoveConserved(besthit, ORF, data)
        End Function

        Public Function RemoveConserved(besthit As SpeciesBesthit, ORF As Dictionary(Of String, GeneTable), data As GenomeScanResult) As GenomeScanResult
            Dim ConservedRegions = besthit.GetConservedRegions
            Dim LQuery = (From ls As String()
                          In ConservedRegions
                          Let pos As Integer() = (From id As String In ls Let nn = ORF(id) Select {nn.left, nn.right}).ToVector
                          Let left As Integer = pos.Min
                          Let right As Integer = pos.Max
                          Select ORFList = ls,
                          PartitioningTag = String.Join(", ", ls),
                          Loci = New Location(left, right),
                          LociLeft = left,
                          LociRight = right).ToArray
            Dim locis = LQuery.Select(Function(x) x.Loci).ToArray
            Dim unConserved = LinqAPI.Exec(Of CRISPR) <=
                From loci
                In data.Sites.AsParallel
                Where Not (__isLocatedInConserved(loci.Start, locis) OrElse
                    __isLocatedInConserved(loci.Right, locis))
                Select loci ' 选取所有不落在保守区域的位点数据

            data.Sites = unConserved

            Return data
        End Function

        Private Function __isLocatedInConserved(p As Integer, loci As Location()) As Boolean
            Dim located = LinqAPI.DefaultFirst(Of Location) <=
                From x As Location
                In loci
                Where x.IsInside(p)
                Select x

            If located Is Nothing Then ' 没有落在保守的片段区域
                Return False
            Else
                Return True
            End If
        End Function

        ''' <summary>
        ''' 请注意，这个函数的使用请务必要保证文件名除却拓展名以外都是是相同的
        ''' </summary>
        ''' <param name="scan">这个是CRT的批量扫描输出的文件夹</param>
        ''' <param name="besthit_source">这个是最佳双向比对的输出文件夹</param>
        ''' <param name="EXPORT"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("batch.trim_conserved",
                   Info:="Please make sure the filename is the same between the scan_source folder and besthit_source folder!! or the result file will not be proceeded.")>
        Public Function BatchTrimConserved(scan$, besthit_source$, CDS_info As IEnumerable(Of GeneTable), EXPORT$) As GenomeScanResult()
            Dim ScanningResults = (From path
                                   In scan.LoadSourceEntryList("*.xml")
                                   Let CRISPR = path.Value.LoadXml(Of GenomeScanResult)()
                                   Select path,
                                       CRISPR).ToDictionary(Function(x) x.path.Key)
            Dim BesthitsResults = (From path
                                   In besthit_source.LoadSourceEntryList("*.xml")
                                   Let bh = path.Value.LoadXml(Of SpeciesBesthit)()
                                   Select path,
                                       besthit = bh).ToDictionary(Function(x) x.path.Key)
            Dim ORF = (From g As GeneTable
                       In CDS_info
                       Select g
                       Group By g.locus_id Into Group) _
                             .ToDictionary(Function(g) g.locus_id,
                                           Function(g)
                                               Return g.Group.First
                                           End Function)
            Dim GroupResult = From x
                              In ScanningResults
                              Where BesthitsResults.ContainsKey(x.Key)
                              Select CRISPR = x.Value.CRISPR,
                                  Besthit = BesthitsResults(x.Key).besthit,
                                  Entry = x.Value.path
            Dim LQuery = (From x
                          In GroupResult
                          Select x.Entry,
                              CRISPR = RemoveConserved(x.Besthit, ORF, x.CRISPR)).ToArray

            Call (From g
                  In LQuery
                  Let xml = EXPORT & "/" & g.Entry.Key & ".xml"
                  Let doc = g.CRISPR.GetXml
                  Select doc.SaveTo(xml)).ToArray

            Return LQuery.Select(Function(x) x.CRISPR).ToArray
        End Function

        <ExportAPI("batch.export_csv")>
        Public Function BatchExportCsv(source As IEnumerable(Of GenomeScanResult), EXPORT$) As Boolean
            Dim LQuery = From g
                         In source.AsParallel
                         Select CSV = TabularDumps.Export(g.Sites),
                             g
            Dim save = LinqAPI.Exec(Of Boolean) <=
                From x
                In LQuery.AsParallel
                Let path = EXPORT & "/" & x.g.Tag & ".csv"
                Select x.CSV.Save(path, False)

            Return Not save.IsNullOrEmpty
        End Function

        <ExportAPI("export.csv")>
        Public Function Export(dat As IEnumerable(Of SearchingModel.CRISPR)) As IO.File
            Dim out As New IO.File

            out += {"Position", "Repeats", "Spacer Sequence", "Repeat Length", "Spacer Length"}
            out += {""}

            If Not dat.Any Then
                Return out
            End If

            For Each i As SeqValue(Of SearchingModel.CRISPR) In dat.SeqIterator
                Dim CRISPR As SearchingModel.CRISPR = i

                out += {"------> CRISPR " & i.i}

                For j As Integer = 0 To CRISPR.NumberOfRepeats - 1
                    Dim row As New RowObject

                    Call row.Add(CRISPR.RepeatAt(j) + 1)
                    Call row.Add(CRISPR.RepeatStringAt(j))

                    If j < CRISPR.NumberOfSpacers Then
                        Dim Spacer As String = CRISPR.SpacerStringAt(j)

                        Call row.Add(Spacer)
                        Call row.Add(CRISPR.RepeatStringAt(j).Length)
                        Call row.Add(CRISPR.SpacerStringAt(j).Length)
                    End If

                    out += row
                Next

                out += {""}
            Next

            Return out
        End Function
    End Module
End Namespace
