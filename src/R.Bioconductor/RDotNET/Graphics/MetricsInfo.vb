Namespace Graphics

    Public Structure MetricsInfo
        Public Property Ascent() As Double
            Get
                Return m_Ascent
            End Get
            Set
                m_Ascent = Value
            End Set
        End Property
        Private m_Ascent As Double

        Public Property Descent() As Double
            Get
                Return m_Descent
            End Get
            Set
                m_Descent = Value
            End Set
        End Property
        Private m_Descent As Double

        Public Property Width() As Double
            Get
                Return m_Width
            End Get
            Set
                m_Width = Value
            End Set
        End Property
        Private m_Width As Double
    End Structure
End Namespace