#Region "Microsoft.VisualBasic::44a18c97ec3efa3a2614ad192165ca43, ..\interops\visualize\Cytoscape\Cytoscape\Graph\Xgmml\Edge.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

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
