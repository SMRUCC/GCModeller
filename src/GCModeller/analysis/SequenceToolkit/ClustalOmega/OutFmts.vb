#Region "Microsoft.VisualBasic::cf3704efae643f960bf1771b3b7222bc, GCModeller\analysis\SequenceToolkit\ClustalOmega\OutFmts.vb"

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

    '   Total Lines: 43
    '    Code Lines: 11
    ' Comment Lines: 29
    '   Blank Lines: 3
    '     File Size: 1.06 KB


    ' Enum OutFmts
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

''' <summary>
''' Formats options for alignment inputs and outputs.
''' 
''' ``--outfmt={a2m=fa[sta],clu[stal],msf,phy[lip],selex,st[ockholm],vie[nna]}`` MSA output file format (default: ``fasta``)
''' </summary>
Public Enum OutFmts As Integer

    ''' <summary>
    ''' 不显示申明这个命令行参数，由系统自动选择
    ''' </summary>
    auto = -1

    ''' <summary>
    ''' fa[sta], Default
    ''' </summary>
    <Description("fa")> fasta = 0
    ''' <summary>
    ''' clu[stal]
    ''' </summary>
    <Description("clu")> clustal
    ''' <summary>
    ''' msf
    ''' </summary>
    <Description("msf")> msf
    ''' <summary>
    ''' phy[lip]
    ''' </summary>
    <Description("phy")> phylip
    ''' <summary>
    ''' selex
    ''' </summary>
    <Description("selex")> selex
    ''' <summary>
    ''' st[ockholm]
    ''' </summary>
    <Description("st")> stockholm
    ''' <summary>
    ''' vie[nna]
    ''' </summary>
    <Description("vie")> vienna
End Enum
