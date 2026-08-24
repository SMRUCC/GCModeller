#Region "Microsoft.VisualBasic::0247ac87e0d060ab35480f263d49eb16, analysis\OperonMapper\NTCluster\FingerprintMatrixWriter.vb"

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

    '   Total Lines: 52
    '    Code Lines: 32 (61.54%)
    ' Comment Lines: 10 (19.23%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 10 (19.23%)
    '     File Size: 1.88 KB


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
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON

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

