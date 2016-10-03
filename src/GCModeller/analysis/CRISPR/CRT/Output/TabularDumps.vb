#Region "Microsoft.VisualBasic::0bd3cd02afe6ae198735241fe77d2c36, ..\GCModeller\analysis\CRISPR\CRT\Output\TabularDumps.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Analysis
Imports SMRUCC.genomics.SequenceModel

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
        Public Function RemoveConserved(besthit As BestHit, CDSInfo As IEnumerable(Of GeneDumpInfo), data As GenomeScanResult) As GenomeScanResult
            Dim ORF = (From pro As GeneDumpInfo
                   In CDSInfo
                       Select pro
                       Group By pro.LocusID Into Group).ToDictionary(Function(item) item.LocusID,
                                                                 Function(item) item.Group.First)
            Return RemoveConserved(besthit, ORF, data)
        End Function

        Public Function RemoveConserved(besthit As BestHit, ORF As Dictionary(Of String, GeneDumpInfo), data As GenomeScanResult) As GenomeScanResult
            Dim ConservedRegions = besthit.GetConservedRegions
            Dim LQuery = (From ls As String() In ConservedRegions
                          Let pos As Integer() = (From id As String In ls Let nn = ORF(id) Select {nn.Left, nn.Right}).ToArray.MatrixToVector
                          Let left As Integer = pos.Min
                          Let right As Integer = pos.Max
                          Select ORFList = ls,
                          PartitioningTag = String.Join(", ", ls),
                          Loci = New Location(left, right),
                          LociLeft = left,
                          LociRight = right).ToArray
            Dim locis = LQuery.ToArray(Function(x) x.Loci)
            Dim UnConserved = (From loci In data.Sites.AsParallel
                               Where Not (__isLocatedInConserved(loci.Start, locis) OrElse
                               __isLocatedInConserved(loci.Right, locis))
                               Select loci).ToArray '选取所有不落在保守区域的位点数据
            data.Sites = UnConserved
            Return data
        End Function

        Private Function __isLocatedInConserved(p As Integer, loci As Location()) As Boolean
            Dim LLLLQuery = (From item In loci Where item.ContainSite(p) Select 1).ToArray
            If LLLLQuery.IsNullOrEmpty Then
                '没有落在保守的片段区域
                Return False
            Else
                Return True
            End If
        End Function

        ''' <summary>
        ''' 请注意，这个函数的使用请务必要保证文件名除却拓展名以外都是是相同的
        ''' </summary>
        ''' <param name="scan_source">这个是CRT的批量扫描输出的文件夹</param>
        ''' <param name="besthit_source">这个是最佳双向比对的输出文件夹</param>
        ''' <param name="export"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("batch.trim_conserved", Info:="Please make sure the filename is the same between the scan_source folder and besthit_source folder!! or the result file will not be proceeded.")>
        Public Function BatchTrimConserved(scan_source As String, besthit_source As String, Cds_info As IEnumerable(Of GeneDumpInfo), export As String) As GenomeScanResult()
            Dim ScanningResults = (From path In scan_source.LoadSourceEntryList("*.xml")
                                   Select path, CRISPR = path.Value.LoadXml(Of GenomeScanResult)()).ToArray.ToDictionary(Function(item) item.path.Key)
            Dim BesthitsResults = (From path In besthit_source.LoadSourceEntryList("*.xml")
                                   Select path, Besthit = path.Value.LoadXml(Of BestHit)()).ToArray.ToDictionary(Function(item) item.path.Key)
            Dim ORF = (From item In Cds_info Select item Group By item.LocusID Into Group).ToArray.ToDictionary(Function(item) item.LocusID, elementSelector:=Function(item) item.Group.First)
            Dim GroupResult = (From item In ScanningResults Where BesthitsResults.ContainsKey(item.Key) Select CRISPR = item.Value.CRISPR, Besthit = BesthitsResults(item.Key).Besthit, Entry = item.Value.path).ToArray
            Dim LQuery = (From item In GroupResult Select item.Entry, CRISPR = RemoveConserved(item.Besthit, ORF, item.CRISPR)).ToArray
            Dim SaveOperations = (From item In LQuery Select item.CRISPR.GetXml.SaveTo(export & "/" & item.Entry.Key & ".xml"))

            Return (From item In LQuery Select item.CRISPR).ToArray
        End Function

        <ExportAPI("batch.export_csv")>
        Public Function BatchExportCsv(source As IEnumerable(Of GenomeScanResult), export As String) As Boolean
            Dim LQuery = (From item In source.AsParallel Select CSV = TabularDumps.Export(item.Sites), item).ToArray
            Dim SaveLQuery = (From item In LQuery.AsParallel Select item.CSV.Save(export & "/" & item.item.Tag & ".csv", False)).ToArray
            Return Not SaveLQuery.IsNullOrEmpty
        End Function

        <ExportAPI("export.csv")>
        Public Function Export(dat As IEnumerable(Of SearchingModel.CRISPR)) As DocumentStream.File
            Dim File As DocumentStream.File = New DocumentStream.File

            Call File.AppendLine(New String() {"Position", "Repeats", "Spacer Sequence", "Repeat Length", "Spacer Length"})
            Call File.AppendLine()

            If dat.IsNullOrEmpty Then
                Return File
            End If

            For i As Integer = 0 To dat.Count - 1
                Dim CRISPR As SearchingModel.CRISPR = dat(i)
                Call File.AppendLine(New String() {"=====> CRISPR " & i})

                For j As Integer = 0 To CRISPR.NumberOfRepeats - 1
                    Dim Row As DocumentStream.RowObject = New DocumentStream.RowObject
                    Call Row.Add(CRISPR.RepeatAt(j) + 1)
                    Call Row.Add(CRISPR.RepeatStringAt(j))

                    If j < CRISPR.NumberOfSpacers Then
                        Dim Spacer As String = CRISPR.SpacerStringAt(j)

                        Call Row.Add(Spacer)
                        Call Row.Add(CRISPR.RepeatStringAt(j).Length)
                        Call Row.Add(CRISPR.SpacerStringAt(j).Length)
                    End If

                    Call File.AppendLine(Row)
                Next

                Call File.AppendLine()
            Next

            Return File
        End Function
    End Module
End Namespace
