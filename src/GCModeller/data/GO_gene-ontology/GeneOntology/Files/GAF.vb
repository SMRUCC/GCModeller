#Region "Microsoft.VisualBasic::2a7850ba9d83fe9779e27988eac60a60, GCModeller\data\GO_gene-ontology\GeneOntology\Files\GAF.vb"

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

    '   Total Lines: 410
    '    Code Lines: 118
    ' Comment Lines: 259
    '   Blank Lines: 33
    '     File Size: 21.53 KB


    ' Class GAF
    ' 
    '     Properties: [Date], AnnotationExtension, Aspect, AssignedBy, DB
    '                 DBObjectID, DBObjectName, DBObjectSymbol, DBObjectSynonym, DBObjectType
    '                 DBReference, EvidenceCode, GeneProductFormID, GOID, Qualifier
    '                 Taxon, WithOrFrom
    ' 
    '     Function: GenerateLine, (+2 Overloads) Load, Save, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Reflection

''' <summary>
''' GO Annotation File (GAF) Format 2.0
''' </summary>
''' <remarks></remarks>
Public Class GAF

    ''' <summary>
    ''' ```
    ''' !GOC Validation Date: 10/04/2014 $
    ''' !Submission Date: 10/4/2014
    ''' !
    ''' !The above ""Submission Date"" Is when the annotation project provided
    ''' !this file to the Gene Ontology Consortium (GOC).  The ""GOC Validation
    ''' !Date"" indicates when this file was last changed as a result of a GOC
    ''' !validation And filtering process.  
    ''' !
    ''' !Note: The contents Of this file may differ from that submitted To the
    ''' !GOC.The identifiers And syntax Of the file have been checked, rows of
    ''' !data Not meeting the standards set by the GOC have been removed. This
    ''' !file may also have annotations removed because the annotations for the
    ''' !listed Taxonomy identifier are only allowed in a file provided by
    ''' !another annotation project.  The original submitted file Is available from
    ''' !http : //www.geneontology.org/gene-associations/submission/
    ''' !
    ''' !For information on which taxon are allowed in which files please see
    ''' !http : //www.geneontology.org/GO.annotation.shtml#script
    ''' !
    ''' !gaf-version:  2.0
    ''' !Project_name: Saccharomyces Genome Database (SGD)
    ''' !URL :  http://www.yeastgenome.org/
    ''' !Contact Email:  sgd-helpdesk@lists.stanford.edu
    ''' !Funding :  NHGRI of US National Institutes of Health, HG001315
    ''' !Date :  10/04/2014 $
    ''' !
    ''' ```
    ''' </summary>
    Const GAF_Heads As String = "
!GOC Validation Date: 10/04/2014 $
!Submission Date: 10/4/2014
!
! The above ""Submission Date"" is when the annotation project provided
! this file to the Gene Ontology Consortium (GOC).  The ""GOC Validation
! Date"" indicates when this file was last changed as a result of a GOC
! validation and filtering process.  
!
! Note: The contents of this file may differ from that submitted to the
! GOC. The identifiers and syntax of the file have been checked, rows of
! data not meeting the standards set by the GOC have been removed. This
! file may also have annotations removed because the annotations for the
! listed Taxonomy identifier are only allowed in a file provided by
! another annotation project.  The original submitted file is available from:
!  http://www.geneontology.org/gene-associations/submission/
!
! For information on which taxon are allowed in which files please see:
!  http://www.geneontology.org/GO.annotation.shtml#script
!
!gaf-version: 2.0
!Project_name: Saccharomyces Genome Database (SGD)
!URL: http://www.yeastgenome.org/
!Contact Email: sgd-helpdesk@lists.stanford.edu
!Funding: NHGRI of US National Institutes of Health, HG001315
!Date: 10/04/2014 $
!
"

#Region "Definitions and requirements for field contents"

    ''' <summary>
    ''' refers to the database from which the identifier in DB object ID (column 2) is drawn. This is not necessarily the 
    ''' group submitting the file. If a UniProtKB ID is the DB object ID (column 2), DB (column 1) should be UniProtKB.
    ''' must be one of the values from the set of GO database cross-references
    ''' this field is mandatory, cardinality 1
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(1)> Public Property DB As String ' (column 1)

    ''' <summary>
    ''' a unique identifier from the database in DB (column 1) for the item being annotated
    ''' this field is mandatory, cardinality 1
    ''' In GAF 2.0 format, the identifier must reference a top-level primary gene or gene product identifier: either a gene, 
    ''' or a protein that has a 1:1 correspondence to a gene. Identifiers referring to particular protein isoforms or 
    ''' post-translationally cleaved or modified proteins are not legal values in this field.
    ''' The DB object ID (column 2) is the identifier for the database object, which may or may not correspond exactly to 
    ''' what is described in a paper. For example, a paper describing a protein may support annotations to the gene encoding 
    ''' the protein (gene ID in DB object ID field) or annotations to a protein object (protein ID in DB object ID field).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(2)> Public Property DBObjectID As String ' (column 2)

    ''' <summary>
    ''' a (unique and valid) symbol to which DB object ID is matched
    ''' can use ORF name for otherwise unnamed gene or protein
    ''' if gene products are annotated, can use gene product symbol if available, or many gene product annotation entries 
    ''' can share a gene symbol this field is mandatory, cardinality 1
    ''' The DB Object Symbol field should be a symbol that means something to a biologist wherever possible (a gene symbol, 
    ''' for example). It is not an ID or an accession number (DB object ID [column 2] provides the unique identifier), 
    ''' although IDs can be used as a DB object symbol if there is no more biologically meaningful symbol available (e.g., 
    ''' when an unnamed gene is annotated).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(3)> Public Property DBObjectSymbol As String ' (column 3)

    ''' <summary>
    ''' flags that modify the interpretation of an annotation
    ''' one (or more) of NOT, contributes_to, colocalizes_with
    ''' this field is not mandatory; cardinality 0, 1, >1; for cardinality >1 use a pipe to separate entries (e.g. NOT|contributes_to)
    ''' See also the documentation on qualifiers in the GO annotation guide
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(4)> Public Property Qualifier As String ' (column 4)

    ''' <summary>
    ''' the GO identifier for the term attributed to the DB object ID
    ''' this field is mandatory, cardinality 1
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(5)> Public Property GOID As String ' (column 5)

    ''' <summary>
    ''' one or more unique identifiers for a single source cited as an authority for the attribution of the GO ID to the DB 
    ''' object ID. This may be a literature reference or a database record. The syntax is DB:accession_number.
    ''' Note that only one reference can be cited on a single line in the gene association file. If a reference has identifiers 
    ''' in more than one database, multiple identifiers for that reference can be included on a single line. For example, 
    ''' if the reference is a published paper that has a PubMed ID, we strongly recommend that the PubMed ID be included, 
    ''' as well as an identifier within a model organism database. Note that if the model organism database has an identifier 
    ''' for the reference, that identifier should always be included, even if a PubMed ID is also used.
    ''' this field is mandatory, cardinality 1, >1; for cardinality >1 use a pipe to separate entries 
    ''' (e.g. SGD_REF:S000047763|PMID:2676709).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(6)> Public Property DBReference As String ' (column 6)

    ''' <summary>
    ''' see the GO evidence code guide for the list of valid evidence codes for GO annotations
    ''' this field is mandatory, cardinality 1
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(7)> Public Property EvidenceCode As String ' (column 7)

    ''' <summary>
    ''' Also referred to as with, from or the with/from column
    ''' one of:
    ''' DB:gene_symbol
    ''' DB:gene_symbol[allele_symbol]
    ''' DB:gene_id
    ''' DB:protein_name
    ''' DB:sequence_id
    ''' GO:GO_id
    ''' CHEBI:CHEBI_id
    ''' this field is not mandatory overall, but is required for some evidence codes (see below and the evidence code 
    ''' documentation for details); cardinality 0, 1, >1; for cardinality >1 use a pipe to separate entries 
    ''' (e.g. CGSC:pabA|CGSC:pabB)
    ''' Note: This field is used to hold an additional identifier for annotations using certain evidence codes 
    ''' (IC, IEA, IGI, IPI, ISS). For example, it can identify another gene product to which the annotated gene product 
    ''' is similar (ISS) or interacts with (IPI). More information on the meaning of with or from column entries is 
    ''' available in the evidence code documentation entries for the relevant codes.
    ''' Cardinality = 0 is not recommended, but is permitted because cases can be found in literature where no database 
    ''' identifier can be found (e.g. physical interaction or sequence similarity to a protein, but no ID provided). 
    ''' Cardinality = 0 is not allowed for ISS annotations made after October 1, 2006. Annotations where evidence is IGI, 
    ''' IPI, or ISS and with cardinality = 0 should link to an explanation of why there is no entry in with. Cardinality 
    ''' may be >1 for any of the evidence codes that use with; for IPI and IGI cardinality >1 has a special meaning (see 
    ''' evidence documentation for more information). For cardinality >1 use a pipe to separate entries 
    ''' (e.g. FB:FBgn1111111|FB:FBgn2222222).
    ''' Note that a gene ID may be used in the with column for a IPI annotation, or for an ISS annotation based on amino 
    ''' acid sequence or protein structure similarity, if the database does not have identifiers for individual gene 
    ''' products. A gene ID may also be used if the cited reference provides enough information to determine which gene ID 
    ''' should be used, but not enough to establish which protein ID is correct.
    ''' 'GO:GO_id' is used only when the evidence code is IC, and refers to the GO term(s) used as the basis of a curator 
    ''' inference. In these cases the entry in the 'DB:Reference' column will be that used to assign the GO term(s) from 
    ''' which the inference is made. This field is mandatory for evidence code IC.
    ''' The ID is usually an identifier for an individual entry in a database (such as a sequence ID, gene ID, GO ID, etc.). 
    ''' Identifiers from the Center for Biological Sequence Analysis (CBS), however, represent tools used to find homology 
    ''' or sequence similarity; these identifiers can be used in the with column for ISS annotations.
    ''' The with column may not be used with the evidence codes IDA, TAS, NAS, or ND.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(8)> Public Property WithOrFrom As String ' (column 8)

    ''' <summary>
    ''' refers to the namespace or ontology to which the GO ID (column 5) belongs; one of P (biological process), F (molecular function) or C (cellular component)
    ''' this field is mandatory; cardinality 1
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(9)> Public Property Aspect As String ' (column 9)

    ''' <summary>
    ''' name of gene or gene product
    ''' this field is not mandatory, cardinality 0, 1 [white space allowed]
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(10)> Public Property DBObjectName As String ' (column 10)

    ''' <summary>
    ''' Gene symbol [or other text] Note that we strongly recommend that gene synonyms are included in the gene association 
    ''' file, as this aids the searching of GO.
    ''' this field is not mandatory, cardinality 0, 1, >1 [white space allowed]; for cardinality >1 use a pipe to separate 
    ''' entries (e.g. YFL039C|ABY1|END7|actin gene)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(11)> Public Property DBObjectSynonym As String ' (column 11)

    ''' <summary>
    ''' A description of the type of gene product being annotated. If a gene product form ID (column 17) is supplied, the DB 
    ''' object type will refer to that entity; if no gene product form ID is present, it will refer to the entity that the DB 
    ''' object symbol (column 2) is believed to produce and which actively carries out the function or localization described. 
    ''' one of the following: protein_complex; protein; transcript; ncRNA; rRNA; tRNA; snRNA; snoRNA; any subtype of ncRNA 
    ''' in the Sequence Ontology. If the precise product type is unknown, gene_product should be used. this field is mandatory, 
    ''' cardinality 1
    ''' The object type (gene_product, transcript, protein, protein_complex, etc.) listed in the DB object type field must 
    ''' match the database entry identified by the gene product form ID, or, if this is absent, the expected product of the 
    ''' DB object ID. Note that DB object type refers to the database entry (i.e. it represents a protein, functional RNA, etc.); 
    ''' this column does not reflect anything about the GO term or the evidence on which the annotation is based. For example, 
    ''' if your database entry represents a protein-encoding gene, then protein goes in the DB object type column. The text 
    ''' entered in the DB object name and DB object symbol should refer to the entity in DB object ID. For example, several 
    ''' alternative transcripts from one gene may be annotated separately, each with the same gene ID in DB object ID, and 
    ''' specific gene product identifiers in gene product form ID, but list the same gene symbol in the DB object symbol column.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(12)> Public Property DBObjectType As String ' (column 12)

    ''' <summary>
    ''' taxonomic identifier(s) For cardinality 1, the ID of the species encoding the gene product. For cardinality 2, to be 
    ''' used only in conjunction with terms that have the biological process term multi-organism process or the cellular component 
    ''' term host cell as an ancestor. The first taxon ID should be that of the organism encoding the gene or gene product, and
    ''' the taxon ID after the pipe should be that of the other organism in the interaction. this field is mandatory, 
    ''' cardinality 1, 2; for cardinality 2 use a pipe to separate entries (e.g. taxon:1|taxon:1000) See the GO annotation 
    ''' conventions for more information on multi-organism terms.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(13)> Public Property Taxon As String ' (column 13)

    ''' <summary>
    ''' Date on which the annotation was made; format is YYYYMMDD
    ''' this field is mandatory, cardinality 1
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(14)> Public Property [Date] As String ' (column 14)

    ''' <summary>
    ''' The database which made the annotation
    ''' one of the values from the set of GO database cross-references
    ''' Used for tracking the source of an individual annotation. Default value is value entered as the DB (column 1).
    ''' Value will differ from column 1 for any annotation that is made by one database and incorporated into another.
    ''' this field is mandatory, cardinality 1
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(15)> Public Property AssignedBy As String ' (column 15)

    ''' <summary>
    ''' one of:
    ''' DB:gene_id
    ''' DB:sequence_id
    ''' CHEBI:CHEBI_id
    ''' Cell Type Ontology:CL_id
    ''' GO:GO_id
    ''' Contains cross references to other ontologies that can be used to qualify or enhance the annotation. The cross-reference 
    ''' is prefaced by an appropriate GO relationship; references to multiple ontologies can be entered. For example, if a gene 
    ''' product is localized to the mitochondria of lymphocytes, the GO ID (column 5) would be mitochondrion ; GO:0005439, 
    ''' and the annotation extension column would contain a cross-reference to the term lymphocyte from the Cell Type Ontology.
    ''' Targets of certain processes or functions can also be included in this field to indicate the gene, gene product, or chemical 
    ''' involved; for example, if a gene product is annotated to protein kinase activity, the annotation extension column would 
    ''' contain the UniProtKB protein ID for the protein phosphorylated in the reaction.
    ''' See the documentation on using the annotation extension column for details of practical usage; a wider discussion of the 
    ''' annotation extension column can be found on the GO wiki.
    ''' this field is optional, cardinality 0 or greater
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(16)> Public Property AnnotationExtension As String ' (column 16)

    ''' <summary>
    ''' As the DB Object ID (column 2) entry must be a canonical entity—a gene OR an abstract protein that has a 1:1 
    ''' correspondence to a gene—this field allows the annotation of specific variants of that gene or gene product. 
    ''' Contents will frequently include protein sequence identifiers: for example, identifiers that specify distinct 
    ''' proteins produced by to differential splicing, alternative translational starts, post-translational cleavage or 
    ''' post-translational modification. Identifiers for functional RNAs can also be included in this column.
    ''' The identifier used must be a standard 2-part global identifier, e.g. UniProtKB:OK0206-2
    ''' When the gene product form ID (column 17) is filled with a protein identifier, the value in DB object type (column 12) 
    ''' must be protein. Protein identifiers can include UniProtKB accession numbers, NCBI NP identifiers or Protein Ontology 
    ''' (PRO) identifiers.
    ''' When the gene product form ID (column 17) is filled with a functional RNA identifier, the DB object type (column 12) 
    ''' must be either ncRNA, rRNA, tRNA, snRNA, or snoRNA.
    ''' This column may be left blank; if so, the value in DB object type (column 12) will provide a description of the 
    ''' expected gene product.
    ''' More information and examples are available from the GO wiki page on column 17.
    ''' Note that several fields contain database cross-reference (dbxrefs) in the format dbname:dbaccession. 
    ''' The fields are: GO ID [column 5], where dbname is always GO; DB:Reference (column 6); With or From (column 8); and 
    ''' Taxon (column 13), where dbname is always taxon. For GO IDs, do not repeat the 'GO:' prefix (i.e. always use GO:0000000, 
    ''' not GO:GO:0000000)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Field(17)> Public Property GeneProductFormID As String ' (column 17)

