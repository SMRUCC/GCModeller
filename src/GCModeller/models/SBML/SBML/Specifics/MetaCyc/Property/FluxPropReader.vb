Imports SMRUCC.genomics.Model.SBML.Components

Namespace Specifics.MetaCyc

    Public Class FluxPropReader : Inherits ReaderBase(Of FluxProperties)

        Sub New(note As Notes)
            Call MyBase.New(note.Properties, PropertyParser.FluxKeyMaps)

            Me.BIOCYC = __getValue(FluxProperties.BIOCYC)
            Me.ConfidenceLevel = Scripting.CastDouble(__getValue(FluxProperties.ConfidenceLevel))
            Me.ECNumber = GetEcList(__getValue(FluxProperties.ECNumber))
            Me.GENE_ASSOCIATION = GetGenes(__getValue(FluxProperties.GENE_ASSOCIATION))
            Me.SUBSYSTEM = __getValue(FluxProperties.SUBSYSTEM)
        End Sub

        Public ReadOnly Property BIOCYC As String
        Public ReadOnly Property ECNumber As String()
        Public ReadOnly Property SUBSYSTEM As String
        Public ReadOnly Property GENE_ASSOCIATION As String()
        Public ReadOnly Property ConfidenceLevel As Double
    End Class
End Namespace