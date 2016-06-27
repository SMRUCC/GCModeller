Imports LANS.SystemsBiology.Assembly.SBML.Components

Namespace Specifics.MetaCyc

    Public Class SpeciesPropReader : Inherits ReaderBase(Of SpeciesProperties)

        Sub New(note As Notes)
            Call MyBase.New(note.Properties, PropertyParser.SpeciesKeyMaps)

            Me.BIOCYC = __getValue(SpeciesProperties.BIOCYC)
            Me.CAS = __getValue(SpeciesProperties.CAS)
            Me.CHARGE = __getValue(SpeciesProperties.CHARGE)
            Me.CHEBI = __getValue(SpeciesProperties.CHEBI)
            Me.CHEMSPIDER = __getValue(SpeciesProperties.CHEMSPIDER)
            Me.DRUGBANK = __getValue(SpeciesProperties.DRUGBANK)
            Me.FORMULA = __getValue(SpeciesProperties.FORMULA)
            Me.HMDB = __getValue(SpeciesProperties.HMDB)
            Me.INCHI = __getValue(SpeciesProperties.INCHI)
            Me.KEGG = __getValue(SpeciesProperties.KEGG)
            Me.METABOLIGHTS = __getValue(SpeciesProperties.METABOLIGHTS)
            Me.PUBCHEM = __getValue(SpeciesProperties.PUBCHEM)
        End Sub

        Public ReadOnly Property BIOCYC As String
        Public ReadOnly Property INCHI As String
        Public ReadOnly Property CHEBI As String
        Public ReadOnly Property HMDB As String
        Public ReadOnly Property CHEMSPIDER As String
        Public ReadOnly Property PUBCHEM As String
        Public ReadOnly Property DRUGBANK As String
        Public ReadOnly Property CAS As String
        Public ReadOnly Property METABOLIGHTS As String
        Public ReadOnly Property KEGG As String
        Public ReadOnly Property FORMULA As String
        Public ReadOnly Property CHARGE As String
    End Class
End Namespace