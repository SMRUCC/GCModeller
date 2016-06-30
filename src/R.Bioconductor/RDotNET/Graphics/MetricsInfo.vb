Namespace Graphics

    Public Structure MetricsInfo
        Public Property Ascent() As Double
            Get
                Return m_Ascent
            End Get
            Set(value As Double)
                m_Ascent = value
            End Set
        End Property
        Private m_Ascent As Double

        Public Property Descent() As Double
            Get
                Return m_Descent
            End Get
            Set(value As Double)
                m_Descent = value
            End Set
        End Property
        Private m_Descent As Double

        Public Property Width() As Double
            Get
                Return m_Width
            End Get
            Set(value As Double)
                m_Width = value
            End Set
        End Property
        Private m_Width As Double
    End Structure
End Namespace