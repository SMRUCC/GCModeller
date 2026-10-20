#Region "Microsoft.VisualBasic::2b8c6d1e0f3a5b7c9d1e0f3a5b7c9d1e, analysis\SequenceToolkit\gwANI\GwaniResult.vb"

' Author:
'
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
'
' Copyright (c) 2018 GPL3 Licensed
'
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

Option Strict On


    ''' <summary>
    ''' One row of the gwANI result matrix.
    ''' </summary>
    ''' <remarks>
    ''' <see cref="Identities"/> is indexed by the column sequence index, and it only
    ''' carries the upper triangle of the matrix, exactly like the output of the
    ''' original panito tool: the cell is <see cref="GwaniValues.LowerTriangle"/> when
    ''' the column index is less than <see cref="RowIndex"/>, and it is
    ''' <see cref="GwaniValues.SelfIdentity"/> on the diagonal.
    ''' </remarks>
    Public NotInheritable Class GwaniRow

        ''' <summary> The sequence id of the current row. </summary>
        Public Property ID As String

        ''' <summary> The row index inside the alignment. </summary>
        Public Property RowIndex As Integer

        ''' <summary> The identity values of the current row. </summary>
        Public Property Identities As Double()

    End Class

    ''' <summary>
    ''' The special marker values that are used inside a gwANI result matrix.
    ''' </summary>
    Public NotInheritable Class GwaniValues

        ''' <summary> Lower triangle placeholder, printed as "-" by the panito tool. </summary>
        Public Const LowerTriangle As Double = -1.0

        ''' <summary> Diagonal value. </summary>
        Public Const SelfIdentity As Double = 100.0

        ''' <summary>
        ''' The pair was skipped by the sketch pre-filter, so its exact identity is
        ''' unknown(it is guaranteed to be below the configured threshold).
        ''' </summary>
        Public Const Skipped As Double = -2.0

    End Class


#End Region
