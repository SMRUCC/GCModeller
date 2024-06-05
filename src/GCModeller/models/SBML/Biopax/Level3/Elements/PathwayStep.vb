#Region "Microsoft.VisualBasic::e7f4a0318f2a49e21a9a1372056f6206, models\SBML\Biopax\Level3\Elements\PathwayStep.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 63
    '    Code Lines: 43 (68.25%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 20 (31.75%)
    '     File Size: 1.79 KB


    ' Class Transport
    ' 
    '     Properties: conversionDirection, displayName, left, name, participantStoichiometry
    '                 right, spontaneous
    ' 
    ' Class TransportWithBiochemicalReaction
    ' 
    ' 
    ' 
    ' Class PathwayStep
    ' 
    '     Properties: stepProcess
    ' 
    ' Class BiochemicalPathwayStep
    ' 
    '     Properties: stepConversion
    ' 
    ' Class stepConversion
    ' 
    ' 
    ' 
    ' Class stepProcess
    ' 
    ' 
    ' 
    ' Class Pathway
    ' 
    '     Properties: displayName, name, organism, pathwayComponent, pathwayOrder
    '                 xref
    ' 
    ' Class pathwayComponent
    ' 
    ' 
    ' 
    ' Class pathwayOrder
    ' 
    ' 
    ' 
    ' Class Interaction
    ' 
    '     Properties: displayName, name, participant
    ' 
    ' Class participant
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports SMRUCC.genomics.Model.Biopax.EntityProperties

Public Class Transport : Inherits RDFEntity
    Public Property participantStoichiometry As participantStoichiometry()
    Public Property displayName As displayName
    Public Property right As EntityProperty()
    Public Property left As EntityProperty()
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
