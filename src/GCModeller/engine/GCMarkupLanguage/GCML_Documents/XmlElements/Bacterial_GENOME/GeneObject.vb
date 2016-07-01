Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace GCML_Documents.XmlElements.Bacterial_GENOME

    ''' <summary>
    ''' 基因对象仅是模板信息的承载体，所有的转录动作是发生于转录单元对象之上的
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneObject : Inherits T_MetaCycEntity(Of Slots.Gene)

        ''' <summary>
        ''' NCBI ID
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute("NCBI-AccessionId")> Public Property AccessionId As String
        <XmlElement("COMMON-NAME")> Public Property CommonName As String
        <XmlAttribute> Public Property TranscriptionDirection As String

        ''' <summary>
        ''' 这个基因对象所表达出来的蛋白质分子
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ProteinProduct As String
        ''' <summary>
        ''' 这个基因对象所转录出来的RNA分子
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TranscriptProduct As String

        <XmlType("ProteinProduct")> Public Class Protein : Implements sIdEnumerable
            <XmlAttribute> Public Property Identifier As String Implements sIdEnumerable.Identifier
            <XmlAttribute("Pfam")> Public Property Domains As String()
        End Class

        Public Overrides Function ToString() As String
            Return Identifier
        End Function

        Public Shared Function CastTo(e As Slots.Gene) As GeneObject
            Dim Gene As GeneObject = New GeneObject With {.BaseType = e}
            Gene.Identifier = e.Identifier
            Gene.AccessionId = e.Accession1
            Gene.CommonName = e.CommonName
            Gene.TranscriptionDirection = e.TranscriptionDirection

            Return Gene
        End Function
    End Class
End Namespace