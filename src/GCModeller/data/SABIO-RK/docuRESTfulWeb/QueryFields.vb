#Region "Microsoft.VisualBasic::0b6844cc04a7ae6c2cc70c6bc15bff19, GCModeller\data\SABIO-RK\docuRESTfulWeb\QueryFields.vb"

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

    '   Total Lines: 84
    '    Code Lines: 56
    ' Comment Lines: 27
    '   Blank Lines: 1
    '     File Size: 2.23 KB


    ' Enum QueryFields
    ' 
    '     Activator, AnyRole, AssociatedSpecies, Author, Catalyst
    '     CellularLocation, ChebiID, Cofactor, DataIdentifier, DateSubmitted
    '     ECNumber, EntryID, Enzymename, EnzymeType, ExperimentID
    '     HasKineticData, InChI, Inhibitor, IsRecombinant, KeggCompoundID
    '     KeggID, KeggPathwayID, KeggReactionID, KineticMechanismType, MetaCycReactionID
    '     MetaNetXReactionID, Organism, Organization, OtherModifier, Parametertype
    '     Pathway, pHValueRange, Product, PubChemID, PubMedCID
    '     PubMedID, ReactomeReactionID, RheaReactionID, SabioCompoundID, SabioReactionID
    '     Smiles, Substrate, TemperatureRange, Tissue, Title
    '     UniProtID, UniProtKB_AC, UniprotOmimID, Year
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

''' <summary>
''' The following vocabulary may be used to form queries to search for entries. 
''' These terms are identical to those used in the web interface for forming 
''' queries. An xml document containing all possible search fields is also 
''' accessible at http://sabiork.h-its.org/sabioRestWebServices/searchKineticLaws. 
''' 
''' Queries are formed using one or more of the fields below and should be passed 
''' as a request parameter named "q". Fields may be combined using the boolean 
''' ``AND`` operator to form complex queries.
''' 
''' > http://sabiork.h-its.org/sabioRestWebServices/searchKineticLaws
''' </summary>
Public Enum QueryFields
    ''' <summary>
    ''' EntryID - SABIO-RK entry ID (eg EntryID:123)
    ''' </summary>
    EntryID
    ''' <summary>
    ''' Pathway - The name of the reaction pathway (eg, Pathway:"urea Cycle")
    ''' </summary>
    Pathway
    ''' <summary>
    ''' KeggReactionID - KEGG ID for the reaction (eg KeggReactionID:"R00782")
    ''' </summary>
    KeggReactionID
    ''' <summary>
    ''' SabioReactionID - SABIO-RK ID for the reaction (eg SabioReactionID:14)
    ''' </summary>
    SabioReactionID
    ''' <summary>
    ''' AnyRole - Compound found in any role in a reaction (eg AnyRole:"ATP")
    ''' </summary>
    AnyRole
    Substrate
    Product
    Inhibitor
    Catalyst
    Cofactor
    Activator
    OtherModifier
    PubChemID
    KeggCompoundID
    KeggID
    ChebiID
    SabioCompoundID
    Enzymename
    ECNumber
    UniProtKB_AC
    UniProtID
    Tissue
    Organism
    CellularLocation
    Parametertype
    KineticMechanismType
    AssociatedSpecies
    Title
    Author
    Year
    PubMedID
    PubMedCID
    Organization
    DataIdentifier
    ExperimentID
    pHValueRange
    TemperatureRange
    DateSubmitted
    <Description("GO-Term")> GOTerm
    <Description("SBO-Term")> SBOTerm
    InChI
    Smiles
    <Description("Signalling Modification")> SignallingModification
    <Description("Signalling Event")> SignallingEvent
    IsRecombinant
    HasKineticData
    EnzymeType
    KeggPathwayID
    RheaReactionID
    ReactomeReactionID
    MetaCycReactionID
    MetaNetXReactionID
    UniprotOmimID
End Enum
