#Region "Microsoft.VisualBasic::c904d1ad9c6ee837e7b1a0fe3ca42924, models\Networks\Microbiome\Enzyme.vb"

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

    ' Class Enzyme
    ' 
    '     Properties: EC, KO, name
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Selects
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data

Public Class Enzyme

    Public Property KO As String
    Public Property EC As ECNumber
    Public Property name As String

    Sub New(KO$, geneName$, EC$)
        Me.KO = KO
        Me.name = geneName
        Me.EC = ECNumber.ValueParser(EC)
    End Sub

    ''' <summary>
    ''' Select enzymes
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Selects(repo As ReactionRepository) As IReadOnlyDictionary(Of String, Reaction)
        Return repo _
            .GetWhere(Function(r)
                          Return r.Enzyme _
                              .Any(Function(id)
                                       Return Me.EC.Contains(id) OrElse ECNumber.ValueParser(id).Contains(EC)
                                   End Function)
                      End Function)
    End Function
End Class
