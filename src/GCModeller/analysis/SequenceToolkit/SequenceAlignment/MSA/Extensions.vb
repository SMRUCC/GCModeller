#Region "Microsoft.VisualBasic::5b348e0eece5e71f45a4e9d285cedca1, analysis\SequenceToolkit\SequenceAlignment\MSA\Extensions.vb"

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

    '   Total Lines: 21
    '    Code Lines: 17 (80.95%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (19.05%)
    '     File Size: 776 B


    '     Module Extensions
    ' 
    '         Function: (+2 Overloads) MultipleAlignment
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace MSA

    <HideModuleName>
    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MultipleAlignment(input As IEnumerable(Of FastaSeq), Optional matrix As ScoreMatrix = Nothing) As MSAOutput
            Return New CenterStar(input).Compute(matrix)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MultipleAlignment(input As IEnumerable(Of String), Optional matrix As ScoreMatrix = Nothing) As MSAOutput
            Return New CenterStar(input).Compute(matrix)
        End Function
    End Module
End Namespace
