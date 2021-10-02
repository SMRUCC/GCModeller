#Region "Microsoft.VisualBasic::e1ca2df93610b80e1860ada694e60baa, analysis\SequenceToolkit\MotifScanner\MotifMatch.vb"

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

    ' Class MotifMatch
    ' 
    '     Properties: ends, identities, motif, score1, score2
    '                 seeds, segment, start, title
    ' 
    ' /********************************************************************************/

#End Region

Public Class MotifMatch

    Public Property title As String
    Public Property segment As String
    Public Property identities As Double
    Public Property score1 As Double
    Public Property score2 As Double
    Public Property motif As String
    Public Property start As Integer
    Public Property ends As Integer
    Public Property seeds As String()

End Class
