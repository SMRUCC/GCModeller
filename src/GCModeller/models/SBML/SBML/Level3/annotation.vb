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

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.rdf_xml

Namespace Level3

    ''' <summary>
    ''' 在SBML文件之中通用的RDF注释信息模型
    ''' </summary>
    <XmlType("Description", [Namespace]:=RDFEntity.XmlnsNamespace)>
    Public Class SbmlAnnotationData : Inherits Description

        ''' <summary>
        ''' the id mapping of current object
        ''' </summary>
        ''' <returns></returns>
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
        <XmlElement([Namespace]:=AnnotationInfo.bqbiol)>
        Public Property hasPart As [is]()
        <XmlElement([Namespace]:=AnnotationInfo.bqbiol)>
        Public Property isHomologTo As [is]()

        Sub New()
            Call MyBase.New
        End Sub
    End Class

    Public Class [is]

        <XmlElement("Bag", [Namespace]:=RDFEntity.XmlnsNamespace)>
        Public Property Bag As Array

        Public Overrides Function ToString() As String
            Return Bag.ToString
        End Function
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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetIdMappings() As IEnumerable(Of String)
            Return GetIdList(Function(a) a.is)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetIdComponents() As IEnumerable(Of String)
            Return GetIdList(Function(a) a.hasPart)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetIdHomolog() As IEnumerable(Of String)
            Return GetIdList(Function(a) a.isHomologTo)
        End Function

        Private Iterator Function GetIdList(maps As Func(Of SbmlAnnotationData, [is]())) As IEnumerable(Of String)
            If RDF Is Nothing Then
                Return
            End If

            If RDF.description Is Nothing Then
                Return
            End If

            For Each map As [is] In maps(RDF.description).SafeQuery
                If Not map.Bag Is Nothing Then
                    For Each item As li In map.Bag.list
                        Yield item.resource.Split("/"c).Last
                    Next
                End If
            Next
        End Function
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
