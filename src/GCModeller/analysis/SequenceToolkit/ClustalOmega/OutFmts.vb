#Region "Microsoft.VisualBasic::1f827eba8ec05e2c2399e65ed6fc2e1f, ..\GCModeller\analysis\SequenceToolkit\ClustalOmega\OutFmts.vb"

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

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine.InteropService

''' <summary>
''' Formats options for alignment inputs and outputs.
''' 
''' ``--outfmt={a2m=fa[sta],clu[stal],msf,phy[lip],selex,st[ockholm],vie[nna]}`` MSA output file format (default: ``fasta``)
''' </summary>
Public Enum OutFmts As Integer

    ''' <summary>
    ''' 不显示申明这个命令行参数，由系统自动选择
    ''' </summary>
    <NullOrDefault> auto = -1

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
