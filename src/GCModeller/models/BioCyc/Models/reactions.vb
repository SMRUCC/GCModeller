Public Class reactions : Inherits Model

    <AttributeField("EC-NUMBER")>
    Public Property ec_number As String
    <AttributeField("ENZYMATIC-REACTION")>
    Public Property enzymaticReaction As String()
    <AttributeField("GIBBS-0")>
    Public Property gibbs0 As Double
    <AttributeField("IN-PATHWAY")>
    Public Property inPathway As String()
    <AttributeField("LEFT")>
    Public Property left As String()
    <AttributeField("RIGHT")>
    Public Property right As String()
    <AttributeField("PHYSIOLOGICALLY-RELEVANT?")>
    Public Property physiologicallyRelevant As Boolean
    <AttributeField("REACTION-BALANCE-STATUS")>
    Public Property reactionBalanceStatus As String
    <AttributeField("REACTION-DIRECTION")>
    Public Property reactionDirection As String
    <AttributeField("REACTION-LIST")>
    Public Property reactionList As String()
    <AttributeField("RXN-LOCATIONS")>
    Public Property reactionLocations As String()
    <AttributeField("SPONTANEOUS?")>
    Public Property spontaneous As Boolean
    <AttributeField("ATOM-MAPPINGS")>
    Public Property atomMappings As String()
    <AttributeField("ORPHAN?")>
    Public Property orphan As String
    <AttributeField("SYSTEMATIC-NAME")>
    Public Property systematicName As String
    <AttributeField("CANNOT-BALANCE?")>
    Public Property cannotBalance As Boolean
    <AttributeField("SIGNAL")>
    Public Property signal As String
    <AttributeField("SPECIES")>
    Public Property species As String

End Class
