#Region "Microsoft.VisualBasic::7f7c41177b02f70763f0bab0423cdbba, ..\GCModeller\analysis\SyntenySignature\blastKO\Extensions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
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

#End Region

Imports System.Runtime.CompilerServices

Public Module Extensions

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gff3">The file path or the file content of the gff3 feature table file.</param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function FromGFF3(gff3 As String) As IEnumerable(Of Feature)

    End Function
End Module

