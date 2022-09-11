#Region "Microsoft.VisualBasic::2b9cb0e2a9e6229fab6a99ea8a9d36d6, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\ExportServices\gbEntry.vb"

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

    '   Total Lines: 192
    '    Code Lines: 148
    ' Comment Lines: 18
    '   Blank Lines: 26
    '     File Size: 11.11 KB


    '     Class gbEntryBrief
    ' 
    '         Properties: AccessionID, AverageCDSLength, CDSsWithFunctionalAssignment, Comments, ConservedHypotheticalCDSs
    '                     Definition, GC_Content, GCSkew, GI, HypotheticalCDSs
    '                     II_VirusesEffector, III_VirusesEffector, IV_VirusesEffector, Length, Locus
    '                     Mob, Number_conjugal, Number_of_CDSs, Number_of_IS, Number_of_mobile_element
    '                     Number_of_Transposases, NumberOfGenes, Organism, PSEUDOProteins, Published
    '                     Reference1, Reference2, Reference3, Reference4, Reference5
    '                     Reference6, RepliconCoding, Species, Strain, Submit
    '                     Taxon, Tra, tRNA, VI_VirusesEffector, VirusesProteinCounts
    ' 
    '         Function: ConvertObject, GetSubmitDate, ToString
    ' 
    '     Class Plasmid
    ' 
    '         Properties: Country, Host, isolation_source, IsShortGun, PlasmidID
    '                     PlasmidType
    ' 
    '         Function: Build
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.Language

