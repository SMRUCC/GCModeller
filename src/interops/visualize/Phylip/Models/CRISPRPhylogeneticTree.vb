#Region "Microsoft.VisualBasic::a868b910ed7b5bcf5b4599422ed4a12a, ..\interops\visualize\Phylip\Models\CRISPRPhylogeneticTree.vb"

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

Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Analysis.CRISPR.CRT.Output
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Analysis
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
'''
''' </summary>
''' <remarks></remarks>
'''
<[Namespace]("Phylip.app.crispr_PhylogeneticTree")>
Module CRISPRPhylogeneticTree

    <ExportAPI("exe_test")>
    Public Function ExeTest(source As String, output As String) As Boolean
        Dim io As New Microsoft.VisualBasic.CommandLine.IORedirect("E:\Desktop\phylip-3.695\phylip-3.695\exe\gendist.exe")
        Call io.Start(WaitForExit:=True, PushingData:={source, "Y"}, _DISP_DEBUG_INFO:=True)
        Return True
    End Function

    ''' <summary>
    ''' 将所有该<see cref="SMRUCC.genomics.ComponentModel.Loci.Location">区间</see>之内的位点数据全部清除
    ''' </summary>
    ''' <param name="CRISPRData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TrimData(CRISPRData As GenomeScanResult, loc As Location) As GenomeScanResult
        CRISPRData.Sites = (From item In CRISPRData.Sites
                            Where Not (loc.ContainSite(item.Start) OrElse
                                loc.ContainSite(item.Right))
                            Select item).ToArray
        Return CRISPRData
    End Function

    <ExportAPI("crispr.export_motif")>
    Public Function ExportMotifFasta(dat As Generic.IEnumerable(Of GenomeScanResult)) As FastaFile
        Dim LQuery = (From item In dat.AsParallel Select item.ExportFasta.ToArray).Unlist
        Return New FastaFile(LQuery)
    End Function

    <ExportAPI("invoke.tree_drawing")>
    Public Function InvokeTreeDrawing(tree As Evolview.PhyloTree) As Image
        Return TreeDrawing.InvokeDrawing(tree)
    End Function

    ''' <summary>
    ''' 假若在进行质粒的进化树构建的时候，由于外来基因的水平转移的原因，假若包含有大片段的比对上的基因的话，会对进化树的构建结果产生很大的影响。
    ''' 故而将已经比对上的区域之中的CRISPR位点的数据进行删除，以消除影响。请注意，构建质粒的进化树一定要使用本方法进行数据的筛选
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="cds_Info"></param>
    ''' <param name="besthits"></param>
    ''' <param name="ends">片段区域结束的基因</param>
    ''' <param name="start">片段区域起始的基因</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("plasmid.crispr_trim_besthits")>
    Public Function TrimData(data As IEnumerable(Of GenomeScanResult),
                             cds_Info As IEnumerable(Of GeneDumpInfo),
                             besthits As BestHit,
                             start As String, ends As String) As GenomeScanResult()
        Dim StartAligned = besthits.Hit(start)
        Dim EndsAligned = besthits.Hit(ends)
        Dim LQuery = (From item In data.AsParallel Let r = GetRange(besthits, item.Tag, StartAligned, EndsAligned, cds_Info) Select TrimData(item, loc:=r)).ToArray
        Return LQuery
    End Function

    ''' <summary>
    ''' 由于是在一个所指定的范围之内，所以没有查照到的时候，start往后移动，ends则会往前移动
    ''' </summary>
    ''' <param name="besthits">假设里面的基因都是已经按照顺序排序了的</param>
    ''' <param name="start"></param>
    ''' <param name="ends"></param>
    ''' <returns></returns>
    ''' <param name="subjectTag">目标基因组的标签信息</param>
    ''' <remarks></remarks>
    Public Function GetRange(besthits As BestHit,
                                     subjectTag As String,
                                     start As HitCollection,
                                     ends As HitCollection,
                                     cdsinfo As Generic.IEnumerable(Of SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo)) _
                                 As SMRUCC.genomics.ComponentModel.Loci.Location
        Dim left = start.GetHitByTagInfo(subjectTag)
        Dim right = ends.GetHitByTagInfo(subjectTag)
        Dim p As Integer = besthits.IndexOf(start.QueryName)

        Do While left Is Nothing
            p += 1
            left = besthits.Hits(p).GetHitByTagInfo(subjectTag)
        Loop

        p = besthits.IndexOf(ends.QueryName)
        Do While right Is Nothing
            p -= 1
            right = besthits.Hits(p).GetHitByTagInfo(subjectTag)
        Loop

        Dim r = {(From item In cdsinfo Where String.Equals(item.LocusID, left.HitName) Select item.Location).First, (From item In cdsinfo Where String.Equals(item.LocusID, right.HitName) Select item.Location).First}
        Dim c = {r.First.Left, r.First.Right, r.Last.Left, r.Last.Right}
        Return New SMRUCC.genomics.ComponentModel.Loci.Location(c.Min, c.Max)
    End Function
End Module
