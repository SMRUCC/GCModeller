#Region "Microsoft.VisualBasic::298e72d5eb1460c18fd5907d73583306, models\Networks\EID2\test\Module1.vb"

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

    ' Module Module1
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Module Module1

    Sub Main()
        Dim nnn = EID2.Interaction.BuildSharedPathogens(
            EID2.Interaction.Load("D:\OneDrive\2018-1-15\associations.csv", cargo:="vVirusNameCorrected", carrier:="hHostNameFinal"),
            cut:=2)

        Call nnn.Save("D:\OneDrive\2018-1-15\net")

        Pause()
    End Sub

End Module
