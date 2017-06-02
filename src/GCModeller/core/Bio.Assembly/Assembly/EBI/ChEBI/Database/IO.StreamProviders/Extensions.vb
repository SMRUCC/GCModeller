#Region "Microsoft.VisualBasic::4f09cb53c4a8c558a9ee21ce81a4104c, ..\core\Bio.Assembly\Assembly\EBI.ChEBI\Database\IO.StreamProviders\Extensions.vb"

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

Imports System.Runtime.CompilerServices

Namespace Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv

    Public Module Extensions

        ''' <summary>
        ''' Listing all types in a chebi entity object.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Types(Of T As Tables.Entity)(source As IEnumerable(Of T)) As String()
            Return source.Select(Function(o) o.TYPE).Distinct.ToArray
        End Function
    End Module
End Namespace
