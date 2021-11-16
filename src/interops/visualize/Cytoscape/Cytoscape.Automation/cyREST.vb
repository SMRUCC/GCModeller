#Region "Microsoft.VisualBasic::801c9d908bdb6b116e4a2012aa300e97, visualize\Cytoscape\Cytoscape.Automation\cyREST.vb"

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

    ' Class cyREST
    ' 
    '     Sub: (+2 Overloads) Dispose
    ' 
    ' Class view
    ' 
    '     Properties: data
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.Cyjs
Imports SMRUCC.genomics.Visualize.Cytoscape.Tables

Public MustInherit Class cyREST : Implements IDisposable

    Private disposedValue As Boolean

    Public MustOverride Function layouts() As String()

    ''' <summary>
    ''' Returns a list of all networks as names and their corresponding SUIDs.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function networksNames() As String()

    ''' <summary>
    ''' Creates a new network in the current session from a file or URL source.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function putNetwork(network As [Variant](Of Cyjs, SIF()), Optional collection$ = Nothing, Optional title$ = Nothing) As NetworkReference
    Public MustOverride Function applyLayout(network As Integer, Optional algorithm As String = "force-directed") As String

    ''' <summary>
    ''' Returns the current Network View.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function getViewReference() As Integer
    ''' <summary>
    ''' Gets the Network View specified by the viewId and networkId parameters.
    ''' </summary>
    ''' <param name="networkId">SUID of the Network</param>
    ''' <param name="viewId">SUID of the Network View</param>
    ''' <returns></returns>
    Public MustOverride Function getView(networkId As Integer, viewId As Integer) As Cyjs

    ''' <summary>
    ''' Saves the current session to a file. If successful, the session file location will be returned.
    ''' </summary>
    ''' <param name="file">
    ''' Session file location as an absolute path.(``*.cys``)
    ''' </param>
    ''' <returns></returns>
    Public MustOverride Function saveSession(file As String)
    Public MustOverride Sub destroySession()

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
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

Public Class view
    Public Property data As Dictionary(Of String, String)
End Class
