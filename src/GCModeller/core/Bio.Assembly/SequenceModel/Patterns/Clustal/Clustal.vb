#Region "Microsoft.VisualBasic::ff28ed950d038a4f90a34d3f7852206f, ..\GCModeller\core\Bio.Assembly\SequenceModel\Patterns\Clustal\Clustal.vb"

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
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace SequenceModel.Patterns.Clustal

    ''' <summary>
    ''' Fasta格式的多序列比对的结果
    ''' </summary>
    Public Class Clustal : Inherits ITextFile
        Implements IEnumerable(Of FASTA.FastaToken)

        Dim _innerList As List(Of FastaToken)
        Dim _SRChains As SRChain()

        Public ReadOnly Property Frequency As IReadOnlyDictionary(Of Integer, IReadOnlyDictionary(Of Char, Double))

        Sub New(path As String)
            Call Me.New(New FastaFile(path))
            FilePath = path
        End Sub

        Sub New(source As IEnumerable(Of FastaToken))
            _innerList = source.ToList
            Call __initCommon()
        End Sub

        Sub New(fa As FastaFile)
            Call Me.New(source:=fa)
        End Sub

        Public ReadOnly Property Conservation As SR()

        ''' <summary>
        ''' 计算每一个位点上面的保守性
        ''' </summary>
        Private Sub __initCommon()
            _SRChains = SR.FromAlign(_innerList, levels:=_innerList.Count)
            Dim variations As Patterns.PatternModel = Patterns.Frequency(_innerList)
            Dim dict As IReadOnlyDictionary(Of Integer, IReadOnlyDictionary(Of Char, Double)) =
                variations.Residues.SeqIterator _
               .ToDictionary(Function(x) x.i,
                             Function(y) DirectCast(y.value.Alphabets, IReadOnlyDictionary(Of Char, Double))) _
                                        .As(Of IReadOnlyDictionary(Of Integer, IReadOnlyDictionary(Of Char, Double)))
            _Frequency = dict
            _Conservation = dict.ToArray(Function(x) __getSite(x))
        End Sub

        Private Shared Function __getSite(x As KeyValuePair(Of Integer, IReadOnlyDictionary(Of Char, Double))) As SR
            Dim topSite = (From site As KeyValuePair(Of Char, Double) In x.Value
                           Select site
                           Order By site.Value Descending).First
            Dim residue As SR = New SR With {
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
            Dim LQuery = (From x As FastaToken In _innerList
                          Let midFa As FastaToken = New FastaToken With {
                              .Attributes = x.Attributes,
                              .SequenceData = __mid(left, right, x.SequenceData)
                          }
                          Select midFa).ToArray
            Return New FastaFile(LQuery)
        End Function

        Private Shared Function __mid(left As Integer, right As Integer, s As String) As String
            s = Strings.Mid(s, 1, s.Length - right)
            s = Strings.Mid(s, left - 1)
            Return s
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of FastaToken) Implements IEnumerable(Of FastaToken).GetEnumerator
            For Each fa As FastaToken In _innerList
                Yield fa
            Next
        End Function

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Return New FastaFile(_innerList).Save(-1, getPath(FilePath), getEncoding(Encoding))
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield Me.GetEnumerator()
        End Function
    End Class
End Namespace
