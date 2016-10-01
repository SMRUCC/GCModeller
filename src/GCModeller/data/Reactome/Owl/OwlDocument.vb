#Region "Microsoft.VisualBasic::76730d814c57796d8f3a7142dba77d03, ..\GCModeller\data\Reactome\Owl\OwlDocument.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.Data.Reactome.OwlDocument.Abstract
Imports SMRUCC.genomics.Data.Reactome.OwlDocument.Nodes
Imports SMRUCC.genomics.Data.Reactome.OwlDocument.XrefNodes

Namespace OwlDocument

    <XmlRoot("RDF")>
    Public Class DocumentFile : Inherits ITextFile

        <ResourceCollection> <XmlElement("BiochemicalReaction")> Public Property BiochemicalReactions As BiochemicalReaction()
        <ResourceCollection> <XmlElement("SmallMolecule")> Public Property SmallMolecules As SmallMolecule()
        <ResourceCollection> <XmlElement> Public Property CellularLocationVocabulary As CellularLocationVocabulary()
        <ResourceCollection> <XmlElement> Public Property UnificationXref As UnificationXref()
        <ResourceCollection> <XmlElement> Public Property SmallMoleculeReference As SmallMoleculeReference()
        <ResourceCollection> <XmlElement> Public Property Provenance As Provenance()
        <ResourceCollection> <XmlElement> Public Property Catalysis As Catalysis()
        <ResourceCollection> <XmlElement> Public Property Protein As Protein()
        <ResourceCollection> <XmlElement> Public Property ProteinReference As ProteinReference()
        <ResourceCollection> <XmlElement> Public Property BioSource As BioSource()
        <ResourceCollection> <XmlElement> Public Property FragmentFeature As FragmentFeature()
        <ResourceCollection> <XmlElement> Public Property SequenceInterval As SequenceInterval()
        <ResourceCollection> <XmlElement> Public Property SequenceSite As SequenceSite()
        <ResourceCollection> <XmlElement> Public Property RelationshipXref As RelationshipXref()
        <ResourceCollection> <XmlElement> Public Property RelationshipTypeVocabulary As RelationshipTypeVocabulary()
        <ResourceCollection> <XmlElement> Public Property PublicationXref As PublicationXref()
        <ResourceCollection> <XmlElement> Public Property Complex As Complex()
        <ResourceCollection> <XmlElement> Public Property Stoichiometry As Stoichiometry()
        <ResourceCollection> <XmlElement> Public Property PhysicalEntity As PhysicalEntity()

        Public Overrides Function ToString() As String
            Return FilePath
        End Function

        <XmlIgnore> Public ReadOnly Property ResourceCollection As Dictionary(Of String, ResourceElement)

        ''' <summary>
        ''' 在执行查询之前必须要执行本方法
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub __initDoc()
            Dim ps = GetType(DocumentFile).GetProperties(Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
            Dim Schema = (From p As PropertyInfo In ps
                          Let attrs As Object() = p.GetCustomAttributes(attributeType:=ResourceCollectionAttribute.attrTypeId, inherit:=True)
                          Where Not attrs.IsNullOrEmpty
                          Select p).ToArray

            _ResourceCollection = New Dictionary(Of String, ResourceElement)

            For Each [property] As PropertyInfo In Schema
                Dim ResourceCollection = (From obj In DirectCast([property].GetValue(Me), IEnumerable) Select DirectCast(obj, ResourceElement)).ToArray
                For Each item As ResourceElement In ResourceCollection
                    Call _ResourceCollection.Add(item.ResourceId, item)
                Next
            Next
        End Sub

        Public Shared Function Load(path As String) As DocumentFile
            Dim strData As String = __trim(FileIO.FileSystem.ReadAllText(path))
            Dim Instance = strData.LoadFromXml(Of DocumentFile)()
            Instance.FilePath = path
            Instance.__initDoc()

            Return Instance
        End Function

        Private Shared Function __trim(owl As String) As String
            Dim bufferBuilder As StringBuilder = New StringBuilder(owl)
            Call bufferBuilder.Replace("bp:", "")
            Call bufferBuilder.Replace("owl:", "")
            Call bufferBuilder.Replace("xmlns:", "xmlns_")
            Call bufferBuilder.Replace("xmlns_xsd", "xmlns:xsd")

            bufferBuilder = New StringBuilder(Regex.Replace(bufferBuilder.ToString, "rdf:datatype=""[^""]+""", ""))
            Call bufferBuilder.Replace("rdf:", "")
            Return bufferBuilder.ToString
        End Function

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Throw New NotImplementedException
        End Function
    End Class
End Namespace
