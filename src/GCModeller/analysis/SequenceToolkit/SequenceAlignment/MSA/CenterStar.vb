#Region "Microsoft.VisualBasic::5e3f341b77623a33be777a6aab4fe035, analysis\SequenceToolkit\MSA\CenterStar.vb"

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

    '   Total Lines: 28
    '    Code Lines: 22 (78.57%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (21.43%)
    '     File Size: 962 B


    ' Class CenterStar
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: Compute
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataMining
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace MSA

    Public Class CenterStar

        Dim algorithm As DynamicProgramming.CenterStar

        Sub New(seqs As IEnumerable(Of String))
            algorithm = New DynamicProgramming.CenterStar(seqs)
        End Sub

        Sub New(seqs As IEnumerable(Of FastaSeq))
            algorithm = New DynamicProgramming.CenterStar(seqs.Select(Function(f) New NamedValue(Of String)(f.Title, f.SequenceData)))
        End Sub

        Public Function Compute(matrix As ScoreMatrix) As MSAOutput
            Dim alignments As String() = Nothing
            Dim cost As Double = algorithm.Compute(matrix Or ScoreMatrix.DefaultMatrix, alignments)
            Dim output As New MSAOutput With {
                .cost = cost,
                .MSA = alignments,
                .names = algorithm.NameList
            }

            Return output
        End Function
    End Class
End Namespace