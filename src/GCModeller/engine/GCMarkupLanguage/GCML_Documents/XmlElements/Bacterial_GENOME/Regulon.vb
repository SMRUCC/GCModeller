Imports System.Text
Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream

Namespace GCML_Documents.XmlElements.Bacterial_GENOME

    ''' <summary>
    ''' 基因组中的一个逻辑上的表达单元，一个调控因子与其相对应的基因的关系对的集合
    ''' </summary>
    ''' <remarks>
    ''' {Regulator, EffectFactor, GeneList}
    ''' </remarks>
    Public Class Regulon : Inherits T_MetaCycEntity(Of Slots.Regulon)

        ''' <summary>
        ''' 在物理上分散但是在逻辑上为一个整体的表达单元
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property GeneList As String()
        <XmlElement> Public Property Regulator As Regulon.RegulatorF

        Public Structure RegulatorF
            ''' <summary>
            ''' Regulator UniqueId
            ''' </summary>
            ''' <remarks></remarks>
            <XmlText> Dim UniqueId As String
            <XmlAttribute> Dim Effect As Double
        End Structure

        Public Overrides Function ToString() As String
            Return Regulator.UniqueId
        End Function

        Public Shared Function Create(Data As RowObject, Model As BacterialModel) As Regulon
            Dim IdList As String() = Data(2).Split(CChar(";"))
            Dim HandleGeneList As String() = (From Id As String In IdList
                                              Let GeneId As String = (From Gene In Model.BacteriaGenome.Genes Where String.Equals(Gene.Identifier, Id) Select Gene.Identifier).First
                                              Select GeneId).ToArray
            Return New Regulon With {
                .Identifier = Data.First,
                .GeneList = HandleGeneList,
                .Regulator = New RegulatorF With {
                    .UniqueId = Data.First,
                    .Effect = Val(Data(1))
                }
            }
        End Function
    End Class
End Namespace