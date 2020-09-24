#Region "Microsoft.VisualBasic::d79182054b9111d5fa6346cda4353300, engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\Metabolism\Proteins.vb"

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

    '     Class Polypeptide
    ' 
    ' 
    '         Enum ProteinTypes
    ' 
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: CompositionVector, DynamicsLamda, Identifier, ProteinType
    ' 
    '     Function: CreateObject, GenerateVector
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports Microsoft.VisualBasic

Namespace GCML_Documents.XmlElements.Metabolism

    'Public Class Protein : Inherits MetaCycEntity(Of SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Protein)
    '    Implements Metabolite.IDegradable

    '    <XmlAttribute("UniqueId")> Public Overrides Property UniqueId As String Implements Metabolite.IDegradable.UniqueId

    '    Public Property Citations As List(Of String)
    '    Public Property GoTerms As List(Of String)
    '    Public Property Synonyms As String()
    '    Public Property CommonName As String
    '    ''' <summary>
    '    ''' 对于单个的多肽链蛋白质单体而言，本列表的值为目标蛋白质的结构域列表；对于蛋白质复合物而言，则为多个亚基的结构域的集合
    '    ''' </summary>
    '    ''' <value></value>
    '    ''' <returns></returns>
    '    ''' <remarks></remarks>
    '    Public Property DomainList As String()
    '    Public Property ModifiedProteins As ModifiedProtein()

    '    ''' <summary>
    '    ''' 经过化学修饰的蛋白质
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Public Class ModifiedProtein
    '        <XmlAttribute> Public Property UniqueId As String
    '        <XmlAttribute> Public Property pHandle As String
    '        Public Property ModifiedForm As String
    '    End Class

    '    Public Shared Function CreateDataModel(Prot As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Protein, Metabolites As Metabolite()) As Protein
    '        Dim Protein As Protein = New Protein With {.UniqueId = Prot.UniqueId, .Citations = Prot.Citations, .GoTerms = Prot.GoTerms}
    '        Protein.Lamda = 0.95
    '        Protein.Synonyms = Prot.Synonyms
    '        Protein.CommonName = Prot.CommonName
    '        Return Protein
    '    End Function

    '    <XmlAttribute> Public Property Lamda As Double Implements Metabolite.IDegradable.Lamda
    'End Class

    <XmlType("Polypeptide", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/GCML_Documents.XmlElements/")>
    Public Class Polypeptide : Inherits T_MetaCycEntity(Of Slots.Gene)
        Implements SequenceModel.ISequenceModel
        Implements Metabolite.IDegradable

        Public Enum ProteinTypes
            Enzyme = 0
            Polypeptide = 1
            TranscriptFactor = 2
        End Enum

        <XmlAttribute> Public Property ProteinType As ProteinTypes
        <XmlElement("Polymer_CompositionVector", Namespace:="http://code.google.com/p/genome-in-code/GCModeller/Dynamics")>
        Public Property CompositionVector As SequenceModel.CompositionVector Implements SequenceModel.ISequenceModel.CompositionVector

        Public Shared Function CreateObject(TransScript As Bacterial_GENOME.Transcript, Model As BacterialModel, MetaCyc As DatabaseLoadder) As Polypeptide
            Dim Polypeptide As Polypeptide = New Polypeptide With {
                .BaseType = TransScript.BaseType,
                .Identifier = TransScript.BaseType.CommonName
            }
            Return Polypeptide
        End Function

        Public Function GenerateVector(MetaCyc As MetaCyc.File.FileSystem.DatabaseLoadder) As Integer Implements SequenceModel.ISequenceModel.GenerateVector
            Dim AACollection As Char() = "BDEFHIJKLMNOPQRSVWXYZATGC"
            Dim Seq = (From prot In MetaCyc.Database.FASTAFiles.protseq Where BaseType.Product.IndexOf(prot.UniqueId) > -1 Select prot).First.SequenceData.ToUpper
            Dim Vector As Integer() = New Integer(AACollection.Count - 1) {}

            For i As Integer = 0 To Seq.Count - 1
                Dim idx As Integer = Array.IndexOf(AACollection, Seq(i))
                Vector(idx) += 1
            Next
            CompositionVector = New SequenceModel.CompositionVector With {.T = Vector}

            Return Seq.Count
        End Function

        ''' <summary>
        ''' 降解系数
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("D_DynamicsLamda", Namespace:="http://code.google.com/p/genome-in-code/GCModeller/Dynamics")>
        Public Property DynamicsLamda As Double Implements Metabolite.IDegradable.Lamda
        <XmlAttribute("UniqueId")> Public Overrides Property Identifier As String Implements Metabolite.IDegradable.locusId
    End Class
End Namespace
