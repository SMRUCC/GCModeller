#Region "Microsoft.VisualBasic::570c821ecc26dcb665e05b931614585f, sub-system\BNLearn\DBN\DBNODECoupler.vb"

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

    '   Total Lines: 150
    '    Code Lines: 64 (42.67%)
    ' Comment Lines: 59 (39.33%)
    '    - Xml Docs: 76.27%
    ' 
    '   Blank Lines: 27 (18.00%)
    '     File Size: 5.64 KB


    '     Class DBNODECoupler
    ' 
    '         Properties: DBN
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: [Step], GetGeneTranscriptionRate, GetRNATranscriptionRates
    ' 
    '         Sub: Reset, UpdateParametersOnline
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace DBN

    ' ==================== DBN-ODE Coupler ====================

    ''' <summary>
    ''' Coupling interface between the DBN and metabolic network ODEs.
    ''' Provides a clean interface for bidirectional data exchange.
    ''' 
    ''' Usage in the virtual cell simulation loop:
    ''' 
    '''   Dim coupler As New DBNODECoupler(dbn)
    '''   For Each timeStep In simulation
    '''       ' ODEs -> DBN: pass metabolite and TF abundances
    '''       Dim result = coupler.Step(metaboliteConcentrations, tfAbundances)
    '''       
    '''       ' DBN -> ODEs: get transcription rates for ODE integration
    '''       Dim transcriptionRates = coupler.GetRNATranscriptionRates(result)
    '''       
    '''       ' Use transcriptionRates in ODE solver: dR/dt = k_syn * rate - k_deg * R
    '''       ... integrate ODEs for one time step ...
    '''   Next
    ''' </summary>
    Public Class DBNODECoupler

        Private _dbn As DynamicBayesianNetwork
        Private _currentGeneStates As New Dictionary(Of String, String)


        ''' <summary>The underlying DBN</summary>
        Public ReadOnly Property DBN As DynamicBayesianNetwork
            Get
                Return _dbn
            End Get
        End Property


        ''' <summary>Create a coupler for the given DBN</summary>
        Public Sub New(dbn As DynamicBayesianNetwork)
            If dbn Is Nothing Then
                Throw New ArgumentNullException("dbn")
            End If
            _dbn = dbn
        End Sub


        ''' <summary>
        ''' ODEs -> DBN: Execute one prediction step.
        ''' 
        ''' Takes metabolite concentrations and TF abundances from the ODEs,
        ''' runs DBN inference, and returns the prediction result.
        ''' 
        ''' The coupler internally tracks gene states between steps for
        ''' nodes that lack direct evidence (e.g., genes that are also TFs).
        ''' </summary>
        ''' <param name="metaboliteAbundances">Metabolite ID -> concentration (from ODEs)</param>
        ''' <param name="tfAbundances">TF ID -> abundance (from ODEs)</param>
        ''' <returns>Prediction result with gene states and RNA abundance changes</returns>
        Public Function [Step](
            metaboliteAbundances As Dictionary(Of String, Double),
            tfAbundances As Dictionary(Of String, Double)
        ) As DBNPredictionResult

            ' Run DBN prediction with current gene states as fallback
            Dim result = _dbn.PredictNextState(
                metaboliteAbundances,
                tfAbundances,
                _currentGeneStates
            )

            ' Update internal gene states for next iteration
            _currentGeneStates.Clear()
            For Each kv In result.GeneStates
                _currentGeneStates(kv.Key) = kv.Value
            Next

            Return result
        End Function


        ''' <summary>
        ''' DBN -> ODEs: Get RNA transcript abundance change rates for ODE integration.
        ''' 
        ''' Returns a dictionary mapping each gene ID to its expected transcription rate.
        ''' Genes in the same operon share the same rate (co-transcribed).
        ''' 
        ''' Usage in ODEs:
        '''   dR_gene/dt = k_synthesis * transcriptionRates(gene) - k_degradation * R_gene
        ''' </summary>
        Public Function GetRNATranscriptionRates(
            result As DBNPredictionResult
        ) As Dictionary(Of String, Double)

            Dim rates As New Dictionary(Of String, Double)

            For Each kv In result.RNAAbundanceChanges
                Dim operonId = kv.Key
                Dim rate = kv.Value

                ' If this is an operon, assign rate to all member genes
                If result.OperonGeneMapping.ContainsKey(operonId) Then
                    For Each geneId In result.OperonGeneMapping(operonId)
                        rates(geneId) = rate
                    Next
                Else
                    ' Single gene (not in an operon)
                    rates(operonId) = rate
                End If
            Next

            Return rates
        End Function


        ''' <summary>
        ''' DBN -> ODEs: Get RNA abundance change for a specific gene.
        ''' Convenience method for accessing a single gene's transcription rate.
        ''' </summary>
        Public Function GetGeneTranscriptionRate(
            result As DBNPredictionResult,
            geneId As String
        ) As Double
            Return result.GetGeneRNAAbundanceChange(geneId)
        End Function


        ''' <summary>
        ''' ODEs -> DBN: Provide observed gene states for online parameter update.
        ''' 
        ''' After observing actual gene expression (e.g., from RNAseq or reporter assays),
        ''' call this method to incrementally update the DBN's CPT parameters.
        ''' </summary>
        ''' <param name="currentStates">Discrete states at time t</param>
        ''' <param name="nextStates">Discrete states at time t+1</param>
        Public Sub UpdateParametersOnline(
            currentStates As Dictionary(Of String, String),
            nextStates As Dictionary(Of String, String)
        )
            _dbn.UpdateParametersOnline(currentStates, nextStates)
        End Sub


        ''' <summary>Reset the coupler's internal state (gene state tracking)</summary>
        Public Sub Reset()
            _currentGeneStates.Clear()
        End Sub

    End Class


End Namespace
