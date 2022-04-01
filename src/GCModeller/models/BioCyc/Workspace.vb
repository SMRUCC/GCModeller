Imports System.IO
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Public Class Workspace

    ReadOnly dir As String

    Dim m_reactions As Lazy(Of AttrDataCollection(Of reactions))
    Dim m_pathways As Lazy(Of AttrDataCollection(Of pathways))
    Dim m_enzrxns As Lazy(Of AttrDataCollection(Of enzrxns))
    Dim m_compounds As Lazy(Of AttrDataCollection(Of compounds))

    Public ReadOnly Property compounds As AttrDataCollection(Of compounds)
        Get
            Return m_compounds.Value
        End Get
    End Property

    Public ReadOnly Property reactions As AttrDataCollection(Of reactions)
        Get
            Return m_reactions.Value
        End Get
    End Property

    Public ReadOnly Property pathways As AttrDataCollection(Of pathways)
        Get
            Return m_pathways.Value
        End Get
    End Property

    Public ReadOnly Property enzrxns As AttrDataCollection(Of enzrxns)
        Get
            Return m_enzrxns.Value
        End Get
    End Property

    Sub New(dir As String)
        Me.dir = dir.GetDirectoryFullPath

        If {"reports", "data", "input", "kb", "rawdata"}.All(Function(d) $"{dir}/{d}".DirectoryExists) Then
            Me.dir = $"{Me.dir}/data/"
        End If

        m_enzrxns = New Lazy(Of AttrDataCollection(Of enzrxns))(Function() openFile(Of enzrxns)())
        m_reactions = New Lazy(Of AttrDataCollection(Of reactions))(Function() openFile(Of reactions)())
        m_pathways = New Lazy(Of AttrDataCollection(Of pathways))(Function() openFile(Of pathways)())
        m_compounds = New Lazy(Of AttrDataCollection(Of compounds))(Function() openFile(Of compounds)())
    End Sub

    Private Function openFile(Of T As Model)() As AttrDataCollection(Of T)
        Dim fileName As String = getFileName(Of T)()
        Dim fullName As String = $"{dir}/{fileName}".GetFullPath

        Call Console.WriteLine($"[biocyc_open] {fullName}")

        Using file As Stream = fullName.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=True)
            Return AttrDataCollection(Of T).LoadFile(file)
        End Using
    End Function

    Private Shared Function getFileName(Of T As Model)() As String
        Dim attrs As Object() = GetType(T).GetCustomAttributes(inherit:=True).ToArray
        Dim ref = From attr As Object In attrs Where TypeOf attr Is XrefAttribute
        Dim fileName As XrefAttribute = ref.FirstOrDefault

        If fileName Is Nothing Then
            Throw New MissingFieldException
        Else
            Return fileName.Name
        End If
    End Function

    Public Overrides Function ToString() As String
        Return dir
    End Function

End Class
