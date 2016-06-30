Imports System.Drawing
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel

Namespace CytoscapeGraphView.XGMML

    <XmlType("edge")>
    Public Class Edge : Inherits AttributeDictionary
        Implements IAddressHandle

        <XmlAttribute("id")> Public Property Id As Integer Implements IAddressHandle.Address
        <XmlAttribute("label")> Public Property Label As String
        <XmlElement("graphics")> Public Property Graphics As EdgeGraphics
        <XmlAttribute("source")> Public Property source As Long
        <XmlAttribute("target")> Public Property target As Long

        Public Function ContainsNode(id As Long) As Boolean
            Return source = id OrElse target = id
        End Function

        Public Function ContainsOneOfNode(Id As Integer()) As Boolean
            For Each handle In Id
                If source = handle OrElse target = handle Then
                    Return True
                End If
            Next
            Return False
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0} ""{1}""", Id, Label)
        End Function

        ''' <summary>
        ''' 应用于节点的去重
        ''' </summary>
        ''' <returns></returns>
        Protected Friend ReadOnly Property __internalUID As Long
            Get
                Dim dt = {source, target}
                Return dt.Max * 1000000 + dt.Min
            End Get
        End Property
#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace