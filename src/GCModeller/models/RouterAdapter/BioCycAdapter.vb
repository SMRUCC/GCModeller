Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.BioCyc

Public Class BioCycAdapter



    Sub New(biocyc As Workspace)
        Dim compounds As compounds() = biocyc.compounds.AsEnumerable.ToArray
        Dim reactions As reactions() = biocyc.reactions.AsEnumerable.ToArray

    End Sub

End Class
