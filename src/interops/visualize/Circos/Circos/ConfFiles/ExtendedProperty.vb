#Region "Microsoft.VisualBasic::ca9c263bef77d5196723bd47e5e4a069, ..\interops\visualize\Circos\Circos\ConfFiles\ExtendedProperty.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Settings

Namespace Configurations

    ''' <summary>
    ''' Alias for <see cref="SimpleConfig"/>
    ''' </summary>
    Public Class CircosAttribute : Inherits SimpleConfig
    End Class

    Public Module ExtendedProperty

        <Extension>
        Public Function Ideogram(x As Circos) As Ideogram
            For Each include In x.Includes
                If TypeOf include Is Ideogram Then
                    Return DirectCast(include, Ideogram)
                End If
            Next

            Return Nothing
        End Function
    End Module
End Namespace
