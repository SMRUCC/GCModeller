Namespace LocalBLAST.Application.BBH.Abstract

    Public Interface IBlastHit
        Property locusId As String
        Property Address As String
    End Interface

    Public Interface IQueryHits : Inherits IBlastHit
        ReadOnly Property identities As Double
    End Interface
End Namespace