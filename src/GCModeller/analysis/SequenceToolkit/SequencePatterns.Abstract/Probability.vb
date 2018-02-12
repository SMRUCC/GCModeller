Public Structure Probability

    Dim region As Residue()
    Dim pvalue#
    Dim score#

    Public Structure Residue
        Dim frequency As Dictionary(Of Char, Double)

    End Structure
End Structure
