#Region "Microsoft.VisualBasic::bffa1aad611fae72465dee064acd6d07, GCModeller\analysis\SequenceToolkit\SequencePatterns\DNAOrigami\Project.vb"

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

    '   Total Lines: 41
    '    Code Lines: 20
    ' Comment Lines: 12
    '   Blank Lines: 9
    '     File Size: 1.11 KB


    '     Class Project
    ' 
    '         Properties: get_rev_compl, is_linear, n
    ' 
    '     Class Output
    ' 
    '         Properties: count, count_corrected, count_revcompl, count_revcompl_corrected, n_count
    '                     n_count_revcompl, tuple
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace DNAOrigami

    Public Class Project

        ''' <summary>
        ''' segment length
        ''' </summary>
        ''' <returns></returns>
        Public Property n As Integer = 7
        ''' <summary>
        ''' scaffolds are not circular
        ''' </summary>
        ''' <returns></returns>
        Public Property is_linear As Boolean = False
        ''' <summary>
        ''' also count reverse complementary sequences
        ''' </summary>
        ''' <returns></returns>
        Public Property get_rev_compl As Boolean = False

    End Class

    Public Class Output

        Public Property tuple As String()

        Public Property count As Double
        Public Property count_revcompl As Double
        Public Property count_corrected As Double
        Public Property count_revcompl_corrected As Double
        Public Property n_count As Double()
        Public Property n_count_revcompl As Double()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace
