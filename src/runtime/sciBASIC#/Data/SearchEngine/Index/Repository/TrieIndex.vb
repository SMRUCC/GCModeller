#Region "Microsoft.VisualBasic::eff06b7025b8c1c0e14304c5078d4d17, Data\BinaryData\BinaryData\Repository\BinarySearchIndex.vb"

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

' Class BinarySearchIndex
' 
' 
' 
' Class Index
' 
'     Properties: Key, left, Offset, right
' 
'     Function: Write
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text.Parser
''' <summary>
''' 主要是针对于字符串类型的索引文件的构建, 在这里尝试使用字典树来节省存储空间
''' </summary>
Public Class TrieIndexWriter : Implements IDisposable

    ReadOnly index As BinaryDataWriter

    Public Sub AddTerm(term As String)
        Dim chars As CharPtr = term

    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                Call index.Flush()
                Call index.Close()
                Call index.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class

Public Class CharNode

    Public Property [char] As Char

    ''' <summary>
    ''' 在这里是接下来的字符的在索引文件之中的位置
    ''' </summary>
    ''' <returns></returns>
    Public Property [next] As Long()

End Class