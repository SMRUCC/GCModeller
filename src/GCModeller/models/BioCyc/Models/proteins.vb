Imports System.IO
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

<Xref("proteins.dat")>
Public Class proteins : Inherits Model

    <AttributeField("DBLINKS")>
    Public Property db_xrefs As String()
    <AttributeField("GENE")>
    Public Property gene As String
    <AttributeField("LOCATIONS")>
    Public Property locations As String()

    Public ReadOnly Property db_links As DBLink()
        Get
            Return GetDbLinks(db_xrefs).ToArray
        End Get
    End Property

    Public Property protseq As String

    Public Shared Function OpenFile(fullName As String) As AttrDataCollection(Of proteins)
        Using file As Stream = fullName.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Return AttrDataCollection(Of proteins).LoadFile(file)
        End Using
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function OpenFile(file As Stream) As AttrDataCollection(Of proteins)
        Return AttrDataCollection(Of proteins).LoadFile(file)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function ParseText(data As String) As AttrDataCollection(Of proteins)
        Return AttrDataCollection(Of proteins).LoadFile(New StringReader(data))
    End Function
End Class
