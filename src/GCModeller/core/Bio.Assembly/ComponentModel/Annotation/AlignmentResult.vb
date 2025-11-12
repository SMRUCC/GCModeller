Namespace ComponentModel.Annotation

    ''' <summary>
    ''' A simple alignment annotation result model
    ''' </summary>
    Public Interface IBlastHit

        ''' <summary>
        ''' the target molecule to be annotated
        ''' </summary>
        ''' <returns></returns>
        Property queryName As String
        ''' <summary>
        ''' the id of the subject reference molecule data
        ''' </summary>
        ''' <returns></returns>
        Property hitName As String
        ''' <summary>
        ''' the description of the subject hit reference object, will be used as 
        ''' the annotation text result of the corresponding <see cref="queryName"/> 
        ''' target.
        ''' </summary>
        ''' <returns></returns>
        Property description As String

    End Interface

    ''' <summary>
    ''' the annotation alignment result model with score value
    ''' </summary>
    Public Interface IQueryHits : Inherits IBlastHit

        ''' <summary>
        ''' the alignment silimarity score
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property identities As Double
    End Interface
End Namespace