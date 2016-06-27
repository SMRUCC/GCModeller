Namespace Assembly.MetaCyc.Schema.Reflection

    Public Class TableSchema
        Public Property Table As MetaCyc.File.DataFiles.Slots.Object.Tables
        Public Property LinkInKeys As Reflection.ExternalKey()
        Public Property LinkOutKeys As Reflection.ExternalKey()
        Public Property UnknownKeys As Reflection.ExternalKey()

        Sub New(Type As System.Type, Table As MetaCyc.File.DataFiles.Slots.Object.Tables)
            Dim PropertyCollection As System.Reflection.PropertyInfo() = (From [Property] As System.Reflection.PropertyInfo
                                                                              In Type.GetProperties
                                                                          Let attributes As Object() = [Property].GetCustomAttributes(attributeType:=TableSchema.ExternalLinkKey, inherit:=True)
                                                                          Where Not (attributes Is Nothing OrElse attributes.Count = 0)
                                                                          Select [Property]).ToArray
            Me.LinkOutKeys = GetExternalLink(PropertyCollection, ExternalKey.Directions.Out)
            Me.UnknownKeys = GetExternalLink(PropertyCollection, ExternalKey.Directions.Unknown)
            Me.LinkInKeys = GetExternalLink(PropertyCollection, ExternalKey.Directions.In)
            Me.Table = Table
        End Sub

        Protected Friend Shared ReadOnly ExternalLinkKey As System.Type = GetType(MetaCyc.Schema.Reflection.ExternalKey)

        Private Shared Function GetExternalLink(PropertyCollection As System.Reflection.PropertyInfo(), Direction As Reflection.ExternalKey.Directions) As Reflection.ExternalKey()
            Dim LQuery As Generic.IEnumerable(Of MetaCyc.Schema.Reflection.ExternalKey) =
                    From [Property] As System.Reflection.PropertyInfo
                    In PropertyCollection
                    Let Link As MetaCyc.Schema.Reflection.ExternalKey = DirectCast([Property].GetCustomAttributes(attributeType:=TableSchema.ExternalLinkKey, inherit:=True).First, MetaCyc.Schema.Reflection.ExternalKey)
                    Where Link.Direction = Direction
                    Select Link.Link([Property]) '
            Return LQuery.ToArray
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}, {1} external links", Table.ToString, LinkInKeys.Count + LinkOutKeys.Count + UnknownKeys.Count)
        End Function
    End Class

    ''' <summary>
    ''' ObjA ----> ObjB
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Path
        Public Property ObjA As String
        Public Property ObjB As String
        Public Property Relationship As String

        Public Overrides Function ToString() As String
            Return String.Format("{0} --{1}--> {2}", ObjA, ObjB, Relationship)
        End Function
    End Class
End Namespace