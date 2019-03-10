#Region "Microsoft.VisualBasic::37a36b80aef8ab8a91e63e038aec3020, CLI_tools\S.M.A.R.T\CLI\Filters.vb"

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

    ' Module CLI
    ' 
    '     Function: FiltePureDomain
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Extensions

Partial Module CLI

    ''' <summary>
    ''' 分析出仅含有一个结构域的蛋白质
    ''' </summary>
    ''' <param name="CommandLine"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("pure_domain", Usage:="pure_domain -i <input_smart_log> -o <output_file>")>
    Public Function FiltePureDomain(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        'Dim SMARTLog As ModularArchitecture.SMARTLog = CommandLine("-i").LoadXml(Of ModularArchitecture.SMARTLog)()
        'Dim LQuery = From Protein In SMARTLog.Proteins Let Id = Protein.PureDomain Where Not String.IsNullOrEmpty(Id) Select New DomainTag With {.Id = Id, .Protein = Protein.Title}   '
        'Dim Result = LQuery.ToArray

        'Dim Xml = Result.GetXml
        'Call FileIO.FileSystem.WriteAllText(CommandLine("-o"), Xml, append:=False)

        'Return 0

        Throw New NotImplementedException
    End Function
End Module