#End Region

    Public Overrides Function ToString() As String
        Return String.Format("[{0}] {1}: {2}", GOID, DBObjectID, DBObjectName)
    End Function

    Private Function GenerateLine() As String
        Dim array As String() = {
            DB,
            DBObjectID,
            DBObjectSymbol,
            Qualifier,
            GOID,
            DBReference,
            EvidenceCode,
            WithOrFrom,
            Aspect,
            DBObjectName,
            DBObjectSynonym,
            DBObjectType,
            Taxon,
            [Date],
            AssignedBy,
            AnnotationExtension,
            GeneProductFormID
        }
        Return String.Join(vbTab, array)
    End Function

    Public Shared Function Save(GAFLines As GAF(), path As String) As Boolean
        Dim bufs As New List(Of String)

        Call bufs.Add(GAF.GAF_Heads.Trim(ASCII.CR, ASCII.LF))
        Call bufs.AddRange(
            GAFLines.Select(Function(x) x.GenerateLine))

        Return bufs.SaveTo(path, Encoding.ASCII)
    End Function

    Public Shared Function Load(path As String) As GAF()
        Dim strLines As String() = LinqAPI.Exec(Of String) _
 _
            () <= From strLine As String
                  In path.ReadAllLines
                  Where strLine.First <> "!"c
                  Select strLine

        Dim schemaBufs =
            From x As SchemaMaps.BindProperty(Of Field)
            In LoadClassSchema(Of GAF)().Values
            Select x
            Order By x.field.index Ascending

        Dim classSchema = schemaBufs _
            .Select(Function(x) DirectCast(x.member, PropertyInfo)) _
            .ToArray
        Dim LQuery As GAF() = LinqAPI.Exec(Of GAF) _
 _
            () <= From strLine As String
                  In strLines
                  Select Load(strLine, classSchema)

        Return LQuery
    End Function

    Private Shared Function Load(strValue As String, ClassSchema As PropertyInfo()) As GAF
        Dim bufs As String() = Strings.Split(strValue, vbTab)
        Dim Target As New GAF

        For i As Integer = 0 To bufs.Length - 1
            Call ClassSchema(i).SetValue(Target, bufs(i))
        Next

        Return Target
    End Function
End Class
