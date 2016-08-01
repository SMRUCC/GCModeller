Namespace API

    Public Module base

        ''' <summary>
        ''' Loading/Attaching and Listing of Packages
        ''' </summary>
        ''' <param name="package">the name Of a package, given As a name Or literal character String, Or a character String, 
        ''' depending On whether character.only Is False (Default) Or True).</param>
        ''' <param name="libloc">a character vector describing the location Of R library trees To search through, Or NULL. 
        ''' The Default value Of NULL corresponds To all libraries currently known To .libPaths(). Non-existent library 
        ''' trees are silently ignored.</param>
        ''' <param name="quietly">a logical. If TRUE, no message confirming package attaching is printed, and most often, 
        ''' no errors/warnings are printed if package attaching fails.</param>
        ''' <param name="warnConflicts">logical. If TRUE, warnings are printed about conflicts from attaching the new package. 
        ''' A conflict is a function masking a function, or a non-function masking a non-function.</param>
        ''' <param name="characterOnly">a logical indicating whether package or help can be assumed to be character strings.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' library and require can only load/attach an installed package, and this is detected by having a ‘DESCRIPTION’ 
        ''' file containing a Built: field.
        '''
        ''' Under Unix-alikes, the code checks that the package was installed under a similar operating system As given 
        ''' by R.version$platform (the canonical name Of the platform under which R was compiled), provided it contains 
        ''' compiled code. Packages which Do Not contain compiled code can be Shared between Unix-alikes, but Not To 
        ''' other OSes because Of potential problems With line endings And OS-specific help files. If Sub-architectures 
        ''' are used, the OS similarity Is Not checked since the OS used To build may differ (e.g. i386-pc-linux-gnu 
        ''' code can be built On an x86_64-unknown-linux-gnu OS).
        '''
        ''' The package name given To library And require must match the name given In the package's ‘DESCRIPTION’ file exactly, 
        ''' even on case-insensitive file systems such as are common on Windows and OS X.
        ''' </remarks>
        Public Function require(package As String,
                                Optional libloc As String = NULL,
                                Optional quietly As Boolean = False,
                                Optional warnConflicts As Boolean = True,
                                Optional characterOnly As Boolean = False) As Boolean
            Dim Rscript As String = $"require({package},lib.loc={libloc},quietly={quietly.R},warn.conflicts={warnConflicts.R},character.only={characterOnly.R})"
            Return RServer.Evaluate(Rscript).AsLogical.First
        End Function

        ''' <summary>
        ''' Data Frames
        ''' 
        ''' This function creates data frames, tightly coupled collections of variables which share many of the properties of matrices and of lists, used as the fundamental data structure by most of R's modeling software.
        ''' </summary>
        ''' <param name="x">
        ''' these arguments are Of either the form value Or tag = value. Component names are created based On the tag (If present) Or the deparsed argument itself.
        ''' (其实在这里是R里面的对象的名称的列表)
        ''' </param>
        ''' <param name="rowNames">NULL or a single integer or character string specifying a column to be used as row names, or a character or integer vector giving the row names for the data frame.</param>
        ''' <param name="checkRows">If True Then the rows are checked For consistency Of length And names.</param>
        ''' <param name="checkNames">logical. If TRUE then the names of the variables in the data frame are checked to ensure that they are syntactically valid variable names and are not duplicated. If necessary they are adjusted (by make.names) so that they are.</param>
        ''' <param name="stringsAsFactors">logical: should character vectors be converted To factors? The 'factory-fresh’ default is TRUE, but this can be changed by setting options(stringsAsFactors = FALSE).</param>
        ''' <returns>
        ''' A data frame, a matrix-like structure whose columns may be of differing types (numeric, logical, factor and character and so on).
        '''
        ''' How the names Of the data frame are created Is complex, And the rest Of this paragraph Is only the basic story. 
        ''' If the arguments are all named And simple objects (Not lists, matrices Of data frames) Then the argument names 
        ''' give the column names. For an unnamed simple argument, a deparsed version Of the argument Is used As the name 
        ''' (With an enclosing I(...) removed). For a named matrix/list/data frame argument With more than one named column, 
        ''' the names Of the columns are the name Of the argument followed by a dot And the column name inside the argument: 
        ''' If the argument Is unnamed, the argument's column names are used. For a named or unnamed matrix/list/data frame 
        ''' argument that contains a single column, the column name in the result is the column name in the argument. 
        ''' Finally, the names are adjusted to be unique and syntactically valid unless check.names = FALSE.
        ''' </returns>
        ''' <remarks>
        ''' A data frame is a list of variables of the same number of rows with unique row names, given class "data.frame". If no variables are included, the row names determine the number of rows.
        '''
        ''' The column names should be non-empty, And attempts To use empty names will have unsupported results. Duplicate column names are allowed, but you need To use check.names = False For data.frame To generate such a data frame. However, Not all operations On data frames will preserve duplicated column names: For example matrix-Like subsetting will force column names in the result To be unique.
        '''
        ''' ``data.frame`` converts each of its arguments to a data frame by calling as.data.frame(optional = TRUE). As that Is a generic function, methods can be written to change the behaviour of arguments according to their classes: R comes With many such methods. Character variables passed To data.frame are converted To factor columns unless Protected by I Or argument stringsAsFactors Is False. If a list Or data frame Or matrix Is passed To data.frame it Is As If Each component Or column had been passed As a separate argument (except For matrices Of Class "model.matrix" And those Protected by I).
        '''
        ''' Objects passed To data.frame should have the same number Of rows, but atomic vectors (see Is.vector), factors And character vectors Protected by I will be recycled a whole number Of times If necessary (including As elements Of list arguments).
        '''
        ''' If row names are Not supplied In the Call To data.frame, the row names are taken from the first component that has suitable names, For example a named vector Or a matrix With rownames Or a data frame. (If that component Is subsequently recycled, the names are discarded With a warning.) If row.names was supplied As NULL Or no suitable component was found the row names are the Integer sequence starting at one (And such row names are considered To be 'automatic’, and not preserved by as.matrix).
        '''
        ''' If row names are supplied Of length one And the data frame has a Single row, the row.names Is taken To specify the row names And Not a column (by name Or number).
        '''
        ''' Names are removed from vector inputs Not Protected by I.
        '''
        ''' Default.stringsAsFactors Is a utility that takes getOption("stringsAsFactors") And ensures the result Is TRUE Or FALSE (Or throws an error if the value Is Not NULL).
        ''' </remarks>
        Public Function dataframe(x As IEnumerable(Of String),
                                  Optional rowNames As String() = Nothing,
                                  Optional checkRows As Boolean = False,
                                  Optional checkNames As Boolean = True,
                                  Optional stringsAsFactors As String = "default.stringsAsFactors()") As String

        End Function
    End Module
End Namespace