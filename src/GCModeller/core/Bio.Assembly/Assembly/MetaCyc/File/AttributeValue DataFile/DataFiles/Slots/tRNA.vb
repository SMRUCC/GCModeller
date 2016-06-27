Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Reflection
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    Public Class tRNA : Inherits MetaCyc.File.DataFiles.Slots.Object

        <MetaCycField()> Public Property Anticodon As String
        <MetaCycField(type:=MetaCycField.Types.TStr)>
        Public Property Codons As List(Of String)

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.classes
            End Get
        End Property

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As tRNA
        '    Dim NewObj As tRNA = New tRNA

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of MetaCyc.File.DataFiles.Slots.tRNA) _
        '        (MetaCyc.File.AttributeValue.Object.Format(Genes.AttributeList, e), NewObj)

        '    NewObj.Anticodon = NewObj.Object("")
        '    NewObj.Codons = StringQuery(NewObj.Object, "")

        '    Return NewObj
        'End Operator
    End Class
End Namespace

