
''' <summary>
''' Pathways, including relationships among reactions
''' </summary>
Public Class pathways : Inherits Model

    <AttributeField("TYPES")>
    Public Property types As String

    <AttributeField("COMMON-NAME")>
    Public Property commonName As String

    <AttributeField("CITATIONS")>
    Public Property citations As String()

    <AttributeField("COMMENT")>
    Public Property comment As String

    <AttributeField("CREDITS")>
    Public Property credits As String()

    <AttributeField("INSTANCE-NAME-TEMPLATE")>
    Public Property instanceNameTemplate As String

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

End Class
