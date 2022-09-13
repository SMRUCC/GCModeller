#Region "Microsoft.VisualBasic::416a37039260ec6a28b36766488ea044, GCModeller\data\RegulonDatabase\Regtransbase\StructureObjects\RegulatoryElements.vb"

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

    '   Total Lines: 747
    '    Code Lines: 194
    ' Comment Lines: 520
    '   Blank Lines: 33
    '     File Size: 36.19 KB


    '     Class RegulatoryElement
    ' 
    '         Properties: ArticleGuid, Descript, Description, GenomeGuid, IsRealName
    '                     LastUpdate, Name, PackageGuid
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Class Effector
    ' 
    '         Properties: Guid
    ' 
    '     Class Regulator
    ' 
    '         Properties: Consensus, Family, flag_prot_rna, GeneGuid, Guid
    '                     Name, RefBank1, RefBank2, RefBank3, RefBank4
    '                     RegulatorTypeGuid
    ' 
    '         Function: ExportFasta, GetSequenceData, ToString, TryAssignSequence
    ' 
    '     Class Sites
    ' 
    '         Properties: fl_dna_rna, FuncSiteTypeGuid, Guid, PfoSideGuid, PfoTypeId
    '                     PositionFrom, PositionFromGuid, PositionTo, PositionToGuid, PtoSideGuid
    '                     PtoTypeId, RegulatorGuid, Sequence, Signature, SiteLen
    '                     StructureSiteTypeGuid
    ' 
    '         Function: ExportFasta, FixSequenceError
    ' 
    '     Class Gene
    ' 
    '         Properties: ferment_num, gene_function, Guid, location, metabol_path
    '                     ref_bank1, ref_bank2, ref_bank3, ref_bank4, signature
    ' 
    '     Class Transcript
    ' 
    '         Properties: Guid, pfo_side_guid, pfo_type_id, pos_from, pos_from_guid
    '                     pos_to, pos_to_guid, pto_side_guid, pto_type_id, tr_len
    ' 
    '     Class Operon
    ' 
    '         Properties: Guid
    ' 
    '     Class Locuses
    ' 
    '         Properties: Guid, location
    ' 
    '     Class Regulon
    ' 
    '         Properties: Guid
    ' 
    '     Class sec_structures
    ' 
    '         Properties: Guid, pfo_side_guid, pfo_type_id, pos_from, pos_from_guid
    '                     pos_to, pos_to_guid, pto_side_guid, pto_type_id, sequence
    ' 
    '     Class helices
    ' 
    '         Properties: Guid, pos_from1, pos_from2, pos_to1, pos_to2
    '                     sec_struc_guid
    ' 
    '     Class ObjSynonyms
    ' 
    '         Properties: art_guid, fl_real_name, obj_guid, pkg_guid, syn_name
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports System.Text
Imports SMRUCC.genomics.SequenceModel.FASTA.Reflection
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel

