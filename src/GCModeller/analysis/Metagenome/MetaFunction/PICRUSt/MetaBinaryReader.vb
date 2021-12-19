Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.IO
Imports SMRUCC.genomics.Metagenomics

Namespace PICRUSt

    Public Class MetaBinaryReader : Implements IDisposable

        ReadOnly buffer As BinaryDataReader
        ReadOnly index As New Dictionary(Of String, ko_13_5_precalculated)
        ReadOnly tree As ko_13_5_precalculated

        Dim ko As String()
        Dim disposedValue As Boolean

        Sub New(file As Stream)
            buffer = New BinaryDataReader(file) With {
                .ByteOrder = ByteOrder.BigEndian,
                .Encoding = Encoding.ASCII
            }

            ' verify magic
            If buffer.ReadString(BinaryStringFormat.ZeroTerminated) <> MetaBinaryWriter.Magic Then
                Throw New InvalidDataException("invalid magic header string!")
            Else
                Dim len As Integer = buffer.ReadInt32
                Dim id As New List(Of String)

                For i As Integer = 1 To len
                    Call id.Add(buffer.ReadString(BinaryStringFormat.ZeroTerminated))
                Next

                ko = id.ToArray
            End If

            Call buffer.Seek(buffer.ReadInt64, SeekOrigin.Begin)
            Call loadIndex(root:=tree)
        End Sub

        ''' <summary>
        ''' get taxonomy information via greengenes OTU id
        ''' </summary>
        ''' <param name="OTU_id"></param>
        ''' <returns></returns>
        Public Function GetTaxonomy(OTU_id As String) As Taxonomy
            If Not index.ContainsKey(OTU_id) Then
                Return Nothing
            Else
                Dim node As ko_13_5_precalculated = index(OTU_id)
                Dim lineage As String = node.QualifyName
                Dim tax As Taxonomy = BIOMTaxonomyParser.Parse(lineage)

                Return tax
            End If
        End Function

        Public Function findRawByTaxonomy(taxonomy As Taxonomy) As Single()
            Dim nodes As String() = taxonomy.ToArray
            Dim target As ko_13_5_precalculated = tree.FindNode(nodes)

            If target Is Nothing Then
                Return Nothing
            End If

            Dim offset As Long = target.Data

            If offset = 0 Then
                Return Nothing
            Else
                Dim v As Single()

                buffer.Seek(offset, SeekOrigin.Begin)
                v = buffer.ReadSingles(ko.Length)

                Return v
            End If
        End Function

        Public Function findByTaxonomy(taxonomy As Taxonomy) As Dictionary(Of String, Double)
            Dim v As Single() = findRawByTaxonomy(taxonomy)

            If v Is Nothing Then
                Return Nothing
            End If

            Dim data As New Dictionary(Of String, Double)

            For i As Integer = 0 To v.Length - 1
                Call data.Add(ko(i), v(i))
            Next

            Return data
        End Function

        Public Function getRawByOTUId(id As String) As Single()
            If Not index.ContainsKey(id) Then
                Return Nothing
            Else
                Dim offset As Long = index(id).Data
                Dim v As Single()

                If offset = 0 Then
                    Return Nothing
                End If

                buffer.Seek(offset, SeekOrigin.Begin)
                v = buffer.ReadSingles(ko.Length)

                Return v
            End If
        End Function

        Public Function getByOTUId(id As String) As Dictionary(Of String, Double)
            Dim v As Single() = getRawByOTUId(id)

            If v Is Nothing Then
                Return Nothing
            End If

            Dim data As New Dictionary(Of String, Double)

            For i As Integer = 0 To v.Length - 1
                Call data.Add(ko(i), v(i))
            Next

            Return data
        End Function

        Private Sub loadIndex(ByRef root As ko_13_5_precalculated)
            root = loadIndexTree()
        End Sub

        Private Function loadIndexTree() As ko_13_5_precalculated
            Dim node As New ko_13_5_precalculated With {
                .Childs = New Dictionary(Of String, Tree(Of Long)),
                .ID = buffer.ReadInt32,
                .label = buffer.ReadString(BinaryStringFormat.ZeroTerminated),
                .taxonomy = buffer.ReadInt32,
                .ggId = buffer.ReadString(BinaryStringFormat.ZeroTerminated),
                .Data = buffer.ReadInt64
            }
            Dim size As Integer = buffer.ReadInt32

            If node.ggId <> "#" Then
                Call index.Add(node.ggId, node)
            End If

            For i As Integer = 1 To size
                Call node.Add(loadIndexTree())
            Next

            Return node
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    Erase ko

                    ' TODO: dispose managed state (managed objects)
                    Call buffer.Dispose()
                    Call index.Clear()
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