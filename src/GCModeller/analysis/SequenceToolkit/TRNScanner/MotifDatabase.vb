Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns

Public Class MotifDatabase : Inherits PWMDatabase

    Public Sub New(s As Stream, Optional is_readonly As Boolean = False)
        MyBase.New(New StreamPack(s, [readonly]:=is_readonly))
    End Sub

    Public Shared Function OpenReadOnly(s As Stream) As MotifDatabase
        Return New MotifDatabase(s, is_readonly:=True)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overloads Shared Function LoadMotifs(s As Stream) As Dictionary(Of String, Probability())
        Return LoadMotifs(New StreamPack(s, [readonly]:=True))
    End Function
End Class
