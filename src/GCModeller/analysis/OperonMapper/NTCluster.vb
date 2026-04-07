#Region "Microsoft.VisualBasic::0b2e7f644fb65b537d952d2adb75aae8, analysis\OperonMapper\NTCluster.vb"

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

    '   Total Lines: 108
    '    Code Lines: 80 (74.07%)
    ' Comment Lines: 10 (9.26%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 18 (16.67%)
    '     File Size: 4.16 KB


    ' Class NTCluster
    ' 
    '     Properties: biom_string, cluster, fingerprint, gb_acc, left
    '                 locus_tag, right, strand
    ' 
    '     Function: MakeFingerprint, ToString
    ' 
    ' Class FingerprintMatrixWriter
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: BSONReader
    ' 
    '     Sub: Add, (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure.Kmer
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class NTCluster

    Public Property gb_acc As String
    Public Property locus_tag As String
    Public Property left As Integer
    Public Property right As Integer
    Public Property strand As String
    Public Property biom_string As String
    Public Property cluster As String
    Public Property fingerprint As Double()

    Public Overrides Function ToString() As String
        Return $"{gb_acc}.{locus_tag} {biom_string}"
    End Function

    Public Shared Iterator Function MakeFingerprint(nt As IEnumerable(Of FastaSeq),
                                                    Optional size As Integer = 4096,
                                                    Optional radius As Integer = 3) As IEnumerable(Of NTCluster)
        Dim builder As New MorganFingerprint(size)
        Dim forwardReverse As Index(Of String) = {"forward", "reverse"}

        For Each seq As FastaSeq In nt.SafeQuery
            Dim header As NamedValue(Of String) = seq.Title.GetTagValue("|")
            Dim metadata As String() = header.Name _
                .StringSplit("[\s~.#]") _
                .Where(Function(str) str <> "") _
                .ToArray

            If metadata.Length <> 5 OrElse Not (metadata(4) Like forwardReverse) Then
                Call $"invalid header format: {seq.Title}".warning
                Continue For
            End If

            Dim kmer As KMerGraph = KMerGraph.FromSequence(seq)
            Dim checksum As Byte() = builder.CalculateFingerprintCheckSum(kmer, radius)

            Yield New NTCluster With {
                .biom_string = header.Value,
                .fingerprint = checksum _
                    .Select(Function(b) CDbl(b)) _
                    .ToArray,
                .gb_acc = metadata(0),
                .locus_tag = metadata(1),
                .left = metadata(2),
                .right = metadata(3),
                .strand = metadata(4)
            }
        Next
    End Function

End Class

Public Class FingerprintMatrixWriter : Implements IDisposable

    Dim disposedValue As Boolean
    Dim s As Stream

    Sub New(s As Stream)
        Me.s = s
    End Sub

    Public Sub Add(fingerprint As NTCluster)
        Dim buffer As Byte() = BSONFormat.SafeGetBuffer(fingerprint.CreateJSONElement).ToArray
        Call s.Write(buffer, Scan0, buffer.Length)
    End Sub

    Public Shared Function BSONReader(s As Stream) As IEnumerable(Of NTCluster)
        Return BSONFormat.LoadList(s, tqdm:=False).Select(Function(json) json.CreateObject(Of NTCluster))
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call s.Flush()
                Call s.Close()
                Call s.Dispose()
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
