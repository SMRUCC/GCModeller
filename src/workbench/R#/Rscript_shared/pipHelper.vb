#Region "Microsoft.VisualBasic::9a48b1c780547a1d050c2b3587553c2a, Rscript_shared\pipHelper.vb"

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

    ' Module pipHelper
    ' 
    '     Function: getUniprotData
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object

Module pipHelper

    Public Function getUniprotData(uniprot As Object, env As Environment) As [Variant](Of IEnumerable(Of entry), Message)
        If uniprot Is Nothing Then
            Return DirectCast(New entry() {}, IEnumerable(Of entry))
        End If

        If TypeOf uniprot Is entry() OrElse TypeOf uniprot Is IEnumerable(Of entry) Then
            Return New [Variant](Of IEnumerable(Of entry), Message)(DirectCast(uniprot, IEnumerable(Of entry)))
        ElseIf TypeOf uniprot Is pipeline AndAlso DirectCast(uniprot, pipeline).elementType Like GetType(entry) Then
            Return New [Variant](Of IEnumerable(Of entry), Message)(DirectCast(uniprot, pipeline).populates(Of entry)(env))
        ElseIf TypeOf uniprot Is vector AndAlso DirectCast(uniprot, vector).elementType Like GetType(entry) Then
            Return New [Variant](Of IEnumerable(Of entry), Message)(DirectCast(uniprot, vector).data.AsObjectEnumerator(Of entry))
        Else
            Return Internal.debug.stop($"invalid data source input: {uniprot.GetType.FullName}!", env)
        End If
    End Function
End Module

