#Region "Microsoft.VisualBasic::0e3cee21aa5353516ef15785bc722afa, core\Bio.Assembly\Assembly\KEGG\Web\Map\KGML\Extensions.vb"

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

    '   Total Lines: 37
    '    Code Lines: 32 (86.49%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (13.51%)
    '     File Size: 1.38 KB


    '     Module Extensions
    ' 
    '         Function: CompoundList, GetIdlistByType, KOlist
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.WebServices.KGML

    <HideModuleName> Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function KOlist(kgml As pathway) As String()
            Return kgml.GetIdlistByType("ortholog")
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function GetIdlistByType(kgml As pathway, type$) As String()
            Return kgml.entries _
                .Where(Function(entry) entry.type = type) _
                .Select(Function(entry)
                            Return entry.name _
                                .StringSplit("\s+") _
                                .Select(Function(id)
                                            Return id.GetTagValue(":").Value
                                        End Function)
                        End Function) _
                .IteratesALL _
                .Distinct _
                .ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function CompoundList(kgml As pathway) As String()
            Return kgml.GetIdlistByType("compound")
        End Function
    End Module
End Namespace
