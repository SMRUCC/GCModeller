Imports System.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Namespace vcXML

    Public Class Reader : Implements IDisposable

        Dim fs As StreamReader
        ''' <summary>
        ''' [module -> [type -> offset]]
        ''' </summary>
        Dim index As Dictionary(Of String, Dictionary(Of String, List(Of offset)))
        Dim entities As Dictionary(Of String, Dictionary(Of String, String()))
        Dim hash As String

        Public ReadOnly Property allFrames As offset()
            Get
                Return index.Values _
                    .IteratesALL _
                    .Values _
                    .IteratesALL _
                    .ToArray
            End Get
        End Property

        Sub New(file As String)
            fs = file.OpenReader()
            fs.BaseStream.Seek(-128, SeekOrigin.End)

            Call loadOffsets()
        End Sub

        Public Function getStreamIndex(name As String) As Dictionary(Of String, List(Of offset))
            Return index(name)
        End Function

        Public Function getFrameVector(offset As Long) As Double()

        End Function

        Private Sub loadOffsets()
            Dim line As Value(Of String) = ""
            Dim content = fs.ReadToEnd
            Dim i As Long = content _
                .Match("<indexOffset>\d+</indexOffset>") _
                .GetValue _
                .DoCall(AddressOf Long.Parse)

            hash = content.Match("<md5>.+</md5>", RegexICSng).GetValue
            fs.BaseStream.Seek(i, SeekOrigin.Begin)
            content = fs.ReadLine
            index = New Dictionary(Of String, Dictionary(Of String, List(Of offset)))

            Dim numOfFrames As Integer = content.attr("size").DoCall(AddressOf Integer.Parse)
            Dim module$
            Dim type$
            Dim attrs As Dictionary(Of String, String)
            Dim offset As offset

            Do While numOfFrames > 0 AndAlso Not (line = fs.ReadLine).StringEmpty
                attrs = line.Value _
                    .TagAttributes _
                    .ToDictionary(Function(a) a.Name,
                                  Function(a)
                                      Return a.Value
                                  End Function)
                [module] = attrs!module
                [type] = attrs!content_type

                If Not index.ContainsKey([module]) Then
                    index.Add([module], New Dictionary(Of String, List(Of offset)))
                End If
                If Not index([module]).ContainsKey(type) Then
                    index([module]).Add(type, New List(Of offset))
                End If

                numOfFrames -= 1
                offset = New offset With {
                    .offset = line.Value _
                        .GetValue _
                        .DoCall(AddressOf Long.Parse),
                    .content_type = type,
                    .id = Integer.Parse(attrs!id),
                    .[module] = [module]
                }
                index([module])(type).Add(offset)
            Loop

            fs.ReadLine()

            content = fs.ReadLine
            numOfFrames = content.attr("size").DoCall(AddressOf Integer.Parse)

            Do While numOfFrames > 0 AndAlso Not (line = fs.ReadLine).StringEmpty
                attrs = line.Value _
                   .TagAttributes _
                   .ToDictionary(Function(a) a.Name,
                                 Function(a)
                                     Return a.Value
                                 End Function)
                [module] = attrs!module
                [type] = attrs!content_type

                If Not index.ContainsKey([module]) Then
                    index.Add([module], New Dictionary(Of String, List(Of offset)))
                End If
                If Not index([module]).ContainsKey(type) Then
                    index([module]).Add(type, New List(Of offset))
                End If

                numOfFrames -= 1
            Loop
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call fs.Close()
                    Call fs.Dispose()
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
End Namespace