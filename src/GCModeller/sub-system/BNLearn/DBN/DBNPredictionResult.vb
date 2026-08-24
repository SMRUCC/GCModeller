#Region "Microsoft.VisualBasic::b33e2e8173f89809ff2f70b617bed298, sub-system\BNLearn\DBN\DBNPredictionResult.vb"

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

    '   Total Lines: 78
    '    Code Lines: 35 (44.87%)
    ' Comment Lines: 28 (35.90%)
    '    - Xml Docs: 89.29%
    ' 
    '   Blank Lines: 15 (19.23%)
    '     File Size: 3.21 KB


    '     Class DBNPredictionResult
    ' 
    '         Properties: GeneProbabilities, GeneStateProbabilities, GeneStates, OperonGeneMapping, RNAAbundanceChanges
    ' 
    '         Function: GetGeneRNAAbundanceChange, GetGeneState
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace DBN


    ' ==================== Prediction Result ====================

    ''' <summary>
    ''' Result of a DBN prediction step.
    ''' Contains gene expression states, probability distributions, and RNA abundance
    ''' change rates for coupling with the metabolic network ODEs.
    ''' </summary>
    Public Class DBNPredictionResult

        ''' <summary>Gene/operon ID -> predicted state ("Low", "Medium", or "High")</summary>
        Public Property GeneStates As New Dictionary(Of String, String)

        ''' <summary>Gene/operon ID -> full probability distribution over states [P(Low), P(Med), P(High)]</summary>
        Public Property GeneProbabilities As New Dictionary(Of String, Double())

        ''' <summary>Gene/operon ID -> probability of the predicted (most likely) state</summary>
        Public Property GeneStateProbabilities As New Dictionary(Of String, Double)

        ''' <summary>
        ''' Gene/operon ID -> expected RNA transcript abundance change rate.
        ''' Range: [LowTranscriptionRate, HighTranscriptionRate] (default [0, 1]).
        ''' This is the expected transcription rate: E[rate] = sum(P(state) * rate(state)).
        ''' 
        ''' Usage in ODEs:
        '''   dR/dt = k_synthesis * RNAAbundanceChange - k_degradation * R
        ''' where R is the RNA transcript concentration.
        ''' </summary>
        Public Property RNAAbundanceChanges As New Dictionary(Of String, Double)

        ''' <summary>Operon ID -> list of gene IDs in that operon</summary>
        Public Property OperonGeneMapping As New Dictionary(Of String, List(Of String))


        ''' <summary>
        ''' Get RNA abundance change for a specific gene (based on its operon's prediction).
        ''' Returns 0.0 if the gene is not found in any operon.
        ''' </summary>
        Public Function GetGeneRNAAbundanceChange(geneId As String) As Double
            For Each kv In OperonGeneMapping
                If kv.Value.Contains(geneId) Then
                    If RNAAbundanceChanges.ContainsKey(kv.Key) Then
                        Return RNAAbundanceChanges(kv.Key)
                    End If
                End If
            Next
            ' Also check direct gene ID (not in an operon)
            If RNAAbundanceChanges.ContainsKey(geneId) Then
                Return RNAAbundanceChanges(geneId)
            End If
            Return 0.0
        End Function


        ''' <summary>
        ''' Get predicted state for a specific gene (based on its operon's prediction).
        ''' Returns "Medium" if the gene is not found.
        ''' </summary>
        Public Function GetGeneState(geneId As String) As String
            For Each kv In OperonGeneMapping
                If kv.Value.Contains(geneId) Then
                    If GeneStates.ContainsKey(kv.Key) Then
                        Return GeneStates(kv.Key)
                    End If
                End If
            Next
            If GeneStates.ContainsKey(geneId) Then
                Return GeneStates(geneId)
            End If
            Return "Medium"
        End Function

    End Class


End Namespace
