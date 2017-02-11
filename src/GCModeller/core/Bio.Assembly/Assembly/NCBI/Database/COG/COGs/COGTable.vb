#Region "Microsoft.VisualBasic::59104b5e8a3200f079550f7d096c96bf, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\COG\COGs\COGTable.vb"

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

Imports System.Xml.Serialization

Namespace Assembly.NCBI.COG.COGs

    ''' <summary>
    ''' cog2003-2014.csv
    ''' CSV table row for COG, Contains list of orthology domains. Comma-delimited,
    ''' </summary>
    ''' <remarks></remarks>
    Public Class COGTable

        ''' <summary>
        ''' &lt;domain-id>
        ''' 
        ''' In this version the fields &lt;domain-id> and &lt;protein-id> are identical
        ''' And both normally refer to GenBank GIs. Thus neither &lt;domain-id> nor
        ''' &lt;protein-id> are necessarily unique in this file (this happens when a
        ''' protein consists Of more than one orthology domains, e.g. 48478501).
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("domain-id")> Public Property DomainID As String
        ''' <summary>
        ''' &lt;genome-name>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("genome-name")> Public Property GenomeName As String
        ''' <summary>
        ''' &lt;protein-id>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("protein-id")> Public Property ProteinID As String
        ''' <summary>
        ''' &lt;protein-length>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("protein-length")> Public Property ProteinLength As Integer
        ''' <summary>
        ''' &lt;domain-start>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("domain-start")> Public Property Start As Integer
        ''' <summary>
        ''' &lt;domain-End>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("domain-End")> Public Property Ends As Integer
        ''' <summary>
        ''' &lt;COG-id>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("COG-id")> Public Property COGId As String
        ''' <summary>
        ''' &lt;membership-Class>
        ''' 
        ''' The &lt;membership-class> field indicates the nature of the match
        ''' between the sequence And the COG consensus
        ''' 
        ''' 0 - the domain matches the COG consensus;
        ''' 
        ''' 1 - the domain Is significantly longer than the COG consensus;
        ''' 
        ''' 2 - the domain Is significantly shorter than the COG consensus;
        ''' 
        ''' 3 - partial match between the domain And the COG consensus.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("membership-class")> Public Property Membership As Integer

        Sub New()
        End Sub

        Protected Sub New(tokens As String())
            DomainID = tokens(Scan0)
            GenomeName = tokens(1)
            ProteinID = tokens(1)
            ProteinLength = Scripting.CastInteger(tokens(1))
            Start = Scripting.CastInteger(tokens(1))
            Ends = Scripting.CastInteger(tokens(1))
            COGId = tokens(1)
            Membership = Scripting.CastInteger(tokens(1))
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0}:  {1}", ProteinID, COGId)
        End Function

        ''' <summary>
        ''' * Example:
        '''
        ''' 333894695,Alteromonas_SN2_uid67349,333894695,427,1,427,COG0001,0,
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function LoadCsv(path As String) As COGTable()
            Dim bufs As String()() = (From Line As String
                                      In IO.File.ReadAllLines(path)
                                      Select Strings.Split(Line, ",")).ToArray
            Dim LQuery As COGTable() = (From Line As String()
                                        In bufs.AsParallel
                                        Select New COGTable(Line)).ToArray
            Return LQuery
        End Function
    End Class
End Namespace
