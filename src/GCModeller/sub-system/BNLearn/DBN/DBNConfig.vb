#Region "Microsoft.VisualBasic::b71a9665585a6b34c4ba22cf95799ee7, sub-system\BNLearn\DBN\DBNConfig.vb"

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

    '   Total Lines: 52
    '    Code Lines: 13 (25.00%)
    ' Comment Lines: 24 (46.15%)
    '    - Xml Docs: 95.83%
    ' 
    '   Blank Lines: 15 (28.85%)
    '     File Size: 2.19 KB


    '     Class DBNConfig
    ' 
    '         Properties: BasalTranscriptionRate, HighThreshold, HighTranscriptionRate, LowThreshold, LowTranscriptionRate
    '                     OnlineLearningRate, Seed, SmoothingAlpha, UseMultinomialSampling
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace DBN


    ' ==================== DBN Configuration ====================

    ''' <summary>
    ''' Configuration options for the Dynamic Bayesian Network.
    ''' Controls discretization thresholds, smoothing, transcription rate mapping, etc.
    ''' </summary>
    Public Class DBNConfig

        ''' <summary>
        ''' Smoothing parameter for parameter learning (Dirichlet prior concentration).
        ''' Larger values give more weight to the topology-based prior.
        ''' When alpha = 0: pure Maximum Likelihood Estimation (data only).
        ''' When alpha is large: prior dominates (topology only).
        ''' Default = 1.0 (Laplace smoothing with topology prior).
        ''' </summary>
        Public Property SmoothingAlpha As Double = 1.0

        ''' <summary>
        ''' If true, sample from the probability distribution (stochastic prediction).
        ''' If false, take the most likely state (deterministic, argmax).
        ''' Default = false (deterministic).
        ''' </summary>
        Public Property UseMultinomialSampling As Boolean = False

        ''' <summary>Lower threshold for discretization (values below this = "Low")</summary>
        Public Property LowThreshold As Double = 0.33

        ''' <summary>Upper threshold for discretization (values above this = "High")</summary>
        Public Property HighThreshold As Double = 0.66

        ''' <summary>Transcription rate for "High" expression state</summary>
        Public Property HighTranscriptionRate As Double = 1.0

        ''' <summary>Transcription rate for "Medium" expression state (basal)</summary>
        Public Property BasalTranscriptionRate As Double = 0.5

        ''' <summary>Transcription rate for "Low" expression state</summary>
        Public Property LowTranscriptionRate As Double = 0.0

        ''' <summary>Random seed for reproducible stochastic sampling</summary>
        Public Property Seed As Integer = 42

        ''' <summary>Learning rate for online parameter updates (exponential moving average)</summary>
        Public Property OnlineLearningRate As Double = 0.1

    End Class


End Namespace
