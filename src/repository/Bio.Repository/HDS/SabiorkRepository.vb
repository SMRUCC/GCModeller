Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports SMRUCC.genomics.Data.SABIORK
Imports sbXML = SMRUCC.genomics.Model.SBML.Level3.XmlFile(Of SMRUCC.genomics.Data.SABIORK.SBML.SBMLReaction)

Public Class SabiorkRepository : Implements IDisposable

    ReadOnly cache As StreamPack
    ReadOnly webRequest As ModelQuery

    Private disposedValue As Boolean

    Sub New(file As Stream)
        Me.cache = New StreamPack(file, meta_size:=16 * 1024 * 1024, [readonly]:=False)
        Me.webRequest = New ModelQuery(cache)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ec_number"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' url = `https://sabiork.h-its.org/sabioRestWebServices/searchKineticLaws/sbml?q=ecnumber:${num}`;
    ''' </remarks>
    Public Function GetByECNumber(ec_number As String) As sbXML
        Dim q As New Dictionary(Of QueryFields, String) From {
            {QueryFields.ECNumber, ec_number}
        }
        Dim result = webRequest.Query(Of sbXML)(q)

        Return result
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call cache.Dispose()
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
