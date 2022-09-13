#Region "Microsoft.VisualBasic::703b199f9c0115ce40642c3308151d2d, GCModeller\core\Bio.Annotation\GFF\Region.vb"

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

    '   Total Lines: 38
    '    Code Lines: 26
    ' Comment Lines: 6
    '   Blank Lines: 6
    '     File Size: 1.34 KB


    '     Class SeqRegion
    ' 
    '         Properties: accessId, ends, start
    ' 
    '         Function: Parser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace Assembly.NCBI.GenBank.TabularFormat.GFF

    ''' <summary>
    ''' sequence-region  (##sequence-region &lt;seqname> &lt;start> &lt;end>)
    ''' To indicate that this file only contains entries for the specified subregion of a sequence.
    ''' 
    ''' [##sequence-region CP000050.1 1 5148708]
    ''' </summary>
    Public Class SeqRegion

        <XmlAttribute> Public Property accessId As String
        <XmlAttribute> Public Property start As Integer
        <XmlAttribute> Public Property ends As Integer

        Friend Shared Function Parser(s As String) As SeqRegion
            If String.IsNullOrWhiteSpace(s) Then
                Return New SeqRegion
            Else
                Dim tokens As String() = s.Split
                Dim acc As String = tokens(Scan0)
                Dim start As Integer = CInt(Val(tokens(1)))
                Dim ends As Integer = CInt(Val(tokens(2)))

                Return New SeqRegion With {
                    .accessId = acc,
                    .start = start,
                    .ends = ends
                }
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"{accessId} {start} {ends}"
        End Function
    End Class
End Namespace
