#Region "Microsoft.VisualBasic::aa25fa879f2820afab48741787c74636, core\Bio.Assembly\SequenceModel\FASTA\IO\KSeq.vb"

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

    '   Total Lines: 57
    '    Code Lines: 20 (35.09%)
    ' Comment Lines: 28 (49.12%)
    '    - Xml Docs: 7.14%
    ' 
    '   Blank Lines: 9 (15.79%)
    '     File Size: 2.23 KB


    '     Class KSeq
    ' 
    '         Properties: Seq
    ' 
    '         Function: GetSequenceData, Kmers, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace SequenceModel.FASTA

    ' The MIT License
    '
    '   Copyright (c) 2008, 2009, 2011 Attractive Chaos <attractor@live.co.uk>
    '
    '   Permission is hereby granted, free of charge, to any person obtaining
    '   a copy of this software and associated documentation files (the
    '   "Software"), to deal in the Software without restriction, including
    '   without limitation the rights to use, copy, modify, merge, publish,
    '   distribute, sublicense, and/or sell copies of the Software, and to
    '   permit persons to whom the Software is furnished to do so, subject to
    '   the following conditions:
    '
    '   The above copyright notice and this permission notice shall be
    '   included in all copies or substantial portions of the Software.
    '
    '   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    '   EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    '   MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    '   NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
    '   BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
    '   ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
    '   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    '   SOFTWARE.
    '

    ' Last Modified: 05MAR2012 

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class KSeq : Inherits ISequenceBuilder

        <XmlAttribute> Public Property Seq As Char()

        Public Overrides Function GetSequenceData() As String
            Return New String(Seq)
        End Function

        Public Overrides Function ToString() As String
            Return Name
        End Function

        Public Shared Iterator Function Kmers(seq As ISequenceProvider, k As Integer) As IEnumerable(Of KSeq)
            Dim seq_str As String = seq.GetSequenceData

            For i As Integer = 0 To seq_str.Length - k - 1
                Yield New KSeq With {
                    .Seq = seq_str.Substring(i, length:=k).ToArray
                }
            Next
        End Function
    End Class
End Namespace
