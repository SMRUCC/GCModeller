#Region "Microsoft.VisualBasic::24cb07a976d7064d96f12ab757750aa7, analysis\SequenceToolkit\MotifScanner\Scanner\MotifMatch.vb"

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

'   Total Lines: 17
'    Code Lines: 12 (70.59%)
' Comment Lines: 3 (17.65%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 2 (11.76%)
'     File Size: 498 B


' Class MotifMatch
' 
'     Properties: ends, identities, motif, pvalue, score1
'                 score2, seeds, segment, start, title
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

''' <summary>
''' motif model sequence site matches result
''' </summary>
''' <remarks>
'''  A single TFBS match found by scanning a sequence with a motif PWM.
''' </remarks>
Public Class MotifMatch

    Public Property title As String

    ''' <summary>
    ''' ##### the motif matched site sequence
    ''' 
    ''' The matched sequence fragment, read 5'->3' on the strand it was
    ''' found (i.e. reverse-complemented for '-' strand matches).
    ''' </summary>
    ''' <returns></returns>
    Public Property segment As String
    Public Property identities As Double
    Public Property score1 As Double
    Public Property score2 As Double
    Public Property motif As String
    Public Property start As Integer
    Public Property ends As Integer

    ''' <summary>Strand: '+' for forward, '-' for reverse complement.</summary>
    <XmlAttribute> Public Property strand As Char

    Public Property seeds As String()

    ''' <summary>
    ''' P-value under the null (background) model:
    ''' P(random background window scores >= observed score).
    ''' </summary>
    Public Property pvalue As Double

    Public Overrides Function ToString() As String
        Return $"{title} [{segment}|{motif}] p.val:{pvalue}"
    End Function

End Class
