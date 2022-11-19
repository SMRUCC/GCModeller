#Region "Microsoft.VisualBasic::2f3c9d51e3cb3e356fb4f73ef1b9f333, GCModeller\models\ncbi-BioSystems\ProjectWriter.vb"

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

    '   Total Lines: 68
    '    Code Lines: 43
    ' Comment Lines: 11
    '   Blank Lines: 14
    '     File Size: 2.54 KB


    ' Class ProjectWriter
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: (+2 Overloads) Dispose, WriteMetabolicNetwork, WriteProject
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class ProjectWriter : Implements IDisposable

    ReadOnly stream As StreamPack
    ReadOnly proteinWriter As PtfWriter

    Private disposedValue As Boolean

    Sub New(file As Stream)
        stream = New StreamPack(file)
        proteinWriter = New PtfWriter(stream, {})
    End Sub

    Public Sub WriteProject(proj As Project)
        Dim fasta As New List(Of FastaSeq)

        Call stream.WriteText(proj.metadata.GetXml, "/metadata.xml")

        For Each protein As ProteinAnnotation In proj.proteins.proteins.Values
            Call proteinWriter.AddProtein(protein)
            Call fasta.Add(New FastaSeq With {.SequenceData = protein.sequence, .Headers = {protein.geneId}})
        Next

        ' save fasta sequence pack
        Call stream.WriteText(New FastaFile(fasta).Generate, "/workspace/protein_set.fasta", Encodings.ASCII)
    End Sub

    Public Sub WriteMetabolicNetwork(compartment As String, enzymes As IEnumerable(Of Enzyme))
        Dim path As String = $"/metabolic/{compartment}.json"
        Dim json As String = enzymes.ToArray.GetJson

        Call stream.WriteText(json, path)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call proteinWriter.Dispose()
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

