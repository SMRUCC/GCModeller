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