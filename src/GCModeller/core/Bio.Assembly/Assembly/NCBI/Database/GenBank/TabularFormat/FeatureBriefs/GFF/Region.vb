#Region "Microsoft.VisualBasic::b049e22b3b0ff87493e6766136bb3864, Bio.Assembly\Assembly\NCBI\Database\GenBank\TabularFormat\FeatureBriefs\GFF\Region.vb"

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

    '     Class SeqRegion
    ' 
    '         Properties: AccessId, Ends, Start
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

        <XmlAttribute> Public Property AccessId As String
        <XmlAttribute> Public Property Start As Integer
        <XmlAttribute> Public Property Ends As Integer

        Public Shared Function Parser(s As String) As SeqRegion
            If String.IsNullOrWhiteSpace(s) Then
                Return New SeqRegion
            End If

            Dim Tokens As String() = s.Split
            Dim acc As String = Tokens(Scan0)
            Dim start As Integer = CInt(Val(Tokens(1)))
            Dim ends As Integer = CInt(Val(Tokens(2)))

            Return New SeqRegion With {
                .AccessId = acc,
                .Start = start,
                .Ends = ends
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"{AccessId} {Start} {Ends}"
        End Function
    End Class
End Namespace
