#Region "Microsoft.VisualBasic::2a2c04e09e5268f87aa9e96dbd3aa143, sub-system\FBA\FBA_DP\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: GenerateLine, PToken, Width
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq

<Extension> Module Extensions

    Public Const Scan0 = 0

    <Extension> Public Function GenerateLine(Line As Double()) As String
        Return String.Join(", ", Line.Select(Function(x) CStr(x)))
    End Function

    <Extension> Public Function PToken(s As Match) As KeyValuePair(Of Integer, Double)
        Dim m As MatchCollection = Regex.Matches(s.Value, "\d+")

        If m.Count = 2 Then
            Return New KeyValuePair(Of Integer, Double)(Val(m(1).Value), Val(m(0).Value))
        Else
            Return New KeyValuePair(Of Integer, Double)(Val(m(0).Value), 1)
        End If
    End Function

    <Extension> Public Function Width(m As List(Of Double())) As Long
        Return m.First.Length
    End Function
End Module
