#Region "Microsoft.VisualBasic::f21bbaf886cbda495930e2a0ecd4c0db, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\Bacterial_GENOME\Transcript.vb"

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

    '   Total Lines: 105
    '    Code Lines: 63
    ' Comment Lines: 28
    '   Blank Lines: 14
    '     File Size: 5.29 KB


    '     Class Transcript
    ' 
    '         Properties: CompositionVector, Identifier, Lamda, PolypeptideCompositionVector, Product
    '                     Template, TranscriptType
    ' 
    '         Function: CreateMetabolite, CreateObject, ToString
    '         Enum TranscriptTypes
    ' 
    '             mRNA, rRNA, siRNA, tRNA
    ' 
    ' 
    ' 
    '  
    ' 
    '     Function: GenerateVector
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.SequenceModel
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports Microsoft.VisualBasic

Namespace GCML_Documents.XmlElements.Bacterial_GENOME

    ''' <summary>
    ''' 每一种RNA分子仅生成一种产物分子
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Transcript : Inherits T_MetaCycEntity(Of Slots.Gene)
        Implements Metabolite.IDegradable
        Implements ISequenceModel

        ''' <summary>
        ''' UniqueId of the protein element in the metabolites list collection.
        ''' (指向Metabolites列表中的蛋白质对象的UniqueId)
        ''' 【This slot holds the ID of a polypeptide or tRNA frame, which is the product of this gene. 
        ''' This slot may contain multiple values for two possible reasons: a given gene might be 
        ''' translated from more than one start codon, giving rise to products of different lengths; 
        ''' the product of the gene may undergo chemical modification. In the latter case, the gene 
        ''' lists all modified forms of the protein in its Product slot.】
        ''' 【这个属性值为本基因的表达产物：一个多肽链单体蛋白或者tRNA分子的UniqueId属性值，本属性由于两个原因
        ''' 可能包含有多个值：
        '''  1. 基因可能从不同的翻译起始密码子开始翻译，从而产生不同长度的产物；
        '''  2. 基因的产物可能在经过化学修饰，当为这种情况的时候，本属性将会列举出蛋白质产物的所有修饰形式】
        ''' (对于MetaCyc数据库而言，本属性值包含有所有类型的蛋白质对象的UniqueID，但是在编译后的计算机模型之中，
        ''' 仅包含有不同启动子而形成的所有不同长度的多肽链)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Product As String
        ''' <summary>
        ''' 核酸碱基构成，ATGC
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property CompositionVector As CompositionVector Implements ISequenceModel.CompositionVector
        <XmlElement> Public Property PolypeptideCompositionVector As SequenceModel.CompositionVector
        <XmlAttribute> Public Property Template As String

        Public Shared Function CreateObject(MetaCyc As DatabaseLoadder, Gene As GeneObject, Model As BacterialModel) As Transcript()
            Dim List As List(Of Transcript) = New List(Of Transcript)
            Dim Proteins = (From prot In MetaCyc.Database.FASTAFiles.protseq Where Gene.BaseType.Product.IndexOf(prot.UniqueId) > -1 Select prot.UniqueId).ToArray

            For Each prot In Proteins
                Call List.Add(New Transcript With {.Identifier = Gene.AccessionId & ".TRANSCRIPT", .Lamda = 0.98, .BaseType = Gene.BaseType})
            Next

            Return List.ToArray
        End Function

        Public Function CreateMetabolite() As Metabolite
            Return New Metabolite With {
                .BoundaryCondition = False,
                .Identifier = Identifier,
                .CommonName = Identifier,
                .InitialAmount = 0
            }
        End Function

        Public Overrides Function ToString() As String
            Return Identifier
        End Function

        <XmlAttribute> Public Property Lamda As Double Implements Metabolite.IDegradable.Lamda
        <XmlAttribute("UniqueId")> Public Overrides Property Identifier As String Implements Metabolite.IDegradable.locusId

        <XmlAttribute> Public Property TranscriptType As TranscriptTypes

        Public Enum TranscriptTypes
            mRNA
            tRNA
            rRNA
            siRNA
        End Enum

        Public Function GenerateVector(MetaCyc As DatabaseLoadder) As Integer Implements ISequenceModel.GenerateVector
            Dim Genes = MetaCyc.Database.FASTAFiles.DNAseq
            Dim AccessionId As String = Me.BaseType.CommonName
            Dim Gene = (From gObj In Genes Where String.Equals(gObj.AccessionId, AccessionId) Select gObj).First
            Dim Vector As Integer() = New Integer(3) {}
            Dim ATGC As Char() = "ATGC"
            Dim Seq As Char() = Gene.SequenceData.ToUpper

            For i As Integer = 0 To Gene.SequenceData.Count - 1
                Dim idx = Array.IndexOf(ATGC, Seq(i))
                Vector(idx) += 1
            Next

            Me.CompositionVector = New CompositionVector With {
                .T = Vector
            }
            Return Seq.Count
        End Function
    End Class
End Namespace
