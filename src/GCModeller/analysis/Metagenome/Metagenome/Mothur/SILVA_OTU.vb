#Region "Microsoft.VisualBasic::08698f6381fddb206551f55147c81de6, GCModeller\analysis\Metagenome\Metagenome\Mothur\SILVA_OTU.vb"

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


    ' Code Statistics:

    '   Total Lines: 76
    '    Code Lines: 50
    ' Comment Lines: 18
    '   Blank Lines: 8
    '     File Size: 2.85 KB


    ' Module SILVA_OTU
    ' 
    '     Function: OTUsilvaTaxonomy, ParseOTUrep, RemovesOTUlt
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module SILVA_OTU

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="fasta">通过mothur的GetOTUrep
    ''' 命令获取得到的OTU代表序列的fasta文件数据</param>
    ''' <returns></returns>
    <Extension>
    Public Function ParseOTUrep(fasta As IEnumerable(Of FastaSeq)) As Dictionary(Of String, NamedValue(Of Integer))
        Dim table As New Dictionary(Of String, NamedValue(Of Integer))
        Dim OTU$()

        For Each kseq As FastaSeq In fasta
            With kseq.Title.Split(ASCII.TAB)
                OTU = .Last.Split("|"c)
                table(.First) = New NamedValue(Of Integer) With {
                    .Name = OTU(0),
                    .Value = Val(OTU(1))
                }
            End With
        Next

        Return table
    End Function

    ''' <summary>
    ''' Removes all of the OTU that counts less than <paramref name="cutoff"/> percentage.
    ''' </summary>
    ''' <param name="OTUrep">Loaded fasta header data from mothur ``get.oturep`` output.</param>
    ''' <param name="cutoff#">The sequence number cutoff percentage.</param>
    ''' <returns></returns>
    <Extension>
    Public Function RemovesOTUlt(OTUrep As Dictionary(Of String, NamedValue(Of Integer)), Optional cutoff# = 0.0001) As Dictionary(Of String, NamedValue(Of Integer))
        Dim sum# = OTUrep _
            .Values _
            .Select(Function(o) o.Value) _
            .Sum

        cutoff = sum * cutoff

        Return OTUrep _
            .Where(Function(OTU) OTU.Value.Value >= cutoff) _
            .ToDictionary
    End Function

    ''' <summary>
    ''' Assign OTU taxonomy from OTU blastn against silva database with votes
    ''' </summary>
    ''' <param name="blastn"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function OTUsilvaTaxonomy(blastn As IEnumerable(Of Query),
                                     OTUs As Dictionary(Of String, NamedValue(Of Integer)),
                                     Optional min_pct# = 0.97) As IEnumerable(Of gastOUT)
        Return blastn.gastTaxonomyInternal(
            getTaxonomy:=Function(hitName)
                             Dim t$ = hitName _
                                 .GetTagValue(vbTab, trim:=True) _
                                 .Value
                             Return New Taxonomy(t)
                         End Function,
            getOTU:=OTUs,
            min_pct:=min_pct
        )
    End Function
End Module
