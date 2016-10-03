#Region "Microsoft.VisualBasic::2bcbef30ee527e4462641e5e5b4e7976, ..\GCModeller\engine\GCModeller\EngineSystem\Services\DataAcquisition\DataAdapters\DisposableSystem.vb"

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

Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    Public Class DisposableSystem(Of MolecularType As EngineSystem.ObjectModels.Entity.IDisposableCompound)
        Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.DisposerSystem(Of MolecularType))
        Implements IDataAdapter

        Dim Type As EngineSystem.ObjectModels.Entity.IDisposableCompound.DisposableCompoundTypes

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "Disposal_Of_" & Type.ToString
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.DisposerSystem(Of MolecularType), Type As EngineSystem.ObjectModels.Entity.IDisposableCompound.DisposableCompoundTypes)
            Call MyBase.New(System)
            Me.Type = Type
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return System.DataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            If Type = ObjectModels.Entity.IDisposableCompound.DisposableCompoundTypes.Transcripts Then
                Dim ChunkBuffer = (From item In MyBase.System.NetworkComponents Select New HandleF With {.Handle = item.Handle, .Identifier = item.Identifier.Replace(".TRANSCRIPT", "")}).ToArray
                Return ChunkBuffer
            Else
                Return MyBase.System.get_DataSerializerHandles
            End If
        End Function
    End Class
End Namespace
