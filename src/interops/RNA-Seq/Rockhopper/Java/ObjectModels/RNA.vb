#Region "Microsoft.VisualBasic::31a85b32abff634cb3e938b4b9119e27, ..\interops\RNA-Seq\Rockhopper\Java\ObjectModels\RNA.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
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

Namespace Java

    ''' <summary>
    ''' *********************************
    ''' **********   RNA CLASS   **********
    ''' </summary>
    Friend Class RNA

        Public start As Integer
        Public [stop] As Integer
        Public strand As Char

        Public Sub New(start As Integer, [stop] As Integer, strand As Char)
            Me.start = start
            Me.[stop] = [stop]
            Me.strand = strand
        End Sub

        Public Overridable Overloads Function ToString() As String
            Return start & vbTab & [stop] & vbTab & strand
        End Function
    End Class

End Namespace