Namespace Regtransbase.StructureObjects

    ''' <summary>
    ''' Regulatory elements    
    ''' For each article, annotator input information about regulatory elements and set up links between them.
    ''' Regulatory elements are “players” in experiments described in the article. There are 10 types of 
    ''' regulatory elements (corresponding to 10 objects): Inductor (Effector), Regulator, Site, Gene, 
    ''' Transcript, Operon, Locus, Regulon, Helix, и Secondary Structure.
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class RegulatoryElement
        ''' <summary>
        ''' [regelem]_guid - unique identifier
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Property Guid As Integer
        ''' <summary>
        ''' pkg_guid - determine package (link to packages)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DatabaseField("pkg_guid")> Public Property PackageGuid As Integer
        ''' <summary>
        ''' art_guid - determine article (link to articles), id of the article, which contain the 
        ''' regulatory element  
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DatabaseField("art_guid")> Public Property ArticleGuid As Integer
        ''' <summary>
        ''' genome_guid - determine genome (link to dict_genomes)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DatabaseField("genome_guid")> Public Property GenomeGuid As Integer
        ''' <summary>
        ''' name - regulatory element name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DatabaseField("name")> Public Overridable Property Name As String
        ''' <summary>
        ''' fl_real_name - boolean, determine whether name is "real" (i.e. does it belong to some systematic 
        ''' nomenclature or was introduced by article authors)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DatabaseField("fl_real_name")> Public Property IsRealName As Integer
        ''' <summary>
        ''' descript - some description in free form, description of the regulatory element
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DatabaseField("descript")> Public Property Descript As Byte()
        <DatabaseField("last_update")> Public Property LastUpdate As DateTime

        Public ReadOnly Property Description As String
            Get
                Return System.Text.Encoding.UTF8.GetString(Descript)
            End Get
        End Property

        Protected Friend Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class

    ''' <summary>
    ''' Inductor/Effector. Inductor, or Effector, is a substance or physical effect affecting any regulatory interaction.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Class Effector : Inherits RegulatoryElement

        Public Overrides Property Guid As Integer
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE `regulators` (
    '''   `regulator_guid` int(11) NOT NULL DEFAULT '0',
    '''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
    '''   `art_guid` int(11) NOT NULL DEFAULT '0',
    '''   `name` varchar(50) DEFAULT NULL,
    '''   `fl_real_name` int(1) DEFAULT NULL,
    '''   `genome_guid` int(11) DEFAULT NULL,
    '''   `fl_prot_rna` int(1) DEFAULT NULL,
    '''   `regulator_type_guid` int(11) DEFAULT '0',
    '''   `gene_guid` int(11) DEFAULT NULL,
    '''   `ref_bank1` varchar(255) DEFAULT NULL,
    '''   `ref_bank2` varchar(255) DEFAULT NULL,
    '''   `ref_bank3` varchar(255) DEFAULT NULL,
    '''   `ref_bank4` varchar(255) DEFAULT NULL,
    '''   `consensus` varchar(50) DEFAULT NULL,
    '''   `family` varchar(20) DEFAULT NULL,
    '''   `descript` blob,
    '''   `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    '''   PRIMARY KEY (`regulator_guid`),
    '''   KEY `FK_regulators-pkg_guid` (`pkg_guid`),
    '''   KEY `FK_regulators-art_guid` (`art_guid`),
    '''   KEY `FK_regulators-genome_guid` (`genome_guid`),
    '''   KEY `FK_regulators-regulator_type_guid` (`regulator_type_guid`),
    '''   KEY `FK_regulators-gene_guid` (`gene_guid`),
    '''   CONSTRAINT `FK_regulators-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
    '''   CONSTRAINT `FK_regulators-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
    '''   CONSTRAINT `FK_regulators-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
    '''   CONSTRAINT `FK_regulators-regulator_type_guid` FOREIGN KEY (`regulator_type_guid`) REFERENCES `dict_regulator_types` (`regulator_type_guid`),
    '''   CONSTRAINT `regulators_ibfk_1` FOREIGN KEY (`gene_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
    ''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
    ''' </remarks>
    <TableName("regulators")> Public Class Regulator : Inherits RegulatoryElement
        Implements ISequenceProvider

        <FastaAttributeItem(0)> <DatabaseField("regulator_guid")> Public Overrides Property Guid As Integer
        ''' <summary>
        ''' flag_prot_rna: Protein/RNA flag 
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("fl_prot_rna")> Public Property flag_prot_rna As Integer
        ''' <summary>
        ''' gene_guid: id of the gene encoded the regulator
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("gene_guid")> Public Property GeneGuid As Integer?
        ''' <summary>
        ''' consensus: binding site consensus for the regulator
        ''' </summary>
        ''' <remarks></remarks>
        <FastaAttributeItem(3, Precursor:="consensus")> <DatabaseField("consensus")> Public Property Consensus As String
        ''' <summary>
        ''' family: regulator family
        ''' </summary>
        ''' <remarks></remarks>
        <FastaAttributeItem(2)> <DatabaseField("family")> Public Property Family As String
        <DatabaseField("regulator_type_guid")> Public Property RegulatorTypeGuid As Integer?
        ''' <summary>
        ''' ref_bank1 – ref_bank: id of the protein in external databases, such as NCBI
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DatabaseField("ref_bank1")> Public Property RefBank1 As String
        <DatabaseField("ref_bank2")> Public Property RefBank2 As String
        <DatabaseField("ref_bank3")> Public Property RefBank3 As String
        <DatabaseField("ref_bank4")> Public Property RefBank4 As String

        ''' <summary>
        ''' name - regulatory element name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <FastaAttributeItem(1)> <DatabaseField("name")> Public Overrides Property Name As String

        Public Overrides Function ToString() As String
            Return String.Format("{0} (Family:={1})", Name, Family)
        End Function

        Public Shared Function ExportFasta(regulator As Regulator, Genes As Regtransbase.StructureObjects.Gene(), Optional TryAutoFixed As Boolean = False) As SMRUCC.genomics.SequenceModel.FASTA.FastaSeq
            If regulator.GeneGuid Is Nothing OrElse regulator.GeneGuid = -1 Then
                Return Nothing
            Else
                Dim Fsa As New FASTA.FastaSeq
                Dim Gene = (From g In Genes Where regulator.GeneGuid = g.Guid Select g).First

                If TypeExtensions.IsProteinSource(Gene) Then
                    Fsa.SequenceData = Gene.signature
                    Fsa.Headers = New String() {"regulator", regulator.Guid, regulator.Family, String.Format("TypeGuid:={0}", regulator.RegulatorTypeGuid), regulator.Name, regulator.Consensus, "*"}
                Else
                    Fsa.SequenceData = Gene.signature.ToUpper
                    If TryAutoFixed AndAlso Sites.FixSequenceError(Fsa.SequenceData) Then '序列中包含有错误
                        Fsa.Headers = New String() {"regulator", regulator.Guid, regulator.Family, String.Format("TypeGuid:={0}", regulator.RegulatorTypeGuid), regulator.Name, regulator.Consensus, Sites.SEQUENCE_ERROR_FIXED}
                    Else
                        Fsa.Headers = New String() {"regulator", regulator.Guid, regulator.Family, String.Format("TypeGuid:={0}", regulator.RegulatorTypeGuid), regulator.Name, regulator.Consensus}
                    End If
                    Fsa.SequenceData = SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.Translate(Fsa.SequenceData)
                End If
                Return Fsa
            End If
        End Function

        Dim _SequenceData As String

        Public Function TryAssignSequence(Genes As Regtransbase.StructureObjects.Gene()) As Regulator
            If Not (Me.GeneGuid Is Nothing OrElse Me.GeneGuid = -1) Then
                Dim Gene = (From g In Genes Where Me.GeneGuid = g.Guid Select g).First
                If TypeExtensions.IsProteinSource(Gene) Then
                    _SequenceData = Gene.signature
                Else
                    _SequenceData = Gene.signature.ToUpper
                    Call Sites.FixSequenceError(_SequenceData)
                    _SequenceData = SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.Translate(_SequenceData)
                End If
            End If

            Return Me
        End Function

        Public Function GetSequenceData() As String Implements ISequenceProvider.GetSequenceData
            Return _SequenceData
        End Function
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE `sites` (
    '''   `site_guid` int(11) NOT NULL DEFAULT '0',
    '''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
    '''   `art_guid` int(11) NOT NULL DEFAULT '0',
    '''   `name` varchar(50) DEFAULT NULL,
    '''   `fl_real_name` int(1) DEFAULT NULL,
    '''   `genome_guid` int(11) DEFAULT NULL,
    '''   `func_site_type_guid` int(11) DEFAULT NULL,
    '''   `struct_site_type_guid` int(11) DEFAULT NULL,
    '''   `regulator_guid` int(11) DEFAULT '0',
    '''   `fl_dna_rna` int(1) DEFAULT NULL,
    '''   `pos_from` int(11) DEFAULT NULL,
    '''   `pos_from_guid` int(11) DEFAULT NULL,
    '''   `pfo_type_id` int(11) DEFAULT NULL,
    '''   `pfo_side_guid` int(11) DEFAULT NULL,
    '''   `pos_to` int(11) DEFAULT NULL,
    '''   `pos_to_guid` int(11) DEFAULT NULL,
    '''   `pto_type_id` int(11) DEFAULT NULL,
    '''   `pto_side_guid` int(11) DEFAULT NULL,
    '''   `site_len` int(11) DEFAULT NULL,
    '''   `sequence` text,
    '''   `signature` varchar(255) DEFAULT NULL,
    '''   `descript` blob,
    '''   `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    '''   PRIMARY KEY (`site_guid`),
    '''   KEY `FK_sites-pkg_guid` (`pkg_guid`),
    '''   KEY `FK_sites-art_guid` (`art_guid`),
    '''   KEY `FK_sites-genome_guid` (`genome_guid`),
    '''   KEY `FK_sites-func_site_type_guid` (`func_site_type_guid`),
    '''   KEY `FK_sites-struct_site_type_guid` (`struct_site_type_guid`),
    '''   KEY `FK_sites-regulator_guid` (`regulator_guid`),
    '''   CONSTRAINT `FK_sites-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
    '''   CONSTRAINT `FK_sites-func_site_type_guid` FOREIGN KEY (`func_site_type_guid`) REFERENCES `dict_func_site_types` (`func_site_type_guid`),
    '''   CONSTRAINT `FK_sites-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
    '''   CONSTRAINT `FK_sites-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
    '''   CONSTRAINT `FK_sites-struct_site_type_guid` FOREIGN KEY (`struct_site_type_guid`) REFERENCES `dict_struct_site_types` (`struct_site_type_guid`),
    '''   CONSTRAINT `sites_ibfk_1` FOREIGN KEY (`regulator_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
    ''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
    ''' </remarks>
    <TableName("sites")> Public Class Sites : Inherits RegulatoryElement

        ''' <summary>
        ''' functional_site_type_guid: id of the functional type of the site (from FunctionalSiteType dictionary (see below, part 5)).
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("func_site_type_guid")> Public Property FuncSiteTypeGuid As Integer
        ''' <summary>
        ''' structural_site_type_guid: id of the functional type of the site (from StructuralSiteType dictionary (see below, part 5)).
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("struct_site_type_guid")> Public Property StructureSiteTypeGuid As Integer?
        ''' <summary>
        ''' fl_dna_rna: DNA/RNA flag 
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("fl_dna_rna")> Public Property fl_dna_rna As Integer
        ''' <summary>
        ''' pos_from, pos_to: first and last positions of the regulatory element
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("pos_from")> Public Property PositionFrom As Integer
        ''' <summary>
        ''' pos_from_guid, pos_to_guid: id of the reference regulatory element, which used as point of origin for positions indicated in pos_from, pos_to fields
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("pos_from_guid")> Public Property PositionFromGuid As Integer?
        ''' <summary>
        ''' pos_from, pos_to: first and last positions of the regulatory element
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("pos_to")> Public Property PositionTo As Integer
        ''' <summary>
        ''' pfo_type_id, pto_type_id: type of the reference regulatory element, which used as point of origin for positions indicated in pos_from, pos_to fields
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("pfo_type_id")> Public Property PfoTypeId As Integer?
        ''' <summary>
        ''' pfo_side_guid, pto_side_guid: id of the relation between reference regulatory element and current regulatory element, 
        ''' which positions are indicated  in pos_from, pos_to fields (from ObjSideType dictionary (for instance, transcription 
        ''' start, translation start, transcription end, translation end; see part 5)). 
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("pfo_side_guid")> Public Property PfoSideGuid As Integer?
        ''' <summary>
        ''' pfo_side_guid, pto_side_guid: id of the relation between reference regulatory element and current regulatory element, 
        ''' which positions are indicated  in pos_from, pos_to fields (from ObjSideType dictionary (for instance, transcription 
        ''' start, translation start, transcription end, translation end; see part 5)). 
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("pos_to_guid")> Public Property PositionToGuid As Integer?
        ''' <summary>
        ''' pfo_type_id, pto_type_id: type of the reference regulatory element, which used as point of origin for positions 
        ''' indicated in pos_from, pos_to fields
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("pto_type_id")> Public Property PtoTypeId As Integer?
        ''' <summary>
        ''' pfo_side_guid, pto_side_guid: id of the relation between reference regulatory element and current regulatory element, 
        ''' which positions are indicated  in pos_from, pos_to fields (from ObjSideType dictionary (for instance, transcription 
        ''' start, translation start, transcription end, translation end; see part 5)). 
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("pto_side_guid")> Public Property PtoSideGuid As Integer?
        ''' <summary>
        ''' site_len: site length	
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("site_len")> Public Property SiteLen As Integer
        ''' <summary>
        ''' sequence: site sequence
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("sequence")> Public Property Sequence As String
        ''' <summary>
        ''' signature: site signature (if site sequence is too short for certain localization in the genome, 
        ''' annotator had to input longer sequence fragment in “signature” field). Signature must be at least 
        ''' 30 nt. signature - site signature (part of sequence long enough to find site in genome reliably 
        ''' if site sequence is too short) (a number can be placed within this field to represent a particular 
        ''' length of unknown sequence (N's)
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("signature")> Public Property Signature As String
        <DatabaseField("regulator_guid")> Public Property RegulatorGuid As Integer?
        <DatabaseField("site_guid")> Public Overrides Property Guid As Integer

        Public Shared Function ExportFasta(site As Sites, Optional TryAutoFixed As Boolean = False) As SMRUCC.genomics.SequenceModel.FASTA.FastaSeq
            If String.IsNullOrEmpty(site.Sequence) Then
                Return Nothing
            Else
                Dim Fsa As SMRUCC.genomics.SequenceModel.FASTA.FastaSeq = New SequenceModel.FASTA.FastaSeq
                Fsa.SequenceData = site.Sequence.ToUpper
                If TryAutoFixed AndAlso FixSequenceError(Fsa.SequenceData) Then '序列中包含有错误
                    Fsa.Headers = New String() {"site", site.Guid, "tfrg", If(site.RegulatorGuid Is Nothing, -1, site.RegulatorGuid), site.Name, SEQUENCE_ERROR_FIXED}
                    Call Console.WriteLine("Sequence ""{0}"" contains some error, try to fixed it!", Fsa.Title)
                Else '序列中没有任何错误
                    Fsa.Headers = New String() {"site", site.Guid, "tfrg", If(site.RegulatorGuid Is Nothing, -1, site.RegulatorGuid), site.Name}
                End If

                Return Fsa
            End If
        End Function

        Public Const SEQUENCE_ERROR_FIXED As String = "* Fixed!"

        ''' <summary>
        ''' 针对DNA序列进行修复
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Shared Function FixSequenceError(ByRef Sequence As String) As Boolean
            Dim ExistsErrorAndFixed As Boolean = False
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            For Each ch In Sequence
                If ch = "A"c OrElse ch = "G"c OrElse ch = "C"c OrElse ch = "T"c OrElse ch = "U"c Then
                    Call sBuilder.Append(ch)
                Else
                    ExistsErrorAndFixed = True
                End If
            Next
            Sequence = sBuilder.ToString

            Return ExistsErrorAndFixed
        End Function
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE `genes` (
    '''  `gene_guid` int(11) NOT NULL DEFAULT '0',
    '''  `pkg_guid` int(11) NOT NULL DEFAULT '0',
    '''  `art_guid` int(11) NOT NULL DEFAULT '0',
    '''  `name` varchar(50) DEFAULT NULL,
    '''  `fl_real_name` int(1) DEFAULT NULL,
    '''  `genome_guid` int(11) DEFAULT NULL,
    '''  `location` varchar(50) DEFAULT NULL,
    '''  `ref_bank1` varchar(255) DEFAULT NULL,
    '''  `ref_bank2` varchar(255) DEFAULT NULL,
    '''  `ref_bank3` varchar(255) DEFAULT NULL,
    '''  `ref_bank4` varchar(255) DEFAULT NULL,
    '''  `signature` text,
    '''  `metabol_path` varchar(100) DEFAULT NULL,
    '''  `ferment_num` varchar(20) DEFAULT NULL,
    '''  `gene_function` varchar(100) DEFAULT NULL,
    '''  `descript` blob,
    '''  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    ''' PRIMARY KEY (`gene_guid`),
    ''' KEY `FK_genes-pkg_guid` (`pkg_guid`),
    ''' KEY `FK_genes-art_guid` (`art_guid`),
    ''' KEY `FK_genes-genome_guid` (`genome_guid`),
    ''' CONSTRAINT `FK_genes-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
    ''' CONSTRAINT `FK_genes-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
    ''' CONSTRAINT `FK_genes-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
    ''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
    ''' </remarks>
    Public Class Gene : Inherits RegulatoryElement
        Implements IPolymerSequenceModel

        ''' <summary>
        ''' location: localization in genome
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("location")> Public Property location As String
        <DatabaseField("ref_bank1")> Public Property ref_bank1 As String
        <DatabaseField("ref_bank2")> Public Property ref_bank2 As String
        <DatabaseField("ref_bank3")> Public Property ref_bank3 As String
        <DatabaseField("ref_bank4")> Public Property ref_bank4 As String
        ''' <summary>
        ''' signature: gene signature (30 nt beginning from start codon or 30 aa from N-end of the protein).
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("signature")> Public Property signature As String Implements IPolymerSequenceModel.SequenceData
        ''' <summary>
        ''' metabol_path: metabolic pathway, for genes encoding enzymes or transporters
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("metabol_path")> Public Property metabol_path As String
        ''' <summary>
        ''' ferment_num: EC number
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("ferment_num")> Public Property ferment_num As String
        ''' <summary>
        ''' gene_function: function of the protein encoded by the gene
        ''' </summary>
        ''' <remarks></remarks>
        <DatabaseField("gene_function")> Public Property gene_function As String
        ''' <summary>
        ''' gene_guid
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DatabaseField("gene_guid")> Public Overrides Property Guid As Integer
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE `transcripts` (
    '''   `transcript_guid` int(11) NOT NULL DEFAULT '0',
    '''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
    '''   `art_guid` int(11) NOT NULL DEFAULT '0',
    '''   `name` varchar(50) DEFAULT NULL,
    '''   `fl_real_name` int(1) DEFAULT NULL,
    '''   `genome_guid` int(11) DEFAULT NULL,
    '''   `pos_from` int(11) DEFAULT NULL,
    '''   `pos_from_guid` int(11) DEFAULT NULL,
    '''   `pfo_type_id` int(11) DEFAULT NULL,
    '''   `pfo_side_guid` int(11) DEFAULT NULL,
    '''   `pos_to` int(11) DEFAULT NULL,
    '''   `pos_to_guid` int(11) DEFAULT NULL,
    '''   `pto_type_id` int(11) DEFAULT NULL,
    '''   `pto_side_guid` int(11) DEFAULT NULL,
    '''   `tr_len` int(11) DEFAULT NULL,
    '''   `descript` blob,
    '''   `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    '''   PRIMARY KEY (`transcript_guid`),
    '''   KEY `FK_transcripts-pkg_guid` (`pkg_guid`),
    '''   KEY `FK_transcripts-art_guid` (`art_guid`),
    '''   KEY `FK_transcripts-genome_guid` (`genome_guid`),
    '''   CONSTRAINT `FK_transcripts-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
    '''   CONSTRAINT `FK_transcripts-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
    '''   CONSTRAINT `FK_transcripts-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
    ''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
    ''' </remarks>
    Public Class Transcript : Inherits RegulatoryElement
        ''' <summary>
        ''' pos_from - start position (relative to some regulatory element)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property pos_from As Integer
        ''' <summary>
        ''' pos_from_guid - regulatory element to which start of the secondary structure is binded 
        ''' (link to some table with regulatory elements)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property pos_from_guid As Integer
        ''' <summary>
        ''' pos_to - end position (relative to some regulatory element)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property pos_to As Integer
        ''' <summary>
        ''' pfo_type_id - type of this regulatory element (link to obj_types)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property pfo_type_id As Integer
        ''' <summary>
        ''' pfo_side_guid - location in this regulatory element to which start is binded (e.g. start, 
        ''' end, transcription start, translation start etc.; link to dict_obj_side_types)        
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property pfo_side_guid As Integer
        ''' <summary>
        ''' pos_to_guid - regulatory element dto which end position is binded (link to some table with
        ''' regulatory elements)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property pos_to_guid As Integer
        ''' <summary>
        ''' pto_type_id - type of this regulatory element (link to obj_types)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property pto_type_id As Integer
        ''' <summary>
        ''' pto_side_guid - location in this regulatory element to which end is binded (e.g. start, end, 
        ''' transcription start, translation start etc.; link to dict_obj_side_types)  
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property pto_side_guid As Integer
        ''' <summary>
        ''' transcript length
        ''' </summary>
        ''' <remarks></remarks>
        Public Property tr_len As Integer
        ''' <summary>
        ''' transcript_guid
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Property Guid As Integer
    End Class

    Public Class Operon : Inherits RegulatoryElement
        Public Overrides Property Guid As Integer
    End Class

    Public Class Locuses : Inherits RegulatoryElement
        Public Property location As String
        Public Overrides Property Guid As Integer
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE `regulons` (
    '''   `regulon_guid` int(11) NOT NULL DEFAULT '0',
    '''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
    '''   `art_guid` int(11) NOT NULL DEFAULT '0',
    '''   `name` varchar(50) DEFAULT NULL,
    '''   `fl_real_name` int(1) DEFAULT NULL,
    '''   `genome_guid` int(11) DEFAULT NULL,
    '''   `regulator_guid` int(11) DEFAULT NULL,
    '''   `descript` blob,
    '''   `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    '''   PRIMARY KEY (`regulon_guid`),
    '''   KEY `FK_regulons-pkg_guid` (`pkg_guid`),
    '''   KEY `FK_regulons-art_guid` (`art_guid`),
    '''   KEY `FK_regulons-genome_guid` (`genome_guid`),
    '''   KEY `FK_regulons-regulator_guid` (`regulator_guid`),
    '''   CONSTRAINT `FK_regulons-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
    '''   CONSTRAINT `FK_regulons-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
    '''   CONSTRAINT `FK_regulons-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
    '''   CONSTRAINT `regulons_ibfk_1` FOREIGN KEY (`regulator_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
    ''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
    ''' SELECT * FROM dbregulation_update.sites;
    ''' </remarks>
    Public Class Regulon : Inherits RegulatoryElement
        ''' <summary>
        ''' regulon_guid: id of the regulator for the regulon
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Property Guid As Integer
    End Class

    ''' <summary>
    ''' Secondary structure
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE `sec_structures` (
    '''   `sec_struct_guid` int(11) NOT NULL DEFAULT '0',
    '''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
    '''   `art_guid` int(11) NOT NULL DEFAULT '0',
    '''   `name` varchar(50) DEFAULT NULL,
    '''   `fl_real_name` int(1) DEFAULT NULL,
    '''   `genome_guid` int(11) DEFAULT NULL,
    '''   `pos_from` int(11) DEFAULT NULL,
    '''   `pos_from_guid` int(11) DEFAULT NULL,
    '''   `pfo_type_id` int(11) DEFAULT NULL,
    '''   `pfo_side_guid` int(11) DEFAULT NULL,
    '''   `pos_to` int(11) DEFAULT NULL,
    '''   `pos_to_guid` int(11) DEFAULT NULL,
    '''   `pto_type_id` int(11) DEFAULT NULL,
    '''   `pto_side_guid` int(11) DEFAULT NULL,
    '''   `sequence` varchar(255) DEFAULT NULL,
    '''   `descript` blob,
    '''   PRIMARY KEY (`sec_struct_guid`),
    '''   KEY `FK_sec_structures-pkg_guid` (`pkg_guid`),
    '''   KEY `FK_sec_structures-art_guid` (`art_guid`),
    '''   KEY `FK_sec_structures-genome_guid` (`genome_guid`),
    '''   CONSTRAINT `FK_sec_structures-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
    '''   CONSTRAINT `FK_sec_structures-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
    '''   CONSTRAINT `FK_sec_structures-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
    ''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
    ''' </remarks>
    Public Class sec_structures : Inherits RegulatoryElement
        ''' <summary>
        ''' sequence: sequence of the RNA fragment
        ''' </summary>
        ''' <remarks></remarks>
        Public Property sequence As String
        Public Property pos_from As Integer
        Public Property pos_from_guid As Integer
        Public Property pos_to As Integer
        Public Property pfo_type_id As Integer
        Public Property pfo_side_guid As Integer
        Public Property pos_to_guid As Integer
        Public Property pto_type_id As Integer
        Public Property pto_side_guid As Integer
        Public Overrides Property Guid As Integer
    End Class

    ''' <summary>
    ''' helices - determine secondary structure
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE `helices` (
    '''   `helix_guid` int(11) NOT NULL DEFAULT '0',
    '''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
    '''   `art_guid` int(11) NOT NULL DEFAULT '0',
    '''   `name` varchar(50) DEFAULT NULL,
    '''   `fl_real_name` int(1) DEFAULT NULL,
    '''   `genome_guid` int(11) DEFAULT NULL,
    '''   `sec_struct_guid` int(11) DEFAULT NULL,
    '''   `pos_from1` int(11) DEFAULT NULL,
    '''   `pos_to1` int(11) DEFAULT NULL,
    '''   `pos_from2` int(11) DEFAULT NULL,
    '''   `pos_to2` int(11) DEFAULT NULL,
    '''   `descript` blob,
    '''   PRIMARY KEY (`helix_guid`),
    '''   KEY `FK_helices-pkg_guid` (`pkg_guid`),
    '''   KEY `FK_helices-art_guid` (`art_guid`),
    '''   KEY `FK_helices-genome_guid` (`genome_guid`),
    '''   KEY `FK_helices-sec_struct_guid` (`sec_struct_guid`),
    '''   CONSTRAINT `FK_helices-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
    '''   CONSTRAINT `FK_helices-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
    '''   CONSTRAINT `FK_helices-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
    '''   CONSTRAINT `FK_helices-sec_struct_guid` FOREIGN KEY (`sec_struct_guid`) REFERENCES `sec_structures` (`sec_struct_guid`),
    '''   CONSTRAINT `helices_ibfk_1` FOREIGN KEY (`sec_struct_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
    ''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
    ''' </remarks>
    Public Class helices : Inherits RegulatoryElement
        ''' <summary>
        ''' sec_struc_guid: id of the RNA secondary structure containing the helix
        ''' </summary>
        ''' <remarks></remarks>
        Public Property sec_struc_guid As Integer
        ''' <summary>
        ''' pos_from1, pos_to1, pos_from2, pos_to2 : helix coordinates relative to start of RNA secondary structure
        ''' </summary>
        ''' <remarks></remarks>
        Public Property pos_from1 As Integer
        Public Property pos_to1 As Integer
        Public Property pos_from2 As Integer
        Public Property pos_to2 As Integer
        ''' <summary>
        ''' helix_guid
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Property Guid As Integer
    End Class

    ''' <summary>
    ''' All regulatory elements except Inductor (Effector) can have name synonyms. Name synonyms are stored 
    ''' in ObjSynonym table.
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE `obj_synonyms` (
    '''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
    '''   `art_guid` int(11) NOT NULL DEFAULT '0',
    '''   `obj_guid` int(11) NOT NULL DEFAULT '0',
    '''   `syn_name` varchar(50) NOT NULL DEFAULT '',
    '''   `fl_real_name` int(1) DEFAULT NULL,
    '''   PRIMARY KEY (`obj_guid`,`syn_name`),
    '''   KEY `pkg_guid` (`pkg_guid`),
    '''   KEY `art_guid` (`art_guid`),
    '''   CONSTRAINT `obj_synonyms_ibfk_1` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
    '''   CONSTRAINT `obj_synonyms_ibfk_2` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
    '''   CONSTRAINT `obj_synonyms_ibfk_3` FOREIGN KEY (`obj_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
    ''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
    ''' </remarks>
    Public Class ObjSynonyms
        Public Property pkg_guid As Integer
        Public Property art_guid As Integer
        ''' <summary>
        ''' obj_guid: id of object, for which the synonym is used
        ''' </summary>
        ''' <remarks></remarks>
        Public Property obj_guid As Integer
        ''' <summary>
        ''' syn_name: name synonym
        ''' </summary>
        ''' <remarks></remarks>
        Public Property syn_name As String
        Public Property fl_real_name As Integer
    End Class
End Namespace
