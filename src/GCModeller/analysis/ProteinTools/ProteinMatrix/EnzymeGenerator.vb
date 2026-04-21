#Region "Microsoft.VisualBasic::f75e3c9707dad9af16e2e3aae47c4467, analysis\ProteinTools\ProteinMatrix\EnzymeGenerator.vb"

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

'   Total Lines: 66
'    Code Lines: 51 (77.27%)
' Comment Lines: 2 (3.03%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 13 (19.70%)
'     File Size: 2.32 KB


' Module EnzymeGenerator
' 
'     Function: BuildProteinSequence, MakeModel
' 
'     Sub: CreateTrainingSet
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.MachineLearning.Transformer
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Slicer

Public Module EnzymeGenerator

    <Extension>
    Private Sub CreateTrainingSet(enzymes As IEnumerable(Of FastaSeq),
                                  k As Integer,
                                  <Out> ByRef ec_number As List(Of List(Of String)),
                                  <Out> ByRef kmers As List(Of List(Of String)))

        ec_number = New List(Of List(Of String))
        kmers = New List(Of List(Of String))

        For Each seq As FastaSeq In enzymes
            Dim kmer As KSeq() = KSeq.Kmers(seq, k).ToArray

            Call ec_number.Add(seq.Headers(0).Split("."c).AsList)
            Call kmers.Add(kmer.Select(Function(ki) ki.seq).AsList)
        Next
    End Sub

    <Extension>
    Public Function MakeModel(enzymes As IEnumerable(Of FastaSeq), Optional k As Integer = 3) As TransformerModel
        Dim ec_number As List(Of List(Of String)) = Nothing
        Dim kmers As List(Of List(Of String)) = Nothing

        Call enzymes.CreateTrainingSet(k, ec_number, kmers)

        ' Transformer setup
        Dim batchSize = 2
        Dim embeddingSize = 8
        Dim dk = 4
        Dim dv = 4
        Dim h = 2
        Dim dff = 16
        Dim Nx = 2
        Dim dropout = 0.0
        Dim transformer As New TransformerModel(Nx, embeddingSize, dk, dv, h, dff, batchSize, dropout, ec_number, kmers)
        ' Training
        Dim nrEpochs = 10
        Dim nrTrainingSteps = 10
        Dim learningRate = 0.01

        Call transformer.Train(nrEpochs, nrTrainingSteps, learningRate, batchSize, ec_number, kmers)

        Return transformer
    End Function

    <Extension>
    Public Iterator Function BuildProteinSequence(model As TransformerModel, ec_numbers As IEnumerable(Of String)) As IEnumerable(Of FastaSeq)
        For Each id As String In ec_numbers
            Dim predict = model.Infer(id.Split("."c))
            Dim seq As String

            Yield New FastaSeq With {
                .Headers = {id},
                .SequenceData = seq
            }
        Next
    End Function

End Module

