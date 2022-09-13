#Region "Microsoft.VisualBasic::232bbe365947ea59bd014276a37bb70c, GCModeller\models\SBML\SBML\Level3\annotation.vb"

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

    '   Total Lines: 61
    '    Code Lines: 44
    ' Comment Lines: 3
    '   Blank Lines: 14
    '     File Size: 2.06 KB


    '     Class SbmlAnnotationData
    ' 
    '         Properties: [is], hasTaxon, isDescribedBy, isVersionOf, occursIn
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class [is]
    ' 
    '         Properties: Bag
    ' 
    '     Class annotation
    ' 
    '         Properties: RDF
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class AnnotationInfo
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml

Namespace Level3

    ''' <summary>
    ''' 在SBML文件之中通用的RDF注释信息模型
    ''' </summary>
    <XmlType("Description", [Namespace]:=RDFEntity.XmlnsNamespace)>
    Public Class SbmlAnnotationData : Inherits Description

        <XmlElement([Namespace]:=AnnotationInfo.bqbiol)>
        Public Property [is] As [is]()
        <XmlElement([Namespace]:=AnnotationInfo.bqbiol)>
        Public Property isDescribedBy As [is]()
        <XmlElement([Namespace]:=AnnotationInfo.bqbiol)>
        Public Property isVersionOf As [is]
        <XmlElement([Namespace]:=AnnotationInfo.bqbiol)>
        Public Property hasTaxon As [is]
        <XmlElement([Namespace]:=AnnotationInfo.bqbiol)>
        Public Property occursIn As [is]

        Sub New()
            Call MyBase.New
        End Sub
    End Class

    Public Class [is]

        <XmlElement("Bag", [Namespace]:=RDFEntity.XmlnsNamespace)>
        Public Property Bag As Array
    End Class

    <XmlType("annotation", [Namespace]:=sbmlXmlns)>
    Public Class annotation

        <XmlElement("RDF", [Namespace]:=RDFEntity.XmlnsNamespace)>
        Public Property RDF As AnnotationInfo

        <XmlNamespaceDeclarations()>
        Public xmlns As New XmlSerializerNamespaces

        Sub New()
            xmlns.Add("rdf", RDFEntity.XmlnsNamespace)
        End Sub
    End Class

    <XmlType("annoinfo", [Namespace]:=RDFEntity.XmlnsNamespace)>
    Public Class AnnotationInfo : Inherits RDF(Of SbmlAnnotationData)

        Public Const bqbiol As String = "http://biomodels.net/biology-qualifiers/"
        Public Const bqmodel As String = "http://biomodels.net/model-qualifiers/"

        Sub New()
            Call MyBase.New

            Call MyBase.xmlns.Add("bqbiol", "http://biomodels.net/biology-qualifiers/")
            Call MyBase.xmlns.Add("bqmodel", "http://biomodels.net/model-qualifiers/")
        End Sub
    End Class
End Namespace
