#Region "Microsoft.VisualBasic::1a223f978b459dec47c25f5e3838f05c, GCModeller\analysis\SequenceToolkit\DNA_Comparative\DeltaSimilarity1998\NucleicAcid.vb"

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

    '   Total Lines: 127
    '    Code Lines: 83
    ' Comment Lines: 26
    '   Blank Lines: 18
    '     File Size: 4.64 KB


    '     Class NucleicAcid
    ' 
    '         Properties: length, UserTag
    ' 
    '         Constructor: (+5 Overloads) Sub New
    '         Function: __createSigma, CreateFragments, GetValue, slideWindows
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Conversion

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' 为了加快计算速度而生成的窗口计算缓存，请注意，在生成缓存的时候已经进行了并行化，所以在内部生成缓存的时候，不需要再进行并行化了
    ''' </summary>
    ''' <remarks></remarks>
    Public Class NucleicAcid

        Protected Friend biasTable As New Dictionary(Of String, Double)

        ''' <summary>
        ''' 为了防止反复重新创建划窗而构建出来的计算数据缓存
        ''' </summary>
        Protected Friend DNA_segments As SlideWindow(Of DNA)()
        Protected Friend ReadOnly nt As DNA()

        Public ReadOnly Property UserTag As String

        Public ReadOnly Property length As Integer
            Get
                Return nt.Length
            End Get
        End Property

        ''' <summary>
        ''' Get value by using a paired of base.
        ''' </summary>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <returns></returns>
        Public Function GetValue(X As DNA, Y As DNA) As Double
            Return biasTable($"{ToChar(X)} -> {ToChar(Y)}")
        End Function

        ''' <summary>
        ''' 窗口数据或者缓存数据
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <remarks></remarks>
        Sub New(nt As DNA())
            Me.nt = nt

            ' 20190622
            ' 因为__createSigma函数需要这个滑窗数据，所以需要先于__createSigma函数进行调用
            ' 因为全基因可能会非常长，所以在这里就不可以使用通用的滑窗数据创建方法了
            ' 否则会非常慢
            DNA_segments = slideWindows().ToArray

            For Each X As (a As DNA, B As DNA) In {
                (DNA.dAMP, DNA.dAMP),
                (DNA.dAMP, DNA.dCMP),
                (DNA.dAMP, DNA.dGMP),
                (DNA.dAMP, DNA.dTMP),
                (DNA.dCMP, DNA.dAMP),
                (DNA.dCMP, DNA.dCMP),
                (DNA.dCMP, DNA.dGMP),
                (DNA.dCMP, DNA.dTMP),
                (DNA.dGMP, DNA.dAMP),
                (DNA.dGMP, DNA.dCMP),
                (DNA.dGMP, DNA.dGMP),
                (DNA.dGMP, DNA.dTMP),
                (DNA.dTMP, DNA.dAMP),
                (DNA.dTMP, DNA.dCMP),
                (DNA.dTMP, DNA.dGMP),
                (DNA.dTMP, DNA.dTMP)
            }
                With __createSigma(Me, X.a, X.B)
                    Call biasTable.Add(.Key, .Value)
                End With
            Next
        End Sub

        Private Iterator Function slideWindows() As IEnumerable(Of SlideWindow(Of DNA))
            Dim len = nt.Length

            For i As Integer = 0 To len - 2
                Yield New SlideWindow(Of DNA) With {
                    .Index = i,
                    .Items = {nt(i), nt(i + 1)},
                    .left = .Index
                }
            Next
        End Function

        ''' <summary>
        ''' Fasta序列会自动使用<see cref="FastaSeq.Title"/>来作为序列的<see cref="UserTag"/>
        ''' </summary>
        ''' <param name="nt"></param>
        Sub New(nt As FastaSeq)
            Call Me.New(New NucleotideModels.NucleicAcid(nt, strict:=False).ToArray)
            Me.UserTag = nt.Title
        End Sub

        Sub New(nt$)
            Call Me.New(New NucleotideModels.NucleicAcid(nt).ToArray)
        End Sub

        Sub New(nt As NucleotideModels.NucleicAcid)
            Call Me.New(nt.ToArray)
            Me.UserTag = nt.UserTag
        End Sub

        Private Sub New(nt As IEnumerable(Of DNA))
            Call Me.New(nt.ToArray)
        End Sub

        Public Iterator Function CreateFragments(winSize%, step%) As IEnumerable(Of NucleicAcid)
            For Each region In nt.SlideWindows(winSize, offset:=[step])
                Yield New NucleicAcid(region)
            Next
        End Function

        Private Shared Function __createSigma(nt As NucleicAcid,
                                              X As DNA,
                                              Y As DNA) As KeyValuePair(Of String, Double)
            Dim KEY As String = $"{ToChar(X)} -> {ToChar(Y)}"
            Dim n As Double = GenomeSignatures.DinucleotideBIAS_p(nt, X, Y)
            Return New KeyValuePair(Of String, Double)(KEY, n)
        End Function
    End Class
End Namespace
