Namespace Kinetics

    Public MustInherit Class KineticsLDM

        ''' <summary>
        ''' The formulation evaluation.
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function GetValue() As Double

#Region "LDM opr LDM"

        Public Shared Operator *(x As KineticsLDM, y As KineticsLDM) As Double
            Return x.GetValue * y.GetValue
        End Operator

        Public Shared Operator +(x As KineticsLDM, y As KineticsLDM) As Double
            Return x.GetValue + y.GetValue
        End Operator

        Public Shared Operator -(x As KineticsLDM, y As KineticsLDM) As Double
            Return x.GetValue - y.GetValue
        End Operator

        Public Shared Operator /(x As KineticsLDM, y As KineticsLDM) As Double
            Return x.GetValue / y.GetValue
        End Operator

        Public Shared Operator Mod(x As KineticsLDM, y As KineticsLDM) As Double
            Return x.GetValue Mod y.GetValue
        End Operator

        Public Shared Operator \(x As KineticsLDM, y As KineticsLDM) As Double
            Return CDbl(x.GetValue \ y.GetValue)
        End Operator

        Public Shared Operator ^(x As KineticsLDM, y As KineticsLDM) As Double
            Return x.GetValue ^ y.GetValue
        End Operator
#End Region

#Region "LDM opr <n>"

        Public Shared Operator *(x As KineticsLDM, y As Double) As Double
            Return x.GetValue * y
        End Operator

        Public Shared Operator +(x As KineticsLDM, y As Double) As Double
            Return x.GetValue + y
        End Operator

        Public Shared Operator -(x As KineticsLDM, y As Double) As Double
            Return x.GetValue - y
        End Operator

        Public Shared Operator /(x As KineticsLDM, y As Double) As Double
            Return x.GetValue / y
        End Operator

        Public Shared Operator Mod(x As KineticsLDM, y As Double) As Double
            Return x.GetValue Mod y
        End Operator

        Public Shared Operator \(x As KineticsLDM, y As Double) As Double
            Return CDbl(x.GetValue \ y)
        End Operator

        Public Shared Operator ^(x As KineticsLDM, y As Double) As Double
            Return x.GetValue ^ y
        End Operator

#End Region

#Region "<n> opr LDM"

        Public Shared Operator *(x As Double, y As KineticsLDM) As Double
            Return x * y.GetValue
        End Operator

        Public Shared Operator +(x As Double, y As KineticsLDM) As Double
            Return x + y.GetValue
        End Operator

        Public Shared Operator -(x As Double, y As KineticsLDM) As Double
            Return x - y.GetValue
        End Operator

        Public Shared Operator /(x As Double, y As KineticsLDM) As Double
            Return x / y.GetValue
        End Operator

        Public Shared Operator Mod(x As Double, y As KineticsLDM) As Double
            Return x Mod y.GetValue
        End Operator

        Public Shared Operator \(x As Double, y As KineticsLDM) As Double
            Return CDbl(x \ y.GetValue)
        End Operator

        Public Shared Operator ^(x As Double, y As KineticsLDM) As Double
            Return x ^ y.GetValue
        End Operator
#End Region

    End Class
End Namespace