#Region "Microsoft.VisualBasic::7410482726bf4ad7f0c41cc132b8317c, ..\GCModeller\engine\GCModeller.Framework.Kernel_Driver\DataServices\StorageInterface\Specialized.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel

Namespace DataStorage.FileModel

    Public Class CHUNK_BUFFER_TransitionStates : Inherits DataSerials(Of Boolean)
    End Class

    Public Class CHUNK_BUFFER_EntityQuantities : Inherits DataSerials(Of Double)
    End Class

    ''' <summary>
    ''' Inherits DataSerials(Of Integer)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CHUNK_BUFFER_StateEnumerations : Inherits DataSerials(Of Integer)
        Public Overrides Function ToString() As String
            Return Me.UniqueId & "  " & String.Join("; ", Me.Samples)
        End Function
    End Class

End Namespace
