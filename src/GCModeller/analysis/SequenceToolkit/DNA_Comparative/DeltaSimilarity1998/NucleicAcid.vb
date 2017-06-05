#Region "Microsoft.VisualBasic::c41ad19c354b2e85f92d46a72c05fd1d, ..\GCModeller\analysis\SequenceToolkit\DNA_Comparative\Sigma\NucleicAcid.vb"

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
    Public Class NucleicAcid : Inherits NucleotideModels.NucleicAcid

        Protected Friend __biasTable As New Dictionary(Of String, Double)

        ''' <summary>
        ''' 为了防止反复重新创建划窗而构建出来的计算数据缓存
        ''' </summary>
        Protected Friend __DNA_segments As SlideWindow(Of DNA)()

        ''' <summary>
        ''' Get value by using a paired of base.
        ''' </summary>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <returns></returns>
        Public Function GetValue(X As DNA, Y As DNA) As Double
            Return __biasTable($"{ToChar(X)} -> {ToChar(Y)}")
        End Function

        ''' <summary>
        ''' 窗口数据或者缓存数据
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <remarks></remarks>
        Sub New(nt As DNA())
            Call MyBase.New(nt)

            ' 因为__createSigma函数需要这个滑窗数据，所以需要先于__createSigma函数进行调用
            __DNA_segments = Me.SlideWindows(2, offset:=1).ToArray

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
                    Call __biasTable.Add(.Key, .Value)
                End With
            Next
        End Sub

        ''' <summary>
        ''' Fasta序列会自动使用<see cref="FastaToken.Title"/>来作为序列的<see cref="UserTag"/>
        ''' </summary>
        ''' <param name="nt"></param>
        Sub New(nt As FastaToken)
            Call Me.New(New NucleotideModels.NucleicAcid(nt, strict:=False).ToArray)
            Me.UserTag = nt.Title
        End Sub

        Sub New(nt$)
            Call Me.New(New NucleotideModels.NucleicAcid(nt).ToArray)
        End Sub

        Private Shared Function __createSigma(nt As NucleicAcid,
                                              X As DNA,
                                              Y As DNA) As KeyValuePair(Of String, Double)
            Dim KEY As String = $"{ToChar(X)} -> {ToChar(Y)}"
            Dim n As Double = GenomeSignatures.DinucleotideBIAS_p(nt, X, Y)
            Return New KeyValuePair(Of String, Double)(KEY, n)
        End Function
    End Class
End Namespace