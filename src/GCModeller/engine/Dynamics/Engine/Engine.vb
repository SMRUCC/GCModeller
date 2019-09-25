Namespace Core

    Public Class Engine

        ReadOnly massTable As MassTable

        Public Function GetMass(names As IEnumerable(Of String)) As IEnumerable(Of Factor)
            Return massTable.GetByKey(names)
        End Function
    End Class
End Namespace