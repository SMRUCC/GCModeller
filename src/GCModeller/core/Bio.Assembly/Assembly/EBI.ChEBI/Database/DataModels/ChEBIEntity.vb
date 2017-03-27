Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.EBI.ChEBI

    ''' <summary>
    ''' The complete entity including synonyms, database links and chemical structures.
    ''' (ChEBI数据库之中的一个对某种代谢物的完整的描述的数据模型)
    ''' </summary>
    ''' <remarks>
    ''' 这个对象的XML布局是根据ChEBI的Web Services来生成的，所以为了能够正确的读取ChEBI的数据，不能够再随意修改了
    ''' </remarks>
    Public Class ChEBIEntity : Implements INamedValue

        Public Property chebiId As String Implements INamedValue.Key
        Public Property chebiAsciiName As String
        Public Property definition As String
        Public Property status As String
        Public Property smiles As String
        Public Property inchi As String
        Public Property inchiKey As String
        Public Property charge As Integer
        Public Property mass As Double
        Public Property entityStar As Integer
        <XmlElement>
        Public Property Synonyms As Synonyms()
        Public Property Formulae As Formulae
        <XmlElement>
        Public Property RegistryNumbers As RegistryNumbers()
        <XmlElement>
        Public Property ChemicalStructures As ChemicalStructures()
        <XmlElement>
        Public Property DatabaseLinks As DatabaseLinks()
        <XmlElement>
        Public Property OntologyParents As OntologyParents()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class OntologyParents
        Public Property chebiName As String
        Public Property chebiId As String
        Public Property type As String
        Public Property status As String
        Public Property cyclicRelationship As Boolean
    End Class

    Public Class DatabaseLinks
        Public Property data As String
        Public Property type As String
    End Class

    Public Class ChemicalStructures
        Public Property [structure] As String
        Public Property type As String
        Public Property dimension As String
        Public Property defaultStructure As String
    End Class

    Public Class Synonyms
        Public Property data As String
        Public Property source As String
        Public Property type As String
    End Class

    Public Class RegistryNumbers
        Public Property data As String
        Public Property source As String
        Public Property type As String
    End Class

    Public Class Formulae
        Public Property data As String
        Public Property source As String
    End Class
End Namespace