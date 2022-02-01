
''' <summary>
''' Pathways, including relationships among reactions
''' </summary>
Public Class pathways : Inherits Model

    <AttributeField("IN-PATHWAY")>
    Public Property inPathway As String()
    <AttributeField("SUPER-PATHWAYS")>
    Public Property superPathways As String()
    <AttributeField("SUB-PATHWAYS")>
    Public Property subPathways As String()

    <AttributeField("PATHWAY-LINKS")>
    Public Property pathwayLinks As String()

    <AttributeField("PREDECESSORS")>
    Public Property predecessors As String()

    <AttributeField("PRIMARIES")>
    Public Property primaries As String()

    <AttributeField("REACTION-LAYOUT")>
    Public Property reactionLayout As String()

    <AttributeField("REACTION-LIST")>
    Public Property reactionList As String()

    <AttributeField("ENZYMES-NOT-USED")>
    Public Property enzymesNotUsed As String()

    Public Overrides Function ToString() As String
        Return If(commonName, uniqueId)
    End Function

End Class
