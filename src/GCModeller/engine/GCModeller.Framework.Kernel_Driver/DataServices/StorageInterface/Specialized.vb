#Region "Microsoft.VisualBasic::9b1b81c2275430b35eaa756eab2a728e, GCModeller\engine\GCModeller.Framework.Kernel_Driver\DataServices\StorageInterface\Specialized.vb"

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

    '   Total Lines: 27
    '    Code Lines: 18
    ' Comment Lines: 4
    '   Blank Lines: 5
    '     File Size: 933 B


    '     Class CHUNK_BUFFER_TransitionStates
    ' 
    ' 
    ' 
    '     Class CHUNK_BUFFER_EntityQuantities
    ' 
    ' 
    ' 
    '     Class CHUNK_BUFFER_StateEnumerations
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData

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
