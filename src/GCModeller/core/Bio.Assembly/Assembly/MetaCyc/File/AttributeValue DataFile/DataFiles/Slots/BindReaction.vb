Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Reflection
Imports LANS.SystemsBiology.Assembly.MetaCyc.Schema.Reflection
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    ''' <summary>
    ''' Binding reaction between proteins and DNA binding sites such as promoters
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BindReaction : Inherits MetaCyc.File.DataFiles.Slots.Object

        <ExternalKey("compounds,proteins,protligandcplxes,dnabindsites", "active", ExternalKey.Directions.In)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property Activators As List(Of String)

        <ExternalKey("compounds,proteins,protligandcplxes,dnabindsites", "inhibit", ExternalKey.Directions.In)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property Inhibitors As List(Of String)

        <MetaCycField(name:="OFFICIAL-EC?", type:=MetaCycField.Types.String)> Public Property OfficialEC As String

        <ExternalKey("compounds,proteins,protligandcplxes,dnabindsites,promoters", "have reactant", ExternalKey.Directions.Out)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property Reactants As List(Of String)

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.bindrxns
            End Get
        End Property

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As BindReaction
        '    Dim NewObj As BindReaction = New BindReaction

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of BindReaction) _
        '        (MetaCyc.File.AttributeValue.Object.Format(MetaCyc.File.DataFiles.BindRxns.AttributeList, e), NewObj)

        '    NewObj.Activators = StringQuery(NewObj.Object, "ACTIVATORS( \d+)?")
        '    NewObj.Inhibitors = StringQuery(NewObj.Object, "INHIBITORS( \d+)?")
        '    NewObj.Reactants = StringQuery(NewObj.Object, "REACTANTS( \d+)?")
        '    If NewObj.Object.ContainsKey("OFFICIAL-EC?") Then NewObj.OfficialEC = NewObj.Object("OFFICIAL-EC?") Else NewObj.OfficialEC = String.Empty

        '    Return NewObj
        'End Operator
    End Class
End Namespace