#Region "Microsoft.VisualBasic::52f9bdec4602c55f5d3b1d3cca649065, core\test\minhashtest.vb"

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

'   Total Lines: 26
'    Code Lines: 21 (80.77%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 5 (19.23%)
'     File Size: 899 B


' Module minhashtest
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.HashMaps.MinHash
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Slicer

Module minhashtest

    Sub Main()
        Dim seqset = FastaFile.Read("U:\metagenomics_LLMs\demo\seq.fa")
        Dim seqs As New List(Of SequenceItem)
        Dim idset As New List(Of String)
        Dim id As i32 = 0

        For Each seq As FastaSeq In TqdmWrapper.Wrap(seqset.Take(10000).ToArray)
            Call idset.Add(seq.Title)
            Call seqs.Add(KSeq.KmerSpans(seq.SequenceData, k:=12).CreateSequenceData(++id))
        Next

        For Each result As SimilarityIndex In LSH.FindSimilarItems(seqs.ToArray)
            Call Console.WriteLine(result)
        Next

        Pause()
    End Sub
End Module

