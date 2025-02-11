Namespace EMD

    ''' <summary>
    ''' @author Telmo Menezes (telmo@telmomenezes.com)
    ''' 
    ''' </summary>
    Public Class Signature
        Private numberOfFeaturesField As Integer
        Private featuresField As Feature()
        Private weightsField As Double()

        Public Overridable Property NumberOfFeatures As Integer
            Get
                Return numberOfFeaturesField
            End Get
            Set(value As Integer)
                numberOfFeaturesField = value
            End Set
        End Property


        Public Overridable Property Features As Feature()
            Get
                Return featuresField
            End Get
            Set(value As Feature())
                featuresField = value
            End Set
        End Property


        Public Overridable Property Weights As Double()
            Get
                Return weightsField
            End Get
            Set(value As Double())
                weightsField = value
            End Set
        End Property

    End Class
End Namespace