Namespace Assembly.NCBI.GenBank.CsvExports

    ''' <summary>
    ''' Genbank数据库的简要信息Csv表格
    ''' </summary>
    ''' <remarks></remarks>
    Public Class gbEntryBrief : Implements INamedValue

        Public Property Definition As String
        Public Property Length As Integer
        ''' <summary>
        ''' 基因组的编号
        ''' </summary>
        ''' <returns></returns>
        Public Property Locus As String Implements INamedValue.Key
        Public Property AccessionID As String
        Public Property GI As String
        Public Property Organism As String
        Public ReadOnly Property Species As String
            Get
                If String.IsNullOrEmpty(Strain) Then
                    Return Organism
                Else
                    Return Organism.Replace(Strain, "").Replace(" str. ", "")
                End If
            End Get
        End Property
        Public Property Strain As String
        Public Property Taxon As String
        Public Property Submit As String
        Public Property Reference1 As String
        Public Property Reference2 As String
        Public Property Reference3 As String
        Public Property Reference4 As String
        Public Property Reference5 As String
        Public Property Reference6 As String
        Public Property Comments As String
        Public Property NumberOfGenes As String
        Public Property PSEUDOProteins As Integer

        Public Property CDSsWithFunctionalAssignment As Integer
        Public Property ConservedHypotheticalCDSs As Integer
        Public Property HypotheticalCDSs As Integer
        Public Property RepliconCoding As Double
        Public Property AverageCDSLength As Double

        Public Property Number_of_mobile_element As Integer
        Public Property Number_of_CDSs As Integer
        Public Property Number_of_IS As Integer
        Public Property Number_of_Transposases As Integer
        Public Property tRNA As Integer
        Public Property IV_VirusesEffector As Integer
        Public Property III_VirusesEffector As Integer
        Public Property II_VirusesEffector As Integer
        Public Property VI_VirusesEffector As Integer
        Public Property VirusesProteinCounts As Integer
        ''' <summary>
        ''' 这个属性可以当作质粒对象的特征，虽然在生物学上面无法区分不同质粒，但是在去除重复的时候，这个值却是唯一的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GC_Content As Double

        Public Property Tra As Integer
        Public Property Mob As Integer
        Public Property Number_conjugal As String

        Public ReadOnly Property Published As Boolean
            Get
                Return InStr(Reference1, "[PUBMED") > 0 OrElse
                    InStr(Reference2, "[PUBMED") > 0 OrElse
                    InStr(Reference3, "[PUBMED") > 0 OrElse
                    InStr(Reference4, "[PUBMED") > 0 OrElse
                    InStr(Reference5, "[PUBMED") > 0 OrElse
                    InStr(Reference6, "[PUBMED") > 0
            End Get
        End Property

        Public Function GetSubmitDate() As Date
            Dim result As Date
            Dim n = Date.TryParse(Submit, result)
            If n = True Then
                Return result
            Else
                Return Now
            End If
        End Function

        Public Property GCSkew As Double

        Public Shared Function ConvertObject(Of TGBKEntryBrief As gbEntryBrief)(gbk As NCBI.GenBank.GBFF.File) As TGBKEntryBrief
            Dim GBKEntryBrief As TGBKEntryBrief = Activator.CreateInstance(Of TGBKEntryBrief)()
            Dim CDS = (From f In gbk.Features._innerList.AsParallel Where String.Equals(f.KeyName, "CDS") Select f).ToArray

            On Error Resume Next

            GBKEntryBrief.ConservedHypotheticalCDSs = (From protein In CDS Let anno = protein.Query("product") Where InStr(anno, "Conserved", CompareMethod.Text) > 0 Select 1).ToArray.Length
            GBKEntryBrief.HypotheticalCDSs = (From protein In CDS Let anno = protein.Query("product") Where InStr(anno, "Hypothetical", CompareMethod.Text) > 0 Select 1).ToArray.Length
            GBKEntryBrief.RepliconCoding = (From orf In CDS Select orf.Location.ContiguousRegion.FragmentSize).ToArray.Sum / gbk.Origin.SequenceData.Length
            GBKEntryBrief.AverageCDSLength = (From orf In CDS Select orf.Location.ContiguousRegion.FragmentSize).ToArray.Average

            GBKEntryBrief.AccessionID = gbk.Accession.AccessionId
            GBKEntryBrief.Comments = gbk.Comment.Comment
            GBKEntryBrief.Definition = gbk.Definition.Value
            GBKEntryBrief.GI = gbk.Version.GI
            GBKEntryBrief.Length = gbk.Origin.SequenceData.Length
            GBKEntryBrief.Locus = gbk.Locus.AccessionID
            GBKEntryBrief.Organism = gbk.Source.SpeciesName
            GBKEntryBrief.Submit = gbk.Locus.UpdateTime
            GBKEntryBrief.Strain = gbk.Features.source.Query("strain")
            GBKEntryBrief.Taxon = gbk.Taxon
            GBKEntryBrief.NumberOfGenes = (From item In gbk.Features._innerList.AsParallel Where String.Equals("gene", item.KeyName, StringComparison.OrdinalIgnoreCase) Select 1).ToArray.Length
            GBKEntryBrief.PSEUDOProteins = (From Feature In gbk.Features._innerList.AsParallel
                                            Where Not (From ref As String In Feature.QueryDuplicated("db_xref") Let Tokens As String() = ref.Split(CChar(":")) Where String.Equals(Tokens.First, "PSEUDO") Select 1).ToArray.IsNullOrEmpty
                                            Select 1).ToArray.Length

            GBKEntryBrief.CDSsWithFunctionalAssignment = CDS.Length - GBKEntryBrief.ConservedHypotheticalCDSs - GBKEntryBrief.HypotheticalCDSs

            Dim p As i32 = 0
            If p <= gbk.Reference.ReferenceList.Length - 1 Then GBKEntryBrief.Reference1 = gbk.Reference.ReferenceList(++p).ToString
            If p <= gbk.Reference.ReferenceList.Length - 1 Then GBKEntryBrief.Reference2 = gbk.Reference.ReferenceList(++p).ToString
            If p <= gbk.Reference.ReferenceList.Length - 1 Then GBKEntryBrief.Reference3 = gbk.Reference.ReferenceList(++p).ToString
            If p <= gbk.Reference.ReferenceList.Length - 1 Then GBKEntryBrief.Reference4 = gbk.Reference.ReferenceList(++p).ToString
            If p <= gbk.Reference.ReferenceList.Length - 1 Then GBKEntryBrief.Reference5 = gbk.Reference.ReferenceList(++p).ToString
            If p <= gbk.Reference.ReferenceList.Length - 1 Then GBKEntryBrief.Reference6 = gbk.Reference.ReferenceList(++p).ToString


            GBKEntryBrief.Number_of_mobile_element = (From f In gbk.Features._innerList.AsParallel Where String.Equals(f.KeyName, "mobile_element", StringComparison.OrdinalIgnoreCase) Select 1).ToArray.Length
            GBKEntryBrief.Number_of_Transposases = (From protein In CDS Let annotation = protein.Query("product") Let is_transpoos = InStr(annotation, Transposases, CompareMethod.Text) > 0 Select If(is_transpoos, 1, 0)).ToArray.Sum
            GBKEntryBrief.Number_of_CDSs = CDS.Length

            Dim FunctionAnnotations = (From protein In CDS Select ID = protein.Query(FeatureQualifiers.locus_tag), anno = protein.Query("product"), geneName = protein.Query("gene")).ToArray

            GBKEntryBrief.VI_VirusesEffector = (From protein In CDS Let anno = protein.Query("product") Where InStr(anno, "typ e VI", CompareMethod.Text) > 0 Select 1).ToArray.Length
            GBKEntryBrief.II_VirusesEffector = (From protein In CDS Let anno = protein.Query("product") Where InStr(anno, "type II", CompareMethod.Text) > 0 Select 1).ToArray.Length
            GBKEntryBrief.III_VirusesEffector = (From protein In CDS Let anno = protein.Query("product") Where InStr(anno, "Type III", CompareMethod.Text) > 0 Select 1).ToArray.Length
            GBKEntryBrief.IV_VirusesEffector = (From protein In CDS Let anno = protein.Query("product") Where InStr(anno, "type IV", CompareMethod.Text) > 0 Select 1).ToArray.Length
            GBKEntryBrief.VirusesProteinCounts = (From protein In CDS Let anno = protein.Query("product") Where InStr(anno, "vir", CompareMethod.Text) > 0 Select 1).ToArray.Length
            GBKEntryBrief.Number_conjugal = (From protein In CDS Let anno = protein.Query("product") Where InStr(anno, "conjugal", CompareMethod.Text) > 0 Select 1).ToArray.Length
            GBKEntryBrief.Number_of_IS = (From protein In CDS Let anno = protein.Query("product") Where InStr(anno, "IS") > 0 Select 1).ToArray.Length
            GBKEntryBrief.GC_Content = NucleicAcidStaticsProperty.GCContent(gbk.Origin)
            GBKEntryBrief.GCSkew = gbk.Origin.GCSkew
            GBKEntryBrief.Tra = (From protein In CDS Let name = protein.Query("gene") Where InStr(name, "tra", CompareMethod.Text) = 1 Select 1).ToArray.Length
            GBKEntryBrief.Mob = (From protein In CDS Let name = protein.Query("gene") Where InStr(name, "mob", CompareMethod.Text) = 1 Select 1).ToArray.Length
            GBKEntryBrief.tRNA = (From f In gbk.Features._innerList.AsParallel Where String.Equals(f.KeyName, "tRNA", StringComparison.OrdinalIgnoreCase) Select 1).ToArray.Length

            Return GBKEntryBrief
        End Function

        ''' <summary>
        ''' 转座酶
        ''' </summary>
        ''' <remarks></remarks>
        Const Transposases As String = "transposase"

        Public Overrides Function ToString() As String
            Return Me.Organism
        End Function
    End Class

    Public Class Plasmid : Inherits gbEntryBrief

        Public Property PlasmidID As String
        Public Property PlasmidType As String
        Public Property isolation_source As String
        Public Property Country As String
        Public Property Host As String
        Public ReadOnly Property IsShortGun As Boolean
            Get
                Return InStr(Definition, "shotgun", CompareMethod.Text) > 0
            End Get
        End Property

        Public Overloads Shared Function Build(gbk As NCBI.GenBank.GBFF.File) As Plasmid
            Dim Plasmid As Plasmid = ConvertObject(Of Plasmid)(gbk)
            Plasmid.PlasmidID = gbk.Features.source.Query("plasmid")
            Plasmid.Host = gbk.Features.source.Query("host")
            Plasmid.Country = gbk.Features.source.Query("country")
            Plasmid.isolation_source = gbk.Features.source.Query("isolation_source")

            Return Plasmid
        End Function
    End Class
End Namespace
