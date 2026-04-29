#Region "Microsoft.VisualBasic::1341386dbe41777e52db6773e6294f6d, data\Xfam\Rfam\Stockholm.vb"

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

' Class Stockholm
' 
'     Properties: AC, Alignments, AU, BM, CB
'                 CC, CL, DC, DE, DR
'                 FR, GA, ID, KW, MB
'                 NC, NE, NH, NL, PI
'                 RA, RC, RF, RL, RM
'                 RN, RT, SE, SM, SQ
'                 SS, SS_cons, TC, TN, TP
'                 WK
' 
'     Function: __fieldsParser, DatabaseParser, Parser, ParseSchema
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

        Public Function PopulateAlignment() As IEnumerable(Of FastaSeq)
            If msa Is Nothing Then
                Return {}
            Else
                Return msa.PopulateAlignment
            End If
        End Function

        Public Overrides Function ToString() As String
            Return seq_cons
        End Function
    End Class
End Namespace