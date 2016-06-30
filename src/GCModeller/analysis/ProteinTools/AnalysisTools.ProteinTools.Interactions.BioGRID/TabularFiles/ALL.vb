Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

''' <summary>
''' BIOGRID-ALL-3.4.131.tab2.zip
''' This zip archive contains a Single file formatted In BioGRID Tab 2.0 Delimited Text file format containing all interaction 
''' And associated annotation data from the BIOGRID For all species And experimental systems. 
''' 
''' BIOGRID-MV-Physical-3.4.131.tab2.zip
''' This zip file contains a listing Of Physical multi-validated interactions For the entire BioGRID dataset. 
''' For details On how this file Is generated, visit our multi-validated details wiki page. 
''' The file Is provided In BioGRID Tab 2.0 format.
''' 
''' BIOGRID-ORGANISM-3.4.131.tab2.zip
''' This zip archive contains multiple files formatted In BioGRID Tab 2.0 Delimited Text file format containing all interaction 
''' And associated annotation data from the BIOGRID separated into seperate distinct files based On Organism.
''' 
''' BIOGRID-SYSTEM-3.4.131.tab2.zip
''' This zip archive contains multiple files formatted In BioGRID Tab 2.0 Delimited Text file format containing all interaction And 
''' associated annotation data from the BIOGRID separated into seperate distinct files based On Experimental System. 
''' </summary>
Public Class ALL
    <Column("#BioGRID Interaction ID")> Public Property uid As String
    <Column("Entrez Gene Interactor A")> Public Property ezgA As String
    <Column("Entrez Gene Interactor B")> Public Property ezgB As String
    <Column("BioGRID ID Interactor A")> Public Property bgA As String
    <Column("BioGRID ID Interactor B")> Public Property bgB As String
    <Column("Systematic Name Interactor A")> Public Property NamedA As String
    <Column("Systematic Name Interactor B")> Public Property NamedB As String
    <Column("Official Symbol Interactor A")> Public Property ofSblA As String
    <Column("Official Symbol Interactor B")> Public Property ofSblB As String
    <Column("Synonyms Interactor A")> Public Property SynoA As String
    <Column("Synonyms Interactor B")> Public Property SynoB As String
    <Column("Experimental System")> Public Property ExprSys As String
    <Column("Experimental System Type")> Public Property ExprType As String
    Public Property Author As String
    <Column("Pubmed ID")> Public Property Pubmed As String
    <Column("Organism Interactor A")> Public Property OgsmA As String
    <Column("Organism Interactor B")> Public Property OgsmB As String
    Public Property Throughput As String
    Public Property Score As String
    Public Property Modification As String
    Public Property Phenotypes As String
    Public Property Qualifications As String
    Public Property Tags As String
    <Column("Source Database")> Public Property Source As String
End Class

''' <summary>
''' BIOGRID-CHEMICALS-3.4.131.chemtab.zip
''' This zip file contains a listing Of protein-chemical interactions For the entire BioGRID dataset. 
''' The file Is a modified version Of the BioGRID Tab 2.0 format, designed To add In columns For chemical specific annotation. 
''' </summary>
Public Class Chemical
    <Column("#BioGRID Chemical Interaction ID")> Public Property uid As String
    <Column("BioGRID Gene ID")> Public Property bgId As String
    <Column("Entrez Gene ID")> Public Property ezgId As String
    <Column("Systematic Name")> Public Property sysName As String
    <Column("Official Symbol")> Public Property ofSbl As String
    Public Property Synonyms As String
    <Column("Organism ID")> Public Property OgsmId As String
    Public Property Organism
    Public Property Action
    <Column("Interaction Type")> Public Property itrType As String
    Public Property Author
    <Column("Pubmed ID")> Public Property Pubmed As String
    <Column("BioGRID Publication ID")> Public Property bgPubId As String
    <Column("BioGRID Chemical ID")> Public Property bgCheId As String
    <Column("Chemical Name")> Public Property Name As String
    <Column("Chemical Synonyms")> Public Property Syno As String
    <Column("Chemical Brands")> Public Property Brands As String
    <Column("Chemical Source")> Public Property Source As String
    <Column("Chemical Source ID")> Public Property SourceId As String
    <Column("Molecular Formula")> Public Property Formula As String
    <Column("Chemical Type")> Public Property Type As String
    <Column("ATC Codes")> Public Property ATC As String
    <Column("CAS Number")> Public Property CAS As String
    <Column("Curated By")> Public Property Curated As String
End Class

Namespace PTMS

    ''' <summary>
    ''' BIOGRID-PTMS-3.4.131.ptm.zip
    ''' This zip archive contains multiple files formatted In PTM And 
    ''' PTMREL format containing post translational modification data And 
    ''' associated annotation data from the BIOGRID.
    ''' </summary>
    Public Class Relationships
        <Column("#PTM ID")> Public Property uid As String
        <Column("Entrez Gene ID")> Public Property ezgId As String
        <Column("BioGRID ID")> Public Property bgId As String
        <Column("Systematic Name")> Public Property sysName As String
        <Column("Official Symbol")> Public Property ofSbl As String
        Public Property Synonymns As String
        Public Property Relationship As String
        Public Property Identity As String
        Public Property Author As String
        <Column("Pubmed ID")> Public Property Pubmed As String
        <Column("Organism ID")> Public Property OgsmId As String
        <Column("Organism Name")> Public Property OgsmName As String
        <Column("Source Database")> Public Property Source As String
    End Class

    ''' <summary>
    ''' BIOGRID-PTMS-3.4.131.ptm.zip
    ''' This zip archive contains multiple files formatted In PTM And 
    ''' PTMREL format containing post translational modification data And 
    ''' associated annotation data from the BIOGRID.
    ''' </summary>
    Public Class PTM
        <Column("#PTM ID")> Public Property uid As String
        <Column("Entrez Gene ID")> Public Property ezgId As String
        <Column("BioGRID ID")> Public Property bgId As String
        <Column("Systematic Name")> Public Property sysName As String
        <Column("Official Symbol")> Public Property ofSbl As String
        Public Property Synonymns As String
        Public Property Sequence As String
        <Column("Refseq ID")> Public Property Refseq As String
        Public Property Position As String
        <Column("Post Translational Modification")> Public Property PTM As String
        Public Property Residue As String
        Public Property Author As String
        <Column("Pubmed ID")> Public Property Pubmed As String
        <Column("Organism ID")> Public Property OgsmId As String
        <Column("Organism Name")> Public Property OgsmName As String
        <Column("Has Relationships")> Public Property [True] As String
        Public Property Notes As String
        <Column("Source Database")> Public Property Source As String
    End Class
End Namespace

