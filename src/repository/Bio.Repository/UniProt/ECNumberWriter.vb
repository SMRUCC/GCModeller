#Region "Microsoft.VisualBasic::245f400369b12a887d1ffa89aaf9b381, Bio.Repository\UniProt\ECNumberWriter.vb"

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

    '   Total Lines: 93
    '    Code Lines: 68
    ' Comment Lines: 10
    '   Blank Lines: 15
    '     File Size: 3.78 KB


    ' Class ECNumberWriter
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: getTags
    ' 
    '     Sub: AddProtein, (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.ComponentModel.Annotation

Public Class ECNumberWriter : Implements IDisposable

    ReadOnly stream As StreamPack
    ReadOnly rootNames As Dictionary(Of String, String) = Enums(Of EnzymeClasses) _
        .ToDictionary(Function(c) CInt(c).ToString,
                      Function(c)
                          Return CInt(c).ToString & "." & c.Description
                      End Function)
    ReadOnly locations As New Dictionary(Of String, String())

    Private disposedValue As Boolean

    Sub New(file As Stream)
        stream = New StreamPack(file, init_size:=4096, meta_size:=1024 * 1024 * 128)
    End Sub

    Public Sub AddProtein(protein As entry)
        Dim ECnumbers = protein.xrefs.TryGetValue("EC")

        If Not ECnumbers Is Nothing Then
            Dim subcellularLocation As String() = getTags(protein).ToArray
            Dim seq As String = protein.ProteinSequence
            Dim uniqueId As String = protein.accessions(Scan0)

            For Each number As dbReference In ECnumbers
                Dim ec As String = number.id
                Dim tokens As String() = ec.Split("."c)
                Dim rootName As String = rootNames(tokens(Scan0))
                Dim path As String = $"/enzyme/{rootName}/{tokens.Skip(1).JoinBy("/")}/{uniqueId}.txt"

                Call stream.WriteText(seq, path, Encodings.ASCII)
            Next

            If subcellularLocation.Length > 0 Then
                Call locations.Add(uniqueId, subcellularLocation)
            End If
        End If
    End Sub

    Private Shared Iterator Function getTags(protein As entry) As IEnumerable(Of String)
        Dim locs = protein.CommentList.TryGetValue("subcellular location")

        If Not locs Is Nothing Then
            For Each item As comment In locs
                For Each loc As subcellularLocation In item.subcellularLocations.SafeQuery
                    If Not loc.locations.IsNullOrEmpty Then
                        For Each tag As value In loc.locations
                            If Not tag.value.StringEmpty Then
                                Yield tag.value
                            End If
                        Next
                    End If
                Next
            Next
        End If
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call stream.WriteText(locations.ToBEncode.ToBencodedString, "/subcellularLocation.txt")
                Call stream.Dispose()
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
