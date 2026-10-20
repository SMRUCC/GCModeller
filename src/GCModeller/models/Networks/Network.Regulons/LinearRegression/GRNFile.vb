Imports System.IO
Imports System.Runtime.CompilerServices

Namespace LinearRegression

    Module GRNFile

        <Extension>
        Public Sub Save(model As GeneRegulatoryNetwork, zip As Stream)

        End Sub

        Public Function Load(zip As Stream) As GeneRegulatoryNetwork

        End Function
    End Module
End Namespace