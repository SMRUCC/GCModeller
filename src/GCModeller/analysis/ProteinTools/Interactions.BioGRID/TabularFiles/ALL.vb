#Region "Microsoft.VisualBasic::d995046eb3628a100e1e70e3f4b04b0a, analysis\ProteinTools\Interactions.BioGRID\TabularFiles\ALL.vb"

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

    ' Class ALLmitab
    ' 
    '     Properties: A, AliasA, AliasB, AltA, AltB
    '                 Author, B, Confidence, Database, IDM
    '                 InteractType, Publication, TaxidA, TaxidB, uid
    ' 
    '     Function: ToString
    ' 
    ' Class ALL
    ' 
    '     Properties: Author, bgA, bgB, ExprSys, ExprType
    '                 ezgA, ezgB, Modification, NamedA, NamedB
    '                 ofSblA, ofSblB, OgsmA, OgsmB, Phenotypes
    '                 Pubmed, Qualifications, Score, Source, SynoA
    '                 SynoB, Tags, Throughput, uid
    ' 
    ' Class Chemical
    ' 
    '     Properties: Action, ATC, Author, bgCheId, bgId
    '                 bgPubId, Brands, CAS, Curated, ezgId
    '                 Formula, itrType, Name, ofSbl, OgsmId
    '                 Organism, Pubmed, Source, SourceId, Syno
    '                 Synonyms, sysName, Type, uid
    ' 
    '     Class Relationships
    ' 
    '         Properties: Author, bgId, ezgId, Identity, ofSbl
    '                     OgsmId, OgsmName, Pubmed, Relationship, Source
    '                     Synonymns, sysName, uid
    ' 
    '     Class PTM
    ' 
    '         Properties: [True], Author, bgId, ezgId, Notes
    '                     ofSbl, OgsmId, OgsmName, Position, PTM
    '                     Pubmed, Refseq, Residue, Sequence, Source
    '                     Synonymns, sysName, uid
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' ``BIOGRID-ALL-3.4.138.mitab.zip``
''' 
''' This zip archive contains a Single file formatted In PSI MITAB level 2.5 compatible 
''' Tab Delimited Text file format containing all interaction And associated annotation 
''' data from the BIOGRID For all species And experimental systems. 
''' </summary>
Public Class ALLmitab

    ''' <summary>
    ''' #ID Interactor A
    ''' </summary>
    ''' <returns></returns>
    <Column("#ID Interactor A")> Public Property A As String
    ''' <summary>
    ''' ID Interactor B
    ''' </summary>
    ''' <returns></returns>
    <Column("ID Interactor B")> Public Property B As String
    ''' <summary>
    ''' Alt IDs Interactor A
    ''' </summary>
    ''' <returns></returns>
    <Column("Alt IDs Interactor A")> Public Property AltA As String
    ''' <summary>
    ''' Alt IDs Interactor B
    ''' </summary>
    ''' <returns></returns>
    <Column("Alt IDs Interactor B")> Public Property AltB As String
    ''' <summary>
    ''' Aliases Interactor A
    ''' </summary>
    ''' <returns></returns>
    <Column("Aliases Interactor A")> Public Property AliasA As String
    ''' <summary>
    ''' Aliases Interactor B
    ''' </summary>
    ''' <returns></returns>
    <Column("Aliases Interactor B")> Public Property AliasB As String
    ''' <summary>
    ''' Interaction Detection Method
    ''' </summary>
    ''' <returns></returns>
    <Column("Interaction Detection Method")> Public Property IDM As String
    ''' <summary>
    ''' Publication 1St Author
    ''' </summary>
    ''' <returns></returns>
    <Column("Publication 1St Author")> Public Property Author As String
    ''' <summary>
    ''' Publication Identifiers
    ''' </summary>
    ''' <returns></returns>
    <Column("Publication Identifiers")> Public Property Publication As String
    ''' <summary>
    ''' Taxid Interactor A
    ''' </summary>
    ''' <returns></returns>
    <Column("Taxid Interactor A")> Public Property TaxidA As String
    ''' <summary>
    ''' Taxid Interactor B
    ''' </summary>
    ''' <returns></returns>
    <Column("Taxid Interactor B")> Public Property TaxidB As String
    ''' <summary>
    ''' Interaction Types
    ''' </summary>
    ''' <returns></returns>
    <Column("Interaction Types")> Public Property InteractType As String
    ''' <summary>
    ''' Source Database
    ''' </summary>
    ''' <returns></returns>
    <Column("Source Database")> Public Property Database As String
    ''' <summary>
    ''' Interaction Identifiers
    ''' </summary>
    ''' <returns></returns>
    <Column("Interaction Identifiers")> Public Property uid As String
    ''' <summary>
    ''' Confidence Values
    ''' </summary>
    ''' <returns></returns>
    <Column("Confidence Values")> Public Property Confidence As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

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
