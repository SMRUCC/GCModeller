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
