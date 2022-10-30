Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Public Class Reaction

    Public Property entry As String
    Public Property definition As String
    Public Property equation As Equation
    Public Property enzyme As String()

    Friend Shared Function EquationParser(text As String) As Equation
        Dim eq As Equation = Equation.TryParse(text)

        For Each cpd As CompoundSpecieReference In eq.Reactants
            If cpd.ID.IndexOf(","c) > -1 Then
                Dim t = cpd.ID.Split(","c)

                cpd.StoiChiometry = t.Length
                cpd.ID = t(Scan0)
            End If
        Next

        Return eq
    End Function

    Public Overrides Function ToString() As String
        Return definition
    End Function

End Class
