#Region "Microsoft.VisualBasic::dee020152c34c1e1136bce7a776f7ea8, core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\CategoryEntry\ProteinFamily.vb"

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

    '     Class SignalingAndCellularProcesses
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: BacterialToxins, Exosome, ProkaryoticDefenseSystem, SecretionSystem, Transporters
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.KEGG.DBGET.BriteHEntry.ProteinFamily

    Public NotInheritable Class SignalingAndCellularProcesses

        Private Sub New()
        End Sub

        Public Shared Function Transporters() As BriteTerm()
            Return BriteTerm.GetInformation("https://www.kegg.jp/kegg-bin/get_htext?ko02000.keg", "K\d+")
        End Function

        Public Shared Function SecretionSystem() As BriteTerm()
            Return BriteTerm.GetInformation("https://www.kegg.jp/kegg-bin/get_htext?ko02044.keg", "K\d+")
        End Function

        Public Shared Function BacterialToxins() As BriteTerm()
            Return BriteTerm.GetInformation("https://www.kegg.jp/kegg-bin/get_htext?ko02042.keg", "K\d+")
        End Function

        Public Shared Function Exosome() As BriteTerm()
            Return BriteTerm.GetInformation("https://www.kegg.jp/kegg-bin/get_htext?ko04147.keg", "K\d+")
        End Function

        Public Shared Function ProkaryoticDefenseSystem() As BriteTerm()
            Return BriteTerm.GetInformation("https://www.kegg.jp/kegg-bin/get_htext?ko02048.keg", "K\d+")
        End Function
    End Class
End Namespace
