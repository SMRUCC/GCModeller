Imports System.IO
Imports System.Text
Imports RDotNET.Extensions.VisualBasic.API.utils

Namespace Serialization

    Public Class csvHandle : Inherits StreamWriter
        Implements IDisposable

        ReadOnly R_server As ExtendedEngine

        ReadOnly tmp$
        ReadOnly name$

        Public Sub New(path$, var$, append As Boolean, encoding As Encoding, Optional Rengine As ExtendedEngine = Nothing)
            MyBase.New(path, append, encoding)

            tmp = path
            name = var
            R_server = Rengine Or ExtendedEngine.MyDefault
        End Sub

        Public Overrides Function ToString() As String
            Return $"{name} @ {tmp}"
        End Function

        Protected Overrides Sub Dispose(disposing As Boolean)
            ' 在这里发生了文件的写入操作
            ' 并关闭了文件句柄
            MyBase.Dispose(disposing)

            SyncLock R_server
                With R_server
                    .Assign(name) = read.csv(tmp)
                End With
            End SyncLock
        End Sub
    End Class
End Namespace