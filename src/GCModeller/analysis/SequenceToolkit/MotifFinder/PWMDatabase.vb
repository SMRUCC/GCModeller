Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports MotifSet = SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.PWMDatabase

Public Class PWMDatabase : Inherits MotifSet

    Public Sub New(s As Stream, Optional is_readonly As Boolean = False)
        MyBase.New(New StreamPack(s, [readonly]:=is_readonly))
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overloads Shared Function LoadMotifs(s As Stream) As Dictionary(Of String, Probability())
        Return LoadMotifs(New StreamPack(s, [readonly]:=True))
    End Function
End Class
