#Region "Microsoft.VisualBasic::6c6c8e2c2b92525b8a54e86238fdc572, analysis\SequenceToolkit\MSA\GlobalAlignment.vb"

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

'   Total Lines: 9
'    Code Lines: 7 (77.78%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 2 (22.22%)
'     File Size: 212 B


' Structure GlobalAlignment
' 
'     Function: ToString
' 
' /********************************************************************************/

#End Region

Namespace MSA

    Public Structure GlobalAlignment

        Dim seq1, seq2 As String
        Dim score#

        Public Overrides Function ToString() As String
            Return $"[{score}] {seq1} * {seq2}"
        End Function
    End Structure
End Namespace