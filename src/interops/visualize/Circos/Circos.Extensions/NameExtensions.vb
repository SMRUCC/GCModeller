#Region "Microsoft.VisualBasic::c06e0830e9ccafd64097fbf580bf6a7f, visualize\Circos\Circos.Extensions\NameExtensions.vb"

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

    ' Module NameExtensions
    ' 
    '     Function: __loadCommon, DumpNames, RegulonRegulators
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights

Public Module NameExtensions
    ''' <summary>
    ''' 生成Regulon的调控因子的名称预处理数据
    ''' </summary>
    ''' <param name="DIR"></param>
    ''' <param name="gb"></param>
    ''' <returns></returns>
    Public Function DumpNames(DIR As String, gb As PTT) As Name()
        Dim genes = __loadCommon(DIR, gb)
        Return genes.Select(Function(x) New Name With {
                                 .Name = $"[{x.Key}]  {x.Value.Product}",
                                 .Maximum = CInt(x.Value.Location.Right),
                                 .Minimum = CInt(x.Value.Location.Left)})
    End Function

    Private Function __loadCommon(DIR As String, gb As PTT) As KeyValuePair(Of String, GeneBrief)()
        Dim bbh = (From file As String
                   In FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.xml").AsParallel
                   Select file.LoadXml(Of BacteriaRegulome).regulome.regulators).ToArray.Unlist
        Dim Maps = gb.LocusMaps
        Dim regLocus = (From x In bbh Select New With {.Family = x.family, .Locus = x.LocusId}).ToArray

        For i As Integer = 0 To regLocus.Length - 1
            Dim s As String = regLocus(i).Locus

            If Maps.ContainsKey(s) Then
                regLocus(i).Locus = Maps(s)
            End If
        Next

        Dim g = (From s In regLocus
                 Where gb.ExistsLocusId(s.Locus)
                 Select gg = gb(s.Locus),
                     s.Locus,
                     s
                 Group By Locus Into Group) _
                    .Select(Function(x) New KeyValuePair(Of String, GeneBrief)(
                          x.Group.Select(Function(og) og.s.Family).Distinct.JoinBy("; "),
                          x.Group.First.gg))
        Return g
    End Function

    Public Function RegulonRegulators(DIR As String, gb As PTT) As ValueTrackData()
        Dim g = __loadCommon(DIR, gb).Select(Function(x) x.Value)
        Dim list = g.Select(Function(x) New Name With {
                                 .Name = x.Product,
                                 .Maximum = CInt(x.Location.Right),
                                 .Minimum = CInt(x.Location.Left)})
        Return list.Select(Function(x) x.ToMeta)
    End Function
End Module
