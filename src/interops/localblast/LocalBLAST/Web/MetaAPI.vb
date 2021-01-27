#Region "Microsoft.VisualBasic::41bde5a9b3acb2b1a5571226fad8fec0, LocalBLAST\Web\MetaAPI.vb"

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

    '     Module BBHMetaAPI
    ' 
    '         Function: DataParser, DensityScore, GetAlignmentRegion, Offset
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models

Namespace NCBIBlastResult

    ''' <summary>
    ''' <see cref="SpeciesBesthit"/> -> <see cref="AlignmentTable"/>
    ''' </summary>
    Public Module BBHMetaAPI

        <Extension>
        Public Function GetAlignmentRegion(table As AlignmentTable) As Location
            Dim locations%() = table.Hits _
                .Select(Function(hit) {hit.QueryStart, hit.QueryEnd}) _
                .IteratesALL _
                .ToArray
            Dim region As New Location(locations.Min, locations.Max)
            Return region
        End Function

        <Extension>
        Public Function Offset(table As AlignmentTable, region As Location) As AlignmentTable
            Dim left% = region.Left

            For Each hit As HitRecord In table.Hits
                With hit
                    .QueryStart -= left
                    .QueryEnd -= left
                End With
            Next

            Return table
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="bbh"></param>
        ''' <param name="PTT">因为这个是blastp BBH的结果，所以没有基因组的位置信息，在这里使用PTT文档来生成绘图时所需要的位点信息</param>
        ''' <param name="visualGroup">
        ''' 由于在进行blast绘图的时候，程序是按照基因组来分组绘制的，而绘制的对象不需要显示详细的信息，所以在这里为True的话，会直接使用基因组tag来替换名称进而用于blast作图
        ''' </param>
        ''' <returns></returns>
        Public Function DataParser(bbh As SpeciesBesthit,
                                   PTT As PTT,
                                   Optional visualGroup As Boolean = False,
                                   Optional scoreMaps As Func(Of Hit, Double) = Nothing) As AlignmentTable

            If scoreMaps Is Nothing Then
                scoreMaps = Function(x) x.Identities
            End If

            Return New AlignmentTable With {
                .Database = bbh.sp,
                .Program = GetType(SpeciesBesthit).FullName,
                .Query = bbh.sp,
                .RID = Now.ToString,
                .Hits = LinqAPI.Exec(Of HitRecord) <= From prot As HitCollection
                                                      In bbh.hits
                                                      Let ORF As GeneBrief = PTT(prot.QueryName)
                                                      Select From hit As Hit
                                                             In prot.Hits
                                                             Select New HitRecord With {
                                                                 .QueryID = prot.QueryName,
                                                                 .Identity = scoreMaps(hit),
                                                                 .AlignmentLength = ORF.Length,
                                                                 .BitScore = ORF.Length,
                                                                 .QueryEnd = ORF.Location.Ends,
                                                                 .QueryStart = ORF.Location.Start,
                                                                 .SubjectEnd = ORF.Location.Ends,
                                                                 .SubjectStart = ORF.Location.Start,
                                                                 .SubjectIDs = If(visualGroup, hit.tag, hit.HitName),
                                                                 .DebugTag = hit.tag
                                                             }
            }
        End Function

        <Extension>
        Public Function DensityScore(DIR As String, Optional scale As Double = 1) As Func(Of Hit, Double)
            Dim datas As IEnumerable(Of Density) =
                LinqAPI.Exec(Of Density) <= From path As String
                                            In ls - l - r - "*.csv" <= DIR
                                            Select path.LoadCsv(Of Density)
            Dim hash As Dictionary(Of String, Density) =
                (From x As Density
                 In datas
                 Select x
                 Group x By x.locus_tag Into Group) _
                      .ToDictionary(Function(x) x.locus_tag,
                                    Function(x) x.Group.First)
            Return Function(x) _
                If(hash.ContainsKey(x.HitName),
                hash(x.HitName).Abundance * scale,
                0R)
        End Function
    End Module
End Namespace
