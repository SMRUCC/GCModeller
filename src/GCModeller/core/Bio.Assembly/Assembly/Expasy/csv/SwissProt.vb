#Region "Microsoft.VisualBasic::19a2e5b8d6b5e940d064790e68e040a5, GCModeller\core\Bio.Assembly\Assembly\Expasy\csv\SwissProt.vb"

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

    '   Total Lines: 28
    '    Code Lines: 22
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 767 B


    '     Class SwissProt
    ' 
    '         Properties: [Class], Entry, seq
    ' 
    '         Function: CreateObjects, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language

Namespace Assembly.Expasy.Database.csv

    Public Class SwissProt

        Public Property [Class] As String
        Public Property Entry As String
        Public Property seq As String

        Public Overrides Function ToString() As String
            Return Entry
        End Function

        Public Shared Function CreateObjects(Enzyme As Database.Enzyme) As SwissProt()
            Dim LQuery = LinqAPI.Exec(Of SwissProt) <=
 _
                From Id As String
                In Enzyme.SwissProt
                Select New SwissProt With {
                    .Class = Enzyme.Identification,
                    .Entry = Id
                }

            Return LQuery
        End Function
    End Class
End Namespace
