
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

<Xref("compounds.dat")>
Public Class compounds : Inherits Model

    <AttributeField("ATOM-CHARGES")>
    Public Property atomCharges As String()
    <AttributeField("DBLINKS")>
    Public Property dbLinks As String()
    <AttributeField("NON-STANDARD-INCHI")>
    Public Property nonStandardInChI As String()
    <AttributeField("SMILES")>
    Public Property SMILES As String
    <AttributeField("CHEMICAL-FORMULA")>
    Public Property chemicalFormula As String()
    <AttributeField("GIBBS-0")>
    Public Property Gibbs0 As Double
    <AttributeField("INCHI")>
    Public Property InChI As String
    <AttributeField("INCHI-KEY")>
    Public Property InChIKey As String
    <AttributeField("MOLECULAR-WEIGHT")>
    Public Property molecularWeight As Double
    <AttributeField("MONOISOTOPIC-MW")>
    Public Property exactMass As Double
    <AttributeField("COMPONENT-OF")>
    Public Property componentOf As String()

End Class
