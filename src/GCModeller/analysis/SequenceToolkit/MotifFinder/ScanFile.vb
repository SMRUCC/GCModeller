#Region "Microsoft.VisualBasic::c42019eb85e8cd99abfe7223780d65d4, analysis\SequenceToolkit\MotifFinder\ScanFile.vb"

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

'   Total Lines: 61
'    Code Lines: 39 (63.93%)
' Comment Lines: 10 (16.39%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 12 (19.67%)
'     File Size: 2.20 KB


' Class ScanFile
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: LoadAllSeeds
' 
'     Sub: AddSeed, (+2 Overloads) Dispose
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment

Public Class ScanFile : Implements IDisposable

    ReadOnly pack As StreamPack

    Private disposedValue As Boolean

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(file As Stream)
        pack = New StreamPack(file, meta_size:=1024 * 1024 * 32)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub AddSeed(name As String, seed As HSP)
        Dim path = $"/seeds/{Mid(name, 1, 2)}/{name}.json"

        Call pack.Delete(path)
        Call pack.WriteText(seed.GetJson, fileName:=path)
    End Sub

    Public Iterator Function LoadAllSeeds() As IEnumerable(Of HSP)
        Dim folder As StreamGroup = pack.GetObject("/seeds/")

        For Each file As StreamObject In folder.ListFiles
            If TypeOf file Is StreamBlock Then
                Yield New StreamReader(pack.OpenBlock(file)).ReadToEnd.LoadJSON(Of HSP)
            End If
        Next
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call pack.Dispose()
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
