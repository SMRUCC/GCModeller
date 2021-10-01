#Region "Microsoft.VisualBasic::7badad72177114b42f945b3ee7aa9591, meme_suite\MEME\Analysis\Similarity\TomQuery\Encoder.vb"

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

    '     Module Encoder
    ' 
    '         Function: Level, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language

Namespace Analysis.Similarity.TOMQuery

    Module Encoder

        ''' <summary>
        ''' 对残基在列之中出现的概率进行分级
        ''' </summary>
        ''' <param name="p">0-1之间的概率值</param>
        ''' <returns>ABCDE</returns>
        <Extension> Public Function Level(p As Double) As Char
            If p >= 0 AndAlso p < 0.2 Then
                Return "1"c
            ElseIf p >= 0.2 AndAlso p < 0.4 Then
                Return "2"c
            ElseIf p >= 0.4 AndAlso p < 0.6 Then
                Return "3"c
            ElseIf p >= 0.6 AndAlso p < 0.8 Then
                Return "4"c
            Else
                Return "5"c
            End If
        End Function

        Public Function ToString(model As Analysis.MotifScans.AnnotationModel) As String
            Dim chars As New List(Of Char)

            For Each column As MotifScans.ResidueSite In model.PWM  ' ATGC
                Call chars.Add("A"c, column.PWM(0).Level)
                Call chars.Add("T"c, column.PWM(1).Level)
                Call chars.Add("G"c, column.PWM(2).Level)
                Call chars.Add("C"c, column.PWM(3).Level)
            Next

            Dim s As String = New String(chars.ToArray)
            Return s
        End Function
    End Module
End Namespace
