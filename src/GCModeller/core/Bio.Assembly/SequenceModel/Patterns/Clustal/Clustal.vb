#Region "Microsoft.VisualBasic::4d2dcb197f4c3254e1e4558016c2d69d, core\Bio.Assembly\SequenceModel\Patterns\Clustal\Clustal.vb"

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

    '   Total Lines: 109
    '    Code Lines: 80 (73.39%)
    ' Comment Lines: 12 (11.01%)
    '    - Xml Docs: 91.67%
    ' 
    '   Blank Lines: 17 (15.60%)
    '     File Size: 4.42 KB


    '     Class Clustal
    ' 
    '         Properties: Conservation, Frequency
    ' 
    '         Constructor: (+3 Overloads) Sub New
    ' 
    '         Function: __getSite, __mid, GetEnumerator, IEnumerable_GetEnumerator, Mid
    '                   (+3 Overloads) Save
    ' 
    '         Sub: __initCommon
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace SequenceModel.Patterns.Clustal

    ''' <summary>
    ''' Fasta格式的多序列比对的结果
    ''' </summary>
    Public Class Clustal : Implements ISaveHandle
        Implements IEnumerable(Of FastaSeq)

        Dim _innerList As List(Of FastaSeq)
        Dim _SRChains As SRChain()

        Public ReadOnly Property Frequency As IReadOnlyDictionary(Of Integer, IReadOnlyDictionary(Of Char, Double))
        Public ReadOnly Property Conservation As SR()

        ''' <summary>
        ''' read the alignment result file.
        ''' </summary>
        ''' <param name="path"></param>
        Sub New(path As String)
            Call Me.New(New FastaFile(path))
        End Sub

        Sub New(source As IEnumerable(Of FastaSeq))
            _innerList = source.AsList
            Call Initialize()
        End Sub

        Sub New(fa As FastaFile)
            Call Me.New(source:=fa)
        End Sub

        ''' <summary>
        ''' 计算每一个位点上面的保守性
        ''' </summary>
        Private Sub Initialize()
            _SRChains = SR.FromAlign(_innerList, levels:=_innerList.Count)
            Dim variations As Patterns.PatternModel = Patterns.Frequency(_innerList)
            Dim dict As IReadOnlyDictionary(Of Integer, IReadOnlyDictionary(Of Char, Double)) =
                variations.Residues _
                    .SeqIterator _
                    .ToDictionary(Function(x) x.i,
                                  Function(y)
                                      Return DirectCast(y.value.Alphabets, IReadOnlyDictionary(Of Char, Double))
                                  End Function)
            _Frequency = dict
            _Conservation = dict.Select(Function(x) GetSite(x)).ToArray
        End Sub

        Private Shared Function GetSite(x As KeyValuePair(Of Integer, IReadOnlyDictionary(Of Char, Double))) As SR
            Dim topSite = (From site As KeyValuePair(Of Char, Double) In x.Value
                           Select site
                           Order By site.Value Descending).First
            Dim residue As New SR With {
                .Index = x.Key,
                .Frq = topSite.Value,
                .Residue = topSite.Key
            }
            Return residue
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="left">左端到位点的距离</param>
        ''' <param name="right">右端到位点的距离</param>
        ''' <returns></returns>
        Public Function Mid(left As Integer, right As Integer) As FastaFile
            Dim LQuery = (From x As FastaSeq In _innerList
                          Let midFa As FastaSeq = New FastaSeq With {
                              .Headers = x.Headers,
                              .SequenceData = Span(left, right, x.SequenceData)
                          }
                          Select midFa).ToArray
            Return New FastaFile(LQuery)
        End Function

        Private Shared Function Span(left As Integer, right As Integer, s As String) As String
            s = Strings.Mid(s, 1, s.Length - right)
            s = Strings.Mid(s, left - 1)
            Return s
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of FastaSeq) Implements IEnumerable(Of FastaSeq).GetEnumerator
            For Each fa As FastaSeq In _innerList
                Yield fa
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Save(FilePath$, Encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Return New FastaFile(_innerList).Save(lineBreak:=-1, FilePath, encoding:=Encoding)
        End Function

        Public Function Save(s As Stream, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Return New FastaFile(_innerList).Save(-1, s, encoding)
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield Me.GetEnumerator()
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace
