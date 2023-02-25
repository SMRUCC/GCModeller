Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports SMRUCC.genomics.Model.Biopax.EntityProperties

Public Class Transport : Inherits RDFEntity
    Public Property participantStoichiometry As participantStoichiometry()
    Public Property displayName As displayName
    Public Property right As right()
    Public Property left As left()
    Public Property name As name
    Public Property spontaneous As spontaneous
    Public Property conversionDirection As conversionDirection
End Class

Public Class TransportWithBiochemicalReaction : Inherits Transport


End Class

Public Class PathwayStep : Inherits RDFEntity

    Public Property stepProcess As stepProcess
End Class

Public Class BiochemicalPathwayStep : Inherits PathwayStep
    Public Property stepConversion As stepConversion

End Class

Public Class stepConversion : Inherits EntityProperty

End Class

Public Class stepProcess : Inherits EntityProperty

End Class

Public Class Pathway : Inherits RDFEntity
    <XmlElement> Public Property pathwayOrder As pathwayOrder()
    <XmlElement> Public Property pathwayComponent As pathwayComponent()
    <XmlElement> Public Property xref As xref()
    Public Property displayName As displayName
    Public Property name As name
    Public Property organism As organism
End Class

Public Class pathwayComponent : Inherits EntityProperty

End Class

Public Class pathwayOrder : Inherits EntityProperty

End Class

Public Class Interaction : Inherits RDFEntity
    Public Property displayName As displayName
    Public Property name As name()
    Public Property participant As participant()
End Class

Public Class participant : Inherits RDFProperty

End Class