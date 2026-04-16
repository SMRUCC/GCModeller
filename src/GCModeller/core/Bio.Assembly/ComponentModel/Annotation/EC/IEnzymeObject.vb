Namespace ComponentModel.Annotation

    Public Interface IEnzymeObject

        ''' <summary>
        ''' An exact ec number: 1.1.1.1
        ''' An fuzzy ec number pattern: 1.1.-.-
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ECNumber As String

    End Interface

    Public Interface IEnzymeSet

        ReadOnly Property ECNumbers As IEnumerable(Of String)

    End Interface
End Namespace