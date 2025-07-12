#Region "Microsoft.VisualBasic::22fc160f1d802c0b4f4c5f53848239e5, engine\IO\GCMarkupLanguage\v2\Assembly\ZipAssembly.vb"

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

'   Total Lines: 71
'    Code Lines: 57 (80.28%)
' Comment Lines: 5 (7.04%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 9 (12.68%)
'     File Size: 3.35 KB


'     Class ZipAssembly
' 
'         Function: CreateVirtualCellXml, GetComponent, GetText, WriteZip
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Zip
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Metagenomics

Namespace v2

    ''' <summary>
    ''' 将所有的模型数据都放置在同一个XML文件之中
    ''' 会因为文件过大而难于调试
    ''' 在这里进行元素的分块存储
    ''' </summary>
    Public Class ZipAssembly : Implements IDisposable

        Dim zip As ZipStream
        Private disposedValue As Boolean

        Sub New(path As String)
            zip = New ZipStream(path, is_readonly:=True)
        End Sub

        Public Function GetText(path As String) As String
            Dim fileKey As String = path.Trim("/"c, "\"c)
            Dim str As String = zip.ReadAllText(fileKey)
            Return str
        End Function

        Public Function getComponentSet(Of T)(path As String) As T()
            Return reader(Of T)(path).ToArray
        End Function

        Private Iterator Function reader(Of T)(path As String) As IEnumerable(Of T)
            For Each line As String In zip.ReadLines(path)
                Yield line.LoadJSON(Of T)
            Next
        End Function

        Public Function CreateVirtualCellXml() As VirtualCell
            Return New VirtualCell With {
                .genome = New Genome With {
                    .regulations = getComponentSet(Of transcription)($"{NameOf(VirtualCell.genome)}\{NameOf(Genome.regulations)}.jsonl"),
                    .replicons = getComponentSet(Of replicon)($"{NameOf(VirtualCell.genome)}\{NameOf(Genome.replicons)}.jsonl")
                },
                .metabolismStructure = New MetabolismStructure With {
                    .compounds = getComponentSet(Of Compound)($"{NameOf(VirtualCell.metabolismStructure)}\{NameOf(MetabolismStructure.compounds)}.jsonl"),
                    .enzymes = getComponentSet(Of Enzyme)($"{NameOf(VirtualCell.metabolismStructure)}\{NameOf(MetabolismStructure.enzymes)}.jsonl"),
                    .maps = getComponentSet(Of FunctionalCategory)($"{NameOf(VirtualCell.metabolismStructure)}\{NameOf(MetabolismStructure.maps)}.jsonl"),
                    .reactions = New ReactionGroup With {
                        .enzymatic = getComponentSet(Of Reaction)($"{NameOf(VirtualCell.metabolismStructure)}\{NameOf(MetabolismStructure.reactions)}\{NameOf(ReactionGroup.enzymatic)}.jsonl"),
                        .none_enzymatic = getComponentSet(Of Reaction)($"{NameOf(VirtualCell.metabolismStructure)}\{NameOf(MetabolismStructure.reactions)}\{NameOf(ReactionGroup.none_enzymatic)}.jsonl")
                    }
                },
                .taxonomy = GetText($"{NameOf(VirtualCell.taxonomy)}.json").LoadJSON(Of Taxonomy),
                .properties = GetText($"{NameOf(VirtualCell.properties)}.json").LoadJSON(Of CompilerServices.[Property])
            }
        End Function

        Public Shared Function CreateVirtualCellXml(path As String) As VirtualCell
            Using zip As New ZipAssembly(path)
                Return zip.CreateVirtualCellXml
            End Using
        End Function

        Public Shared Function WriteZip(vcell As VirtualCell, zip As String) As Boolean
            Return ZipWriter.WriteZip(vcell, zip)
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call zip.Dispose()
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
