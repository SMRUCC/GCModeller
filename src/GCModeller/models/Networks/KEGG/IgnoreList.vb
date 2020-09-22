Imports Microsoft.VisualBasic.ComponentModel.Collection

Module IgnoreList

    Public Function InOrganicPrimary() As Index(Of String)
        Return {
            "C00001", ' H2O
            "C00007", ' O2
            "C00011"  ' CO2
        }
    End Function
End Module
