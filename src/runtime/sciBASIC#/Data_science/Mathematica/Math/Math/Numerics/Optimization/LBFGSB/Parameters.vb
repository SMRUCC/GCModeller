Namespace org.generateme.lbfgsb
    Public NotInheritable Class Parameters

        Public Enum LINESEARCH
            MORETHUENTE_LBFGSPP
            MORETHUENTE_ORIG
            LEWISOVERTON
        End Enum

        Public m As Integer = 6
        Public epsilon As Double = 1.0e-7
        Public epsilon_rel As Double = 1.0e-7
        Public past As Integer = 3
        Public delta As Double = 1.0e-10
        Public max_iterations As Integer = 1000
        Public max_submin As Integer = 10
        Public max_linesearch As Integer = 20
        Public linesearchField As LINESEARCH = LINESEARCH.MORETHUENTE_ORIG
        Public xtol As Double = 1.0e-8 ' MoreThuente
        Public min_step As Double = 1.0e-20
        Public max_step As Double = 1.0e20
        Public ftol As Double = 1.0e-4
        Public wolfe As Double = 0.9
        Public weak_wolfe As Boolean = True
    End Class

End Namespace
