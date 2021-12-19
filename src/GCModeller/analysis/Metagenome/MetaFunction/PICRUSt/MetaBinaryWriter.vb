Imports System.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Values
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.Metagenome.greengenes
Imports SMRUCC.genomics.Metagenomics

Namespace PICRUSt

    Public Class MetaBinaryWriter : Implements IDisposable

        ''' <summary>
        ''' greengenes id -> bytes offset
        ''' </summary>
        Dim ggIdIndex As New Dictionary(Of String, Long)
        ''' <summary>
        ''' biom taxonomy string -> bytes offset
        ''' </summary>
        Dim taxIndex As New Dictionary(Of String, Long)
        Dim file As Stream
        Dim ggTax As Dictionary(Of String, Taxonomy)
        Private disposedValue As Boolean

        Sub New(file As Stream, ggTax As Dictionary(Of String, Taxonomy))
            Me.file = file
            Me.ggTax = ggTax
        End Sub

        Public Sub ImportsComputes(ko_13_5_precalculated As Stream)
            Using reader As New StreamReader(ko_13_5_precalculated)
                Dim koId As String() = reader.ReadLine.Split(ASCII.TAB)
                Dim line As Value(Of String) = ""
                Dim tokens As String()
                Dim ggId As String
                Dim data As Double()

                Do While Not (line = reader.ReadLine).StringEmpty
                    tokens = line.Split(ASCII.TAB)
                    ggId = tokens(Scan0)
                    data = tokens.Skip(1).Select(Function(d) Double.Parse(d)).ToArray

                Loop
            End Using
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="gg">
        ''' data parsed from the greengenes database via 
        ''' <see cref="otu_taxonomy.Load(String)"/>
        ''' </param>
        ''' <param name="save"></param>
        ''' <returns></returns>
        Public Shared Function CreateWriter(gg As IEnumerable(Of otu_taxonomy), save As Stream) As MetaBinaryWriter
            Dim tax As New Dictionary(Of String, Taxonomy)

            For Each lineage As otu_taxonomy In gg
                tax.Add(lineage.ID, lineage.Taxonomy)
            Next

            Return New MetaBinaryWriter(save, tax)
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call file.Flush()
                    Call file.Close()
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