#Region "Microsoft.VisualBasic::53a4766cf1eb7a7b36d36bb0d4afdb49, analysis\SequenceToolkit\SequenceLogo\SequenceLogo\Alphabet.vb"

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

    '   Total Lines: 57
    '    Code Lines: 25 (43.86%)
    ' Comment Lines: 25 (43.86%)
    '    - Xml Docs: 96.00%
    ' 
    '   Blank Lines: 7 (12.28%)
    '     File Size: 1.87 KB


    '     Class Alphabet
    ' 
    '         Properties: Alphabet, RelativeFrequency
    ' 
    '         Function: CompareTo, Height, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace SequenceLogo

    ''' <summary>
    ''' Alphabet model in the drawing motif model, nt for 4 and aa for 20
    ''' </summary>
    Public Class Alphabet : Implements IComparable

        ''' <summary>
        ''' A alphabet character which represents one residue.(可以代表本残基的字母值)
        ''' </summary>
        ''' <returns></returns>
        Public Property Alphabet As Char
        ''' <summary>
        ''' The relative alphabet frequency at this site position.
        ''' </summary>
        ''' <returns></returns>
        Public Property RelativeFrequency As Double

        ''' <summary>
        ''' Sorts for the logo drawing
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
            If obj Is Nothing Then
                Return 1
            ElseIf obj.GetType <> GetType(Alphabet) Then
                Return 1
            End If

            Dim n As Double = DirectCast(obj, Alphabet).RelativeFrequency

            If RelativeFrequency > n Then
                Return 1
            Else
                Return -1
            End If
        End Function

        ''' <summary>
        ''' The height of letter a in column i Is given by
        ''' 
        ''' ```
        '''    height = f(a,i) x R(i)
        ''' ```
        ''' (该残基之中本类型的字母的绘制的高度)
        ''' </summary>
        ''' <returns></returns>
        Public Function Height(Ri As Double) As Integer
            Return CInt(Me.RelativeFrequency * Ri)
        End Function

        Public Overrides Function ToString() As String
            Return $"{{{Alphabet}}}: {RelativeFrequency.ToString("F4")}"
        End Function
    End Class
End Namespace
