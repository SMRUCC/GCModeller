
Imports System.IO
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.SequenceModel.FASTA

<Xref("genes.dat")>
Public Class genes : Inherits Model

    <AttributeField("ACCESSION-1")>
    Public Property accession1 As String
    <AttributeField("ACCESSION-2")>
    Public Property accession2 As String
    <AttributeField("DBLINKS")>
    Public Property db_xrefs As String()
    <AttributeField("PRODUCT")>
    Public Property product As String

    Public ReadOnly Property db_links As DBLink()
        Get
            Return GetDbLinks(db_xrefs).ToArray
        End Get
    End Property

    Public Property dnaseq As String

    Public Shared Function OpenFile(fullName As String) As AttrDataCollection(Of genes)
        Using file As Stream = fullName.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Return AttrDataCollection(Of genes).LoadFile(file)
        End Using
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function OpenFile(file As Stream) As AttrDataCollection(Of genes)
        Return AttrDataCollection(Of genes).LoadFile(file)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function ParseText(data As String) As AttrDataCollection(Of genes)
        Return AttrDataCollection(Of genes).LoadFile(New StringReader(data))
    End Function
End Class
