Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Regtransbase.StructureObjects.Dictionaries

    ''' <summary>
    ''' RegTransBase contains the following dictionaries: 
    ''' 
    '''  '•	Genomes dictionary (Genome)
    '''  '•	Functional types of site dictionary (FuncSiteType)
    '''  '•	Structural types of site dictionary (StructSiteType)
    '''  '•	Types of position dictionary (ObjSideType)
    '''  '•	Dictionary of experimental techniques (ExpType)
    ''' 
    ''' All dictionaries are created by curator. Complete set of dictionaries can be exported 
    ''' as a single file for import by annotators in their annotation programs. Dictionary 
    ''' entry created by curator has property fl_new=FALSE. Annotator also can add entry into 
    ''' any dictionary. Such entry has property fl_new=TRUE and property user_id contains 
    ''' annotator name.
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class Dictionary
        'Elements of all dictionaries contains following common properties:

        <DatabaseField("name")> Public Property Name As String
        Public MustOverride Property Guid As Integer

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE `dict_genomes` (
    '''  `genome_guid` int(11) NOT NULL DEFAULT '0',
    '''  `name` varchar(100) DEFAULT NULL,
    ''' PRIMARY KEY (`genome_guid`)
    ''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
    ''' </remarks>
    Public Class Genomes : Inherits Regtransbase.StructureObjects.Dictionaries.Dictionary
        <DatabaseField("genome_guid")> Public Overrides Property Guid As Integer
    End Class
End Namespace
