﻿#Region "Microsoft.VisualBasic::0bda7251cbeeaf1e9e934e44bf361ab8, G:/GCModeller/src/repository/Bio.Repository//UniProt/ECNumberReader.vb"

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

    '   Total Lines: 125
    '    Code Lines: 94
    ' Comment Lines: 11
    '   Blank Lines: 20
    '     File Size: 4.91 KB


    ' Class ECNumberReader
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetSubcellularLocations, QueryEnzymeFasta, QueryFasta, QuerySubcellularFasta
    ' 
    '     Sub: (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class ECNumberReader : Implements IDisposable

    Private disposedValue As Boolean
    Private ReadOnly stream As StreamPack

    Public Shared ReadOnly rootNames As IReadOnlyDictionary(Of String, String) = Enums(Of EnzymeClasses) _
        .ToDictionary(Function(c) CInt(c).ToString & "." & c.Description,
                      Function(c)
                          Return CInt(c).ToString
                      End Function)

    Sub New(file As Stream)
        Me.stream = New StreamPack(file, [readonly]:=True)
    End Sub

    Public Function GetSubcellularLocations() As Dictionary(Of String, String())
        Dim btext As String = stream.ReadText("/subcellularLocation.txt")
        Dim bnodes As BDictionary = BencodeDecoder.Decode(btext)(0)
        Dim list As New Dictionary(Of String, String())

        For Each key As BString In bnodes.Keys
            list(key.Value) = bnodes(key) _
                .ToList _
                .Select(Function(str) DirectCast(str, BString).Value) _
                .ToArray
        Next

        Return list
    End Function

    Public Function QueryFasta(Optional q As String = "*", Optional enzymeQuery As Boolean = True) As IEnumerable(Of FastaSeq)
        If enzymeQuery Then
            Return QueryEnzymeFasta(q)
        Else
            Return QuerySubcellularFasta(q)
        End If
    End Function

    Public Iterator Function QuerySubcellularFasta(Optional q As String = "*") As IEnumerable(Of FastaSeq)
        If q = "*" Then
            Dim locations = GetSubcellularLocations()

            For Each file As StreamBlock In DirectCast(stream.GetObject("/enzyme/"), StreamGroup) _
                .ListFiles _
                .Where(Function(f)
                           Return TypeOf f Is StreamBlock
                       End Function)

                Dim seq As String = New StreamReader(stream.OpenBlock(file)).ReadToEnd
                Dim id As String = file.fileName.BaseName

                If Not locations.ContainsKey(id) Then
                    Continue For
                End If

                For Each tag As String In locations(id)
                    Yield New FastaSeq With {
                        .Headers = {tag, id},
                        .SequenceData = seq
                    }
                Next
            Next
        Else
            Throw New NotImplementedException
        End If
    End Function

    Public Iterator Function QueryEnzymeFasta(Optional q As String = "*") As IEnumerable(Of FastaSeq)
        If q = "*" Then
            For Each file As StreamBlock In DirectCast(stream.GetObject("/enzyme/"), StreamGroup) _
                .ListFiles _
                .Where(Function(f)
                           Return TypeOf f Is StreamBlock
                       End Function)

                Dim seq As String = New StreamReader(stream.OpenBlock(file)).ReadToEnd
                ' first element tag is the enzyme folder name
                Dim tokens = file.fullName.Trim("/"c).Split("/"c).Skip(1).ToArray
                Dim classNumber = rootNames(tokens(0))
                Dim ECNumber = classNumber & "." & tokens.Skip(1).Take(tokens.Length - 2).JoinBy(".")
                Dim id As String = file.fileName.BaseName

                Yield New FastaSeq With {
                    .Headers = {ECNumber, id},
                    .SequenceData = seq
                }
            Next
        Else
            Throw New NotImplementedException
        End If
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
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

