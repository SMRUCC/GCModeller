Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Reflection

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    Public Class Terminator : Inherits MetaCyc.File.DataFiles.Slots.Object
        <MetaCycField()> Public Property ComponentsOf As String
        <MetaCycField()> Public Property LeftEndPosition As String
        <MetaCycField()> Public Property RightEndPosition As String

        Public Property DNASeq As String

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.terminators
            End Get
        End Property

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As Terminator
        '    Dim NewObj As Terminator = New Terminator

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of MetaCyc.File.DataFiles.Slots.Terminator) _
        '        (MetaCyc.File.AttributeValue.Object.Format(Terminators.AttributeList, e), NewObj)

        '    If NewObj.Object.ContainsKey("COMPONENT-OF") Then NewObj.ComponentsOf = NewObj.Object("COMPONENT-OF") Else NewObj.ComponentsOf = String.Empty
        '    If NewObj.Object.ContainsKey("LEFT-END-POSITION") Then NewObj.ComponentsOf = NewObj.Object("LEFT-END-POSITION") Else NewObj.ComponentsOf = String.Empty
        '    If NewObj.Object.ContainsKey("RIGHT-END-POSITION") Then NewObj.ComponentsOf = NewObj.Object("RIGHT-END-POSITION") Else NewObj.ComponentsOf = String.Empty

        '    Return NewObj
        'End Operator
    End Class
End Namespace