Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Assembly.NCBI.Taxonomy

    ''' <summary>
    ''' database for <see cref="Accession2Taxid"/>
    ''' </summary>
    Public Class AccessionTaxID : Implements IDisposable

        ReadOnly taxdb As InMemoryDb
        Private disposedValue As Boolean

        Sub New(taxdb As InMemoryDb)
            Me.taxdb = taxdb
        End Sub

        Public Function GetNcbiTaxID(accession_id As String) As String
            If taxdb.HasKey(accession_id) Then
                Return Encoding.ASCII.GetString(taxdb.Get(accession_id))
            End If

            Dim trim As NamedValue(Of String) = accession_id.GetTagValue("."c, failureNoName:=False)

            ' accesss not found
            If trim.Value = "" Then
                Return Nothing
            ElseIf taxdb.HasKey(trim.Name) Then
                Return Encoding.ASCII.GetString(taxdb.Get(trim.Name))
            Else
                Return Nothing
            End If
        End Function

        Public Shared Sub ImportsToCacheDb(accession2Taxid As IEnumerable(Of Accession2Taxid), taxdb As InMemoryDb)
            For Each map As Accession2Taxid In accession2Taxid
                Dim ncbi_taxid As String = CStr(map.taxid)
                Dim taxid_data As Byte() = Encoding.ASCII.GetBytes(ncbi_taxid)

                Call taxdb.Put(map.accession, taxid_data)
                Call taxdb.Put(map.accession_version, taxid_data)
            Next
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call taxdb.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace