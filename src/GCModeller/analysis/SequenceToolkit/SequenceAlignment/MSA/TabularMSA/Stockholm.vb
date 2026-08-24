#Region "Microsoft.VisualBasic::3ee023c6cf45d14aa041eb6f378ccfdd, analysis\SequenceToolkit\SequenceAlignment\MSA\TabularMSA\Stockholm.vb"

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

    '   Total Lines: 39
    '    Code Lines: 26 (66.67%)
    ' Comment Lines: 7 (17.95%)
    '    - Xml Docs: 85.71%
    ' 
    '   Blank Lines: 6 (15.38%)
    '     File Size: 1.56 KB


    '     Class Stockholm
    ' 
    '         Properties: comment, metadata, msa, seq_cons, seq_source
    ' 
    '         Function: PopulateAlignment, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace MSA.Tabular

    ''' <summary>
    ''' Tabular MSA file
    ''' 
    ''' Stockholm format is a Multiple sequence alignment format used by Pfam and Rfam to disseminate protein and RNA sequence alignments. 
    ''' The alignment editors Ralee and Belvu support Stockholm format as do the probabilistic database search tools, 
    ''' Infernal and HMMER, and the phylogenetic analysis tool Xrate.
    ''' </summary>
    Public Class Stockholm

        Public Property metadata As Dictionary(Of String, String)
        Public Property msa As MSAOutput
        Public Property seq_cons As String
        Public Property seq_source As Dictionary(Of String, String)
        Public Property comment As String

        Public Iterator Function PopulateAlignment() As IEnumerable(Of FastaSeq)
            If Not msa Is Nothing Then
                Dim acc_id As String = metadata!AC
                Dim id As String = metadata!ID
                Dim def As String = metadata!DE

                For i As Integer = 0 To msa.names.Length - 1
                    Yield New FastaSeq With {
                        .Headers = {msa.names(i), seq_source(msa.names(i)), acc_id & ";" & id, def},
                        .SequenceData = msa.MSA(i).Replace("."c, "").Trim
                    }
                Next
            End If
        End Function

        Public Overrides Function ToString() As String
            Return seq_cons
        End Function
    End Class
End Namespace
