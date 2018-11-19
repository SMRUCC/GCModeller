Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Module equationparsertest

    Sub Main()

        Dim eq1$ = "C00323 + 3 C00083 <=> C15525 + 4 C00010 + 3 C00011"
        Dim eq2$ = "C00024 + C00400 <=> C00010 + C09870"
        Dim eq3$ = "C00024 + C00083 + 2C00005 + 4C00080 <=> C02843 + C00010 + C00011 + 2C00006 + C00001"

        Call Equation.TryParse(eq1).GetJson.__DEBUG_ECHO
        Call Equation.TryParse(eq2).GetJson.__DEBUG_ECHO
        Call Equation.TryParse(eq3).GetJson.__DEBUG_ECHO

        Pause()
    End Sub
End Module
