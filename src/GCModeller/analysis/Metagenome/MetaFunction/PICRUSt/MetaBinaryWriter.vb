Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Values
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.Metagenome.greengenes
Imports SMRUCC.genomics.Metagenomics

Namespace PICRUSt

    Public Class MetaBinaryWriter : Implements IDisposable

        Friend Const Magic As String = "PICRUSt"

        ''' <summary>
        ''' 1. greengenes id -> bytes offset
        ''' 2. biom taxonomy string -> bytes offset
        ''' </summary>
        Dim offsetIndex As ko_13_5_precalculated

        ReadOnly file As BinaryDataWriter
        ReadOnly ggTax As Dictionary(Of String, Taxonomy)

        Private disposedValue As Boolean

        Sub New(file As Stream, ggTax As Dictionary(Of String, Taxonomy))
            Me.ggTax = ggTax
            Me.file = New BinaryDataWriter(file) With {
                .ByteOrder = ByteOrder.BigEndian,
                .Encoding = Encodings.UTF8WithoutBOM.CodePage
            }
        End Sub

        Dim treeOffset As Long

        Private Sub SaveTreeIndex()
            Dim start As Long = file.Position

            Using tempSeek = file.TemporarySeek(treeOffset, SeekOrigin.Begin)
                Call file.Write(start)
                Call file.Flush()
            End Using

            Call SaveTree(offsetIndex)
        End Sub

        Private Sub SaveTree(tree As ko_13_5_precalculated)
            Call file.Write(tree.ID)
            Call file.Write(tree.label, BinaryStringFormat.ZeroTerminated)
            Call file.Write(tree.taxonomy) ' integer
            Call file.Write(If(tree.ggId, "#"))
            Call file.Write(tree.Data)
            Call file.Write(tree.EnumerateChilds.Count)

            For Each node As ko_13_5_precalculated In tree.Childs.Values
                Call SaveTree(node)
            Next
        End Sub

        Private Sub RunFileImports(reader As StreamReader)
            Dim koId As String() = reader.ReadLine.Split(ASCII.TAB).Skip(1).ToArray
            Dim line As Value(Of String) = ""
            Dim tokens As String()
            Dim ggId As String
            Dim data As Single()
            Dim taxonomy As Taxonomy
            Dim target As ko_13_5_precalculated
            Dim i As i32 = 1
            Dim offset As Long
            Dim name As String

            ' save ko id vector data
            Call file.Write(koId.Length)

            For Each id As String In koId
                Call file.Write(id, BinaryStringFormat.ZeroTerminated)
            Next

            Call file.Flush()
            ' placeholder for the tree index offset
            Call file.Write(0L)

            treeOffset = file.Position - 8

            Do While Not (line = reader.ReadLine).StringEmpty
                tokens = line.Split(ASCII.TAB)
                ggId = tokens(Scan0)
                taxonomy = ggTax(ggId)
                data = tokens _
                    .Skip(1) _
                    .Select(Function(d) Single.Parse(d)) _
                    .ToArray
                tokens = taxonomy.ToArray
                offset = file.Position
                target = offsetIndex

                Call file.Write(data)

                For j As Integer = 0 To tokens.Length - 1
                    name = tokens(j)

                    If Not target.hasNode(name) Then
                        Call New ko_13_5_precalculated With {
                            .Childs = New Dictionary(Of String, Tree(Of Long)),
                            .label = name,
                            .ID = ++i,
                            .Parent = target,
                            .taxonomy = j + 100
                        }.DoCall(AddressOf target.Add)
                    End If

                    target = target(name)
                Next

                target.Data = offset
                target.ggId = ggId

                ' debug test
                If i > 10000 Then
                    Exit Do
                End If
            Loop

            Call file.Flush()
        End Sub

        Public Sub ImportsComputes(ko_13_5_precalculated As Stream)
            Call file.Seek(Scan0, SeekOrigin.Begin)
            Call file.Write(Magic, BinaryStringFormat.ZeroTerminated)

            offsetIndex = New ko_13_5_precalculated With {
                .label = "/",
                .Childs = New Dictionary(Of String, Tree(Of Long)),
                .ID = 0,
                .ggId = "#"
            }

            Using reader As New StreamReader(ko_13_5_precalculated)
                Call RunFileImports(reader)
                Call SaveTreeIndex()
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