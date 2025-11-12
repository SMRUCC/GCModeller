#Region "Microsoft.VisualBasic::de3aae553d20c38490682bca8ccc790c, data\MicrobesOnline\WebParser\fetchLocus.vb"

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

    '   Total Lines: 41
    '    Code Lines: 26 (63.41%)
    ' Comment Lines: 11 (26.83%)
    '    - Xml Docs: 81.82%
    ' 
    '   Blank Lines: 4 (9.76%)
    '     File Size: 1.92 KB


    ' Module fetchLocus
    ' 
    '     Function: Downloads, locusId
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' Some gene sequence is not exists in the KEGG database that can be download from this database.
''' http://www.microbesonline.org/cgi-bin/fetchLocus.cgi?locus=5219138&amp;disp=4
''' </summary>
''' 
<Package("MicrobesOnline.fetchLocus",
                  Category:=APICategories.SoftwareTools,
                  Url:="http://www.microbesonline.org/cgi-bin/fetchLocus.cgi?locus=<gene_locus>&disp=4")>
Public Module MicrobesOnline

    Const FetchLocus As String = "http://www.microbesonline.org/cgi-bin/fetchLocus.cgi?locus={0}&disp=4"

    ''' <summary>
    ''' {prot, nt}, Downloads nt and prot sequence from microbesonline database
    ''' </summary>
    ''' <param name="locusId"></param>
    ''' <returns></returns>
    ''' 
    Public Function Downloads(locusId As String) _
        As <FunctionReturns("The return array has two elements, first element is the prot sequnece and the second element is the nt sequence.")> FastaSeq()
        Dim page As String = String.Format(FetchLocus, locusId).GET
        Dim ms = Regex.Matches(page, "<pre>.+?</pre>", RegexOptions.Singleline Or RegexOptions.IgnoreCase)
        Dim seqs As String() = ms.ToArray(Function(x) Mid(x, 6).Replace("</pre>", ""))
        Dim lstFa As FastaSeq() = seqs.Select(Function(seq) FastaSeq.TryParse(seq)).ToArray
        Return lstFa.Take(2).ToArray
    End Function

    <ExportAPI("Locus.Parser")>
    Public Function locusId(url As String) As String
        Dim id As String = Regex.Match(url, "locus=.+?&", RegexOptions.IgnoreCase).Value
        id = id.Split("="c).Last
        id = Mid(id, 1, Len(id) - 1)
        Return id
    End Function
End Module
