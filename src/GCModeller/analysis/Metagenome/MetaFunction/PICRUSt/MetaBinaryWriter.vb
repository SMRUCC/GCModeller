#Region "Microsoft.VisualBasic::9451546cac058bf3df0642c5ebfefa53, GCModeller\analysis\Metagenome\MetaFunction\PICRUSt\MetaBinaryWriter.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 228
    '    Code Lines: 157
    ' Comment Lines: 26
    '   Blank Lines: 45
    '     File Size: 8.43 KB


    '     Class MetaBinaryWriter
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: CreateWriter
    ' 
    '         Sub: (+2 Overloads) Dispose, ImportsComputes, RunFileImports, SaveTree, SaveTreeIndex
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
            Call file.Write(tree.taxonomyRank) ' integer
            Call file.Write(tree.size)

            For Each id As String In tree.ggId
                Call file.Write(id, BinaryStringFormat.ZeroTerminated)
                Call file.Write(tree.Data(id))
            Next

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
            Dim prog As Double = App.ElapsedMilliseconds
            Dim size As Long = reader.BaseStream.Length
            Dim reportDelta As Integer = size / 25
            Dim pos As Long
            Dim dt As Double
            Dim dsize As Double

            Call Console.WriteLine($"start to process matrix with {koId.Length} KO features and data size {StringFormats.Lanudry(size)}")

            ' save ko id vector data
            Call file.Write(koId.Length)

            For Each id As String In koId
                Call file.Write(id, BinaryStringFormat.ZeroTerminated)
            Next

            Call file.Flush()
            ' placeholder for the tree index offset
            Call file.Write(0L)

            treeOffset = file.Position - 8

            Do While Not (line = reader.ReadLine) Is Nothing
                If line.Value.StringEmpty Then
                    Continue Do
                End If

                tokens = line.Split(ASCII.TAB)
                ggId = tokens(Scan0)

                If Not ggTax.ContainsKey(ggId) Then
                    Call Console.WriteLine($"skip missing id: {ggId}...")
                    Continue Do
                End If

                taxonomy = ggTax(ggId)
                data = tokens _
                    .Skip(1) _
                    .Select(Function(d) Single.Parse(d)) _
                    .ToArray

                If data.Length <> koId.Length Then
                    Call Console.WriteLine($"found invalid line: {line.Value.Substring(0, 32)}...")
                    Continue Do
                End If

                tokens = taxonomy.ToArray
                offset = file.Position
                target = offsetIndex

                Call file.Write(data)

                For j As Integer = 0 To tokens.Length - 1
                    name = tokens(j)

                    If Not target.hasNode(name) Then
                        Call New ko_13_5_precalculated With {
                            .Childs = New Dictionary(Of String, Tree(Of Dictionary(Of String, Long))),
                            .label = name,
                            .ID = ++i,
                            .Parent = target,
                            .taxonomyRank = j + 100,
                            .Data = New Dictionary(Of String, Long)
                        }.DoCall(AddressOf target.Add)
                    End If

                    target = target(name)
                Next

                target.Add(ggId, offset)

                ' debug test
                If (reader.BaseStream.Position - pos) > reportDelta Then
                    dsize = reader.BaseStream.Position - pos
                    pos = reader.BaseStream.Position
                    dt = App.ElapsedMilliseconds - prog
                    prog = App.ElapsedMilliseconds

                    Console.WriteLine($"[{(pos / size * 100).ToString("F0")}%] {StringFormats.Lanudry(pos)}/{StringFormats.Lanudry(size)} ~ {StringFormats.Lanudry(dsize / (dt / 1000))}/s")
                End If
            Loop

            Call file.Flush()
            Call Console.WriteLine("~done!")
        End Sub

        Public Sub ImportsComputes(ko_13_5_precalculated As Stream)
            Call file.Seek(Scan0, SeekOrigin.Begin)
            Call file.Write(Magic, BinaryStringFormat.ZeroTerminated)

            offsetIndex = New ko_13_5_precalculated With {
                .label = "/",
                .Childs = New Dictionary(Of String, Tree(Of Dictionary(Of String, Long))),
                .ID = 0,
                .Data = New Dictionary(Of String, Long)
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
