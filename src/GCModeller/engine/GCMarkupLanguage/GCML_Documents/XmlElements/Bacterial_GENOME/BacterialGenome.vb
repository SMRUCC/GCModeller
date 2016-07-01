Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.DatabaseServices.SabiorkKineticLaws.TabularDump
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME
Imports Microsoft.VisualBasic

Namespace GCML_Documents.XmlElements.Bacterial_GENOME

    <XmlType("BacterialGenome", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/GCML_Documents.XmlElements.BacteriaGenome/")>
    Public Class BacterialGenome : Inherits T_MetaCycEntity(Of Slots.Specie)

        Public Property DbLinks As String()
        Public Property CommonName As String
        Public Property Comment As String

        Public Property Genome As String
        <XmlAttribute> Public Property NCBITaxonomy As String
        Public Property Strain As String
        <XmlAttribute> Public Property OperonCounts As Integer

        ''' <summary>
        ''' The genes collection in this cell model, a gene object in this model just a carrier 
        ''' of template information.
        ''' (这个模型之中的所有的基因的集合，在本计算机模型之中，基因对象仅仅只是遗传数据的承载者，
        ''' 而真正用于计算转录产物浓度的为转录单元对象)
        ''' </summary>
        ''' <remarks></remarks>
        Public Property Genes As GeneObject()
        Public Property TransUnits As List(Of GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit)
        Public Property Regulons As GCML_Documents.XmlElements.Bacterial_GENOME.Regulon()
        Public Property Transcripts As Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript()
        Public Property ExpressionKinetics As EnzymeCatalystKineticLaw()

        Public Function GetExpressionKineticsLaw(EnzymeId As String) As EnzymeCatalystKineticLaw
            Dim LQuery = (From item In ExpressionKinetics Where String.Equals(EnzymeId, item.Enzyme) Select item).First
            Return LQuery
        End Function
    End Class
End Namespace