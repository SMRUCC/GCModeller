#Region "Microsoft.VisualBasic::7debd4e24d93a1b5c3b36dbdf0216a8a, core\Bio.Annotation\GFF\SequenceParser.vb"

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

'   Total Lines: 32
'    Code Lines: 27 (84.38%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 5 (15.62%)
'     File Size: 1.42 KB


' Module SequenceParser
' 
'     Function: ExtractSequence
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.Slicer
Imports std = System.Math

Public Module SequenceParser

    <Extension>
    Public Iterator Function ExtractSequence(gff As GFFTable, seqs As IEnumerable(Of FastaSeq)) As IEnumerable(Of FastaSeq)
        Dim seqdata As Dictionary(Of String, FastaSeq) = seqs _
            .AsEnumerable _
            .ToDictionary(Function(s)
                              Return s.Title
                          End Function)

        For Each feature As Feature In TqdmWrapper.Wrap(gff.features)
            Dim parent_id As String = feature.attributes("parent")
            Dim title As String = $"{parent_id} {feature.feature}.{feature.frame} [{feature.start}-{feature.ends}]"
            Dim chr_id As String = feature.seqname
            Dim seq As FastaSeq = seqdata(chr_id)
            Dim left = std.Min(feature.start, feature.ends)
            Dim right = std.Max(feature.start, feature.ends)
            Dim seq_cut As SimpleSegment = seq.CutSequenceLinear(left, right, title)

            Yield New FastaSeq(seq_cut)
        Next
    End Function

End Module

