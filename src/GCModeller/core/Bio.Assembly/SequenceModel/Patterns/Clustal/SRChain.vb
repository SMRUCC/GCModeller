#Region "Microsoft.VisualBasic::47b273ba75f6edc3c69c35315efa13d9, GCModeller\core\Bio.Assembly\SequenceModel\Patterns\Clustal\SRChain.vb"

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

    '   Total Lines: 36
    '    Code Lines: 30
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.12 KB


    '     Class SRChain
    ' 
    '         Properties: Hits, IsEmpty, lstSR, Name
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

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
