#Region "Microsoft.VisualBasic::1e901f856cb4254ad6425b4e14f1d531, ..\core\Bio.Assembly\SequenceModel\Patterns\Clustal\SRChain.vb"

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

Imports System.Text
Imports System.Xml.Serialization
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace SequenceModel.Patterns.Clustal

    Public Class SRChain

        <XmlElement> Public Property lstSR As SR()
        <XmlAttribute>
        Public Property Name As String

        Public ReadOnly Property IsEmpty As Boolean
            Get
                Dim l As Integer = (From x As SR In lstSR
                                    Where x.Residue = "-"c
                                    Select 1).Sum
                Return l = lstSR.Length
            End Get
        End Property

        Public ReadOnly Property Hits As Integer
            Get
                Dim l As Integer = (From x As SR In lstSR
                                    Where x.Residue <> "-"c
                                    Select 1).Sum
                Return l
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return New String(lstSR.Select(Function(x) x.Residue).ToArray)
        End Function
    End Class
End Namespace
