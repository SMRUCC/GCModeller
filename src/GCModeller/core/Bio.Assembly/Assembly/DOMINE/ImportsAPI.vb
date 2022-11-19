#Region "Microsoft.VisualBasic::d72ea865d8a714908e0e6d2db2d86941, GCModeller\core\Bio.Assembly\Assembly\DOMINE\ImportsAPI.vb"

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

    '   Total Lines: 69
    '    Code Lines: 59
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 2.85 KB


    '     Module ImportsAPI
    ' 
    '         Function: [Imports], Go, ImportsDb, Interaction, Pfam
    '                   PGMap
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Assembly.DOMINE

    <Package("DOMINE.API")>
    Public Module ImportsAPI

        <ExportAPI("ImportsDb")>
        Public Function ImportsDb(DbDIR As String) As DOMINE.Database
            Dim Database As DOMINE.Database = New Database(DbDIR)

            Database.Interaction = [Imports](Of DOMINE.Tables.Interaction)(DbDIR & "/INTERACTION.txt", AddressOf Interaction)
            Database.Pfam = [Imports](Of DOMINE.Tables.Pfam)(DbDIR & "/PFAM.txt", AddressOf Pfam)
            Database.Go = [Imports](Of DOMINE.Tables.Go)(DbDIR & "/GO.txt", AddressOf Go)
            Database.PGMap = [Imports](Of DOMINE.Tables.PGMap)(DbDIR & "/PGMAP.txt", AddressOf PGMap)

            Return Database
        End Function

        Private Function [Imports](Of Table As Class)(FilePath As String, ConstructMethod As System.Func(Of String(), Table)) As Table()
            Dim File As String() = IO.File.ReadAllLines(FilePath)
            Dim LQuery = From Line As String In File
                         Let Tokens As String() = Line.Split(CChar("|"))
                         Select ConstructMethod(Tokens) '
            Return LQuery.ToArray
        End Function

        Private Function Interaction(Tokens As String()) As DOMINE.Tables.Interaction
            Dim vec As List(Of Integer) = (From str As String In Tokens.Skip(2).Take(15)
                                           Select CType(Val(str), Integer)).AsList _
                                                 .Join({CType(Val(Tokens.Last), Integer)})
            Dim Db As New DOMINE.Tables.Interaction With {
                .Domain1 = Tokens(0),
                .Domain2 = Tokens(1),
                .PredictionConfidence = Tokens(17),
                .MetaData = vec.ToArray
            }
            Return Db
        End Function

        <ExportAPI("Imports.Pfam")>
        Public Function Pfam(Tokens As String()) As DOMINE.Tables.Pfam
            Return New DOMINE.Tables.Pfam With {
                .DomainAcc = Tokens(0),
                .DomainId = Tokens(1),
                .DomainDesc = Tokens(2),
                .InterproId = Tokens(3)
            }
        End Function

        <ExportAPI("Imports.PGMap")>
        Public Function PGMap(Tokens As String()) As DOMINE.Tables.PGMap
            Return New DOMINE.Tables.PGMap With {
                .DomainAcc = Tokens(0),
                .GoTerm = Tokens(1)
            }
        End Function

        <ExportAPI("Imports.GO")>
        Public Function Go(Tokens As String()) As DOMINE.Tables.Go
            Return New DOMINE.Tables.Go With {
                .GoTerm = Tokens(0),
                .Ontology = Tokens(1),
                .GoDesc = Tokens(2)
            }
        End Function
    End Module
End Namespace
