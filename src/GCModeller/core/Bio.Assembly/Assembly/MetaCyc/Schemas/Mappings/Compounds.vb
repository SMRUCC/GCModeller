#Region "Microsoft.VisualBasic::33d94de942a8286d09a125256b33a0b0, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\Schemas\Mappings\Compounds.vb"

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

    '   Total Lines: 29
    '    Code Lines: 18
    ' Comment Lines: 4
    '   Blank Lines: 7
    '     File Size: 868 B


    '     Class EffectorMap
    ' 
    '         Properties: CHEBI, CommonName, Effector, EffectorAlias, KEGGCompound
    '                     MetaCycId, PUBCHEM, Synonym
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.MetaCyc.Schema

    ''' <summary>
    ''' Regprecise Effector与MetaCyc Compounds Mapping
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EffectorMap : Implements INamedValue

        Public Property Effector As String Implements INamedValue.Key

        Public Property EffectorAlias As String()
        Public Property MetaCycId As String
        Public Property CommonName As String
        Public Property Synonym As String

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} --> {1}", Effector, MetaCycId)
        End Function

        Public Property CHEBI As String()
        Public Property KEGGCompound As String
        Public Property PUBCHEM As String
    End Class
End Namespace
