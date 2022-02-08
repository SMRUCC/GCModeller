
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.Schema.Metabolism

<Xref("enzrxns.dat")>
Public Class enzrxns : Inherits Model

    <AttributeField("ALTERNATIVE-SUBSTRATES")>
    Public Property alternativeSubstrates As String()
    <AttributeField("ENZYME")>
    Public Property enzyme As String
    <AttributeField("REACTION")>
    Public Property reaction As String
    <AttributeField("REACTION-DIRECTION")>
    Public Property reactionDirection As ReactionDirections
    <AttributeField("COFACTORS")>
    Public Property cofactors As String()
    <AttributeField("KM")>
    Public Property Km As KineticsFactor()
    <AttributeField("KCAT")>
    Public Property Kcat As KineticsFactor()
    <AttributeField("PH-OPT")>
    Public Property PH As Double
    <AttributeField("TEMPERATURE-OPT")>
    Public Property temperature As Double
    <AttributeField("SPECIFIC-ACTIVITY")>
    Public Property specificActivity As Double
    <AttributeField("REGULATED-BY")>
    Public Property regulatedBy As String()
    <AttributeField("ENZRXN-EC-NUMBER")>
    Public Property EC_number As ECNumber
    <AttributeField("VMAX")>
    Public Property Vmax As Double

End Class

Public Class KineticsFactor

    Public Property Km As Double
    Public Property substrate As String
    Public Property citations As String()

End Class