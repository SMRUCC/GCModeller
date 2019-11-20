#Region "Microsoft.VisualBasic::3bde26ddf0811169171957646e40e96f, core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\Protein.vb"

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

    '     Class Protein
    ' 
    '         Properties: Catalyzes, CommonName, ComponentOf, Components, DNAFootprintSize
    '                     Features, Gene, GoTerms, Identifier, IsModifiedProtein
    '                     IsPolypeptide, Locations, ModifiedForm, MolecularWeightExp, MolecularWeightKD
    '                     MolecularWeightSeq, pI, Regulates, Species, Table
    '                     Types, UnmodifiedForm
    ' 
    '         Function: [New], GetEnumerator, GetEnumerator1
    '         Interface IEnzyme
    ' 
    '             Properties: Catalyze, ComponentOf, Name, UniqueId
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    ''' <summary>
    ''' The class of all proteins is divided into two subclasses: protein complexes and polypeptides.
    ''' A polypeptide is a single amino acid chain produced from a single gene. A protein
    ''' complex is a multimeric aggregation of more than one polypeptide subunit. A protein
    ''' complex may in some cases have another protein complex as a component. Many of the
    ''' slots that are applicable to Proteins are also applicable to members of the RNA class.
    ''' (本类型的对象会枚举所有的Component对象的UniqueID)
    ''' </summary>
    ''' <remarks>
    ''' Protein表对象和ProtLigandCplxe表对象相比较：
    ''' Protein表中包含有所有类型的蛋白质对象，而ProtLigandCplxe则仅包含有蛋白质和小分子化合物配合的之后所形成的复合物，
    ''' 所以基因的产物在ProtLigandCplxe表中是无法找到的
    ''' </remarks>
    Public Class Protein : Inherits [Object]
        Implements Protein.IEnzyme, Regulation.IRegulator
        Implements Generic.IEnumerable(Of String) '本类型的对象会枚举所有的Component对象的UniqueID

        <MetaCycField()> <XmlAttribute> Public Overrides Property Identifier As String Implements IEnzyme.UniqueId, Regulation.IRegulator.UniqueId
        <MetaCycField()> <XmlAttribute> Public Overrides Property CommonName As String Implements IEnzyme.Name, Regulation.IRegulator.CommonName
        <MetaCycField(type:=MetaCycField.Types.TStr)> Public Overrides Property Types As List(Of String) Implements Regulation.IRegulator.Types

        ''' <summary>
        ''' This slot lists the complex(es) that this protein is a component of, if any, including protein
        ''' complexes, protein-small-molecule complexes, protein-RNA complexes, and so on.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("proteins,protligandcplxes", "", ExternalKey.Directions.In)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property ComponentOf As List(Of String) Implements IEnzyme.ComponentOf, Regulation.IRegulator.ComponentOf

        ''' <summary>
        ''' For proteins that bind to DNA, the number of base pairs on the DNA strand that the
        ''' binding protein covers.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property DNAFootprintSize As String

        <ExternalKey("proteinfeatures", "", ExternalKey.Directions.Out)> <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property Features As List(Of String)

        ''' <summary>
        ''' Values of this slot are the Gene Ontology terms to which this object is annotated. Each
        ''' value should be annotated with citations, including evidence codes.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property GoTerms As List(Of String)

        ''' <summary>
        ''' This slot describes the one or more cellular locations in which this protein is found. It’s
        ''' values are members of the CCO (Cell Component Ontology) class.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property Locations As List(Of String)

        ''' <summary>
        ''' This slot points from the unmodified form of a protein to one or more chemically modified
        ''' forms of that protein. For example, the slot might point from the unmodified form of
        ''' a polypeptide (or a protein complex) to a phosphorylated form of that polypeptide (or
        ''' protein complex).
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property ModifiedForm As List(Of String)

        ''' <summary>
        ''' This computed slot lists the known molecular weight(s) of a macromolecule by taking the
        ''' union of the slots Molecular-Weight-Seq and Molecular-Weight-Exp. Units: kilodaltons.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(name:="MOLECULAR-WEIGHT-KD")> Public Property MolecularWeightKD As String

        ''' <summary>
        ''' This slot lists the molecular weight of the protein complex or polypeptide, as derived
        ''' from sequence data. Units: kilodaltons.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property MolecularWeightSeq As String

        ''' <summary>
        ''' This slot lists the molecular weight of the protein complex or polypeptide, derived experimentally.
        ''' Multiple values of this slot correspond to multiple experimental observations.
        ''' Units: kilodaltons.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property MolecularWeightExp As String

        ''' <summary>
        ''' This slot lists the pI of the polypeptide.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(name:="PI")> Public Property pI As String

        ''' <summary>
        ''' For proteins that have regulatory activity (e.g. as transcription factors), this slot points to
        ''' the Regulation frames that describe the regulation and link to the regulated entity.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("regulation", "", ExternalKey.Directions.Out)> <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property Regulates As List(Of String) Implements Regulation.IRegulator.Regulates

        ''' <summary>
        ''' This slot is used in proteins only in the MetaCyc DB, in which case it identifies the species
        ''' in which the current protein is found.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property Species As String

        ''' <summary>
        ''' A list of enzymatic reaction unique id that catalyzed by this protein.(本蛋白质所催化的酶促反应的UniqueId的列表)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("reactions,enzrxns", "", ExternalKey.Directions.Out)> <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property Catalyzes As List(Of String) Implements IEnzyme.Catalyze
        <ExternalKey("proteins,protligandcplxes,compounds", "", ExternalKey.Directions.Out)> <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property Components As List(Of String) Implements IEnzyme.Components ', Regulation.IRegulator.Components

        ''' <summary>
        ''' This slot points from a chemically modified form of some protein, to the native unmodified
        ''' form of that protein (e.g., from a phosphorylated form to the unphosphorylated
        ''' form).
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("proteins", "", ExternalKey.Directions.Out)> <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property UnmodifiedForm As List(Of String)

        ''' <summary>
        ''' The gene's UniqueId that indicated that which gene codes this polypeptide.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("gene", "", ExternalKey.Directions.In)> <MetaCycField()> Public Property Gene As String

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.proteins
            End Get
        End Property

        ''' <summary>
        ''' 判断本蛋白质对象是否为经过化学修饰的蛋白质
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsModifiedProtein As Boolean
            Get
                Return Not UnmodifiedForm.IsNullOrEmpty
            End Get
        End Property

        ''' <summary>
        ''' 判断本蛋白质对象是否为一个多肽链对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsPolypeptide As Boolean
            Get
                Return Types.IndexOf("Polypeptides") > -1
            End Get
        End Property

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As Protein
        '    Dim NewObj As Protein = New Protein

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of Protein) _
        '        (MetaCyc.File.AttributeValue.Object.Format(Proteins.AttributeList, e), NewObj)

        '    If NewObj.Object.ContainsKey("DNA-FOOTPRINT-SIZE") Then NewObj.DNAFootprintSize = NewObj.Object("DNA-FOOTPRINT-SIZE") Else NewObj.DNAFootprintSize = String.Empty
        '    If NewObj.Object.ContainsKey("GENE") Then NewObj.Gene = NewObj.Object("GENE") Else NewObj.Gene = String.Empty

        '    NewObj.UnmodifiedForm = StringQuery(NewObj.Object, "UNMODIFIED-FORM( \d+)?")
        '    NewObj.GoTerms = StringQuery(NewObj.Object, "GO-TERMS( \d+)?")
        '    NewObj.Regulates = StringQuery(NewObj.Object, "REGULATES( \d+)?").AsList
        '    NewObj.DBLinks = StringQuery(NewObj.Object, "DBLINKS( \d+)?")
        '    NewObj.Features = StringQuery(NewObj.Object, "FEATURES( \d+)?")
        '    NewObj.ComponentOf = StringQuery(NewObj.Object, "COMPONENT-OF( \d+)?")
        '    NewObj.Locations = StringQuery(NewObj.Object, "LOCATIONS( \d+)?")

        '    Return NewObj
        'End Operator

        ''' <summary>
        ''' A general data structure of the enzyme entity that can catalyze a reaction.(能够催化酶促反应的酶分子的通用数据类型)
        ''' </summary>
        ''' <remarks></remarks>
        Public Interface IEnzyme : Inherits IComplexes

            Property Name As String
            Property UniqueId As String
            ''' <summary>
            ''' 本类型的对象最为其他的对象的组件而存在的时候，则本属性值指明了本对象所能够构成的对象的Unique-Id列表
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Property ComponentOf As List(Of String)
            ''' <summary>
            ''' The catalyzed object of this enzyme instance.(所催化的对象列表)
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Property Catalyze As List(Of String)
        End Interface

        ''' <summary>
        ''' 返回一个新构造出来的Protein对象
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function [New]() As Protein
            Dim Protein As Protein = New Protein
            Protein.Catalyzes = New List(Of String)
            Protein.Citations = New List(Of String)
            Protein.ComponentOf = New List(Of String)
            Protein.Components = New List(Of String)
            Protein.DBLinks = New String() {}
            Protein.Features = New List(Of String)
            Protein.GoTerms = New List(Of String)
            Protein.Locations = New List(Of String)
            Protein.Names = New List(Of String)
            Protein.Regulates = New List(Of String)
            Protein.Synonyms = New String() {}
            Protein.Types = New List(Of String)

            Return Protein
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
            For i As Integer = 0 To Me.Components.Count - 1
                Yield Components(i)
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
