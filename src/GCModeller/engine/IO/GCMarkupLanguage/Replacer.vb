#Region "Microsoft.VisualBasic::8fabb026e0e05d8d1087e321045fcb67, GCModeller\engine\IO\GCMarkupLanguage\Replacer.vb"

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

    '   Total Lines: 22
    '    Code Lines: 17
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 879 B


    ' Class Replacer
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ApplyReplacements, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Public NotInheritable Class Replacer

    Protected Friend Sub New()
        Throw New NotImplementedException
    End Sub

    Public Shared Function ApplyReplacements(Of T_REF As SMRUCC.genomics.ComponentModel.EquaionModel.ICompoundSpecies,
                                                TModel As FLuxBalanceModel.I_FBAC2(Of T_REF))(
                Model As TModel, StringList As IEnumerable(Of Escaping)) As Integer

        Dim n = From Metabolite In Model.Metabolites Select Metabolite.Replace2(StringList) '
        Dim m = From flux In Model.MetabolismNetwork Select flux.Replace(StringList) '
        Return n.ToArray.Count + m.ToArray.Count
    End Function

    Public Overrides Function ToString() As String
        Throw New NotImplementedException
    End Function
End Class
