#Region "Microsoft.VisualBasic::3fde3df0c5ca3c0d7089847e91d6c5b2, GCModeller\analysis\SequenceToolkit\DNA_Comparative\GCOutlier.vb"

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

    '   Total Lines: 104
    '    Code Lines: 76
    ' Comment Lines: 13
    '   Blank Lines: 15
    '     File Size: 3.80 KB


    ' Module GCOutlier
    ' 
    '     Function: GetMethod, OutlierAnalysis
    '     Class lociX
    ' 
    '         Properties: loci, qLevel, right, Title, value
    ' 
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' ``GC%``异常点分析
''' </summary>
Public Module GCOutlier

    Public Function GetMethod(name As String) As NtProperty
        Select Case LCase(name)
            Case "gcskew"
                Return AddressOf GCSkew
            Case "gccontent"
                Return AddressOf GCContent
            Case Else
                Return AddressOf GCContent
        End Select
    End Function

    ''' <summary>
    ''' 计算分析DNA序列之上的特别突出的位点
    ''' </summary>
    ''' <param name="mla"></param>
    ''' <param name="quantiles"></param>
    ''' <param name="winsize"></param>
    ''' <param name="steps"></param>
    ''' <param name="slideSize"></param>
    ''' <param name="method"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function OutlierAnalysis(mla As IEnumerable(Of FastaSeq), quantiles As Double(),
                                     Optional winsize As Integer = 250,
                                     Optional steps As Integer = 50,
                                     Optional slideSize As Integer = 5,
                                     Optional method As NtProperty = Nothing) As IEnumerable(Of lociX)

        Dim data As NamedValue(Of Double())() = GCData(mla, winsize, steps, method)
        Dim iSeq As Integer() = data(Scan0).Value.Sequence.ToArray
        Dim seq As lociX()() = New lociX(slideSize - 1)() {}

        For i As Integer = 0 To slideSize - 1
            Dim a As lociX() = New lociX(data.Length - 1) {}

            For Each x In data.SeqIterator
                a(x.i) = New lociX With {
                    .Title = x.value.Name
                }
            Next

            seq(i) = a
        Next

        For Each index As SlideWindow(Of Integer) In iSeq.SlideWindows(slideSize)
            Dim tmp As New List(Of lociX)

            For Each i In index.SeqIterator ' index里面的数据是fasta序列上面的位点坐标
                Dim a As lociX() = seq(i.i)

                For Each x In data.SeqIterator
                    a(x.i).value = x.value.Value(i.value)
                    a(x.i).loci = i.value
                Next

                tmp += a
            Next

            Dim result = tmp.SelectByQuantile(Function(x) x.value * 1000, quantiles,,).ToArray

            For Each lv In result
                For Each x As lociX In lv.value
                    Yield New lociX With {
                        .value = x.value,
                        .loci = x.loci * steps + 1,
                        .qLevel = lv.Tag,
                        .right = (x.loci + 1) * steps,
                        .Title = x.Title
                    }
                Next
            Next
        Next
    End Function

    Public Class lociX
        Public Property Title As String
        Public Property loci As Integer
        Public Property value As Double
        Public Property qLevel As Double
        Public Property right As Integer

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Module
