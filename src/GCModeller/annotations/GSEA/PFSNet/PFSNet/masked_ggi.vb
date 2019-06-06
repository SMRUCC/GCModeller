Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure

<HideModuleName> Module masked_ggi

    <Extension>
    Public Function ggi_mask(ggi As IEnumerable(Of GraphEdge), genelist As Index(Of String))
        Return ggi _
            .Select(Function(i)
                        If i.g1 Like genelist AndAlso i.g2 Like genelist Then
                            Return True
                        Else
                            Return False
                        End If
                    End Function) _
            .ToArray
    End Function
End Module
