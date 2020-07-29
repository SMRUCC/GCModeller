#Region "Microsoft.VisualBasic::149427a3760527c87a9b9fa214cc57fc, visualize\Phylip\Models\CRISPRPhylogeneticTree.vb"

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

    ' Module CRISPRPhylogeneticTree
    ' 
    '     Function: ExeTest, ExportMotifFasta, GetRange, InvokeTreeDrawing, (+2 Overloads) TrimData
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Analysis.CRISPR.CRT.Output
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models
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
        Call io.Start(waitForExit:=True, pushingData:={source, "Y"}, displaDebug:=True)
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
                            Where Not (loc.IsInside(item.Start) OrElse
                                loc.IsInside(item.Right))
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
                             cds_Info As IEnumerable(Of GeneTable),
                             besthits As SpeciesBesthit,
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
    Public Function GetRange(besthits As SpeciesBesthit,
                                     subjectTag As String,
                                     start As HitCollection,
                                     ends As HitCollection,
                                     cdsinfo As IEnumerable(Of GeneTable)) As Location
        Dim left = start.GetHitByTagInfo(subjectTag)
        Dim right = ends.GetHitByTagInfo(subjectTag)
        Dim p As Integer = besthits.IndexOf(start.QueryName)

        Do While left Is Nothing
            p += 1
            left = besthits.hits(p).GetHitByTagInfo(subjectTag)
        Loop

        p = besthits.IndexOf(ends.QueryName)
        Do While right Is Nothing
            p -= 1
            right = besthits.hits(p).GetHitByTagInfo(subjectTag)
        Loop

        Dim r = {(From item In cdsinfo Where String.Equals(item.locus_id, left.hitName) Select item.Location).First, (From item In cdsinfo Where String.Equals(item.locus_id, right.hitName) Select item.Location).First}
        Dim c = {r.First.Left, r.First.Right, r.Last.Left, r.Last.Right}
        Return New Location(c.Min, c.Max)
    End Function
End Module
